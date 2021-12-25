using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace CoalesceUserMetadata
{
    public class DBAccess : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("DBAccess");

        #region Constants

        static readonly NameValueCollection section = ConfigurationManager.GetSection("DBAccess") as NameValueCollection;
        private static readonly string DBConnectionString = section["DBConnectionString"];

        #endregion

        OracleConnection connection;
        public OracleCommand CreateCommand(string sql, CommandType type, params OracleParameter[] parameters)
        {
            connection = new OracleConnection(DBConnectionString);
            connection.Open();

            OracleCommand command = new OracleCommand(sql, connection)
            {
                CommandType = type
            };
            if (parameters != null && parameters.Length > 0)
            {
                OracleParameterCollection cmdParams = command.Parameters;
                for (int i = 0; i < parameters.Length; i++) { cmdParams.Add(parameters[i]); }
            }
            return command;
        }

        /* Creates a data reader that can be used to iterate through an Oracle out param RefCursor.*/
        public OracleDataReader GetDataReader(string storedProcedure, params OracleParameter[] parameters)
        {
            return CreateCommand(storedProcedure, CommandType.StoredProcedure, parameters).ExecuteReader();
        }

        /*Creates the Oracle RefCursor out param*/
        public static OracleParameter CreateCursorParameter(string name)
        {
            OracleParameter prm = new OracleParameter(name, OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            };
            return prm;
        }

        public static OracleParameter CreateVarchar2OutParameter(string name)
        {
            //This is used to handle the oracle runDateTime that is returned from the Oracle procedure.
            //For values that contain Int16 values you have to declare the upper byte value size.
            OracleParameter prm = new OracleParameter(name, OracleDbType.Varchar2, 32767)
            {
                Direction = ParameterDirection.Output
            };
            return prm;
        }

        /*
         * Create this parameter when you want to pass Oracle User-Defined Type (Custom Type) which is table of Oracle User-Defined Types.                  
         * This way you can pass mutiple records at once.
         * 
         * Parameters:
         * name - Name of the UDT Parameter name in the Stored Procedure.
         * oracleUDTName - Name of the Oracle User Defined Type with Schema Name. (Make sure this is all caps. For ex: DESTINY.COMPANYINFOLIST)
         * 
         * */
        public static OracleParameter CreateCustomTypeArrayInputParameter<t>(string name, string oracleUDTName, t value) where t : IOracleCustomType, INullable
        {
            OracleParameter parameter = new OracleParameter
            {
                ParameterName = name,
                OracleDbType = OracleDbType.Array,
                Direction = ParameterDirection.Input,
                UdtTypeName = oracleUDTName,
                Value = value
            };
            return parameter;
        }

        #region IDisposable Members
        //close connection explictly
        public void Dispose()
        {
            if (connection != null)
                connection = null;
        }


        #endregion
    }
}
