using System.Diagnostics;
using System.Windows.Forms;

namespace Gurimu_Note_Auto_Get_Gold.Data
{
    class MousePositionData
    {
        public static readonly DataStruct.MouseRatio FIRST = new DataStruct.MouseRatio(.5, .75);
        public static readonly DataStruct.MouseRatio SECOND_FD = new DataStruct.MouseRatio(.38, .5);
        public static readonly DataStruct.MouseRatio SECOND_START = new DataStruct.MouseRatio(.95, .75);
        public static readonly DataStruct.MouseRatio THIRD_MATERIAL = new DataStruct.MouseRatio(.5, .9);
        public static readonly DataStruct.MouseRatio THIRD_FO = new DataStruct.MouseRatio(.4, .8);
        public static readonly DataStruct.MouseRatio SKILL = new DataStruct.MouseRatio(.33, .91);
        public static readonly DataStruct.MouseRatio ATK = new DataStruct.MouseRatio(.57, .92);
        public static readonly DataStruct.MouseRatio CHAR_1ST = new DataStruct.MouseRatio(.21, .9);
        public static readonly DataStruct.MouseRatio CHAR_2ND = new DataStruct.MouseRatio(.06, .14);
        public static readonly DataStruct.MouseRatio CHAR_3RD = new DataStruct.MouseRatio(.21, .15);
        public static readonly DataStruct.MouseRatio CHAR_FD = new DataStruct.MouseRatio(.36, .14);

        public static uint getRelativePosition(uint start, uint end, double ratio)
        {
            var ret = start + (uint)((end - start) * ratio);
            return ret;
        }

        /// <summary>
        /// Calculate the ratio of cusor coordinate in target window
        /// </summary>
        /// <param name="winParam">target window parameter</param>
        /// <returns>ratio of X and Y</returns>
        public static DataStruct.MouseRatio getCoorRatioInTargetWindow(DataStruct.Rect winParam, MouseEventArgs e)
        {
            DataStruct.MouseRatio ratio = new DataStruct.MouseRatio();
            ratio.X = (double)(e.X - winParam.left) / (double) (winParam.right - winParam.left);
            ratio.Y = (double)(e.Y - winParam.top) / (double) (winParam.bottom - winParam.top);

            return ratio;
        }
    }
}
