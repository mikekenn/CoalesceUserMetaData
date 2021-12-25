using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Text;

namespace CoalesceUserMetadata
{
    class PersonInfoInterface
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PersonInfoInterface");

        #region Constants
        static readonly NameValueCollection section = ConfigurationManager.GetSection("PersonInfoInterface") as NameValueCollection;
        private static readonly string procName = section["OracleProcedureName"];

        static readonly NameValueCollection DBsection = ConfigurationManager.GetSection("DBAccess") as NameValueCollection;
        private static readonly string DBConnectionString = DBsection["DBConnectionString"];

        #endregion

        public static string UpdatePersonTable(List<Person> PersonList)
        {
            string oracleRunDateTime = string.Empty;

            PersonCollection PersonCollection = new PersonCollection
            {
                personArrayADObjects = PersonList.ToArray()
            };

            using (DBAccess context = new DBAccess())
            {
                try
                {
                    OracleParameter[] parameters = new OracleParameter[2];

                    parameters[0] = DBAccess.CreateCustomTypeArrayInputParameter<PersonCollection>("pCollection", section["OracleUDTPersonRecordCollectionName"], PersonCollection);

                    parameters[1] = DBAccess.CreateVarchar2OutParameter("currDataTime");

                    using (OracleDataReader dr = context.GetDataReader(procName, parameters))
                    {
                        if (!parameters[1].IsNullable)
                        {
                            oracleRunDateTime = parameters[1].Value.ToString();//.Substring(0,22);
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Error(e);
                    GetErrorLogInformation(oracleRunDateTime.Substring(0, 16));
                    Environment.Exit(1);

                }

                return oracleRunDateTime;
            }
        }

        public static void GetErrorLogInformation(string p_date)
        {
            string errMsg = "";
            string errCN = "";
            string errAD_GivenName = "";
            string errAD_Surname = "";
            string errAD_VoiceTelephoneNum = "";
            string errAD_EmailAddress = "";
            string errAD_Description = "";
            string errAD_Department = "";
            string errAD_PhysicalLocation = "";
            string errAD_AD_FAXNUMBER = "";
            string errAD_ORGANIZATIONALUNIT = "";
            string errAD_DISPLAYNAME = "";

            OracleConnection con = new OracleConnection
            {
                ConnectionString = DBConnectionString,
            };
            con.Open();

            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = section["PackageName"] + "." + section["GetErrorLogInformationProcName"];

            cmd.Parameters.Add("p_date", OracleDbType.Varchar2, ParameterDirection.Input).Value = p_date;

            cmd.Parameters.Add("resultset", OracleDbType.RefCursor, ParameterDirection.Output);

            OracleDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                errMsg = rdr.OraclePropertyValue(0);
                errCN = rdr.OraclePropertyValue(1);
                errAD_GivenName = rdr.OraclePropertyValue(2);
                errAD_Surname = rdr.OraclePropertyValue(3);
                errAD_VoiceTelephoneNum = rdr.OraclePropertyValue(4);
                errAD_EmailAddress = rdr.OraclePropertyValue(5);
                errAD_Description = rdr.OraclePropertyValue(6);
                errAD_Department = rdr.OraclePropertyValue(7);
                errAD_PhysicalLocation = rdr.OraclePropertyValue(8);
                errAD_AD_FAXNUMBER = rdr.OraclePropertyValue(9);
                errAD_ORGANIZATIONALUNIT = rdr.OraclePropertyValue(10);
                errAD_DISPLAYNAME = rdr.OraclePropertyValue(11);


                StringBuilder sb = new StringBuilder();

                sb.Append($"Error msg = {errMsg}");
                sb.AppendLine($"Cn = {errCN}");
                sb.AppendLine($"Given Name = {errAD_GivenName}");
                sb.AppendLine($"Surname = {errAD_Surname}");
                sb.AppendLine($"TelePhone = {errAD_VoiceTelephoneNum}");
                sb.AppendLine($"Fax Number: = {errAD_AD_FAXNUMBER}");
                sb.AppendLine($"Email Address = {errAD_EmailAddress}");
                sb.AppendLine($"Description = {errAD_Description}");
                sb.AppendLine($"Department = {errAD_Department}");
                sb.AppendLine($"Physical Location = {errAD_PhysicalLocation}");
                sb.AppendLine($"Organizational Unit = {errAD_ORGANIZATIONALUNIT}");
                sb.AppendLine($"Display Name = {errAD_DISPLAYNAME}");

                log.Error(sb);
            }

            rdr.Close();
            cmd.Dispose();
            con.Close();
        }

        public static void GetChangeLogInformation(string p_date)
        {
            string Date_Time;
            string Action = "";
            string CN = "";
            string Column_Val = "";
            string Old_Val = "";
            string New_Val = "";

            OracleConnection con = new OracleConnection
            {
                ConnectionString = DBConnectionString
            };
            con.Open();

            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = section["PackageName"] + "." + section["GetChangeLogInformationProcName"];

            cmd.Parameters.Add("p_date", OracleDbType.Varchar2, ParameterDirection.Input).Value = p_date;

            OracleParameter out_param = new OracleParameter("resultset", OracleDbType.RefCursor, ParameterDirection.Output);
            cmd.Parameters.Add(out_param);

            // get a data reader
            OracleDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Date_Time = rdr.OraclePropertyValue(0).ToString();
                Action = rdr.OraclePropertyValue(1);
                CN = rdr.OraclePropertyValue(2);
                Column_Val = rdr.OraclePropertyValue(3);
                Old_Val = rdr.OraclePropertyValue(4);
                New_Val = rdr.OraclePropertyValue(5);
                Console.WriteLine($"Date = {Date_Time}\n" +
                    $"Action = {Action}\n" +
                    $"Cn = {CN}\n" +
                    $"Column Name = {Column_Val}\n" +
                    $"Old Value = {Old_Val}\n" +
                    $"New Value = {New_Val}\n\n");
            }

            rdr.Close();
            cmd.Dispose();
        }

