using System.Runtime.InteropServices;
using System.Drawing;
using System;
using System.Windows.Forms;
using System.Threading;

namespace Gurimu_Note_Auto_Get_Gold.Data
{
    class Mouse
    {
        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out DataStruct.POINT lpPoint);

        public static Point GetCursorPosition()
        {
            DataStruct.POINT lpPoint;
            GetCursorPos(out lpPoint);
            //bool success = User32.GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }


        // The following are the simulator parts.

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int WM_LBUTTONDBLCLK = 0x0203;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern bool SetCursorPos(uint x, uint y);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr dwExtraInfo);

        public static void performClick(uint x, uint y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN, x, y, 0, UIntPtr.Zero);
            Thread.Sleep(200);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTUP, x, y, 0, UIntPtr.Zero);
        }

        public static void moveToPos(uint x, uint y)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x, y, 0, UIntPtr.Zero);
        }

        /*Mouse Hook*/
        public class Hook
        {
            public Hook() { }
            ~Hook() { stopHook(); }

            public event MouseEventHandler OnMouseActivity;
            private int hMouseHook = 0;
            public const int WH_MOUSE_LL = 14;

            //hook
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

            //unhook
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern bool UnhookWindowsHookEx(int idHook);

            //next hook
            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

            public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
            private static HookProc callbackDelegate;

            public void startHook()
            {
                if (hMouseHook == 0)
                {
                    callbackDelegate = new HookProc(MouseHookProc);
                    hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, callbackDelegate, Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                    if (hMouseHook == 0)
                    {
                        stopHook();
                        throw new Exception("SetWindowsHookEx failed.");
                    }
                }
            }

            public void stopHook()
            {
                bool ret = true;
                if (hMouseHook != 0)
                {
                    ret = UnhookWindowsHookEx(hMouseHook);
                    hMouseHook = 0;
                    callbackDelegate = null;
                }
            }

            private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
            {
                if ((nCode >= 0) && (OnMouseActivity != null))
                {
                    MouseButtons button = MouseButtons.None;
                    int clickCount = 0;

                    switch (wParam)
                    {
                        case WM_LBUTTONDOWN:
                            button = MouseButtons.Left;
                            clickCount = 1;
                            break;
                        case WM_LBUTTONUP:
                            button = MouseButtons.Left;
                            clickCount = 1;
                            break;
                    }
                    //从回调函数中得到鼠标的信息  
                    DataStruct.MouseHookStruct MyMouseHookStruct = (DataStruct.MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(DataStruct.MouseHookStruct));
                    MouseEventArgs e = new MouseEventArgs(button, clickCount, MyMouseHookStruct.pt.X, MyMouseHookStruct.pt.Y, 0);
                    //if(e.X>700)return 1;//如果想要限制鼠标在屏幕中的移动区域可以在此处设置  
                    OnMouseActivity(this, e);
                }
                return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
            }
        }

    }
}
