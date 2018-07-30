using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CTP;

namespace OuCTP
{
    public class TraderAdapter : CTPTraderAdapter
    {
        #region 字段及属性
        public static int RequestId = 1;
        public static int CurrentOrderRef;

        private string _tradingDay;

        public string TradingDay
        {
            get { return _tradingDay; }
            set { _tradingDay = value; }
        }

        private int _frontId;

        public int FrontId
        {
            get { return _frontId; }
            set { _frontId = value; }
        }

        private int _sessionId;

        public int SessionId
        {
            get { return _sessionId; }
            set { _sessionId = value; }
        }

        private string _brokerId;

        public string BrokerId
        {
            get { return _brokerId; }
            set { _brokerId = value; }
        }

        private string _investorId;

        public string InvestorId
        {
            get { return _investorId; }
            set { _investorId = value; }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string[] _front;

        public string[] Front
        {
            get { return _front; }
            set { _front = value; }
        }


        private ConcurrentDictionary<string, ThostFtdcInvestorPositionField> _positionFields =
            new ConcurrentDictionary<string, ThostFtdcInvestorPositionField>();

        public ConcurrentDictionary<string, ThostFtdcInvestorPositionField> PositionFields
        {
            get { return _positionFields; }
            set { _positionFields = value; }
        }

        private ConcurrentDictionary<string, ThostFtdcOrderField> _unfinishedOrderFields =
            new ConcurrentDictionary<string, ThostFtdcOrderField>();

        public ConcurrentDictionary<string, ThostFtdcOrderField> UnFinishedOrderFields
        {
            get { return _unfinishedOrderFields; }
            set { _unfinishedOrderFields = value; }
        }
        #endregion

        #region 初始化
        public TraderAdapter()
        {
            OnFrontConnected += TraderAdapter_OnFrontConnected;
            OnRspUserLogin += TraderAdapter_OnRspUserLogin;
            OnRspSettlementInfoConfirm += TraderAdapter_OnRspSettlementInfoConfirm;
            OnRspQryInstrument += TraderAdapter_OnRspQryInstrument;
            OnRspQryTradingAccount += TraderAdapter_OnRspQryTradingAccount;
            OnRspQryInvestorPositionDetail += TraderAdapter_OnRspQryInvestorPositionDetail;
            OnRspQryInvestorPosition += TraderAdapter_OnRspQryInvestorPosition;
            OnRtnOrder += TraderAdapter_OnRtnOrder;
            OnErrRtnOrderInsert += TraderAdapter_OnErrRtnOrderInsert;
            OnRspOrderAction += TraderAdapter_OnRspOrderAction;
            OnRspQryDepthMarketData += TraderAdapter_OnRspQryDepthMarketData;
            OnFrontDisconnected += TraderAdapter_OnFrontDisconnected;
            OnRspOrderInsert += TraderAdapter_OnRspOrderInsert;
            OnErrRtnOrderAction += TraderAdapter_OnErrRtnOrderAction;
            OnHeartBeatWarning += TraderAdapter_OnHeartBeatWarning;
            OnRspError += TraderAdapter_OnRspError;
            OnRtnTrade += TraderAdapter_OnRtnTrade;
            OnRspUserLogout += TraderAdapter_OnRspUserLogout;
            OnRspQryOrder += TraderAdapter_OnRspQryOrder;
            OnRspQryTrade += TraderAdapter_OnRspQryTrade;
        }
        #endregion     

        #region 响应及回报
        private void TraderAdapter_OnFrontConnected()
        {
            Login();
        }

        private void TraderAdapter_OnRspUserLogin(ThostFtdcRspUserLoginField pRspUserLogin,
                ThostFtdcRspInfoField pRspInfo, int nRequestId, bool bIsLast)
        {
            try
            {
                if (MyUtils.IsWrongRspInfo(pRspInfo))
                {
                    MyUtils.ReportError(pRspInfo, "登录回报错误");
                    return;
                }
                
                if (bIsLast)
                {
                    var temp =
                        string.Format(
                            "登录回报:经纪公司代码:{0},郑商所时间:{1},大商所时间:{2},中金所时间:{3},前置编号:{4},登录成功时间:{5},最大报单引用:{6},会话编号:{7},上期所时间:{8},交易系统名称:{9},交易日:{10},用户代码:{11}",
                            pRspUserLogin.BrokerID, pRspUserLogin.CZCETime, pRspUserLogin.DCETime,
                            pRspUserLogin.FFEXTime,
                            pRspUserLogin.FrontID, pRspUserLogin.LoginTime, pRspUserLogin.MaxOrderRef,
                            pRspUserLogin.SessionID, pRspUserLogin.SHFETime, pRspUserLogin.SystemName,
                            pRspUserLogin.TradingDay, pRspUserLogin.UserID);

                    _frontId = pRspUserLogin.FrontID;
                    _sessionId = pRspUserLogin.SessionID;
                    _tradingDay = pRspUserLogin.TradingDay;

                    if (string.IsNullOrEmpty(pRspUserLogin.MaxOrderRef))
                    {
                        CurrentOrderRef = 0;
                    }
                    else
                    {
                        CurrentOrderRef = Convert.ToInt32(pRspUserLogin.MaxOrderRef);
                    }

                    //比较交易所时间和本地时间的偏移值
                    DateTime shfeTime;
                    try
                    {
                        shfeTime = Convert.ToDateTime(pRspUserLogin.SHFETime);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            shfeTime = Convert.ToDateTime(pRspUserLogin.CZCETime);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                shfeTime = Convert.ToDateTime(pRspUserLogin.DCETime);
                            }
                            catch (Exception)
                            {
                                throw new Exception("交易所时间格式不正确");
                            }

                        }
                    }

                    MyUtils.ExchangeTimeOffset = ExchangeTime.Instance.GetSecFromDateTime(shfeTime) -
                                               ExchangeTime.Instance.GetSecFromDateTime(DateTime.Now);
                    Thread.Sleep(1000);
                    ReqSettlementInfoConfirm(); //结算确认
                    MyUtils.IsTraderReady = true;
                }
                else
                {
                    MessageBox.Show("登录失败");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("登录失败," + ex.Message);
            }
        }

