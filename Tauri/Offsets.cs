using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tauri
{
       
    [XmlRoot("Offsets")]
    public class Offsets
    {
        [XmlElement("Offset")]
        public List<Offset> Offset { get; set; }
    }


    
    public class Offset
    {
        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("value")]
        public string value { get; set; }
    }

}
