using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OuCTP
{
    /// <summary>
    /// 直连
    /// </summary>
    public class Lines
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
        public string ID = "";

        /// <summary>
        /// 行情地址
        /// </summary>
        [XmlElement("Market")]
        public List<AttrAddress> Markets = new List<AttrAddress>();

        /// <summary>
        /// 交易主机
        /// </summary>
        [XmlElement("Trade")]
        public List<AttrAddress> Trades = new List<AttrAddress>();
    }
}
