using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace OuCTP
{
    public partial class MessageForm : XtraForm
    {
        private List<ErrorInfo> ds = new List<ErrorInfo>();

        public MessageForm()
        {
            InitializeComponent();

            gcMessage.DataSource = ds;
        }

        private void MessageForm_Load(object sender, EventArgs e)
        {
            ds.Clear();
            ds.AddRange(MyUtils.ErrorList);
            gcMessage.RefreshDataSource();
        }
    }
}
