using System.ComponentModel;

namespace OuCTP
{
    public class AccountInfo
    {
        [DisplayName("投资者帐号")]
        public string AccountID { get; set; }

        [DisplayName("可用资金")]
        public double Available { get; set; }

        [DisplayName("期货结算准备金")]
        public double Balance { get; set; }

        [DisplayName("平仓盈亏")]
        public double CloseProfit { get; set; }

        [DisplayName("手续费")]
        public double Commission { get; set; }

        [DisplayName("经纪公司代码")]
        public string BrokerID { get; set; }

        [DisplayName("资金差额")]
        public double CashIn { get; set; }

        [DisplayName("信用额度")]
        public double Credit { get; set; }

        [DisplayName("当前保证金总额")]
        public double CurrMargin { get; set; }

        [DisplayName("投资者交割保证金")]
        public double DeliveryMargin { get; set; }

        [DisplayName("入金金额")]
        public double Deposit { get; set; }

        [DisplayName("交易所交割保证金")]
        public double ExchangeDeliveryMargin { get; set; }

        [DisplayName("交易所保证金")]
        public double ExchangeMargin { get; set; }

        [DisplayName("冻结的资金")]
        public double FrozenCash { get; set; }

        [DisplayName("冻结的手续费")]
        public double FrozenCommission { get; set; }

        [DisplayName("冻结的保证金")]
        public double FrozenMargin { get; set; }

        [DisplayName("利息收入")]
        public double Interest { get; set; }

        [DisplayName("利息基数")]
        public double InterestBase { get; set; }

        [DisplayName("质押金额")]
        public double Mortgage { get; set; }

        [DisplayName("持仓盈亏")]
        public double PositionProfit { get; set; }

        [DisplayName("上次结算准备金")]
        public double PreBalance { get; set; }

        [DisplayName("上次信用额度")]
        public double PreCredit { get; set; }

        [DisplayName("上次存款额")]
        public double PreDeposit { get; set; }

        [DisplayName("上次占用的保证金")]
        public double PreMargin { get; set; }

        [DisplayName("上次质押金额")]
        public double PreMortgage { get; set; }

        [DisplayName("基本准备金")]
        public double Reserve { get; set; }

        [DisplayName("结算编号")]
        public int SettlementID { get; set; }

        [DisplayName("交易日")]
        public string TradingDay { get; set; }

        [DisplayName("出金金额")]
        public double Withdraw { get; set; }

        [DisplayName("可取资金")]
        public double WithdrawQuota { get; set; }
    }
}
