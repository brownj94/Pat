using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
//using Pat.MatchFactory;

namespace Pat
{
    // A custom class mapping to an Oracle collection type.
    public class MatchList : IOracleCustomType, INullable
    {
        // The OracleArrayMapping attribute is required to map .NET class member to Oracle collection type.
        [OracleArrayMapping()]
        public string[,] objMatches;

        // A private member indicating whether this object is null.
        private bool ObjectIsNull;

        // Implementation of interface IOracleCustomType method FromCustomObject.
        // Set Oracle collection value from .NET custom type member with OracleArrayMapping attribute.
        public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, 0, objMatches);
        }

        // Implementation of interface IOracleCustomType method ToCustomObject.
        // Set .NET custom type member with OracleArrayMapping attribute from Oracle collection value.
        public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        {
            objMatches = (string[,])OracleUdt.GetValue(con, pUdt, 0);
        }

        // A property of interface INullable. Indicate whether the custom type object is null.
        public bool IsNull
        {
            get { return ObjectIsNull; }
        }

        // Static null property is required to return a null UDT.
        public static MatchList Null
        {
            get
            {
                 MatchList obj = new MatchList();
                obj.ObjectIsNull = true;
                return obj;
            }
        }
    }
}
