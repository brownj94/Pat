using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;


namespace Pat
{
    [XmlRoot("rows")]
    
    /// <summary>
    /// Represents elements that live in the "rows" root element
    /// </summary>
    public class Matches
    {

        [XmlElement("rows")]
        public string rows { get; set; }

        [XmlElement("row")]
        public List<row> row { get; set; }

        [XmlElement("stats")]
        public stats stats { get; set; }

    }
    
    /// <summary>
    /// Represents attributes of row element
    /// </summary>   
    public class row
    {
        [XmlAttribute("totalMatches")]
        public string totalMatches { get; set; }

        [XmlElement("column")]
        public List<column> column { get; set; }
    }

    public class column
    {
        [XmlText]
        public string Value { get; set; }

        [XmlAttribute("match")]
        public string match { get; set; }

    }

    /// <summary>
    /// Represents attributes of stats element
    /// </summary>
    public class stats
    {

        [XmlAttribute("totalRows")]
        public string totalRows { get; set; }
        
        [XmlAttribute("columnCount")]
        public string columnCount { get; set; }

        [XmlAttribute("recsWMatches")]
        public string recsWMatches { get; set; }

        [XmlAttribute("recsWExactMatches")]
        public string recsWExactMatches { get; set; }
        
    }
}
