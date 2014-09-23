using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.Core;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Defs;
using SF.Expand.SAF.Utils;
using System;
using System.Collections;
using System.IO;
using System.Threading;
namespace SF.Expand.Secure.GenerateTokens
{
	public static class GenerateSeeds
	{
		private const string cBASE_FILE_NAME = "seedsThread";
		private const string cBASE_FILE_NAME_MATRIX = "seedsThreadMatrix";
		public static void Generate(string seedType, string _fname, int tThreads, long tSeeds, bool showProcessing, bool generateMatrix)
		{
			DateTime now = DateTime.Now;
			ArrayList arrayList = new ArrayList();
			Thread[] array = new Thread[tThreads];
			long num = tSeeds / (long)tThreads;
			long num2 = tSeeds % (long)tThreads;
			Thread.Sleep(new Random().Next(1, 9));
			int num3;
			int num4;
			ThreadPool.GetAvailableThreads(out num3, out num4);
			string text = BaseFunctions.GenerateSupplierLotNumber(tSeeds.ToString(), null);
			TokenTypeBaseParams tokenTypeBaseParams = new TokenParamsDAO().loadTokenBaseParams(seedType);
			generateMatrix = (tokenTypeBaseParams.MovingFactorType == TokenMovingFactorType.TransactionAuthenticationNumber);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Thread(new ParameterizedThreadStart(GenerateSeeds.buildSeeds));
				array[i].Start(new object[]
				{
					tokenTypeBaseParams,
					(i == 0) ? (num + num2) : num,
					text,
					i.ToString(),
					arrayList,
					generateMatrix
				});
			}
			bool flag;
			do
			{
				flag = true;
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].IsAlive)
					{
						Thread.Sleep(1000);
						flag = false;
						break;
					}
				}
			}
			while (!flag);
			Console.WriteLine(GenerateSeeds.SyncronizeAllFiles(tThreads, _fname, text, generateMatrix) ? (_fname + " created suceffuly") : (_fname + " not created! ERROR!"));
			Console.WriteLine("Processing completed on [" + DateTime.Now.Subtract(now).ToString() + "]");
		}
		private static bool SyncronizeAllFiles(int iFiles, string outPath, string supplierLotNumber, bool generateMatrix)
		{
			int num = 0;
			StreamWriter streamWriter = null;
			StreamWriter streamWriter2 = null;
			if (generateMatrix)
			{
				streamWriter2 = new StreamWriter(outPath + supplierLotNumber + "Mtx.txt", false);
			}
			streamWriter = new StreamWriter(outPath + supplierLotNumber + ".dat", false);
			for (int i = 0; i < iFiles; i++)
			{
				if (File.Exists("seedsThread." + i.ToString().PadLeft(3, '0')))
				{
					using (StreamReader streamReader = File.OpenText("seedsThread." + i.ToString().PadLeft(3, '0')))
					{
						streamWriter.Write(streamReader.ReadToEnd());
						streamReader.Close();
						num++;
					}
					File.Delete("seedsThread." + i.ToString().PadLeft(3, '0'));
				}
				if (generateMatrix && File.Exists("seedsThreadMatrix." + i.ToString().PadLeft(3, '0')))
				{
					using (StreamReader streamReader2 = File.OpenText("seedsThreadMatrix." + i.ToString().PadLeft(3, '0')))
					{
						streamWriter2.Write(streamReader2.ReadToEnd());
						streamReader2.Close();
					}
					File.Delete("seedsThreadMatrix." + i.ToString().PadLeft(3, '0'));
				}
			}
			streamWriter.Flush();
			streamWriter.Close();
			if (generateMatrix)
			{
				streamWriter2.Flush();
				streamWriter2.Close();
			}
			return num == iFiles;
		}
		private static void buildSeeds(object _params)
		{
			string text = null;
			long num = 0L;
			int num2 = RandomGen.Next();
			InMemoryLogging inMemoryLogging = null;
			long num3 = (long)((object[])_params)[1];
			string loteID = (string)((object[])_params)[2];
			string.Concat(new string[]
			{
				"/Process:",
				Thread.CurrentThread.GetHashCode().ToString(),
				" /thread:",
				((string)((object[])_params)[3]).Trim(),
				" /processing:",
				num3.ToString().Trim(),
				"/{0} "
			});
			ArrayList arrayList = (ArrayList)((object[])_params)[4];
			string masterKey = SAFConfiguration.readMasterKey();
			TokenTypeBaseParams tkTypeBaseParams = (TokenTypeBaseParams)((object[])_params)[0];
			InMemoryLogging logString = InMemoryLogging.GetLogString("seedsThread." + ((string)((object[])_params)[3]).PadLeft(3, '0'), false);
			logString.MaxChars = -1;
			if (tkTypeBaseParams.MovingFactorType == TokenMovingFactorType.TransactionAuthenticationNumber)
			{
				inMemoryLogging = InMemoryLogging.GetLogString("seedsThreadMatrix." + ((string)((object[])_params)[3]).PadLeft(3, '0'), false);
				inMemoryLogging.MaxChars = -1;
			}
			int num4 = 0;
			while ((long)num4 < num3)
			{
				while (true)
				{
					text = new Random(num2++).NextDouble().ToString();
					text = text.Substring(text.Length - 12);
					ArrayList obj;
					Monitor.Enter(obj = arrayList);
					try
					{
						if (arrayList.Contains(text))
						{
							continue;
						}
						arrayList.Add(text);
					}
					finally
					{
						Monitor.Exit(obj);
					}
					break;
				}
				TokenCryptoData tokenCryptoData;
				if (TokensBaseFunctions.TokensCreateNew(tkTypeBaseParams, masterKey, text, "", out tokenCryptoData) == OperationResult.Success)
				{
					logString.Add(BaseImportExportTokens.Export(tokenCryptoData, loteID));
					if (tkTypeBaseParams.MovingFactorType == TokenMovingFactorType.TransactionAuthenticationNumber)
					{
						inMemoryLogging.Add(text + ";" + string.Join(";", TokensBaseFunctions.tokenTANMatrixArrayFetch(tokenCryptoData, masterKey, "")));
					}
					num += 1L;
				}
				num4++;
			}
			logString.Persist();
			if (inMemoryLogging != null)
			{
				inMemoryLogging.Persist();
			}
		}
	}
}
