namespace OuCTP
{
    partial class MessageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gcMessage = new DevExpress.XtraGrid.GridControl();
            this.gvMessage = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.gcMessage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMessage)).BeginInit();
            this.SuspendLayout();
            // 
            // gcMessage
            // 
            this.gcMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcMessage.Location = new System.Drawing.Point(0, 0);
            this.gcMessage.MainView = this.gvMessage;
            this.gcMessage.Name = "gcMessage";
            this.gcMessage.Size = new System.Drawing.Size(546, 298);
            this.gcMessage.TabIndex = 0;
            this.gcMessage.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvMessage});
            // 
            // gvMessage
            // 
            this.gvMessage.GridControl = this.gcMessage;
            this.gvMessage.Name = "gvMessage";
            this.gvMessage.OptionsBehavior.Editable = false;
            this.gvMessage.OptionsView.ShowGroupPanel = false;
            // 
            // MessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 298);
            this.Controls.Add(this.gcMessage);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageForm";
            this.Text = "MessageForm";
            this.Load += new System.EventHandler(this.MessageForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gcMessage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMessage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gcMessage;
        private DevExpress.XtraGrid.Views.Grid.GridView gvMessage;
    }
}