        void TraderAdapter_OnRspQryOrder(ThostFtdcOrderField pOrder, ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {

        }

        void TraderAdapter_OnRspUserLogout(ThostFtdcUserLogoutField pUserLogout, ThostFtdcRspInfoField pRspInfo,
            int nRequestID, bool bIsLast)
        {
            MyUtils.IsTraderReady = false;
        }

        private void TraderAdapter_OnRtnTrade(ThostFtdcTradeField pTrade)
        {
            
        }

        private void TraderAdapter_OnRspQryTrade(ThostFtdcTradeField pTrade, ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {

        }

        private void TraderAdapter_OnRspError(ThostFtdcRspInfoField pRspInfo, int nRequestId, bool bIsLast)
        {

        }

        private void TraderAdapter_OnHeartBeatWarning(int nTimeLapse)
        {

        }

        private void TraderAdapter_OnErrRtnOrderAction(ThostFtdcOrderActionField pOrderAction,
            ThostFtdcRspInfoField pRspInfo)
        {
           
        }

        private void TraderAdapter_OnRspOrderInsert(ThostFtdcInputOrderField pInputOrder, ThostFtdcRspInfoField pRspInfo,
            int nRequestId, bool bIsLast)
        {

        }

        private void TraderAdapter_OnFrontDisconnected(int nReason)
        {
            MyUtils.IsTraderReady = false;
        }

        private void TraderAdapter_OnRspQryDepthMarketData(ThostFtdcDepthMarketDataField pDepthMarketData,
            ThostFtdcRspInfoField pRspInfo, int nRequestId, bool bIsLast)
        {

        }

        private void TraderAdapter_OnRspOrderAction(ThostFtdcInputOrderActionField pInputOrderAction,
    ThostFtdcRspInfoField pRspInfo, int nRequestId, bool bIsLast)
        {

        }

        /// <summary>
        /// 交易所发回的报单错误
        /// </summary>
        /// <param name="pInputOrder"></param>
        /// <param name="pRspInfo"></param>
        private void TraderAdapter_OnErrRtnOrderInsert(ThostFtdcInputOrderField pInputOrder,
            ThostFtdcRspInfoField pRspInfo)
        {

        }

        private void TraderAdapter_OnRtnOrder(ThostFtdcOrderField pOrder)
        {

        }

        //查询返回时，昨仓和今仓会分开返回，所以要分别判断时间，取YDPosition或TodayPosition，Position是总持仓
        private void TraderAdapter_OnRspQryInvestorPosition(ThostFtdcInvestorPositionField pInvestorPosition,
            ThostFtdcRspInfoField pRspInfo, int nRequestId, bool bIsLast)
        {

        }

        private void TraderAdapter_OnRspQryInvestorPositionDetail(
            ThostFtdcInvestorPositionDetailField pInvestorPositionDetail, ThostFtdcRspInfoField pRspInfo,
            int nRequestId, bool bIsLast)
        {

        }

        //资金查询响应
        private void TraderAdapter_OnRspQryTradingAccount(ThostFtdcTradingAccountField pTradingAccount,
            ThostFtdcRspInfoField pRspInfo, int nRequestId, bool bIsLast)
        {
            try
            {
                if (pTradingAccount != null)
                {
                    //查询到的资金信息保存到文件中
                    var temp =
                        string.Format(
                            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29}",
                            pTradingAccount.TradingDay, pTradingAccount.AccountID, pTradingAccount.Available,
                            pTradingAccount.Balance,
                            pTradingAccount.BrokerID, pTradingAccount.CashIn, pTradingAccount.CloseProfit,
                            pTradingAccount.Commission, pTradingAccount.Credit, pTradingAccount.CurrMargin,
                            pTradingAccount.DeliveryMargin, pTradingAccount.Deposit,
                            pTradingAccount.ExchangeDeliveryMargin, pTradingAccount.ExchangeMargin,
                            pTradingAccount.FrozenCash, pTradingAccount.FrozenCommission, pTradingAccount.FrozenMargin,
                            pTradingAccount.Interest, pTradingAccount.InterestBase, pTradingAccount.Mortgage,
                            pTradingAccount.PositionProfit, pTradingAccount.PreBalance, pTradingAccount.PreCredit,
                            pTradingAccount.PreDeposit, pTradingAccount.PreMargin, pTradingAccount.PreMortgage,
                            pTradingAccount.Reserve, pTradingAccount.SettlementID, pTradingAccount.Withdraw,
                            pTradingAccount.WithdrawQuota);

                    var moneyFile = MyUtils.AssemblyPath + "money.csv";
                    try
                    {
                        var dicMoney = new Dictionary<string, string>();

                        if (File.Exists(moneyFile))
                        {
                            var sr = new StreamReader(moneyFile, Encoding.UTF8);
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                dicMoney[line] = line;
                            }
                            sr.Close();

                            dicMoney[temp] = temp;

                            File.Delete(moneyFile);

                            var sw = new StreamWriter(moneyFile, true, Encoding.UTF8);
                            foreach (var s in dicMoney)
                            {
                                sw.WriteLine(s.Value);
                            }

                            sw.Close();
                        }
                        else
                        {
                            const string title =
                                "交易日,投资者帐号,可用资金,期货结算准备金,经纪公司代码,资金差额,平仓盈亏,手续费,信用额度,当前保证金总额,投资者交割保证金,入金金额,交易所交割保证金,交易所保证金,冻结的资金,冻结的手续费,冻结的保证金,利息收入,利息基数,质押金额,持仓盈亏,上次结算准备金,上次信用额度,上次存款额,上次占用的保证金,上次质押金额,基本准备金,结算编号,出金金额,可取资金";

                            var sw = new StreamWriter(moneyFile, true, Encoding.UTF8);
                            sw.WriteLine(title);
                            sw.WriteLine(temp);
                            sw.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorMsg = ex.Message + ex.Source + ex.StackTrace;
                        MessageBox.Show(string.Format("错误:处理{0}出现异常，异常信息：{1}", moneyFile, errorMsg));
                    }
                    MyUtils.UpdateAccountInfo(pTradingAccount);
                }

                if (bIsLast)
                {
                    Thread.Sleep(1000);
                    ReqQryInvestorPositionDetail();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询资金回报异常：" + ex.Message);
            }
        }

        private void TraderAdapter_OnRspSettlementInfoConfirm(
            ThostFtdcSettlementInfoConfirmField pSettlementInfoConfirm,
            ThostFtdcRspInfoField pRspInfo, int nRequestId, bool bIsLast)
        {
            try
            {
                if (MyUtils.IsWrongRspInfo(pRspInfo))
                {
                    MyUtils.ReportError(pRspInfo, "结算信息确认错误");
                    return;
                }
                if (pSettlementInfoConfirm != null)
                {
                    var temp = string.Format("结算信息确认回报:经纪公司代码:{0},确认日期:{1},确认时间:{2},投资者代码:{3}",
                        pSettlementInfoConfirm.BrokerID, pSettlementInfoConfirm.ConfirmDate,
                        pSettlementInfoConfirm.ConfirmTime, pSettlementInfoConfirm.InvestorID);
                }

                if (bIsLast)
                {
                    Thread.Sleep(1000);
                    ReqQryAllInstruments(); //查询所有合约
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("结算确认发生异常，异常信息：" + ex.Message);
            }
        }

        private void TraderAdapter_OnRspQryInstrument(ThostFtdcInstrumentField pInstrument,
            ThostFtdcRspInfoField pRspInfo, int nRequestId, bool bIsLast)
        {
            try
            {
                if (MyUtils.IsWrongRspInfo(pRspInfo))
                {
                    MyUtils.ReportError(pRspInfo, "查询合约错误");
                    return;
                }
 
                if (pInstrument != null)
                {
                    if (!pInstrument.InstrumentID.Contains("efp") && !pInstrument.InstrumentID.Contains("eof"))
                    {
                        if (pInstrument.InstrumentID.Contains("&"))
                        {
                            //套利合约
                            MyUtils.ComboInstrumentInfo[pInstrument.InstrumentID] = pInstrument;
                        }
                        else
                        {
                            //标准
                            MyUtils.StandardInstrumentInfo[pInstrument.InstrumentID] = pInstrument;
                        }
                        MyUtils.CategoryToExchangeId[MyUtils.GetInstrumentCategory(pInstrument.InstrumentID)] =
                            pInstrument.ExchangeID;

                        var temp =
                            string.Format(
                                "查询合约回报: 创建日:{0},交割月:{1},交割年份:{2},结束交割日:{3},交易所代码:{4},合约在交易所的代码:{5},到期日:{6},合约生命周期状态:{7},合约代码:{8},合约名称:{9},当前是否交易:{10},多头保证金率:{11},限价单最大下单量:{12},市价单最大下单量:{13},限价单最小下单量:{14},市价单最小下单量:{15},上市日:{16},持仓日期类型:{17},持仓类型:{18},最小变动价位:{19},产品类型:{20},产品代码:{21},空头保证金率:{22},开始交割日:{23},合约数量乘数:{24}",
                                pInstrument.CreateDate, pInstrument.DeliveryMonth, pInstrument.DeliveryYear,
                                pInstrument.EndDelivDate, pInstrument.ExchangeID, pInstrument.ExchangeInstID,
                                pInstrument.ExpireDate, pInstrument.InstLifePhase, pInstrument.InstrumentID,
                                pInstrument.InstrumentName, pInstrument.IsTrading, pInstrument.LongMarginRatio,
                                pInstrument.MaxLimitOrderVolume, pInstrument.MaxMarketOrderVolume,
                                pInstrument.MinLimitOrderVolume, pInstrument.MinMarketOrderVolume, pInstrument.OpenDate,
                                pInstrument.PositionDateType, pInstrument.PositionType, pInstrument.PriceTick,
                                pInstrument.ProductClass, pInstrument.ProductID, pInstrument.ShortMarginRatio,
                                pInstrument.StartDelivDate, pInstrument.VolumeMultiple);
                    }
                }

                if (bIsLast)
                {
                    Thread.Sleep(1000);
                    ReqQryTradingAccount();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询合约出错：" + ex.Message);
            }
        }
        #endregion

        #region 请求

        public void Connect()
        {
            //SubscribePublicTopic(EnumTeResumeType.THOST_TERT_RESTART); // 注册公有流
            //SubscribePrivateTopic(EnumTeResumeType.THOST_TERT_RESTART); // 注册私有流
            SubscribePublicTopic(EnumTeResumeType.THOST_TERT_QUICK); // 注册公有流
            SubscribePrivateTopic(EnumTeResumeType.THOST_TERT_QUICK); // 注册私有流
            foreach (var server in _front)
            {
                RegisterFront(server);
            }
            Init();
            Join();
        }

        private void ReqQryInvestorPosition()
        {
            var req = new ThostFtdcQryInvestorPositionField {BrokerID = BrokerId, InvestorID = InvestorId};
            int iResult = ReqQryInvestorPosition(req, RequestId++);
        }

        private void ReqQryOrder()
        {
            var req = new ThostFtdcQryOrderField { BrokerID = BrokerId, InvestorID = InvestorId };
            int iResult = ReqQryOrder(req, RequestId++);
        }

        public int ReqOrderAction(int frontId, int sessionId, string orderRef, string instrumentId, EnumActionFlagType actionFlag = EnumActionFlagType.Delete)
        {
            var req = new ThostFtdcInputOrderActionField
            {
                ActionFlag = actionFlag,
                BrokerID = _brokerId,
                InvestorID = _investorId,
                FrontID = frontId,
                SessionID = sessionId,
                OrderRef = orderRef,
                InstrumentID = instrumentId
            };

            int ret = ReqOrderAction(req, ++RequestId);

            return ret;
        }

        public int ReqOrderAction(string exchangeId, string orderSysId, string instrumentId, EnumActionFlagType actionFlag = EnumActionFlagType.Delete)
        {
            var req = new ThostFtdcInputOrderActionField
            {
                ActionFlag = actionFlag,
                ExchangeID = exchangeId,
                OrderSysID = orderSysId,
                InstrumentID = instrumentId
            };

            int ret = ReqOrderAction(req, ++RequestId);

            return ret;
        }

        private void ReqQryInvestorPositionDetail()
        {
            var req = new ThostFtdcQryInvestorPositionDetailField { BrokerID = BrokerId, InvestorID = InvestorId };
            var iResult = ReqQryInvestorPositionDetail(req, RequestId++);
        }

        private void ReqQryTradingAccount()
        {
            var req = new ThostFtdcQryTradingAccountField
            {
                BrokerID = BrokerId,
                InvestorID = InvestorId
            };

            var iResult = ReqQryTradingAccount(req, RequestId++);
        }

        private void ReqQryAllInstruments()
        {
            var req = new ThostFtdcQryInstrumentField();
            var iResult = ReqQryInstrument(req, RequestId++);
        }

        public void ReqSettlementInfoConfirm()
        {
            var req = new ThostFtdcSettlementInfoConfirmField
            {
                BrokerID = _brokerId,
                InvestorID = _investorId
            };

            var iResult = ReqSettlementInfoConfirm(req, RequestId++);
        }

        public int ReqOrderInsert(string instrumentId, EnumDirectionType direction, double price, int nVolume,
            EnumOffsetFlagType openOrClose, EnumTimeConditionType timeCondition,
            EnumVolumeConditionType volumeCondition, string reason)
        {
            return 0;
        }

        public void Login()
        {
            var loginField = new ThostFtdcReqUserLoginField
            {
                BrokerID = _brokerId,
                UserID = _investorId,
                Password = _password,
                UserProductInfo = "MyClient"
            };


            ReqUserLogin(loginField, RequestId++);
        }

        #endregion

        #region 内部方法
        public void CreateNewTrader()
        {
            Task.Run(() =>
            {
                Thread.Sleep(5000);
                var newTrader = new TraderAdapter
                {
                    BrokerId = BrokerId,
                    InvestorId = InvestorId,
                    Password = Password,
                    Front = Front
                };

                ((QuoteAdapter)MyUtils.QuoteMain).Trader = newTrader;

                newTrader.Connect();
            });
        }

        /// <summary>
        /// 根据委托撤单信息构成键,ExchangeID + OrderSysID
        /// </summary>
        /// <param name="pOrder"></param>
        /// <returns></returns>
        private string GetOrderKey(ThostFtdcOrderField pOrder)
        {
            return string.Format("{0}:{1}", pOrder.ExchangeID, pOrder.OrderSysID);
        }

        private string GetOrderKey(ThostFtdcTradeField pTrade)
        {
            return string.Format("{0}:{1}", pTrade.ExchangeID, pTrade.OrderSysID);
        }

        private string GetOrderKey(ThostFtdcInputOrderActionField pInputOrderAction)
        {
            return string.Format("{0}:{1}", pInputOrderAction.ExchangeID, pInputOrderAction.OrderSysID);
        }
        #endregion
    }
}
