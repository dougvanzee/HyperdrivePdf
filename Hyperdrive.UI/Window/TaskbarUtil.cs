using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Shell;

namespace Hyperdrive.UI
{
    class TaskbarUtil
    {

        public void EnableLoadingBarProgress(TaskbarItemInfo taskbarItemInfo)
        {
            taskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
        }

        public void DisableLoadingBarProgress(TaskbarItemInfo taskbarItemInfo)
        {
            taskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
        }

        public void UpdateLoadingBarProgress(TaskbarItemInfo taskbarItemInfo, double progress)
        {

            taskbarItemInfo.ProgressValue = progress;

        }

        [DllImport("user32.dll")]
        public static extern int FlashWindow(IntPtr Hwnd, bool Revert);


        // check if window is minimised

        public void FlashTheWindow(Window window)

        {

            FlashWindow(new WindowInteropHelper(window).Handle, false);

        }


    }
}
