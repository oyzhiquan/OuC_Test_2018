using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTP;

namespace OuCTP
{
    public class InstrumentBaseInfo
    {
        // 摘要: 
        //     创建日
        public string CreateDate { get;set;}
        //
        // 摘要: 
        //     交割月
        public int DeliveryMonth { get;set;}
        //
        // 摘要: 
        //     交割年份
        public int DeliveryYear { get;set;}
        //
        // 摘要: 
        //     结束交割日
        public string EndDelivDate { get;set;}
        //
        // 摘要: 
        //     交易所代码
        public string ExchangeID { get;set;}
        //
        // 摘要: 
        //     合约在交易所的代码
        public string ExchangeInstID { get;set;}
        //
        // 摘要: 
        //     到期日
        public string ExpireDate { get;set;}
        //
        // 摘要: 
        //     合约生命周期状态
        public EnumInstLifePhaseType InstLifePhase { get;set;}
        //
        // 摘要: 
        //     合约代码
        public string InstrumentID { get;set;}
        //
        // 摘要: 
        //     合约名称
        public string InstrumentName { get;set;}
        //
        // 摘要: 
        //     当前是否交易
        public int IsTrading { get;set;}
        //
        // 摘要: 
        //     多头保证金率
        public double LongMarginRatio { get;set;}
        //
        // 摘要: 
        //     限价单最大下单量
        public int MaxLimitOrderVolume { get;set;}
        //
        // 摘要: 
        //     市价单最大下单量
        public int MaxMarketOrderVolume { get;set;}
        //
        // 摘要: 
        //     限价单最小下单量
        public int MinLimitOrderVolume { get;set;}
        //
        // 摘要: 
        //     市价单最小下单量
        public int MinMarketOrderVolume { get;set;}
        //
        // 摘要: 
        //     上市日
        public string OpenDate { get;set;}
        //
        // 摘要: 
        //     持仓日期类型
        public EnumPositionDateTypeType PositionDateType { get;set;}
        //
        // 摘要: 
        //     持仓类型
        public EnumPositionTypeType PositionType { get;set;}
        //
        // 摘要: 
        //     最小变动价位
        public double PriceTick { get;set;}
        //
        // 摘要: 
        //     产品类型
        public EnumProductClassType ProductClass { get;set;}
        //
        // 摘要: 
        //     产品代码
        public string ProductID { get;set;}
        //
        // 摘要: 
        //     空头保证金率
        public double ShortMarginRatio { get;set;}
        //
        // 摘要: 
        //     开始交割日
        public string StartDelivDate { get;set;}
        //
        // 摘要: 
        //     合约数量乘数
        public int VolumeMultiple { get;set;}
    }
}
