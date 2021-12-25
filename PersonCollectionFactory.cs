using Oracle.DataAccess.Types;
using System;

namespace CoalesceUserMetadata
{
    [OracleCustomTypeMapping("SCHEMA.COLLECTIONNAME")]
    public class PersonCollectionFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {

        #region IOracleCustomTypeFactory Members
        public IOracleCustomType CreateObject()
        {
            return new PersonCollection();
        }

        #endregion

        #region IOracleArrayTypeFactory Members
        public Array CreateArray(int numElems)
        {
            return new Person[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }
}
