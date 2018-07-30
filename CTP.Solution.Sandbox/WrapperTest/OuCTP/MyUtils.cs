using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CTP;

namespace OuCTP
{
    public class MyUtils
    {
        #region 字段
        //交易
        public static object Trader;

        //行情
        public static object QuoteMain;

        //交易就绪
        public static bool IsTraderReady = false;

        //交易所时间与本地时间的偏移值
        public static int ExchangeTimeOffset = 0;

        //错误信息集合
        public static List<ErrorInfo> ErrorList = new List<ErrorInfo>();

        //程序执行路径
        public static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";

        //资金信息
        public static List<AccountInfo> DsAccountInfo = new List<AccountInfo>();

        //标准合约基本信息
        public static ConcurrentDictionary<string, ThostFtdcInstrumentField> StandardInstrumentInfo =
            new ConcurrentDictionary<string, ThostFtdcInstrumentField>();

        //组合合约基本信息
        public static ConcurrentDictionary<string, ThostFtdcInstrumentField> ComboInstrumentInfo =
            new ConcurrentDictionary<string, ThostFtdcInstrumentField>();

        //key-合约品种 value-交易所ID 
        public static ConcurrentDictionary<string, string> CategoryToExchangeId =
            new ConcurrentDictionary<string, string>();


        #endregion

        #region 委托与事件
        public delegate void NeedRefreshAccount();

        public static event NeedRefreshAccount OnNeedRefreshAccount;
        #endregion

        #region 方法
        /// <summary>
        /// 回报错误信息
        /// </summary>
        /// <param name="pRspInfo"></param>
        /// <returns>true-有错误 false-没有出错</returns>
        public static bool IsWrongRspInfo(ThostFtdcRspInfoField pRspInfo)
        {
            return pRspInfo != null && pRspInfo.ErrorID != 0;
        }

        public static void ReportError(ThostFtdcRspInfoField pRspInfo, string title)
        {
            ErrorInfo errorInfo = new ErrorInfo
            {
                ErrorID = pRspInfo.ErrorID,
                ErrorMsg = pRspInfo.ErrorMsg,
                IsRead = false,
                HappenTime = DateTime.Now.ToString(CultureInfo.InvariantCulture)
            };
            ErrorList.Add(errorInfo);
        }

        /// <summary>
        /// 更新资金信息
        /// </summary>
        /// <param name="source"></param>
        public static void UpdateAccountInfo(ThostFtdcTradingAccountField source)
        {
            DsAccountInfo.Clear();
            DsAccountInfo.Add(new AccountInfo
            {
                AccountID = source.AccountID,
                Available = source.Available,
                Balance = source.Balance,
                CloseProfit = source.CloseProfit,
                Commission = source.Commission
            });
            if (OnNeedRefreshAccount != null)
            {
                OnNeedRefreshAccount();
            }
        }

        public static string GetInstrumentCategory(string instrumentId)
        {
            var regex = new Regex("^[a-zA-Z]+");
            var match = regex.Match(instrumentId);

            if (match.Success)
            {
                Debug.Assert(match.Value.Length <= 2);
                return match.Value;
            }

            return null;
        }
        #endregion
    }
}
