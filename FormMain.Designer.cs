
namespace Detector
{
    partial class FormMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.Tray_Icon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClearListbox = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.labelDetectedPeopleCnt = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboCams = new System.Windows.Forms.ComboBox();
            this.btnClearEvent = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageBoxFrameGrabber = new Emgu.CV.UI.ImageBox();
            this.chkBoxViewImage = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tbSystemInfo = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.autoStartCheckBox = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxFrameGrabber)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 17;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(518, 436);
            this.listBox1.TabIndex = 2;
            // 
            // Tray_Icon
            // 
            this.Tray_Icon.ContextMenuStrip = this.contextMenuStrip1;
            this.Tray_Icon.Icon = ((System.Drawing.Icon)(resources.GetObject("Tray_Icon.Icon")));
            this.Tray_Icon.Text = "Detector";
            this.Tray_Icon.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showStripMenuItem,
            this.toolStripSeparator1,
            this.exitStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(105, 54);
            // 
            // showStripMenuItem
            // 
            this.showStripMenuItem.Name = "showStripMenuItem";
            this.showStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.showStripMenuItem.Text = "Show";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(101, 6);
            // 
            // exitStripMenuItem
            // 
            this.exitStripMenuItem.Name = "exitStripMenuItem";
            this.exitStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.exitStripMenuItem.Text = "Exit";
            // 
            // btnClearListbox
            // 
            this.btnClearListbox.Location = new System.Drawing.Point(5, 5);
            this.btnClearListbox.Name = "btnClearListbox";
            this.btnClearListbox.Size = new System.Drawing.Size(90, 25);
            this.btnClearListbox.TabIndex = 0;
            this.btnClearListbox.Text = "메시시 삭제";
            this.btnClearListbox.UseVisualStyleBackColor = true;
            this.btnClearListbox.Visible = false;
            this.btnClearListbox.Click += new System.EventHandler(this.btnClearListbox_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(532, 508);
            this.tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.labelDetectedPeopleCnt);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.btnClearEvent);
            this.tabPage1.Controls.Add(this.listView1);
            this.tabPage1.Controls.Add(this.imageBoxFrameGrabber);
            this.tabPage1.Controls.Add(this.chkBoxViewImage);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(524, 478);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "검출";
            // 
            // labelDetectedPeopleCnt
            // 
            this.labelDetectedPeopleCnt.AutoSize = true;
            this.labelDetectedPeopleCnt.BackColor = System.Drawing.Color.White;
            this.labelDetectedPeopleCnt.Font = new System.Drawing.Font("Verdana", 72F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDetectedPeopleCnt.ForeColor = System.Drawing.Color.Black;
            this.labelDetectedPeopleCnt.Location = new System.Drawing.Point(332, 96);
            this.labelDetectedPeopleCnt.Name = "labelDetectedPeopleCnt";
            this.labelDetectedPeopleCnt.Size = new System.Drawing.Size(118, 116);
            this.labelDetectedPeopleCnt.TabIndex = 15;
            this.labelDetectedPeopleCnt.Text = "0";
            this.labelDetectedPeopleCnt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboCams);
            this.groupBox1.Location = new System.Drawing.Point(332, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 60);
            this.groupBox1.TabIndex = 65;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "카메라 목록";
            // 
            // cboCams
            // 
            this.cboCams.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCams.FormattingEnabled = true;
            this.cboCams.Location = new System.Drawing.Point(6, 24);
            this.cboCams.Name = "cboCams";
            this.cboCams.Size = new System.Drawing.Size(170, 25);
            this.cboCams.TabIndex = 63;
            // 
            // btnClearEvent
            // 
            this.btnClearEvent.Location = new System.Drawing.Point(332, 221);
            this.btnClearEvent.Name = "btnClearEvent";
            this.btnClearEvent.Size = new System.Drawing.Size(97, 25);
            this.btnClearEvent.TabIndex = 62;
            this.btnClearEvent.Text = "이벤트 삭제";
            this.btnClearEvent.UseVisualStyleBackColor = true;
            this.btnClearEvent.Click += new System.EventHandler(this.btnClearEvent_Click);
            // 
            // listView1
            // 
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(6, 252);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(508, 223);
            this.listView1.TabIndex = 61;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // imageBoxFrameGrabber
            // 
            this.imageBoxFrameGrabber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBoxFrameGrabber.Location = new System.Drawing.Point(6, 6);
            this.imageBoxFrameGrabber.Name = "imageBoxFrameGrabber";
            this.imageBoxFrameGrabber.Size = new System.Drawing.Size(320, 240);
            this.imageBoxFrameGrabber.TabIndex = 11;
            this.imageBoxFrameGrabber.TabStop = false;
            // 
            // chkBoxViewImage
            // 
            this.chkBoxViewImage.AutoSize = true;
            this.chkBoxViewImage.Location = new System.Drawing.Point(332, 6);
            this.chkBoxViewImage.Name = "chkBoxViewImage";
            this.chkBoxViewImage.Size = new System.Drawing.Size(97, 21);
            this.chkBoxViewImage.TabIndex = 5;
            this.chkBoxViewImage.Text = "이미지 보기";
            this.chkBoxViewImage.UseVisualStyleBackColor = true;
            this.chkBoxViewImage.CheckedChanged += new System.EventHandler(this.chkBoxViewImage_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.splitContainer1);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(524, 478);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "로그";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnClearListbox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listBox1);
            this.splitContainer1.Size = new System.Drawing.Size(518, 472);
            this.splitContainer1.SplitterDistance = 32;
            this.splitContainer1.TabIndex = 6;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.tbSystemInfo);
            this.tabPage3.Location = new System.Drawing.Point(4, 26);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(524, 478);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "정보";
            // 
            // tbSystemInfo
            // 
            this.tbSystemInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSystemInfo.Location = new System.Drawing.Point(0, 0);
            this.tbSystemInfo.Multiline = true;
            this.tbSystemInfo.Name = "tbSystemInfo";
            this.tbSystemInfo.ReadOnly = true;
            this.tbSystemInfo.Size = new System.Drawing.Size(524, 478);
            this.tbSystemInfo.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage4.Controls.Add(this.autoStartCheckBox);
            this.tabPage4.Location = new System.Drawing.Point(4, 26);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(524, 478);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "설정";
            // 
            // autoStartCheckBox
            // 
            this.autoStartCheckBox.AutoSize = true;
            this.autoStartCheckBox.Location = new System.Drawing.Point(18, 19);
            this.autoStartCheckBox.Name = "autoStartCheckBox";
            this.autoStartCheckBox.Size = new System.Drawing.Size(170, 21);
            this.autoStartCheckBox.TabIndex = 0;
            this.autoStartCheckBox.Text = "윈도우 시작시, 자동실행";
            this.autoStartCheckBox.UseVisualStyleBackColor = true;
            this.autoStartCheckBox.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 508);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "Detector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxFrameGrabber)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.NotifyIcon Tray_Icon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitStripMenuItem;
        private System.Windows.Forms.Button btnClearListbox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label labelDetectedPeopleCnt;
        private Emgu.CV.UI.ImageBox imageBoxFrameGrabber;
        private System.Windows.Forms.CheckBox chkBoxViewImage;
        public System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnClearEvent;
        private System.Windows.Forms.ComboBox cboCams;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabPage tabPage3;
        protected System.Windows.Forms.TextBox tbSystemInfo;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.CheckBox autoStartCheckBox;
        private System.Windows.Forms.Timer timer1;
    }
}

