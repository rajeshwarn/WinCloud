using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinRestart
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void w_main_Initialized(object sender, EventArgs e)
        {
            Thread.Sleep(5000);
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + "WinCloud.exe");
            this.Close();
        }
    }
}
