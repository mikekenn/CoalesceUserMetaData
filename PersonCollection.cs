using Oracle.DataAccess.Types;
using INullable = Oracle.DataAccess.Types.INullable;
using System;

namespace CoalesceUserMetadata
{
    public class PersonCollection : INullable, IOracleCustomType
    {

        [OracleArrayMapping()]
        public Person[] personArrayADObjects;

        private bool objectIsNull;

        #region INullable Members

        public bool IsNull
        {
            get { return objectIsNull; }
        }

        public static PersonCollection Null
        {
            get
            {
                PersonCollection obj = new PersonCollection
                {
                    objectIsNull = true
                };
                return obj;
            }
        }

        #endregion

        #region IOracleCustomType Members

        public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, 0, personArrayADObjects);
        }

        public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            personArrayADObjects = (Person[])OracleUdt.GetValue(con, pUdt, 0);
        }

        #endregion
    }
}
