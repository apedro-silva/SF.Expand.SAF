using SF.Expand.LOG;
using SF.Expand.Notification;
using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.CorePublicItf;
using System;
using System.Reflection;
namespace SF.Expand.Secure.Business
{
	public static class EMAILSender
	{
		private const int cDEFAULT_EXPIRED_MESSAGE = 300;
		private const string cMODULE_NAME = "SAFBUSINESS";
		private const string cBASE_NAME = "http://sfexpand.SAFBusiness.EMAILSender.softfinanca.com/";
		public static void getTokenUserEmailAndPhone(string applicationUser, string tokenID, out string phoneNumber, out string emailAddress)
		{
			phoneNumber = null;
			emailAddress = null;
			new TokenBusinessDAO().loadTokenContacts(applicationUser, tokenID, out phoneNumber, out emailAddress);
		}
		public static OperationResult Send(string applicationUser, string tokenID, string emailAddress, string emailSubject, string emailMessage)
		{
			long tokenEventID = -1L;
			int emailGatewayStatus = 0;
			string phoneNumber = null;
			NotifyOperationResult notifyProcessorResult = NotifyOperationResult.Error;
			OperationResult result;
			if (emailAddress == null)
			{
				EMAILSender.getTokenUserEmailAndPhone(applicationUser, tokenID, out phoneNumber, out emailAddress);
				if (emailAddress == null)
				{
					result = OperationResult.Error;
					return result;
				}
			}
			string defaultEMAILProcessor = SAFConfiguration.readParameterExternal("SAFEMAILAssemblyProcessor");
			if (defaultEMAILProcessor == null || defaultEMAILProcessor.Trim().Length < 1)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.EMAILSender.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					"Invalid or inexistent email channel processor!"
				});
				result = OperationResult.Error;
			}
			else
			{
				INotifyChannelProcessor notifyProcessor = NotifyChannelProcessorFactory.LoadSMSAssembly(defaultEMAILProcessor);
				if (notifyProcessor == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
					{
						"http://sfexpand.SAFBusiness.EMAILSender.softfinanca.com/",
						Assembly.GetExecutingAssembly().FullName.ToString(),
						"[CHANNEL PROCESSOR] [" + defaultEMAILProcessor + "]",
						"Invalid or inexistent email channel processor!"
					});
					result = OperationResult.Error;
				}
				else
				{
					string defaultEMAILGateway = SAFConfiguration.readParameterExternal("SAFEMAILDefaultGateway");
					if (defaultEMAILGateway == null || (defaultEMAILGateway ?? "").Trim().Length < 1)
					{
						SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
						{
							"http://sfexpand.SAFBusiness.EMAILSender.softfinanca.com/",
							Assembly.GetExecutingAssembly().FullName.ToString(),
							"eMail default gateway invalid!"
						});
						result = OperationResult.Error;
					}
					else
					{
						int defaultEMAILGatewayTimeout = -1;
						int.TryParse(SAFConfiguration.readParameterExternal("SAFEMAILDefaultGatewayTimeout"), out defaultEMAILGatewayTimeout);
						try
						{
							NotificationEvent _event = NotificationEvent.loadNotificationEvent(0L, "SAFBUSINESS", "SAFBUSINESS", emailAddress.Trim(), emailSubject, emailMessage, NotificationChannel.loadNotificationChannel(0, "", defaultEMAILProcessor, defaultEMAILGatewayTimeout, "EMAIL", 300L, true, defaultEMAILGateway));
							try
							{
								new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 152, (int)notifyProcessorResult, applicationUser, out tokenEventID);
								notifyProcessorResult = notifyProcessor.SendNotification(_event, out emailGatewayStatus);
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
									"http://sfexpand.SAFBusiness.EMAILSender.softfinanca.com/",
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
								"http://sfexpand.SAFBusiness.EMAILSender.softfinanca.com/",
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
