using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OuCTP
{
    public class Brokers
    {
        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute]
        public string Name = "";

        /// <summary>
        /// ID
        /// </summary>
        [XmlAttribute]
        public string Id = "";

        /// <summary>
        /// 直连通道
        /// </summary>
        [XmlElement("Line")]
        public List<Lines> lines = new List<Lines>();
    }
}
