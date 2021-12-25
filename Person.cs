using Oracle.DataAccess.Types;
using System;
using INullable = Oracle.DataAccess.Types.INullable;

namespace CoalesceUserMetadata
{
    public class Person : INullable, IOracleCustomType
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Person");

        private bool objectIsNull;

        [OracleObjectMapping("CN")]
        public string cn { get; set; }

        [OracleObjectMappingAttribute("AD_GIVEN_NAME")]
        public string adGivenName { get; set; }

        [OracleObjectMappingAttribute("AD_SURNAME")]
        public string adSurname { get; set; }

        [OracleObjectMappingAttribute("AD_VOICETELEPHONENUMBER")]
        public string adVoiceTelephoneNumber { get; set; }

        [OracleObjectMappingAttribute("AD_EMAILADDRESS")]
        public string adEmailAddress { get; set; }

        [OracleObjectMappingAttribute("DESCRIPTION")]
        public string Description { get; set; }

        [OracleObjectMappingAttribute("DEPARTMENT")]
        public string Department { get; set; }

        [OracleObjectMappingAttribute("AD_PHYSICALLOCATION")]
        public string adPhysicalLocation { get; set; }

        [OracleObjectMappingAttribute("AD_FAXNUMBER")]
        public string adFaxnumber { get; set; }

        [OracleObjectMappingAttribute("AD_ORGANIZATIONALUNIT")]
        public string adOrganizationalunit { get; set; }

        [OracleObjectMappingAttribute("AD_DISPLAYNAME")]
        public string adDisplayname { get; set; }

        [OracleObjectMappingAttribute("AD_WHENCREATED")]
        public string adWhenCreated { get; set; }

        public static Person Null
        {
            get
            {
                Person person = new Person
                {
                    objectIsNull = true
                };
                return person;
            }
        }

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            try
            {
                // Convert from the Custom Type to Oracle Object
                if (!string.IsNullOrEmpty(cn))
                {
                    OracleUdt.SetValue(con, pUdt, "CN", cn);
                }
                if (!string.IsNullOrEmpty(adGivenName))
                {
                    OracleUdt.SetValue(con, pUdt, "AD_GIVEN_NAME", adGivenName);
                }
                if (!string.IsNullOrEmpty(adSurname))
                {
                    OracleUdt.SetValue(con, pUdt, "AD_SURNAME", adSurname);
                }
                if (!string.IsNullOrEmpty(adVoiceTelephoneNumber))
                {
                    OracleUdt.SetValue(con, pUdt, "AD_VOICETELEPHONENUMBER", adVoiceTelephoneNumber);
                }
                if (!string.IsNullOrEmpty(adEmailAddress))
                {
                    OracleUdt.SetValue(con, pUdt, "AD_EMAILADDRESS", adEmailAddress);
                }
                if (!string.IsNullOrEmpty(Description))
                {
                    OracleUdt.SetValue(con, pUdt, "DESCRIPTION", Description);
                }
                if (!string.IsNullOrEmpty(Department))
                {
                    OracleUdt.SetValue(con, pUdt, "DEPARTMENT", Department);
                }
                if (!string.IsNullOrEmpty(adPhysicalLocation))
                {
                    OracleUdt.SetValue(con, pUdt, "AD_PHYSICALLOCATION", adPhysicalLocation);
                }
                if (!string.IsNullOrEmpty(adFaxnumber))
                {
                    OracleUdt.SetValue(con, pUdt, "AD_FAXNUMBER", adFaxnumber);
                }
                if (!string.IsNullOrEmpty(adOrganizationalunit))
                {
                    OracleUdt.SetValue(con, pUdt, "AD_ORGANIZATIONALUNIT", adOrganizationalunit);
                }
                if (!string.IsNullOrEmpty(adDisplayname))
                {
                    OracleUdt.SetValue(con, pUdt, "AD_DISPLAYNAME", adDisplayname);
                }
                if (!string.IsNullOrEmpty(adWhenCreated))
                {
                    OracleUdt.SetValue(con, pUdt, "AD_WHENCREATED", adWhenCreated);
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }

        }

        public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            try
            {
                cn = (string)OracleUdt.GetValue(con, pUdt, "CN");
                adGivenName = (string)OracleUdt.GetValue(con, pUdt, "AD_GIVEN_NAME");
                adSurname = (string)OracleUdt.GetValue(con, pUdt, "AD_SURNAME");
                adVoiceTelephoneNumber = (string)OracleUdt.GetValue(con, pUdt, "AD_VOICETELEPHONENUMBER");
                adEmailAddress = (string)OracleUdt.GetValue(con, pUdt, "AD_EMAILADDRESS");
                Description = (string)OracleUdt.GetValue(con, pUdt, "DESCRIPTION");
                Department = (string)OracleUdt.GetValue(con, pUdt, "DEPARTMENT");
                adPhysicalLocation = (string)OracleUdt.GetValue(con, pUdt, "AD_PHYSICALLOCATION");
                adFaxnumber = (string)OracleUdt.GetValue(con, pUdt, "AD_FAXNUMBER");
                adOrganizationalunit = (string)OracleUdt.GetValue(con, pUdt, "AD_ORGANIZATIONALUNIT");
                adDisplayname = (string)OracleUdt.GetValue(con, pUdt, "AD_DISPLAYNAME");
            }
            catch (Exception e)
            {
                log.Error(e);
                throw;
            }
        }

    }
}
