using IniParser;
using IniParser.Model;
using log4net;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Detector

{
    public class UserObj
    {
        public bool bShow;
        public TimeSpan Span;
        public UserObj(TimeSpan span, bool bShow)
        {
            Span = span;
            this.bShow = bShow;
        }
    }
    
    public static class Global
    {
        // ini-parser 데이터 로드
        //public static FileIniDataParser parser = new FileIniDataParser();
        //public static IniData iniData = parser.ReadFile("Config.ini");

        public static int nMaxClients = 0;
        public static int[] Ports = null;

        public static string sTitle = "Detector";
        private static RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

        public static bool bUsableDB = true;


        public static int nMonitorTimer = 5000;
        public static int nFrameTimer = 1000;

        public static string sServerIP = "";
        public static string sDBName = "";
        public static string sDBIP = "";
        public static string sDBID = "";
        public static string sDBPW = "";
        //마리아DB Server 정보
        public static string DBConString = "server=172.16.2.67;user id=root;password=admin#123;database=mtek_test;Charset=utf8";
        public static string UserID;
        public static string UserName;
        public static string Authority;

        public static FormMain pMain;
        public static long lCnt = 0;
        public static object lockObject = new object();
        public static bool bViewImage = false;
        //public static MariaDB mariaDB = new MariaDB();


        //public static List<RemoteClient> clients = null;
        public static Rectangle rcChildWnd = new Rectangle();

        public static Dictionary<int, string> dictClient = new Dictionary<int, string>();
        
        //BackgroundWorker 관련
        public static CancellationTokenSource source = new CancellationTokenSource();
        public static CancellationToken token;

        // 로거 ILog 필드
        public static ILog log = LogManager.GetLogger("Program");
        // 로거 사용 : 5가지 레벨의 메서드들
        //log.Debug("Main() started");
        //log.Info("My Info");
        //log.Warn("My Warning");
        //log.Error("My Error");
        //log.Fatal("My Fatal Error");

        //가상 키보드
        [System.Runtime.InteropServices.DllImport("Kernel32.Dll", EntryPoint = "Wow64EnableWow64FsRedirection")]
        public static extern bool EnableWow64FSRedirection(bool enable);
        //public static System.Diagnostics.Process ps = new System.Diagnostics.Process();

        #region 자동 실행 설정
        public static bool IsAutoStartApplication(string applicationName)
        {
            return registryKey.GetValue(applicationName) != null;
        }
        public static void SetAutoStartApplication(string applicationName)
        {
            SetAutoStartApplication(applicationName, Application.ExecutablePath);
        }
        /// <summary>
        /// 자동 실행 애플리케이션 설정 해제하기
        /// </summary>
        /// <param name="applicationName">애플리케이션명</param>
        public static void ResetAutoStartApplication(string applicationName)
        {
            if (registryKey.GetValue(applicationName) != null)
            {
                registryKey.DeleteValue(applicationName, false);
            }
        }
        public static void SetAutoStartApplication(string applicationName, string applicationFilePath)
        {
            if (registryKey.GetValue(applicationName) == null)
            {
                registryKey.SetValue(applicationName, applicationFilePath);
            }
        }
        #endregion

        public static byte[] ConvertImageToBytes(string pathfilenames)
        {
            // 선택된 사진파일 변환B
            byte[] ImageData = null;
            if (pathfilenames != "")
            {
                FileStream fs = new FileStream(pathfilenames, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                ImageData = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
            }

            return ImageData;
        }

        public static void Show_Virtual_KB()
        {
            EnableWow64FSRedirection(false);
            try
            {
                using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                {
                    //p.StartInfo.FileName = "C:\\Windows\\System32\\osk.exe";
                    p.StartInfo.FileName = "osk.exe";
                    p.StartInfo.Arguments = null;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    p.Start()
                        ;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //ps.StartInfo.FileName = "osk.exe";
            //ps.Start();
        }
        public static string[] Get_PCModelName()
        {
            string[] sResult = new string[2];
            ProcessStartInfo proInfo = new ProcessStartInfo();
            Process pro = new Process();

            proInfo.FileName = @"cmd";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            proInfo.RedirectStandardOutput = true;
            proInfo.RedirectStandardInput = true;
            proInfo.RedirectStandardError = true;

            pro.StartInfo = proInfo;
            pro.Start();

            pro.StandardInput.Write("wmic csproduct get name" + Environment.NewLine);
            pro.StandardInput.Close();
            string resultValue = pro.StandardOutput.ReadToEnd();
            pro.WaitForExit();
            pro.Close();

            string[] result = resultValue.Split('\n'); 
            //for (int i = 0; i < result.Length; i++)  // 배열은 0 부터 저장되며, 배열의 길이만큼 순환
            //{
            //    Console.WriteLine(string.Format("{0}번째 배열 ==> {1}", i, result[i].Trim('\r')));
            //}
            sResult[0] = result[5].Trim('\r');

            pro.Start();
            pro.StandardInput.Write("wmic csproduct get vendor" + Environment.NewLine);
            pro.StandardInput.Close();
            resultValue = pro.StandardOutput.ReadToEnd();
            pro.WaitForExit();
            pro.Close();

            result = resultValue.Split('\n');
            //for (int i = 0; i < result.Length; i++)  // 배열은 0 부터 저장되며, 배열의 길이만큼 순환
            //{
            //    Console.WriteLine(string.Format("{0}번째 배열 ==> {1}", i, result[i].Trim('\r')));
            //}
            sResult[1] = result[5].Trim('\r');

            return sResult;
        }
        public static void Hide_Virtual_KB()
        {
            //ps.Kill();
        }
        public static string GetIP()
        {
            string strHostName = Dns.GetHostName();

            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;

            return addr[addr.Length - 1].ToString() + "(" + strHostName + ")";
        }

        #region WinForm UI update
        public static void ListViewAddItem(ListView lvw, int nCnt)
        {
            ListViewItem item = new ListViewItem(string.Format("{0}", lvw.Items.Count + 1));
            item.SubItems.Add(string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now));
            item.SubItems.Add(string.Format("얼굴 인식: {0}명", nCnt));
            //item.SubItems.Add(string.Format("{0}", nCnt));

            string status = "";
            Color statusClr = Color.Lime;
            if (nCnt == 1)
            {
                status = "정상";
                statusClr = Color.Lime;
            }
            else if (nCnt > 1)
            {
                status = "보안위반";
                statusClr = Color.Magenta;
            }
            item.SubItems.Add(status);
            item.UseItemStyleForSubItems = false;   //이거 꼭 써줘야 함 
            item.SubItems[3].BackColor = statusClr;

            pMain.Invoke(new Action(delegate ()
            {
                lvw.Items.Insert(0, item);
                lvw.EnsureVisible(0);
            }
            ));
        }
        public static void Update_listBox1(string data)
        {
            try
            {
                string str = string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), data);
                pMain.listBox1.Invoke(new MethodInvoker(
                     delegate ()
                     {
                         pMain.listBox1.BeginInvoke(new Action(() => pMain.listBox1.Items.Insert(0, str)));

                         // 방금 추가한 아이템에 스크롤을 위치시킨다.
                         pMain.listBox1.BeginInvoke(new Action(() => pMain.listBox1.SelectedIndex = 0));
                     }
                     )
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
    }
}
