
using System;
using System.IO;

using Ionic.Zip;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Configuration;


namespace SF.Expand.SAF.DeployToken
{
    public class DEPLOYJ1JAVAINFOSRV : IDeployToken
    {
        private const string cBLOBFILENAME = "token.dat";
        private const string cBLOBFILEPATH = @"infotoken/";
        private const string cCONTENT_TYPE_NAME = "application/java-archive";
        private const string cTEMPLATE_LOCATION = "DEPLOYJ1JAVAINFOSRV";
        private const string _baseName = @"http://sfexpandsecure.business.DEPLOYJ1JAVAINFOSRV.softfinanca.com::";


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



        /// <summary></summary>
        /// <param name="blobData"></param>
        /// <param name="appNameFileSystem"></param>
        /// <param name="appContentType"></param>
        /// <param name="Base64TokenApplication"></param>
        /// <returns></returns>
        public OperationResult AssembleTokenApplication(Byte[] blobData, out string appContentType, out string Base64TokenApplication)
        {
            appContentType = null;
            Base64TokenApplication = null;

            try
            {
                using (Stream st = new MemoryStream(blobData))
                {
                    using (ZipFile _zip = ZipFile.Read(_getTemplateFile()))
                    {
                        _zip.RemoveEntry(cBLOBFILEPATH+cBLOBFILENAME);
                        _zip.AddFileStream(cBLOBFILENAME, cBLOBFILEPATH, st);

                        using (MemoryStream _oStream = new MemoryStream())
                        {
                            _zip.Save(_oStream);
                            appContentType = cCONTENT_TYPE_NAME;
                            Base64TokenApplication = Convert.ToBase64String(_oStream.ToArray());
                            _oStream.Close();
                        }
                    }
                    st.Close();
                }
                return OperationResult.Success;
            }
            catch
            {
                Base64TokenApplication = null;
                return OperationResult.Error;
            }
        }
    }
}