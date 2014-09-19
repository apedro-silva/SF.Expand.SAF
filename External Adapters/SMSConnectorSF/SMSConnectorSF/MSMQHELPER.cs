using System;
using System.IO;
using System.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
namespace SMSConnectorSF
{
	public static class MSMQHELPER
	{
		private const string cMODULE_NAME = "SMSCONNECTOR";
		private const string cBASE_NAME = "http://sfexpand.SMSConnector.MSMQHELPER.softfinanca.com/";
		public static MessageQueue GetPrivateQueue(string msmqMachine, string msmqPath)
		{
			MessageQueue[] _queueList = MessageQueue.GetPrivateQueuesByMachine(msmqMachine);
			MessageQueue[] array = _queueList;
			MessageQueue result;
			for (int i = 0; i < array.Length; i++)
			{
				MessageQueue objQueue = array[i];
				if (0 == string.Compare(objQueue.Path.ToString(), msmqPath, true))
				{
					result = objQueue;
					return result;
				}
			}
			result = null;
			return result;
		}
		public static bool sendMQMessage(string msmqMachine, string msmqPath, string[] msmqSMSMSG)
		{
			MessageQueue _mq = null;
			Message mMSG = null;
			bool result;
			try
			{
				_mq = MSMQHELPER.GetPrivateQueue(msmqMachine, msmqPath);
				if (null == _mq)
				{
					result = false;
				}
				else
				{
					mMSG = new Message();
					mMSG.BodyStream = MSMQHELPER.SerializeMessage(msmqSMSMSG);
					_mq.Send(mMSG);
					result = true;
				}
			}
			catch
			{
				result = false;
			}
			finally
			{
				if (_mq != null)
				{
					_mq.Close();
					_mq.Dispose();
					_mq = null;
				}
				if (mMSG != null)
				{
					mMSG.Dispose();
					mMSG = null;
				}
			}
			return result;
		}
		public static object DeSerializeMessage(MemoryStream msBuffer)
		{
			object result;
			try
			{
				BinaryFormatter formatter = new BinaryFormatter();
				result = (string[])formatter.Deserialize(msBuffer);
			}
			catch
			{
				throw;
			}
			return result;
		}
		public static MemoryStream SerializeMessage(object msmqSMSMSG)
		{
			MemoryStream result;
			try
			{
				MemoryStream ms = new MemoryStream();
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(ms, (string[])msmqSMSMSG);
				result = ms;
			}
			catch
			{
				throw;
			}
			return result;
		}
	}
}
