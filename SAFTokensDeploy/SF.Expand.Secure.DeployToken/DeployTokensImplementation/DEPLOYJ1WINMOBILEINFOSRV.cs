
using System;
using System.IO;
using System.Text;

using CabLib;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Configuration;


namespace SF.Expand.SAF.DeployToken
{
    public class DEPLOYJ1WINMOBILEINFOSRV : IDeployToken
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
        /// <param name="blobData"></param>
        /// <param name="appContentType"></param>
        /// <param name="Base64TokenApplication"></param>
        /// <returns></returns>
        public OperationResult AssembleTokenApplication(byte[] blobData, out string appContentType, out string Base64TokenApplication)
        {
            appContentType = null;
            Base64TokenApplication = null;

            string _tempFolder = null;
            Extract _cabExtract = new Extract();
            Compress _cabCompress = new Compress();

            try
            {
                _tempFolder = _getTempFolder();
                _cabExtract.ExtractFile(_getTemplateFile(), _tempFolder); 
                _writeBlobFile(_tempFolder, blobData);

                _cabCompress.SwitchCompression(false);  // pocket´s withdout compression
                //_cabCompress.SetEncryptionKey("");
                _cabCompress.CompressFolder(_tempFolder, _tempFolder + cAPPFILESYSTEM_NAME, "", true, true, 0x7FFFFFFF);


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
                _cabExtract = null;
                _cabCompress = null;

                if (Directory.Exists(_tempFolder))
                    Directory.Delete(_tempFolder, true);
            }
        }
    }
}
