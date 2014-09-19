

using System;
using System.IO;
using System.Text;
using System.Diagnostics;

using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Configuration;


namespace SF.Expand.SAF.DeployToken
{
    internal class DEPLOYJ1POCKETPCINFOSRV :IDeployToken 
    {
        private const string cBLOBFILENAME = "000token.001";
        private const string cAPPFILESYSTEM_NAME = "infotoken.cab";
        private const string cCONTENT_TYPE_NAME = "application/vnd.ms-cab-compressed";
        private const string cTEMPLATE_LOCATION = "DEPLOYJ1WINMOBILEINFOSRV";
        private const string cTEMPWORKFOLDER = "DEPLOYJ1WINMOBILEINFOSRV_TEMPFOLDER";
        private const string _baseName = @"http://sfexpandsecure.business.DEPLOYJ1WINMOBILEINFOSRV.softfinanca.com::";

        
        /// <summary></summary>
        /// <returns></returns>
        private string _getTemplateFile()
        {
            string _fTemplateLocation = SAFConfiguration.readParameterExternal(cTEMPLATE_LOCATION);
            if (!File.Exists(_fTemplateLocation))
            {
                return null;
            }
            return _fTemplateLocation;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private string _getTempFolder()
        {
            string _fBaseFolder = SAFConfiguration.readParameterExternal(cTEMPWORKFOLDER);
            if (_fBaseFolder == null || _fBaseFolder.Length < 2)
            {
                _fBaseFolder = Path.GetTempPath();
            }

            string _fTempFolder = _fBaseFolder + (!_fBaseFolder.EndsWith(@"\") ? @"\" : "") + Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + @"\";
            if (!Directory.Exists(_fTempFolder))
            {
                Directory.CreateDirectory(_fTempFolder);
                return _fTempFolder;
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="tempFolder"></param>
        /// <param name="blobData"></param>
        private void _writeBlobFile(string tempFolder, Byte[] blobData)
        {
            using (Stream st = File.Open(tempFolder + cBLOBFILENAME, FileMode.Create, FileAccess.ReadWrite))
            {
                using (BinaryWriter br = new BinaryWriter(st))
                {
                    br.Write(blobData);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="execArgs"></param>
        private bool _executeProcess(string fname, string execArgs)
        {
            string _outputError = null;
            Process _execProc = new Process();

            try
            {
                _execProc.StartInfo.UseShellExecute = false;
                _execProc.StartInfo.RedirectStandardError = true;
                _execProc.StartInfo.CreateNoWindow = true;
                _execProc.StartInfo.FileName = fname;
                _execProc.StartInfo.Arguments = execArgs;
                _execProc.Start();

                _outputError = _execProc.StandardOutput.ReadToEnd();

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (_execProc != null)
                {
                    _execProc.Dispose();
                }
                _execProc = null;
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="_tempFolder"></param>
        private void _writeINFFile(string _tempFolder)
        {
            string _fdata = null;
            string _path2DDFFile = null;
            string _extractPath = null;
            StreamReader _sReader = null;
            StreamWriter _sWriter = null;

            try
            {
                _extractPath = _tempFolder + (!_tempFolder.EndsWith("\\") ? @"\\" : "");
                _path2DDFFile = Path.Combine(_extractPath, "infotoken.inf");

                _sReader = File.OpenText(_path2DDFFile);
                _fdata = _sReader.ReadToEnd();
                _sReader.Close(); File.Delete(_path2DDFFile);

                _sWriter = File.CreateText(_path2DDFFile);
                _sWriter.Write(_fdata.Replace(
                                "###ExtractionFolder###", _extractPath));
                _sWriter.Flush();

            }
            finally
            {
                _sReader = null;
                if (_sWriter != null)
                {
                    _sWriter.Close();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="_tempFolder"></param>
        private void _writeDDFFile(string _tempFolder)
        {
            string _fdata = null;
            string _path2DDFFile = null;
            string _extractPath = null;
            StreamReader _sReader = null;
            StreamWriter _sWriter = null;

            try
            {
                _extractPath = _tempFolder + (!_tempFolder.EndsWith("\\") ? @"\\" : "");
                _path2DDFFile = Path.Combine(_extractPath, "setup.ddf");

                _sReader = File.OpenText(_path2DDFFile);
			    _fdata = _sReader.ReadToEnd();
                _sReader.Close(); File.Delete(_path2DDFFile);
                _sWriter = File.CreateText(_path2DDFFile);
			    _sWriter.Write( _fdata.Replace(
                                    "###FileName###", cAPPFILESYSTEM_NAME).Replace(
                                    "###ExtractionFolder###", _extractPath).Replace(
                                    "###Path###", _extractPath));
			    _sWriter.Flush();

            }
            finally
            {
                _sReader = null;
                if (_sWriter!=null)
                {
                    _sWriter.Close();
                }
            }
        }






        /// <summary>
        /// </summary>
        /// <param name="blobData"></param>
        /// <param name="appContentType"></param>
        /// <param name="Base64TokenApplication"></param>
        /// <returns></returns>
        public OperationResult AssembleTokenApplication(byte[] blobData, out string appContentType, out string Base64TokenApplication)
        {
            appContentType = null;
            Base64TokenApplication = null;
            string _tempFolder = null;

            try
            {
                _tempFolder =_getTempFolder();
                if (!_executeProcess("expand.exe", (_getTemplateFile() + " -F:*.* " + _tempFolder)))
                {
                    return OperationResult.Error;
                }

                _writeBlobFile(_tempFolder, blobData);
                _writeINFFile(_tempFolder);
                _writeDDFFile(_tempFolder);

                if (!_executeProcess("makecab.exe", (" /F " + Path.Combine(_tempFolder, "setup.ddf"))))
                {
                    return OperationResult.Error;
                }

                using (Stream st = File.Open(_tempFolder + cAPPFILESYSTEM_NAME, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(st, Encoding.Default))
                    {
                        Base64TokenApplication = Convert.ToBase64String(br.ReadBytes(Convert.ToInt32(br.BaseStream.Length)));
                        appContentType = cCONTENT_TYPE_NAME;
                    }
                }
                return OperationResult.Success;

            }
            catch
            {
                Base64TokenApplication = null;
                return OperationResult.Error;
            }
            finally
            {
                if (Directory.Exists(_tempFolder))
                    Directory.Delete(_tempFolder, true);
            }
        }
    }
}