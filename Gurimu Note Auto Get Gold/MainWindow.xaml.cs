/// This application is built based on SharpPcap.
/// For details on using SharpPcap, please check https://github.com/chmorgan/sharppcap/tree/master/Examples .
///

using System.Windows;
using Gurimu_Note_Auto_Get_Gold.Logic;
using Gurimu_Note_Auto_Get_Gold.View;
using System.Runtime.InteropServices;
using System;
using Gurimu_Note_Auto_Get_Gold.Data;
using System.Diagnostics;
using System.Collections.Generic;
using SharpPcap;
using SharpPcap.WinPcap;
using System.Net;
using System.Threading;
using PacketDotNet;

namespace Gurimu_Note_Auto_Get_Gold
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hWnd, out DataStruct.Rect IpRect);

        private const string TARGET_PROCESS = "BlueStacks";
        private const string TARGET_PROCESS_WINDOW_TITLE = "Bluestacks App Player";
        private const string TARGET_ADDRESS = "125.1.77.216"; //Warning! May be changed anytime!
        
        public static long loopCount = 0;
        private int flag = 0;
        private IntPtr hwnd;
        //private bool hook_flag = false;
        private bool willAtk;
        private Thread atkThread;

        private WinPcapDeviceList devices;
        private ICaptureDevice device;
        //private Mouse.Hook mouseHook;

        //will be managed by config xml
        public bool willChange1st = true;
        public bool willChange2nd = false;
        public bool willChange3rd = false;
        public bool willChangeFd = false;
        //public bool isAtkAssist = true;
        //public bool isSkillAssist = true;

        public MainWindow()
        {
            processAttachHook(TARGET_PROCESS);
            InitializeComponent();
            initializeListView();

            message_box.Text = "Please select currently using lancard.\nThen press \"Start Capture\".";
        }

        /// <summary>
        /// Initialize ListView
        /// </summary>
        private void initializeListView()
        {
            listView.SelectionMode = System.Windows.Controls.SelectionMode.Single;
            try
            {
                devices = WinPcapDeviceList.Instance;
            }
            catch(Exception e)
            {
                MessageBox.Show("Please go to http://www.winpcap.org/install/default.htm to download the WinPcap.");
                Debug.WriteLine(e.StackTrace);
            }
            var x = 0;
            foreach (var device in devices)
            {
                listView.Items.Add($@"[{x++}] {device.Interface.FriendlyName}");
            }
        }

        /// <summary>
        /// Get packets by using SharpPcap
        /// Binding with "Start Mining" button
        /// </summary>
        private void startCapture(object sender, RoutedEventArgs e)
        {
            if (devices.Equals(null))
            {
                Dialog fatalErr = Dialog.Instance;
                fatalErr.showMsgBox("Fatal Error!", "Fatal Error Occured!\nPlease restart this program!");
            }
            else
            {
                if (listView.SelectedItem == null)
                {
                    Dialog noChoiceErr = Dialog.Instance;
                    noChoiceErr.showMsgBox("Error!", "Please choose the lancard you are using first!");
                }
                else
                {
                    set_message_box("You have start mining!\nYou can have a meal now!");
                    hwnd = (new GetWindow()).FindWindow(TARGET_PROCESS_WINDOW_TITLE);
                    Mouse.SetForegroundWindow(hwnd);
                    device = devices[listView.SelectedIndex];
                    device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
                    device.Open(DeviceMode.Promiscuous, 2000);
                    device.Filter = "ip and tcp";
                    device.StartCapture();
                    clickSimulate(MousePositionData.FIRST);
                }
            }
        }

        /// <summary>
        /// Invoked when packet received
        /// </summary>
        /// <param name="e">Packet</param>
        private void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            var tcpPacket = (TcpPacket)packet.Extract(typeof(TcpPacket));

            if (tcpPacket != null)
            {
                var ipPacket = (IpPacket)tcpPacket.ParentPacket;
                IPAddress srcIP = ipPacket.SourceAddress;
                IPAddress destIP = ipPacket.DestinationAddress;
                int srcPort = tcpPacket.SourcePort;
                int dstPort = tcpPacket.DestinationPort;
                
                //6 Fin will appear in a loop
                if (destIP.ToString() == TARGET_ADDRESS && tcpPacket.Fin)
                {
                    flag++;
                    //set_message_box("Flag: " + flag);
                    if (flag < 4 || flag > 5)
                        new Thread(() => { startMining(); }).Start();
                }
                else if (flag == 2 && willAtk)
                {
                    if (atkThread == null)
                        atk();
                    else if (!atkThread.IsAlive)
                        atk();
                }
            }
        }

        private void atk()
        {
            if (flag == 3 || !willAtk) return;
            atkThread = new Thread(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    clickSimulate(MousePositionData.ATK, 500);
                }
                clickSimulate(MousePositionData.SKILL, 500);
            });
            atkThread.Start();
        }

        private void startMining()
        {
            Thread.Sleep(2500);
            switch (flag)
            {
                case 1:
                    Thread.Sleep(3000);
                    clickSimulate(MousePositionData.SECOND_FD, 5000);
                    clickSimulate(MousePositionData.SECOND_START);
                    break;

                case 2:
                    Thread.Sleep(16000);
                    if (willChange1st)  clickSimulate(MousePositionData.CHAR_1ST, 1000);
                    if (willChange2nd)  clickSimulate(MousePositionData.CHAR_2ND, 1000);
                    if (willChange3rd)  clickSimulate(MousePositionData.CHAR_3RD, 1000);
                    if (willChangeFd)   clickSimulate(MousePositionData.CHAR_FD, 1000);
                    willAtk = true;
                    break;

                case 3:
                    willAtk = false;
                    if (atkThread.IsAlive) atkThread.Abort();
                    Thread.Sleep(2000);
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(4000);
                        clickSimulate(MousePositionData.THIRD_MATERIAL);
                    }
                    Thread.Sleep(1500);
                    for (int i = 0; i < 2; i++)
                    {
                        Thread.Sleep(3000);
                        clickSimulate(MousePositionData.THIRD_FO);
                    }
                    break;
                    
                case 6:
                case 7:
                    set_message_box("You have farm " + ++loopCount + " games already.");
                    flag = 0;
                    Thread.Sleep(2500);
                    clickSimulate(MousePositionData.FIRST);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// DEPRECATED!
        /// Invoke PortMapper.GetNetStatPorts() to get occupied ports list
        /// Then filter with process name and get port number
        /// </summary>
        /// <param name="process">Target process</param>
        /// <returns>Port number of target process</returns>
        private string getTargetPort(string process)
        {
            string result = null;

            List<PortMapper.Port> portList = PortMapper.GetNetStatPorts();
            foreach (var listItem in portList)
            {
                if (listItem.process_name == process)
                    result = listItem.port_number;
            }

            return result;
        }

        /// <summary>
        /// Ensure target process is opened
        /// </summary>
        /// <param name="processName">Target process</param>
        private void processAttachHook(string processName)
        {
            while (!ProcessCheck.programExistChecking(processName))
            {
                Dialog dialog = Dialog.Instance;
                dialog.showMsgBox("Error", "BlueStacks is not opened!");
            }
        }

        /// <summary>
        /// Get process window parameters
        /// </summary>
        /// <param name="process">Target process</param>
        /// <returns>Four dimension of target window</returns>
        private DataStruct.Rect windowParam(string process)
        {
            Process[] ps = Process.GetProcessesByName(process);
            DataStruct.Rect rect = new DataStruct.Rect();

            foreach (Process p in ps)
            {
                GetWindowRect(p.MainWindowHandle, out rect);
            }

            return rect;
        }

        /// <summary>
        /// Stop checking packets.
        /// Bind with "Stop Capture" Button
        /// </summary>
        private void stopCapture(object sender, RoutedEventArgs e)
        {
            if (device != null)
            {
                try
                {
                    device.StopCapture();
                    Thread.Sleep(1000);
                    device.Close();
                }
                catch (Exception err)
                {
                    //nothing happen: sometimes Close() will cause program terminate
                    Debug.WriteLine(err.StackTrace);
                }
                loopCount = 0;
                flag = 0;
                if (atkThread.IsAlive) atkThread.Abort();
                set_message_box("Mining Stopped!");
            }
        }
        
        public void set_message_box(string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                message_box.Text = message;
            }));
        }

        //Used for debug - cusor coordinate detection
        private void posBtn_Click(object sender, RoutedEventArgs e)
        { }/*
            if (!hook_flag && mouseHook == null)
            {
                mouseHook = new Mouse.Hook();
                mouseHook.OnMouseActivity += new System.Windows.Forms.MouseEventHandler(mouse_OnMouseActivity);
                mouseHook.startHook();
            }
            else
            {
                mouseHook.stopHook();
                mouseHook = null;
            }
            hook_flag = !hook_flag;
        }

        void mouse_OnMouseActivity(object sender, MouseEventArgs e)
        {
            var windowPosition = MousePositionData.getCoorRatioInTargetWindow(windowParam(TARGET_PROCESS), e);
            set_message_box("X:" + e.X + "  Y:" + e.Y + "\nX ratio:" + windowPosition.X.ToString("N2") + " Y ratio:" + windowPosition.Y.ToString("N2"));
        }*/

        /// <summary>
        /// Used to invoke Mouse.performClick()
        /// </summary>
        /// <param name="ratio">ratio struct</param>
        /// <param name="delay">Thread delay after clicking</param>
        private void clickSimulate(DataStruct.MouseRatio ratio, int delay = 0)
        {
            DataStruct.Rect windowPosition = windowParam(TARGET_PROCESS);

            var left = Convert.ToUInt32(windowPosition.left);
            var right = Convert.ToUInt32(windowPosition.right);
            var top = Convert.ToUInt32(windowPosition.top);
            var bottom = Convert.ToUInt32(windowPosition.bottom);

            Mouse.performClick(MousePositionData.getRelativePosition(left, right, ratio.X), MousePositionData.getRelativePosition(top, bottom, ratio.Y));

            Thread.Sleep(delay);
        }
    }
}
