using SF.Expand.LOG;
using SF.Expand.Notification;
using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.CorePublicItf;
using System;
using System.Reflection;
namespace SF.Expand.Secure.Business
{
	public static class SMSSender
	{
		private const int cDEFAULT_EXPIRED_MESSAGE = 300;
		private const string cMODULE_NAME = "SAFBUSINESS";
		private const string cBASE_NAME = "http://sfexpand.SAFBusiness.SMSSender.softfinanca.com/";
		public static void getTokenUserEmailAndPhone(string applicationUser, string tokenID, out string phoneNumber, out string emailAddress)
		{
			phoneNumber = null;
			emailAddress = null;
			new TokenBusinessDAO().loadTokenContacts(applicationUser, tokenID, out phoneNumber, out emailAddress);
		}
		public static OperationResult Send(string applicationUser, string tokenID, string phoneNumber, string smsMessage)
		{
			long tokenEventID = -1L;
			int smsGatewayStatus = 0;
			string emailAddr = null;
			NotifyOperationResult notifyProcessorResult = NotifyOperationResult.Error;
			OperationResult result;
			if (phoneNumber == null)
			{
				SMSSender.getTokenUserEmailAndPhone(applicationUser, tokenID, out phoneNumber, out emailAddr);
				if (phoneNumber == null)
				{
					result = OperationResult.Error;
					return result;
				}
			}
			string defaultSMSProcessor = SAFConfiguration.readParameterExternal("SAFSMSAssemblyProcessor");
			if (defaultSMSProcessor == null || defaultSMSProcessor.Trim().Length < 1)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.SMSSender.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					"Invalid or inexistent SMS channel processor!"
				});
				result = OperationResult.Error;
			}
			else
			{
				INotifyChannelProcessor notifyProcessor = NotifyChannelProcessorFactory.LoadSMSAssembly(defaultSMSProcessor);
				if (notifyProcessor == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
					{
						"http://sfexpand.SAFBusiness.SMSSender.softfinanca.com/",
						Assembly.GetExecutingAssembly().FullName.ToString(),
						"[CHANNEL PROCESSOR] [" + defaultSMSProcessor + "]",
						"Invalid or inexistent SMS channel processor!"
					});
					result = OperationResult.Error;
				}
				else
				{
					string defaultSMSGateway = SAFConfiguration.readParameterExternal("SAFSMSDefaultGateway");
					if (defaultSMSGateway == null || defaultSMSGateway.Trim().Length < 1)
					{
						SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
						{
							"http://sfexpand.SAFBusiness.SMSSender.softfinanca.com/",
							Assembly.GetExecutingAssembly().FullName.ToString(),
							"Sms default gateway invalid!"
						});
						result = OperationResult.Error;
					}
					else
					{
						int defaultSMSGatewayTimeout = -1;
						int.TryParse(SAFConfiguration.readParameterExternal("SAFSMSDefaultGatewayTimeout"), out defaultSMSGatewayTimeout);
						try
						{
							NotificationEvent _event = NotificationEvent.loadNotificationEvent(0L, "SAFBUSINESS", "SAFBUSINESS", phoneNumber.Trim(), smsMessage, null, NotificationChannel.loadNotificationChannel(0, "", defaultSMSProcessor, defaultSMSGatewayTimeout, "SMS", 300L, true, defaultSMSGateway));
							try
							{
								new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 151, (int)notifyProcessorResult, applicationUser, out tokenEventID);
								notifyProcessorResult = notifyProcessor.SendNotification(_event, out smsGatewayStatus);
								if (tokenEventID > 0L)
								{
									new TokensBusinessEventsDAO().updateEventStatus(tokenEventID.ToString(), (byte)notifyProcessorResult);
								}
								result = ((notifyProcessorResult == NotifyOperationResult.Success) ? OperationResult.Success : OperationResult.Error);
							}
							catch (Exception ex)
							{
								SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
								{
									"http://sfexpand.SAFBusiness.SMSSender.softfinanca.com/",
									Assembly.GetExecutingAssembly().FullName.ToString(),
									ex.ToString()
								});
								result = OperationResult.Error;
							}
						}
						catch (Exception ex)
						{
							SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
							{
								"http://sfexpand.SAFBusiness.SMSSender.softfinanca.com/",
								Assembly.GetExecutingAssembly().FullName.ToString(),
								ex.ToString()
							});
							result = OperationResult.Error;
						}
						finally
						{
						}
					}
				}
			}
			return result;
		}
	}
}
