using Gurimu_Note_Auto_Get_Gold.Data;
using Gurimu_Note_Auto_Get_Gold.View;
using PacketDotNet;
using SharpPcap;
using SharpPcap.WinPcap;
using System;
using System.Net;
using System.Threading;
using System.Windows.Controls;

namespace Gurimu_Note_Auto_Get_Gold.Logic
{
    class PacketHandler
    {
        private const string TARGET_PROCESS_WINDOW_TITLE = "Bluestacks App Player";
        private const string TARGET_ADDRESS = "125.1.77.216";

        private ICaptureDevice device;
        private IntPtr hwnd;

        private int flag;
        private Thread atkThread;
        

        PacketHandler(WinPcapDeviceList devices, ListView listView)
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

        public void run()
        {

        }

        public void stop()
        {

        }
    }
}