        public static int GetErrorLogCount(string p_date)
        {

            OracleConnection con = new OracleConnection
            {
                ConnectionString = DBConnectionString
            };
            con.Open();

            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = section["PackageName"] + "." + section["GetErrorLogCountProcName"];
            cmd.Parameters.Add("numOfErrorLogs", OracleDbType.Decimal, ParameterDirection.ReturnValue);
            cmd.Parameters.Add("p_date", OracleDbType.Varchar2, ParameterDirection.Input).Value = p_date;

            cmd.ExecuteNonQuery();

            int numOfErrors = (int)(OracleDecimal)cmd.Parameters["numOfErrorLogs"].Value;

            con.Close();
            cmd.Dispose();

            return numOfErrors;
        }

        public static int GetNewRecordCount(string p_date)
        {

            OracleConnection con = new OracleConnection
            {
                ConnectionString = DBConnectionString
            };
            con.Open();

            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = section["PackageName"] + "." + section["GetNewRecordCountProcName"];
            cmd.Parameters.Add("return_value", OracleDbType.Decimal, ParameterDirection.ReturnValue);
            cmd.Parameters.Add("p_date", OracleDbType.Varchar2, ParameterDirection.Input).Value = p_date;

            cmd.ExecuteNonQuery();

            int numOfNewRecords = (int)(OracleDecimal)cmd.Parameters["return_value"].Value;

            con.Close();
            cmd.Dispose();

            return numOfNewRecords;
        }

        public static int GetUpdatedRecordCount(string p_date)
        {

            OracleConnection con = new OracleConnection
            {
                ConnectionString = DBConnectionString
            };
            con.Open();

            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = section["PackageName"] + "." + section["GetUpdatedRecordCountProcName"];
            cmd.Parameters.Add("return_value", OracleDbType.Decimal, ParameterDirection.ReturnValue);
            cmd.Parameters.Add("p_date", OracleDbType.Varchar2, ParameterDirection.Input).Value = p_date;

            cmd.ExecuteNonQuery();

            int numOfNewRecords = (int)(OracleDecimal)cmd.Parameters["return_value"].Value;

            con.Close();
            cmd.Dispose();

            return numOfNewRecords;
        }

        public static int GetDeletedRecordCount(string p_date)
        {

            OracleConnection con = new OracleConnection
            {
                ConnectionString = DBConnectionString
            };
            con.Open();

            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = section["PackageName"] + "." + section["GetDeletedRecordCountProcName"];
            cmd.Parameters.Add("return_value", OracleDbType.Decimal, ParameterDirection.ReturnValue);
            cmd.Parameters.Add("p_date", OracleDbType.Varchar2, ParameterDirection.Input).Value = p_date.Substring(0, 16);

            cmd.ExecuteNonQuery();

            int numOfDeletedRecords = (int)(OracleDecimal)cmd.Parameters["return_value"].Value;

            con.Close();
            cmd.Dispose();

            return numOfDeletedRecords;
        }

    }
}
