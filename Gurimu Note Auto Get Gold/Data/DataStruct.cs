using System.Drawing;
using System.Runtime.InteropServices;

namespace Gurimu_Note_Auto_Get_Gold.Data
{
    /// <summary>
    /// Data Structure Template Class
    /// All data structure should be store in here
    /// New Instance will be created if necessary by using new
    /// </summary>
    class DataStruct
    {
        /// <summary>
        /// Used to store window parameter
        /// </summary>
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        };

        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// Mouse coor ratio
        /// </summary>
        public struct MouseRatio
        {
            public double X;
            public double Y;

            public MouseRatio(double _x, double _y)
            {
                X = _x;
                Y = _y;
            }
        };

        /// <summary>
        /// Hook Struct of mouse message
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }
    }
}
