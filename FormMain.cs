using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using JCS;
using System.Management;
//using Detector.DataStructures;
using Detector.Classes;

namespace Detector
{

    public partial class FormMain : Form
    {
        Boolean bVisible = false;
        Boolean bSystemClose = false;

        private BackgroundWorker bgwFrameGrab;
        public static CancellationTokenSource sourceFrameGrab = new CancellationTokenSource();
        public static CancellationToken tokenFrameGrab;

        [DllImport("User32", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern void SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;


        //Declararation of all variables, vectors and haarcascades
        Image<Bgr, Byte> currentFrame;
        Emgu.CV.Capture grabber;
        HaarCascade face;
        HaarCascade eye;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray = null;
        List<string> labels = new List<string>();
        string name = null;
        FilterInfoCollection _device;

        Icon[] icons = new Icon[2];
        int currentIcon = 0;

        #region 초기화, 종료
        public FormMain()
        {
            InitializeComponent();

            Global.pMain = this;
            Global.log.Debug("프로그램시작");

            icons[0] = new Icon(@"Icon/Icon1.ico");
            icons[1] = new Icon(@"Icon/Icon2.ico");

            Tray_Icon.Visible = true;
            //Tray_Icon.ShowBalloonTip(100);
            Tray_Icon.MouseDoubleClick += Tray_Icon_MouseDoubleClick;
            showStripMenuItem.Click += ShowStripMenuItem_Click;
            exitStripMenuItem.Click += ExitStripMenuItem_Click;
                        
            bgwFrameGrab = new BackgroundWorker();
            bgwFrameGrab.WorkerReportsProgress = true;
            bgwFrameGrab.WorkerSupportsCancellation = true;
            bgwFrameGrab.DoWork += new DoWorkEventHandler(bgwFrameGrab_DoWork);
            bgwFrameGrab.ProgressChanged += new ProgressChangedEventHandler(bgwFrameGrab_ProgressChanged);

            Select_Cam();
            Get_SystemInfo();

            FaceDetect_Start();            
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            // 리스트뷰 아이템을 업데이트 하기 시작.
            // 업데이트가 끝날 때까지 UI 갱신 중지.
            listView1.BeginUpdate();

            // 뷰모드 지정
            listView1.View = View.Details;

            // 컬럼명과 컬럼사이즈 지정
            listView1.Columns.Add("No", 60, HorizontalAlignment.Center);
            listView1.Columns.Add("탐지 발생시각", 135, HorizontalAlignment.Center);
            listView1.Columns.Add("탐지 내용", 150, HorizontalAlignment.Center);
            listView1.Columns.Add("상태", 90, HorizontalAlignment.Center);

            // 리스뷰를 Refresh하여 보여줌
            listView1.EndUpdate();

            chkBoxViewImage.Checked = true;
            this.ShowInTaskbar = false;
            this.Visible = true;
            Global.log.Debug("5");
        }
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.TaskManagerClosing)
            {
                int k = 0;
            }
            if (e.CloseReason == CloseReason.UserClosing && !bSystemClose)
            {
                Tray_Icon.Visible = true;
                this.Hide();
                e.Cancel = true;
                return;
            }            

 //           if (Global.bUsableDB)
 //               (new MariaCRUD()).TransLogCreate(Global.Authority, Global.UserID, "O", "FormMain", "Logout", "프로그램 종료~~~", null);

            FaceDetect_Stop();
        }
        private void autoStartCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.autoStartCheckBox.Checked)
            {
                Global.SetAutoStartApplication(Global.sTitle);
            }
            else
            {
                Global.ResetAutoStartApplication(Global.sTitle);
            }
        }
        private void Select_Cam()
        {
            // 카메라 목록 체크
            _device = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            for (var i = 0; i < _device.Count; i++)
                cboCams.Items.Add(_device[i].Name);
            //Surface의 경우, Rear카메라가 사용자를 바라본다.
            bool bIsFound = false;
            int nIdx = 0;
            foreach (var it in cboCams.Items)
            {
                if (it.ToString().Contains("Rear"))
                {
                    cboCams.SelectedIndex = nIdx;
                    bIsFound = true;
                    break;
                }
                nIdx++;
            }
            if (!bIsFound && cboCams.Items.Count > 0)
                cboCams.SelectedIndex = 0;
        }
        private void Get_SystemInfo()
        {
            StringBuilder sb = new StringBuilder(String.Empty);

            //==================================================
            PCInfo pcInfo = new PCInfo();

            string[] sTemp = Global.Get_PCModelName();
            pcInfo.PCModelName = sTemp[0];
            pcInfo.PCVendorName = sTemp[1];

            Global.UserID = Environment.UserName;
             

            sb.AppendLine("시스템 정보");
            sb.AppendLine("----------------------------");
            sb.AppendLine(String.Format("Name = {0}", OSVersionInfo.Name));
            sb.AppendLine(String.Format("Edition = {0}", OSVersionInfo.Edition));
            if (OSVersionInfo.ServicePack != string.Empty)
                sb.AppendLine(String.Format("Service Pack = {0}", OSVersionInfo.ServicePack));
            else
                sb.AppendLine("Service Pack = None");
            sb.AppendLine(String.Format("Version = {0}", OSVersionInfo.VersionString));
            sb.AppendLine(String.Format("ProcessorBits = {0}", OSVersionInfo.ProcessorBits));
            sb.AppendLine(String.Format("OSBits = {0}", OSVersionInfo.OSBits));
            sb.AppendLine(String.Format("ProgramBits = {0}", OSVersionInfo.ProgramBits));
            sb.AppendLine(String.Format("No.of Processor of Current Machine = {0}", Environment.ProcessorCount));

            sb.AppendLine(String.Format("PC Model Name = {0}", pcInfo.PCModelName));
            sb.AppendLine(String.Format("PC Vendor = {0}", pcInfo.PCVendorName));

            GetHardwareInfo("Win32_Processor", "Name", "CPU = {0}",  ref sb);
            GetHardwareInfo("Win32_OperatingSystem", "TotalVisibleMemorySize", "메모리 = {0}[GB]", ref sb);
            //GetHardwareInfo("Win32_Processor", "ProcessorID");
            //GetHardwareInfo("Win32_SystemBIOS", "PartComponent");
            //GetHardwareInfo("Win32_NetworkAdapter", "MACAddress");
            //GetHardwareInfo("Win32_DiskDrive", "SerialNumber");

            sb.AppendLine("\r\n사용자 정보");
            sb.AppendLine("----------------------------");
            sb.AppendLine(String.Format("IP(컴퓨터이름) = {0}", Global.GetIP()));
            sb.AppendLine(String.Format("사용자이름 = {0}", Global.UserID));
            int nLine = 1;
            foreach (var cam in cboCams.Items)
            {
                sb.AppendLine(String.Format("카메라{0} = {1}", nLine, cam.ToString()));
                nLine++;
            }

            tbSystemInfo.Text = sb.ToString();

            // 사용등급(A.Admin/B.관리자/C.일반사용자/D.조회만)
            Global.Authority = "C";
//            if (Global.bUsableDB)
//                (new MariaCRUD()).TransLogCreate(Global.Authority, Global.UserID, "I", "FormLogin", "Login", "프로그램 시작!!!", null);            
        }
        private void GetHardwareInfo(string classname, string syntax, string sExplain, ref StringBuilder sb)
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from " + classname);
            foreach (ManagementObject Source in mos.Get())
            {
                //Console.WriteLine(syntax + " - " + Source[syntax]);
                if (syntax == "TotalVisibleMemorySize")
                    sb.AppendLine(String.Format(sExplain, Convert.ToInt64(Source[syntax]) / (1024 * 1024)));
                else
                    sb.AppendLine(String.Format(sExplain, Source[syntax]));
            }
        }
        private void FaceDetect_Start()
        {
            if (bgwFrameGrab.IsBusy)
                return;

            if (cboCams.SelectedIndex < 0)
            {
                MessageBox.Show(
                new Form() { WindowState = FormWindowState.Maximized, TopMost = true },
                "장착된 카메라가 없습니다!!!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                face = new HaarCascade("haarcascade_frontalface_default.xml");
 //               face = new HaarCascade("haarcascade_frontalface_alt.xml");

                //Initialize the capture device
                grabber = new Emgu.CV.Capture(cboCams.SelectedIndex);
                grabber.QueryFrame();
                //Initialize the FrameGraber event
                //Application.Idle += new EventHandler(FrameGrabber);
                threadFrameGrab_Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(
               new Form() { WindowState = FormWindowState.Maximized, TopMost = true },
               ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }
        private void FaceDetect_Stop()
        {
            if (!bgwFrameGrab.IsBusy)
                return;

            threadFrameGrab_Stop();
            grabber.Dispose();
        }
        #endregion

        #region BackgroundWorker
        private void threadFrameGrab_Start()
        {
            if (!bgwFrameGrab.IsBusy)
            {
                sourceFrameGrab = new CancellationTokenSource();
                tokenFrameGrab = sourceFrameGrab.Token;
                bgwFrameGrab.RunWorkerAsync();
            }
        }
        private void threadFrameGrab_Stop()
        {
            if (bgwFrameGrab.IsBusy)
            {
                sourceFrameGrab.Cancel();
                bgwFrameGrab.CancelAsync();
                while (bgwFrameGrab.IsBusy)
                    Application.DoEvents();
            }
        }
        void bgwFrameGrab_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (!bgwFrameGrab.CancellationPending)
                {
                    //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                    //DateTime sTime = DateTime.Now;

                    //Get the current frame form capture device
//                    currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                    currentFrame = grabber.QueryFrame().Resize(160, 120, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                    //Convert it to Grayscale
                    gray = currentFrame.Convert<Gray, Byte>();

                    //Face Detector
                    MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
                          face,
                          1.2,
                          10,
                          Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                          new Size(20, 20));

                    //Action for each element detected
                    foreach (MCvAvgComp f in facesDetected[0])
                    {
                        result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                        // 찾은 얼굴에 적색 사각형
                        //draw the face detected in the 0th (gray) channel with blue color
                        if (Global.bViewImage)
                            currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);
                    }

                    //Set the number of faces detected on the scene
                    this.labelDetectedPeopleCnt.Invoke(new Action(delegate ()
                    {
                        labelDetectedPeopleCnt.Text = facesDetected[0].Length.ToString();
                        if (facesDetected[0].Length == 0)
                        {
                            labelDetectedPeopleCnt.Text = "0";
                            labelDetectedPeopleCnt.ForeColor = Color.Black;
                        }
                        else if (facesDetected[0].Length == 1)
                        {
                            //if (Global.bUsableDB)
                            //{
                            //    string str = @"temp.jpg";
                            //    currentFrame.Save(str);
                            //    (new MariaCRUD()).TransLogCreate(Global.Authority, Global.UserID, "F", "탐지", "인물",
                            //        string.Format("얼굴 인식: {0}명", facesDetected[0].Length), Global.ConvertImageToBytes(str));
                            //}

                            labelDetectedPeopleCnt.ForeColor = Color.Lime;
                            Global.ListViewAddItem(listView1, facesDetected[0].Length);
                        }
                        else if (facesDetected[0].Length > 1)
                        {
                            labelDetectedPeopleCnt.ForeColor = Color.Magenta;
                            Global.ListViewAddItem(listView1, facesDetected[0].Length);

                            if (Global.bUsableDB)
                            {
                                string str = @"temp.jpg";
                                currentFrame.Save(str);
//                                (new MariaCRUD()).TransLogCreate(Global.Authority, Global.UserID, "F", "탐지", "인물",
//                                    string.Format("얼굴 인식: {0}명", facesDetected[0].Length), Global.ConvertImageToBytes(str));
                            }
                        }
                    }));

                    //Show the faces procesed and recognized
                    if (Global.bViewImage)
                        imageBoxFrameGrabber.Image = currentFrame;

                    // 소요시간 구하기
                    //TimeSpan gapTime = DateTime.Now - sTime;
                    //Debug.WriteLine(string.Format("{0:N0} ms", gapTime.TotalMilliseconds));

                    Task.Delay(Global.nFrameTimer, tokenFrameGrab).Wait();
                }

                // 외부에서 작업을 취소하였는가?
                //if (bgwUpload.CancellationPending)
                //{
                //    e.Cancel = true;  // 작업 취소
                //    return;
                //}
            }
            catch (Exception ex)
            {
                //무시한다.
                //MessageBox.Show(ex.Message);
            }
        }
        void bgwFrameGrab_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }
        #endregion

        #region 컨트롤 클릭 이벤트
        private void btnClearEvent_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            Tray_Icon.BalloonTipIcon = ToolTipIcon.Info;
            Tray_Icon.BalloonTipTitle = "Notify Icon Test Application";
            Tray_Icon.BalloonTipText = "You have just minimized the application." +
                                        Environment.NewLine +
                                        "Right-click on the icon for more options.";
            Tray_Icon.ShowBalloonTip(5000);
        }

        private void chkBoxViewImage_CheckedChanged(object sender, EventArgs e)
        {
            Global.bViewImage = chkBoxViewImage.Checked;
        }
        private void btnClearListbox_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }
        #endregion

        #region 시스템 트레이
        private void Tray_Icon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (bVisible)
            {
                this.Hide();
            }
            else
            {
                // 윈도우 타이틀명으로 핸들을 찾는다
                IntPtr hWnd = FindWindow(null, this.Text);
                if (!hWnd.Equals(IntPtr.Zero))
                {
                    // 윈도우가 최소화 되어 있다면 활성화 시킨다
                    ShowWindowAsync(hWnd, SW_SHOWNORMAL);
                    // 윈도우에 포커스를 줘서 최상위로 만든다
                    SetForegroundWindow(hWnd);
                }
            }
            bVisible = !bVisible;
        }
        private void ExitStripMenuItem_Click(object sender, EventArgs e)
        {
            bSystemClose = true;
            this.Close();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Tray_Icon.Icon = icons[currentIcon];
            currentIcon++;
            if (currentIcon == 2)
                currentIcon = 0;
        }

        private void ShowStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!this.Visible)
            //{
                this.ShowInTaskbar = true;
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
            //}
            //else
            //    this.Visible = false;
        }
        #endregion
    }
}
