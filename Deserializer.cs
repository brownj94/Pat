using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pat
{
    class Deserializer
    {
        /// <summary>
        /// take in string and turn into objects found in the Matches class
        /// </summary>    
        public static Matches deserialize(string xmlstring, Matches p_mcol)
        {

            XmlSerializer deserializer = new XmlSerializer(typeof(Matches));

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlstring)))
            {
                try
                {
                    p_mcol = (Matches)deserializer.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }

            }

            return p_mcol;
        }
    }
}
