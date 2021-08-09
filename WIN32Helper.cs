using System;
using System.Runtime.InteropServices;

namespace Detector
{
    /// <summary>
    /// WIN32 헬퍼
    /// </summary>
    public static class WIN32Helper
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Import
        ////////////////////////////////////////////////////////////////////////////////////////// Static
        //////////////////////////////////////////////////////////////////////////////// Private

        #region 시스템 매개 변수 정보 처리하기 - SystemParametersInfo(action, parameter1, parameter2, flag)

        /// <summary>
        /// 시스템 매개 변수 정보 처리하기
        /// </summary>
        /// <param name="action">액션</param>
        /// <param name="parameter1">매개 변수 1</param>
        /// <param name="parameter2">매개 변수 2</param>
        /// <param name="flag">플래그</param>
        /// <returns>처리 결과</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint action, uint parameter1, ref bool parameter2, int flag);

        #endregion
        #region 데스크톱 열기 - OpenDesktop(desktopName, flag, inherit, desiredAccess)

        /// <summary>
        /// 데스크톱 열기
        /// </summary>
        /// <param name="desktopName">데스크톱명</param>
        /// <param name="flag">플래그</param>
        /// <param name="inherit">상속 여부</param>
        /// <param name="desiredAccess">희망 액세스</param>
        /// <returns>데스크톱 핸들</returns>
        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr OpenDesktop(string desktopName, uint flag, bool inherit, uint desiredAccess);

        #endregion
        #region 사용자 입력받는 데스크톱 열기 - OpenInputDesktop(flag, inherit, desiredAccess)

        /// <summary>
        /// 사용자 입력받는 데스크톱 열기
        /// </summary>
        /// <param name="flag">플래그</param>
        /// <param name="inherit">상속 여부</param>
        /// <param name="desiredAccess">희망 액세스</param>
        /// <returns>데스크톱 핸들</returns>
        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr OpenInputDesktop(uint flag, bool inherit, uint desiredAccess);

        #endregion
        #region 데스크톱 닫기 - CloseDesktop(desktopHandle)

        /// <summary>
        /// 데스크톱 닫기
        /// </summary>
        /// <param name="desktopHandle">데스크톱 핸들</param>
        /// <returns>처리 결과</returns>
        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr CloseDesktop(IntPtr desktopHandle);

        #endregion
        #region 데스크톱 전환하기 - SwitchDesktop(desktopHandle)

        /// <summary>
        /// 데스크톱 전환하기
        /// </summary>
        /// <param name="desktopHandle">데스크톱 핸들</param>
        /// <returns>처리 결과</returns>
        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SwitchDesktop(IntPtr desktopHandle);

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Field
        ////////////////////////////////////////////////////////////////////////////////////////// Private

        #region Field

        /// <summary>
        /// DESKTOP_SWITCHDESKTOP
        /// </summary>
        private const int DESKTOP_SWITCHDESKTOP = 256;

        /// <summary>
        /// SPI_GETSCREENSAVERRUNNING
        /// </summary>
        private const int SPI_GETSCREENSAVERRUNNING = 114;

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// Static
        //////////////////////////////////////////////////////////////////////////////// Public

        #region 화면 잠금 여부 구하기 - IsScreenLocked()

        /// <summary>
        /// 화면 잠금 여부 구하기
        /// </summary>
        /// <returns>화면 잠금 여부</returns>
        public static bool IsScreenLocked()
        {
            IntPtr desktopHandle = OpenInputDesktop(0, false, DESKTOP_SWITCHDESKTOP);

            if(desktopHandle == IntPtr.Zero)
            {
                desktopHandle = OpenDesktop("Default", 0, false, DESKTOP_SWITCHDESKTOP);
            }

            if(desktopHandle != IntPtr.Zero)
            {
                if(SwitchDesktop(desktopHandle))
                {
                    CloseDesktop(desktopHandle);
                }
                else
                {
                    CloseDesktop(desktopHandle);

                    return true;
                }
            }

            return false;
        }

        #endregion
        #region 화면 보호기 실행 여부 구하기 - IsScreenSaverRunning()

        /// <summary>
        /// 화면 보호기 실행 여부 구하기
        /// </summary>
        /// <returns>화면 보호기 실행 여부</returns>
        //public static bool IsScreenSaverRunning()
        //{
        //    bool isRunning = false;

        //    if(!SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, ref isRunning, 0))
        //    {
        //        return false;
        //    }

        //    if(isRunning)
        //    {
        //        return true;
        //    }

        //    return false;
        //}
        // Check if the screensaver is busy running.
        public static bool IsScreensaverRunning()
        {
            const int SPI_GETSCREENSAVERRUNNING = 114;
            bool isRunning = false;

            if (!SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, ref isRunning, 0))
            {
                // Could not detect screen saver status...
                return false;
            }

            if (isRunning)
            {
                // Screen saver is ON.
                return true;
            }

            // Screen saver is OFF.
            return false;
        }

        #endregion
    }
}