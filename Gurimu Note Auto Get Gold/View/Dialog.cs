using System;
using System.Windows;
using System.Windows.Forms;

namespace Gurimu_Note_Auto_Get_Gold.View
{
    /// <summary>
    /// Used to generate dialog
    /// </summary>
    class Dialog
    {
        public const string TAG_ERR = "ERROR";
        public const string TAG_NOR = "NORMAL";

        private MessageBoxButton buttons { get; set; }
        private MessageBoxImage image { get; set; }
        private DialogResult result { get; set; }
        private string tag { get; set; }

        /// <summary>
        /// Single instance
        /// </summary>
        private static Dialog instance;
        public static Dialog Instance
        {
            get
            {
                if (instance == null)
                    instance = new Dialog();
                return instance;
            }
        }
        
        /// <summary>
        /// Common method to show dialog box
        /// </summary>
        /// <param name="title">Title of dialog box</param>
        /// <param name="msg">Message body of dialog box</param>
        public void showMsgBox(string title, string msg, string tag = TAG_NOR)
        {
            this.tag = tag;
            switch (tag)
            {
                case TAG_ERR:
                    buttons = MessageBoxButton.OK;
                    image = MessageBoxImage.Error;
                    break;
                case TAG_NOR:
                    buttons = MessageBoxButton.OKCancel;
                    image = MessageBoxImage.None;
                    break;
            }

            result = (DialogResult) System.Windows.MessageBox.Show(msg, title, buttons, image);
            if ((tag.Equals(Dialog.TAG_NOR) && result == DialogResult.Cancel) || (tag.Equals(Dialog.TAG_ERR) && result == DialogResult.OK))
            {
                Environment.Exit(Environment.ExitCode);
            }
        }
    }
}
