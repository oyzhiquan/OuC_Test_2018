using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OuCTP
{
    public partial class LoginForm : Form
    {
        private ServerInfo serverInfos = new ServerInfo();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!chkNoTradeSpan.Checked)
            {
                object[] selectedValue = cboServer.SelectedValue as object[];
                string borkerID = selectedValue[0].ToString();
                string LineID = selectedValue[1].ToString();
                string BorkerName = selectedValue[2].ToString();
                Brokers brokers = serverInfos.broker.Find(s => s.Id == borkerID);
                Lines lines = brokers.lines.Find(s => s.ID == LineID);
                List<string> tradeFront = new List<string>();
                lines.Trades.ForEach(x => tradeFront.Add(x.Address));
                List<string> marketFront = new List<string>();
                lines.Markets.ForEach(x => marketFront.Add(x.Address));

                MyUtils.Trader = new TraderAdapter
                {
                    BrokerId = borkerID,
                    InvestorId = txtName.Text,
                    Password = txtPwd.Text,
                    Front = tradeFront.ToArray()
                };

                MyUtils.QuoteMain = new QuoteAdapter((TraderAdapter)MyUtils.Trader)
                {
                    BrokerId = borkerID,
                    InvestorId = txtName.Text,
                    Password = txtPwd.Text,
                    Front = marketFront.ToArray()
                };
                Task.Run(() => { ((QuoteAdapter)MyUtils.QuoteMain).Connect(); });

                while (!((QuoteAdapter)MyUtils.QuoteMain).IsReady)
                {
                    lblTips.Text = "等待行情连接";
                    Thread.Sleep(100);
                }

                lblTips.Text = "行情连接成功！！！";

                Task.Run(() => { ((TraderAdapter)MyUtils.Trader).Connect(); });

                while (!MyUtils.IsTraderReady)
                {
                    lblTips.Text = "等待交易连接";
                    Thread.Sleep(1000);
                }

                lblTips.Text = "交易连接成功！！！";
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void Init()
        {
            //读服务配置
            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Value", typeof(object));
            cboServer.DisplayMember = "Name";
            cboServer.ValueMember = "Value";
            string fileName = Path.Combine(@".\Config\ServerInfo.xml");
            if (File.Exists(fileName))
            {
                serverInfos = FormHelper.XmlDeserializeObject<ServerInfo>(File.ReadAllText(fileName));
                serverInfos.broker.ForEach(s =>
                {
                    s.lines.ForEach(ss =>
                    {
                        dt.Rows.Add(string.Format("{0} [{1}]", s.Name, ss.Name), new object[]
                        {
                            s.Id,
                            ss.ID,
                            s.Name
                        });
                    });
                });
                cboServer.DataSource = dt;
                if (cboServer.Items.Count > 0)
                {
                    cboServer.SelectedIndex = 0;
                }
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void cboServer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
