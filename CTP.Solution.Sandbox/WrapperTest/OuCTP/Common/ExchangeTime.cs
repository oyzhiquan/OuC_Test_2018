using System;
using System.Collections.Generic;

namespace OuCTP
{
    public class ExchangeTime
    {
        private ExchangeTime()
        {
            try
            {
                //无夜盘的品种
                m_sExchsh = "SHFE 9:0:0-10:15:0;10:30:0-11:30:0;13:30:0-15:0:0";
                m_sExchdl = "DCE 9:0:0-10:15:0;10:30:0-11:30:0;13:30:0-15:0:0";
                m_sExchzz = "CZCE 9:0:0-10:15:0;10:30:0-11:30:0;13:30:0-15:0:0";

                //所有的夜盘
                m_sExchzzNight = "RM;FG;MA;SR;TA;ZC;CF; 21:0:0-23:30:0";
                m_sExchdlNight = "i;j;jm;a;m;p;y; 21:0:0-23:30:0";
                m_sExchshNight1 = "ag;au; 21:0:0-23:59:59;0:0:0-2:30:0";
                m_sExchshNight3 = "rb;bu;hc 21:0:0-23:0:0";

                m_mapTradingTime = new Dictionary<string, List<TradingTime>>();
                m_sLog = "交易时间";

                InitDefault();
            }
            catch (Exception ex)
            {
            }
        }

        public static ExchangeTime Instance
        {
            get { return _exchangeTime; }
        }

        // sCate在交易时间段返回true
        public bool IsTradingTime(string category, string exchange)
        {
            var result = true;
            try
            {
                foreach (var kvp in m_mapTradingTime)
                {
                    var s = new List<string>();

                    if (kvp.Key.Contains(";"))
                    {
                        var instrumentIds = kvp.Key.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        s.AddRange(instrumentIds);
                    }

                    if (s.Contains(category) || kvp.Key.Equals(exchange))
                    {
                        var currentDateTime = DateTime.Now;
                        var currentSecond = GetSecFromDateTime(currentDateTime) + MyUtils.ExchangeTimeOffset;
                        foreach (var time in kvp.Value)
                        {
                            if ((currentSecond >= time.StartSecond) && (currentSecond <= time.EndSecond))
                            {
                                return true;
                            }
                        }
                    }

                    result = false;
                }
            }
            catch (Exception ex)
            {
                return true;
            }

            return result;
        }

        private void ParseFromString(string sLine)
        {
            try
            {
                var sKeyValue = sLine.Split(' ');
                var sKey = sKeyValue[0];
                m_sLog += sKey;

                var sValueString = sKeyValue[1];
                var sValue = sValueString.Split(';');
                foreach (var s in sValue)
                {
                    var s1 = s.Split('-');
                    var dtStart = Convert.ToDateTime(s1[0]);
                    var dtEnd = Convert.ToDateTime(s1[1]);
                    var time = new TradingTime
                    {
                        StartTimeString = dtStart.ToLongTimeString(),
                        EndTimeString = dtEnd.ToLongTimeString(),
                        StartSecond = GetSecFromDateTime(dtStart),
                        EndSecond = GetSecFromDateTime(dtEnd)
                    };
                    if (!m_mapTradingTime.ContainsKey(sKey))
                    {
                        m_mapTradingTime[sKey] = new List<TradingTime>();
                    }
                    m_mapTradingTime[sKey].Add(time);
                    m_sLog += s1[0];
                    m_sLog += s1[1];
                }
            }
            catch (Exception ex)
            {
            }
        }

        public int GetSecFromDateTime(DateTime dt)
        {
            try
            {
                return dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        private void InitDefault()
        {
            try
            {
                if (m_mapTradingTime.Count > 0)
                    m_mapTradingTime.Clear();

                ParseFromString(m_sExchsh);
                ParseFromString(m_sExchdl);
                ParseFromString(m_sExchzz);
                ParseFromString(m_sExchzzNight);
                ParseFromString(m_sExchdlNight);
                ParseFromString(m_sExchshNight1);
                ParseFromString(m_sExchshNight2);
                ParseFromString(m_sExchshNight3);
            }
            catch (Exception ex)
            {
            }
        }

        private static readonly ExchangeTime _exchangeTime = new ExchangeTime();

        private string m_sLog;
        private readonly string m_sExchsh; // 上海
        private readonly string m_sExchdl; // 大连
        private readonly string m_sExchzz; // 郑州

        private readonly string m_sExchzzNight; // TA;SR;CF;RM;ME;MA夜盘
        private readonly string m_sExchdlNight; // p;j;a;b;m;y;jm;i夜盘
        private readonly string m_sExchshNight1; // ag;au夜盘
        private string m_sExchshNight2; // cu;al;zn;pb;rb;hc;bu夜盘
        private string m_sExchshNight3; // ru夜盘

        private Dictionary<string, List<TradingTime>> m_mapTradingTime;
    }

    public class TradingTime
    {
        public string StartTimeString;
        public string EndTimeString;
        public int StartSecond; // 时间段开始时间的秒数
        public int EndSecond;
    }
}
