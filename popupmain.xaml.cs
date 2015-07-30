using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace WinCloud
{
    public partial class popupmain : MetroWindow
    {
        string targu;
        public popupmain(string poptarg)
        {
            InitializeComponent();
            targu = poptarg;
        }

        private void w_popmain_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.fs == true)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }

            string ps1 = "pages";
            string ps2 = "numbers";
            string ps3 = "keynote";
            int targusource;

            if (targu.Contains(ps1) == true)
            {
                this.Title = "WinCloud - Pages";
            }
            if (targu.Contains(ps2) == true)
            {
                this.Title = "WinCloud - Numbers";
            }
            if (targu.Contains(ps3) == true)
            {
                this.Title = "WinCloud - Keynote";
            }

            c_pop.Load(targu);
        }
    }
}