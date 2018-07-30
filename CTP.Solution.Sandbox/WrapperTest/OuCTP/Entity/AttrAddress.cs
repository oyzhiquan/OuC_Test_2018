using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OuCTP
{
    /// <summary>
    /// 地址
    /// </summary>
    public class AttrAddress
    {
        /// <summary>
        /// 地址
        /// </summary>
        [XmlAttribute]
        public string Address = "";
    }
}
