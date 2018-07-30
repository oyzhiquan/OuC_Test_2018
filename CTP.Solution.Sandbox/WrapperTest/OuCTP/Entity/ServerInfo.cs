using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OuCTP
{
    [XmlRoot("root")]
    public class ServerInfo
    {
        /// <summary>
        /// 通道
        /// </summary>
        [XmlElement("Brokers")]
        public List<Brokers> broker = new List<Brokers>();
    }
}
