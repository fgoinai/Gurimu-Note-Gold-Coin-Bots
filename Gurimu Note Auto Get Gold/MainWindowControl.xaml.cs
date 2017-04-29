using System.Windows;
using System.Windows.Controls;

namespace Gurimu_Note_Auto_Get_Gold
{
    /// <summary>
    /// Interaction logic for MainWindowControl.xaml
    /// </summary>
    public partial class MainWindowControl : UserControl
    {
        public MainWindowControl()
        {
            InitializeComponent();
        }
           
        public int LancardSelection
        {
            get { return (int)GetValue(LancardSelectionProperty); }
            set { SetValue(LancardSelectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LancardSelection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LancardSelectionProperty =
            DependencyProperty.Register("LancardSelection", typeof(int), typeof(MainWindowControl), new PropertyMetadata(0));


    }
}
