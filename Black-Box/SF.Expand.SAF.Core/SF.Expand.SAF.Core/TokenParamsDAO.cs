using SF.Expand.LOG;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Defs;
using System;
using System.Data;
using System.Reflection;
namespace SF.Expand.SAF.Core
{
	public class TokenParamsDAO : DALSqlServer
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.TokenParamsDAO.softfinanca.com/";
		private const string spGET_TOKEN_BASE_PARAMS = "GetTokenBaseParams";
		public TokenTypeBaseParams loadTokenBaseParams(string tokenParamsID)
		{
			IDataReader _rd = null;
			IDbCommand _cmd = null;
			TokenTypeBaseParams result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("GetTokenBaseParams", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenParamsID));
				base.Connection.Open();
				_rd = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				_rd.Read();
				result = new TokenTypeBaseParams((int)((byte)_rd[0]), (int)((byte)_rd[1]), (int)_rd[2], (long)_rd[3], (TokenSeedType)((byte)_rd[4]), (TokenMovingFactorType)((byte)_rd[5]), (long)((int)_rd[6]), tokenParamsID, (int)_rd[7]);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokenParamsDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = default(TokenTypeBaseParams);
			}
			finally
			{
				if (_rd != null)
				{
					_rd.Dispose();
				}
				if (_cmd != null)
				{
					_cmd.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
	}
}
