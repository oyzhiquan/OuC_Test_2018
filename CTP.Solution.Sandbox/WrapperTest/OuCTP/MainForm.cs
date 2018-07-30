using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CTP;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraEditors;

namespace OuCTP
{
    public partial class MainForm : XtraForm
    {
        #region 字段
        private List<AccountInfo> lstAccount = new List<AccountInfo>();

        private List<InstrumentBaseInfo> lstInstrument = new List<InstrumentBaseInfo>();

        #endregion

        #region 初始化

        public MainForm()
        {
            InitializeComponent();

            gvAccount.OptionsBehavior.Editable = false;
            if (File.Exists(Path.Combine(MyUtils.AssemblyPath, "dock.xml")))
            {
                dockManager1.RestoreLayoutFromXml(Path.Combine(MyUtils.AssemblyPath, "dock.xml"));
            }
        }
        #endregion

        #region 事件
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SkinHelper.InitSkinPopupMenu(iPaintStyle);
            //注册刷新事件
            MyUtils.OnNeedRefreshAccount += RefreshAccount;
        }

        private void btnMessage_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MessageForm msgForm = new MessageForm();
            msgForm.Show(this);
        }
        #endregion

        #region 方法

        private void RefreshAccount()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(RefreshAccount));
                return;
            }
            lstAccount.Clear();
            lstAccount.AddRange(MyUtils.DsAccountInfo);
            gcAccount.DataSource = lstAccount;

            gcAccount.RefreshDataSource();

            lstInstrument.Clear();
            foreach (KeyValuePair<string, ThostFtdcInstrumentField> kvp in MyUtils.StandardInstrumentInfo)
            {
                lstInstrument.Add(new InstrumentBaseInfo()
                {
                    InstrumentID = kvp.Value.InstrumentID,
                    InstrumentName = kvp.Value.InstrumentName
                });
            }
            gcInstrument.DataSource = lstInstrument;
            gcInstrument.RefreshDataSource();
        }

        #endregion

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            dockManager1.SaveLayoutToXml(Path.Combine(MyUtils.AssemblyPath, "dock.xml"));
        }

    }
}
