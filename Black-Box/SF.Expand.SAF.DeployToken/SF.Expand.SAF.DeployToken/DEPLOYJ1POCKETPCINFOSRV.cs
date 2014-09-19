using SF.Expand.LOG;
using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.CorePublicItf;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
namespace SF.Expand.SAF.DeployToken
{
	internal class DEPLOYJ1POCKETPCINFOSRV : IDeployToken
	{
		private const string cMODULE_NAME = "SAFBUSINESSDEPLOY";
		private const string cBASE_NAME = "http://sfexpand.SAFDeploy.DEPLOYJ1POCKETPCINFOSRV.softfinanca.com/";
		private const string cBLOBFILENAME = "000token.001";
		private const string cAPPFILESYSTEM_NAME = "infotoken.cab";
		private const string cCONTENT_TYPE_NAME = "application/vnd.ms-cab-compressed";
		private const string cTEMPLATE_LOCATION = "DEPLOYJ1WINMOBILEINFOSRV";
		private const string cTEMPWORKFOLDER = "DEPLOYJ1WINMOBILEINFOSRV_TEMPFOLDER";
		private string _getTemplateFile()
		{
			string _fTemplateLocation = SAFConfiguration.readParameterExternal("DEPLOYJ1WINMOBILEINFOSRV");
			string result;
			if (!File.Exists(_fTemplateLocation))
			{
				result = null;
			}
			else
			{
				result = _fTemplateLocation;
			}
			return result;
		}
		private string _getTempFolder()
		{
			string _fBaseFolder = SAFConfiguration.readParameterExternal("DEPLOYJ1WINMOBILEINFOSRV_TEMPFOLDER");
			if (_fBaseFolder == null || _fBaseFolder.Length < 2)
			{
				_fBaseFolder = Path.GetTempPath();
			}
			string _fTempFolder = _fBaseFolder + ((!_fBaseFolder.EndsWith("\\")) ? "\\" : "") + Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + "\\";
			string result;
			if (!Directory.Exists(_fTempFolder))
			{
				Directory.CreateDirectory(_fTempFolder);
				result = _fTempFolder;
			}
			else
			{
				result = null;
			}
			return result;
		}
		private void _writeBlobFile(string tempFolder, byte[] blobData)
		{
			using (Stream st = File.Open(tempFolder + "000token.001", FileMode.Create, FileAccess.ReadWrite))
			{
				using (BinaryWriter br = new BinaryWriter(st))
				{
					br.Write(blobData);
				}
			}
		}
		private bool _executeProcess(string fname, string execArgs)
		{
			Process _execProc = new Process();
			bool result;
			try
			{
				_execProc.StartInfo.UseShellExecute = false;
				_execProc.StartInfo.RedirectStandardError = true;
				_execProc.StartInfo.CreateNoWindow = true;
				_execProc.StartInfo.FileName = fname;
				_execProc.StartInfo.Arguments = execArgs;
				_execProc.Start();
				result = true;
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESSDEPLOY", new string[]
				{
					"http://sfexpand.SAFDeploy.DEPLOYJ1POCKETPCINFOSRV.softfinanca.com/",
					ex.ToString()
				});
				result = false;
			}
			finally
			{
				if (_execProc != null)
				{
					_execProc.Dispose();
				}
				_execProc = null;
			}
			return result;
		}
		private void _writeINFFile(string _tempFolder)
		{
			StreamWriter _sWriter = null;
			try
			{
				string _extractPath = _tempFolder + ((!_tempFolder.EndsWith("\\")) ? "\\\\" : "");
				string _path2DDFFile = Path.Combine(_extractPath, "infotoken.inf");
				StreamReader _sReader = File.OpenText(_path2DDFFile);
				string _fdata = _sReader.ReadToEnd();
				_sReader.Close();
				File.Delete(_path2DDFFile);
				_sWriter = File.CreateText(_path2DDFFile);
				_sWriter.Write(_fdata.Replace("###ExtractionFolder###", _extractPath));
				_sWriter.Flush();
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESSDEPLOY", new string[]
				{
					"http://sfexpand.SAFDeploy.DEPLOYJ1POCKETPCINFOSRV.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
			}
			finally
			{
				if (_sWriter != null)
				{
					_sWriter.Close();
				}
			}
		}
		private void _writeDDFFile(string _tempFolder)
		{
			StreamWriter _sWriter = null;
			try
			{
				string _extractPath = _tempFolder + ((!_tempFolder.EndsWith("\\")) ? "\\\\" : "");
				string _path2DDFFile = Path.Combine(_extractPath, "setup.ddf");
				StreamReader _sReader = File.OpenText(_path2DDFFile);
				string _fdata = _sReader.ReadToEnd();
				_sReader.Close();
				File.Delete(_path2DDFFile);
				_sWriter = File.CreateText(_path2DDFFile);
				_sWriter.Write(_fdata.Replace("###FileName###", "infotoken.cab").Replace("###ExtractionFolder###", _extractPath).Replace("###Path###", _extractPath));
				_sWriter.Flush();
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESSDEPLOY", new string[]
				{
					"http://sfexpand.SAFDeploy.DEPLOYJ1POCKETPCINFOSRV.softfinanca.com/",
					ex.ToString()
				});
			}
			finally
			{
				if (_sWriter != null)
				{
					_sWriter.Close();
				}
			}
		}
		public OperationResult AssembleTokenApplication(byte[] blobData, out string appContentType, out string Base64TokenApplication)
		{
			appContentType = null;
			Base64TokenApplication = null;
			string _tempFolder = null;
			SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.INFORMATION, "SAFBUSINESSDEPLOY", new string[]
			{
				"http://sfexpand.SAFDeploy.DEPLOYJ1POCKETPCINFOSRV.softfinanca.com/",
				"AssembleTokenApplication started!"
			});
			OperationResult result;
			try
			{
				_tempFolder = this._getTempFolder();
				if (!this._executeProcess("expand.exe", this._getTemplateFile() + " -F:*.* " + _tempFolder))
				{
					result = OperationResult.Error;
				}
				else
				{
					this._writeBlobFile(_tempFolder, blobData);
					this._writeINFFile(_tempFolder);
					this._writeDDFFile(_tempFolder);
					if (!this._executeProcess("makecab.exe", " /F " + Path.Combine(_tempFolder, "setup.ddf")))
					{
						result = OperationResult.Error;
					}
					else
					{
						using (Stream st = File.Open(_tempFolder + "infotoken.cab", FileMode.Open, FileAccess.Read))
						{
							using (BinaryReader br = new BinaryReader(st, Encoding.Default))
							{
								Base64TokenApplication = Convert.ToBase64String(br.ReadBytes(Convert.ToInt32(br.BaseStream.Length)));
								appContentType = "application/vnd.ms-cab-compressed";
							}
						}
						SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.INFORMATION, "SAFBUSINESSDEPLOY", new string[]
						{
							"http://sfexpand.SAFDeploy.DEPLOYJ1POCKETPCINFOSRV.softfinanca.com/",
							"AssembleTokenApplication terminate!"
						});
						result = OperationResult.Success;
					}
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESSDEPLOY", new string[]
				{
					"http://sfexpand.SAFDeploy.DEPLOYJ1POCKETPCINFOSRV.softfinanca.com/",
					ex.ToString()
				});
				Base64TokenApplication = null;
				result = OperationResult.Error;
			}
			finally
			{
				if (Directory.Exists(_tempFolder))
				{
					Directory.Delete(_tempFolder, true);
				}
			}
			return result;
		}
	}
}
