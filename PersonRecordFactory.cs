using Oracle.DataAccess.Types;

namespace CoalesceUserMetadata
{
    [OracleCustomTypeMapping("SCHEME.USER_RECORD")]
    public class PersonRecordFactory : IOracleCustomTypeFactory
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PersonRecordFactory");

        #region IOracleCustomTypeFactory Members

        public IOracleCustomType CreateObject()
        {
            return new Person();
        }

        #endregion
    }
}
