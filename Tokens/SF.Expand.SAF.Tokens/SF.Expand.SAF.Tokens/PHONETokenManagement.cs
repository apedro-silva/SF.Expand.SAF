using SF.Expand.SAF.Blobs;
using SF.Expand.SAF.Core;
using SF.Expand.SAF.CorePublicItf;
using System;
namespace SF.Expand.SAF.Tokens
{
	public class PHONETokenManagement : ITokens
	{
		private const string _infoTkType = "J1";
		private const string _infoSrvBlobProcessor = "SF.Expand.SAF.Blobs.BLOBStructInfSrv, SF.Expand.SAF.Blobs";
		public OperationResult Create(string tokenVendorID, DateTime expirationDate, string supplierSerialNumber, string creationLotID, string pin, out TokenInfoCore tokenInfoCore)
		{
			string _tokenBlob = null;
			OperationResult result;
			if (pin == null || (pin ?? "").Length < 2)
			{
				tokenInfoCore = new TokenInfoCore();
				result = OperationResult.Error;
			}
			else
			{
				OperationResult _hResult = new TokensDAO().newTokenFromPreInsertedSeed(tokenVendorID, TokenStatus.ReadyToDeploy, out tokenInfoCore);
				if (_hResult == OperationResult.Success)
				{
					IBLOBData _blobProcessor = BLOBDataFactory.LoadAssembly("SF.Expand.SAF.Blobs.BLOBStructInfSrv, SF.Expand.SAF.Blobs");
					if (_blobProcessor.Export(pin, "J1", null, new TokensDAO().loadTokenCryptoData(tokenInfoCore.InternalID.ToString()), out _tokenBlob))
					{
						if (OperationResult.Success != new TokensDAO().updateCryptoData(tokenInfoCore.InternalID.ToString(), _tokenBlob))
						{
							this.UndoCreate(tokenInfoCore.InternalID.ToString());
							_hResult = OperationResult.Error;
						}
					}
				}
				result = _hResult;
			}
			return result;
		}
		public OperationResult UndoCreate(string tokenInternalID)
		{
			return new TokensDAO().undoUpdateForNewToken(tokenInternalID);
		}
		public OperationResult Cancel(string tokenInternalID)
		{
			return new TokensDAO().updateTokenStatus(tokenInternalID, TokenStatus.Canceled);
		}
		public OperationResult InhibitedUse(string tokenInternalID)
		{
			return new TokensDAO().updateTokenStatus(tokenInternalID, TokenStatus.Disabled);
		}
		public OperationResult AllowedUse(string tokenInternalID)
		{
			return new TokensDAO().updateTokenStatus(tokenInternalID, TokenStatus.Enabled);
		}
		public OperationResult CheckStatus(string tokenInternalID, out TokenStatus tokenStatus)
		{
			return new TokensDAO().tokenStatus(tokenInternalID, out tokenStatus);
		}
	}
}
