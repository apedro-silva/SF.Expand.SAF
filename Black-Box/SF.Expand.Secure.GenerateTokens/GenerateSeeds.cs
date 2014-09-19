

using System;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Threading;

using SF.Expand.SAF.Core;
using SF.Expand.SAF.Defs;
using SF.Expand.SAF.Utils;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Configuration;


namespace SF.Expand.Secure.GenerateTokens
{
    public static class GenerateSeeds
    {
        private const string cBASE_FILE_NAME = @"seedsThread";
        private const string cBASE_FILE_NAME_MATRIX = @"seedsThreadMatrix";

        public static void Generate(string seedType, string _fname, int tThreads, long tSeeds, bool showProcessing, bool generateMatrix)
        {
            int intAvailableThreads;
            int intAvailableIoAsynThreds;
            DateTime _initProc = DateTime.Now;
            ArrayList _vSerial = new ArrayList();
            Thread[] m_Threads = new Thread[tThreads];
            long _processLoad = tSeeds / tThreads;
            long __processLoad = tSeeds % tThreads;

            Thread.Sleep(new Random().Next(1, 9));  //induce entropy 
            ThreadPool.GetAvailableThreads(out intAvailableThreads, out intAvailableIoAsynThreds);
            string _supplierLotNumber = BaseFunctions.GenerateSupplierLotNumber(tSeeds.ToString(), null);
            TokenTypeBaseParams _tkTypeBaseParams = new TokenParamsDAO().loadTokenBaseParams(seedType);
            generateMatrix = _tkTypeBaseParams.MovingFactorType == TokenMovingFactorType.TransactionAuthenticationNumber ? true : false;


            for (int i = 0; i < m_Threads.Length; i++)
            {
                m_Threads[i] = new Thread(new ParameterizedThreadStart(buildSeeds));
                m_Threads[i].Start(new object[] { (object)_tkTypeBaseParams, (object)(i == 0 ? _processLoad + __processLoad : _processLoad), (object)_supplierLotNumber, (object)i.ToString(), (object)_vSerial, (object)generateMatrix });
            }

            while (true)
            {
                bool _fl = true;
                for (int i1 = 0; i1 < m_Threads.Length; i1++)
                {
                    if (m_Threads[i1].IsAlive)
                    {
                        Thread.Sleep(1000);
                        _fl = false;
                        break;
                    }
                }
                if (_fl) break;
            }

            Console.WriteLine(SyncronizeAllFiles(tThreads, _fname, _supplierLotNumber,generateMatrix) ? _fname + " created suceffuly" : _fname + " not created! ERROR!");
            Console.WriteLine("Processing completed on [" + DateTime.Now.Subtract(_initProc).ToString() + "]");
        }

        private static bool SyncronizeAllFiles(int iFiles, string outPath, string supplierLotNumber, bool generateMatrix)
        {
            int _fProcessed = 0;
            StreamWriter sw = null;
            StreamWriter swMx = null;

            if (generateMatrix)
            {
                swMx = new StreamWriter(outPath + supplierLotNumber + "Mtx.txt", false);
            }

            sw = new StreamWriter(outPath + supplierLotNumber + ".dat", false);
            for (int i = 0; i < iFiles; i++)
            {
                if (File.Exists(cBASE_FILE_NAME + "." + i.ToString().PadLeft(3, '0')))
                {
                    using (StreamReader sr = File.OpenText(cBASE_FILE_NAME + "." + i.ToString().PadLeft(3, '0')))
                    {
                        sw.Write(sr.ReadToEnd()); sr.Close();
                        _fProcessed += 1;
                    }
                    File.Delete(cBASE_FILE_NAME + "." + i.ToString().PadLeft(3, '0'));
                }

                if (generateMatrix && File.Exists(cBASE_FILE_NAME_MATRIX + "." + i.ToString().PadLeft(3, '0')))
                {
                    using (StreamReader srMx = File.OpenText(cBASE_FILE_NAME_MATRIX + "." + i.ToString().PadLeft(3, '0')))
                    {
                        swMx.Write(srMx.ReadToEnd()); srMx.Close();
                    }
                    File.Delete(cBASE_FILE_NAME_MATRIX + "." + i.ToString().PadLeft(3, '0'));
                }
            }

            sw.Flush(); sw.Close();
            if (generateMatrix) { swMx.Flush(); swMx.Close(); };
            return _fProcessed == iFiles ? true : false;
        }

        private static void buildSeeds(object _params)
        {
            string sVendorSerial = null;
            long _ntotalCreated = 0;
            int seed = RandomGen.Next();
            InMemoryLogging loggerMatrixFile = null;
            long _nRequest = (long)((object[])_params)[1];
            string _nSerie = (string)((object[])_params)[2];
            string _outInf = "/Process:" + Thread.CurrentThread.GetHashCode().ToString() + " /thread:" + ((string)((object[])_params)[3]).Trim() + " /processing:" + _nRequest.ToString().Trim() + "/{0} ";
            ArrayList _vSm = (ArrayList)((object[])_params)[4];
            string _masterKey = SAFConfiguration.readMasterKey();

            TokenCryptoData _TokenCryptoData;
            TokenTypeBaseParams _tkParams = (TokenTypeBaseParams)((object[])_params)[0];
            InMemoryLogging logger = InMemoryLogging.GetLogString(cBASE_FILE_NAME + "." + ((string)((object[])_params)[3]).PadLeft(3, '0'), false);
            logger.MaxChars = -1;


            if (_tkParams.MovingFactorType == TokenMovingFactorType.TransactionAuthenticationNumber)
            {
                loggerMatrixFile = InMemoryLogging.GetLogString(cBASE_FILE_NAME_MATRIX + "." + ((string)((object[])_params)[3]).PadLeft(3, '0'), false);
                loggerMatrixFile.MaxChars = -1;
            }

            for (int i = 0; i < _nRequest; i++)
            {
                while (true)
                {
                    sVendorSerial = new Random(seed++).NextDouble().ToString();
                    sVendorSerial = sVendorSerial.Substring(sVendorSerial.Length - 12);

                    lock (_vSm)
                    {
                        if (!_vSm.Contains((object)sVendorSerial))
                        {
                            _vSm.Add((object)sVendorSerial);
                            break;
                        }
                    }
                }

                if (OperationResult.Success == TokensBaseFunctions.TokensCreateNew(_tkParams, _masterKey, sVendorSerial, "", out _TokenCryptoData))
                {
                    logger.Add(BaseImportExportTokens.Export(_TokenCryptoData, _nSerie));
                    if (_tkParams.MovingFactorType == TokenMovingFactorType.TransactionAuthenticationNumber)
                    {
                        loggerMatrixFile.Add(sVendorSerial + ";" + string.Join(";", TokensBaseFunctions.tokenTANMatrixArrayFetch(_TokenCryptoData, _masterKey, "")));
                    }
                    _ntotalCreated += 1;
                }
            }
            logger.Persist();
            if (loggerMatrixFile!=null) loggerMatrixFile.Persist();
        }
    }

    public static class RandomGen
    {
        private static Random _global = new Random();
        [ThreadStatic]
        private static Random _local;

        public static int Next()
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.Next();
        }
    }
}
