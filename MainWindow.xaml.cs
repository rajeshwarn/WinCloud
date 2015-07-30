using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using System.IO;
using System.Net;
using MahApps.Metro.Controls;
using MahApps.Metro;
using System.Security.Cryptography;
using WindowsInput;
using MahApps.Metro.Controls.Dialogs;

namespace WinCloud
{
    public partial class MainWindow : MetroWindow
    {
        string vnumb = "0.0.1";
        string vtype = "ALPHA RELEASE";
        string changeset = "- Add option to consult the wiki on GitHub."
            + System.Environment.NewLine +
            "- Add option to submit an issue to GitHub."
            + System.Environment.NewLine +
            "- Replace WPF MessageBox with MahApps one."
            + System.Environment.NewLine +
            "- Fix title and content issue with web link.";
        string version;

        int notifenable = 0;
        int notitime = 5000;
        WindowState modechange;
        string notisource;
        int snozee;
        int trayedoption = 0;
        int beta;
        int minimi;
        int startingup = 1;
        string prevnoti = "";
        string c_key = "dhJFOhTfgnOUwJKOtdcNI9==";
        string c_iv = "kOTfHJVjuwNbMNxhoEWnKo==";
        string encry;
        string decry;

        public MainWindow()
        {
            version = vtype + " " + vnumb;

            Process Currentproc = Process.GetCurrentProcess();

            Process[] procByName = Process.GetProcessesByName("WinCloud");
            if (procByName.Length > 1)
            {
                MessageBox.Show("WinCloud is already running!", "Application Running", MessageBoxButton.OK, MessageBoxImage.Stop);
                App.Current.Shutdown();
            }

            InitializeComponent();

            Properties.Settings.Default.ctrlsetting = 0;
            Properties.Settings.Default.Save();

            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            c_web.DownloadHandler = new DownloadHandler();
            c_web.LifeSpanHandler = new LifeSpanHandler();
        }

        public class LifeSpanHandler : ILifeSpanHandler
        {
            public bool OnBeforePopup(IWebBrowser c_web, string sourceUrl, string targetUrl, ref int x, ref int y, ref int width, ref int height)
            {
                if (targetUrl.Contains("create#instanceId") == false && Properties.Settings.Default.ctrlsetting == 0 && targetUrl.Contains("pages") == true || targetUrl.Contains("create#instanceId") == false && Properties.Settings.Default.ctrlsetting == 0 && targetUrl.Contains("numbers") == true || targetUrl.Contains("create#instanceId") == false && Properties.Settings.Default.ctrlsetting == 0 && targetUrl.Contains("keynote") == true)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        DelayAction(500, new Action(() =>
                        {
                            pressenter();
                        }));

                        DelayAction(600, new Action(() =>
                        {
                            var popupmain = new popupmain(targetUrl);
                            popupmain.Show();
                        }));
                    }));

                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void pressenter()
            {
                InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
            }

            public void OnBeforeClose(IWebBrowser browser)
            { }
        }

        public class DownloadHandler : IDownloadHandler
        {
            public bool OnBeforeDownload(DownloadItem downloadItem, out string downloadPath, out bool showDialog)
            {
                string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string pathDownload = System.IO.Path.Combine(pathUser, "Downloads");

                downloadPath = pathDownload + "\\" + downloadItem.SuggestedFileName;
                showDialog = true;

                return true;
            }

            public bool OnDownloadUpdated(DownloadItem downloadItem)
            {
                return false;
            }
        }

        private void w_main_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.theme == "Blue")
            {
                ts_theme.IsChecked = true;
            }
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Properties.Settings.Default.theme), ThemeManager.GetAppTheme("BaseLight"));

            versioncontrol.Content = version;
            changelogtext.Content = changeset;

            if (Properties.Settings.Default.fs == true)
            {
                this.WindowState = WindowState.Maximized;
                ts_fully.IsChecked = true;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                ts_fully.IsChecked = false;
            }

            notifenable = Properties.Settings.Default.noti;
            if (notifenable == 1)
            {
                ts_desknotif.IsChecked = true;
                DelayAction(notitime, new Action(() => { notifcheck(); }));
            }
            else
            {
                b_snotray.IsEnabled = false;
                b_snotray.Fill = Brushes.LightSlateGray;

                b_clearnoti.IsEnabled = false;
                ts_snozee.IsEnabled = false;
                ts_desknotifsnooze.IsEnabled = false;
                mNotiIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + @"Resources\logo_snozed.ico");
            }

            beta = Properties.Settings.Default.beta;
            if (beta == 1)
            {
                ts_ibeta.IsChecked = true;

                string sourcebeta = "https://beta.icloud.com/";
                c_web.Load(sourcebeta);
            }
            else
            {
                ts_ibeta.IsChecked = false;

                string sourcebeta = "https://www.icloud.com/";
                c_web.Load(sourcebeta);
            }

            minimi = Properties.Settings.Default.minimi;
            if (minimi == 1)
            {
                ts_minitray.IsChecked = true;
            }
            else
            {
                ts_minitray.IsChecked = false;
            }

            if (Properties.Settings.Default.shasno == 1)
            {
                ts_desknotifsnooze.IsChecked = true;
                snoozeed();
            }

            if (Properties.Settings.Default.user == "" || Properties.Settings.Default.pass == "")
            {
                l_user.Content = "Username";
                tb_user.IsEnabled = true;
                l_pass.Content = "Password";
                tb_pass.IsEnabled = true;
                b_save.Content = "Save Credential";
            }
            else
            {
                l_user.Content = "Username - Encrypted";
                tb_user.IsEnabled = false;
                tb_user.Text = Properties.Settings.Default.user;
                l_pass.Content = "Password - Encrypted";
                tb_pass.IsEnabled = false;
                tb_pass.Password = Properties.Settings.Default.pass;
                b_save.Content = "Reset Credential";
                b_save.IsTabStop = false;
            }

            var settings = new CefSettings();
            settings.CachePath = AppDomain.CurrentDomain.BaseDirectory + @"cache";

            Cef.Initialize(settings);
            Cef.SetCookiePath(AppDomain.CurrentDomain.BaseDirectory + @"cache", true);

            startingup = 0;
        }

        private void w_main_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            string currentadress = c_web.Address.ToString();

            if (currentadress.Contains("pages") == true || currentadress.Contains("numbers") == true || currentadress.Contains("keynote") == true)
            {
                w_main.ContextMenu.Visibility = Visibility.Collapsed;
            }
            else
            {
                w_main.ContextMenu.Visibility = Visibility.Visible;
            }
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                if (Properties.Settings.Default.ctrlsetting == 0)
                {
                    Properties.Settings.Default.ctrlsetting = 1;
                    Properties.Settings.Default.Save();

                    mi_natpop.Header = "Press F12 to open Native Pop-Up";
                }
                else
                {
                    Properties.Settings.Default.ctrlsetting = 0;
                    Properties.Settings.Default.Save();

                    mi_natpop.Header = "Press F12 to open Chrome Pop-Up";
                }
            }

            if (e.Key == Key.Escape)
            {
                string sourceupdate = "https://www.icloud.com/";
                c_web.Load(sourceupdate);
            }
            if (e.Key == Key.F1)
            {
                string sourceupdate = "https://www.icloud.com/#mail";
                c_web.Load(sourceupdate);
            }
            if (e.Key == Key.F2)
            {
                string sourceupdate = "https://www.icloud.com/#contacts";
                c_web.Load(sourceupdate);
            }
            if (e.Key == Key.F3)
            {
                string sourceupdate = "https://www.icloud.com/#calendar";
                c_web.Load(sourceupdate);
            }
            if (e.Key == Key.F4)
            {
                string sourceupdate = "https://www.icloud.com/#photos";
                c_web.Load(sourceupdate);
            }
            if (e.Key == Key.F5)
            {
                string sourceupdate = "https://www.icloud.com/#iclouddrive";
                c_web.Load(sourceupdate);
            }
            if (e.Key == Key.F6)
            {
                string sourceupdate = "https://www.icloud.com/#find";
                c_web.Load(sourceupdate);
            }
            if (e.Key == Key.F7)
            {
                string sourceupdate = "https://www.icloud.com/#notes2";
                c_web.Load(sourceupdate);
            }
            if (e.Key == Key.F8)
            {
                string sourceupdate = "https://www.icloud.com/#reminders";
                c_web.Load(sourceupdate);
            }
            if (e.Key == Key.F9)
            {
                string sourceupdate = "https://www.icloud.com/#pages";
                c_web.Load(sourceupdate);
            }
            if (e.Key == Key.F10)
            {
                string sourceupdate = "https://www.icloud.com/#numbers";
                c_web.Load(sourceupdate);
            }
            if (e.Key == Key.F11)
            {
                string sourceupdate = "https://www.icloud.com/#keynote";
                c_web.Load(sourceupdate);
            }

            if (e.Key == Key.PageDown)
            {
                if (Properties.Settings.Default.user != "" && Properties.Settings.Default.pass != "")
                {
                    if (c_web.Address.ToString() == "https://www.icloud.com/")
                    {
                        DecryptText(Properties.Settings.Default.user);
                        Clipboard.SetText(decry, TextDataFormat.Text);
                        c_web.Paste();

                        DelayAction(200, new Action(() => { tabsendpassword(); }));
                    }
                }
                else
                {
                    fo_main.IsOpen = false;
                    DialogManager.ShowMessageAsync(this, "Missing Account", "Set username and password in the flyout core menu.", MessageDialogStyle.Affirmative);
                }
            }

            if (e.Key == Key.PageUp)
            {
                if (Properties.Settings.Default.pass != "")
                {
                    if (c_web.Address.ToString() == "https://www.icloud.com/#find")
                    {
                        DecryptText(Properties.Settings.Default.pass);
                        Clipboard.SetText(decry, TextDataFormat.Text);
                        c_web.Paste();
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
                    }
                }
                else
                {
                    fo_main.IsOpen = false;
                    DialogManager.ShowMessageAsync(this, "Missing Account", "Set username and password in the flyout core menu.", MessageDialogStyle.Affirmative);
                }
            }
        }

        public void tabsendpassword()
        {
            InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
            DelayAction(200, new Action(() => { sendpassword(); }));
        }

        public void sendpassword()
        {
            DecryptText(Properties.Settings.Default.pass);
            Clipboard.SetText(decry, TextDataFormat.Text);
            c_web.Paste();
            InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
        }

        public bool GetDownloadHandler(IWebBrowser browser, string mimeType, string fileName, long contentLength, ref IDownloadHandler handler)
        {
            throw new NotImplementedException();
        }

        private void ts_minitray_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.minimi = 1;
            Properties.Settings.Default.Save();
        }

        private void ts_minitray_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.minimi = 0;
            Properties.Settings.Default.Save();
        }

        private void w_main_StateChanged(object sender, EventArgs e)
        {
            fo_main.IsOpen = false;

            if (Properties.Settings.Default.minimi == 1)
            {
                if (this.WindowState == WindowState.Minimized)
                {
                    trayed();
                }
            }
        }

        private void w_main_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            modechange = this.WindowState;
        }

        private void b_setting_Click(object sender, RoutedEventArgs e)
        {
            if (fo_main.IsOpen == true)
            {
                fo_main.IsOpen = false;
            }
            else
            {
                fo_main.IsOpen = true;
            }
        }

        private void b_snotray_Click(object sender, RoutedEventArgs e)
        {
            fo_main.IsOpen = false;
            snozee = 0;
            snoozeed();
            trayed();
        }

        private void ts_fully_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.fs = true;
            Properties.Settings.Default.Save();
        }

        private void ts_fully_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.fs = false;
            Properties.Settings.Default.Save();
        }

        private void ts_desknotif_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.noti = 1;
            Properties.Settings.Default.Save();

            desknotiques();
        }

        private void ts_desknotif_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.noti = 0;
            Properties.Settings.Default.Save();

            desknotiques();
        }

        private void ts_desknotifsnooze_Checked(object sender, RoutedEventArgs e)
        {
            snoozeed();

            Properties.Settings.Default.shasno = 1;
            Properties.Settings.Default.Save();
        }

        private void ts_desknotifsnooze_Unchecked(object sender, RoutedEventArgs e)
        {
            snoozeed();

            Properties.Settings.Default.shasno = 0;
            Properties.Settings.Default.Save();
        }

        public async void desknotiques()
        {
            if (startingup == 0)
            {
                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Restart Now",
                    NegativeButtonText = "Manualy Later",
                };
                fo_main.IsOpen = false;
                var diagresponse = await DialogManager.ShowMessageAsync(this, "Restart Needed", "Restart needed for change to be activated, do you want to restart now?", MessageDialogStyle.AffirmativeAndNegative, mySettings);

                switch (diagresponse)
                {
                    case MessageDialogResult.Affirmative:
                        Properties.Settings.Default.Save();
                        DelayAction(1000, new Action(() => { restart(); }));
                        break;

                    case MessageDialogResult.Negative:
                        break;
                }
            }
        }

        private void ts_snozee_UnAndChecked(object sender, RoutedEventArgs e)
        {
            snoozeed();
        }

        private void b_clearnoti_Click(object sender, RoutedEventArgs e)
        {
            fo_main.IsOpen = false;
            mNotiIcon.ToolTipText = "WinCloud Desktop";
            prevnoti = "";
            lb_notif.Items.Clear();
            b_clearnoti.Visibility = Visibility.Collapsed;
        }

        private void mNotiIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            snoozeed();
        }

        public void snoozeed()
        {
            if (notifenable == 1)
            {
                if (snozee == 0)
                {
                    mNotiIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + @"Resources\logo_snozed.ico");
                    ts_snozee.IsChecked = true;
                    snozee = 1;
                }
                else
                {
                    mNotiIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + @"Resources\logo.ico");
                    ts_snozee.IsChecked = false;
                    snozee = 0;
                }
            }
        }

        private void b_tray_Click(object sender, RoutedEventArgs e)
        {
            fo_main.IsOpen = false;
            trayed();
        }

        private void mNotiIcon_TrayRightMouseUp(object sender, RoutedEventArgs e)
        {
            trayed();
        }

        public void trayed()
        {
            if (trayedoption == 0)
            {
                this.Hide();
                trayedoption = 1;
            }
            else
            {
                this.Show();
                trayedoption = 0;

                this.WindowState = modechange;
            }
        }

        private void b_refresh_Click(object sender, RoutedEventArgs e)
        {
            fo_main.IsOpen = false;
            c_web.Reload();
            c_web.WebBrowser.Reload();
        }

        private void mNotiIcon_TrayMiddleMouseUp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mi_quitapp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ts_ibeta_Checked(object sender, RoutedEventArgs e)
        {
            ts_ibeta.IsChecked = true;

            string sourcebeta = "https://beta.icloud.com/";
            c_web.Load(sourcebeta);

            Properties.Settings.Default.beta = 1;
            Properties.Settings.Default.Save();

            beta = 1;
        }

        private void ts_ibeta_Unchecked(object sender, RoutedEventArgs e)
        {
            ts_ibeta.IsChecked = false;

            string sourcebeta = "https://www.icloud.com/";
            c_web.Load(sourcebeta);

            Properties.Settings.Default.beta = 0;
            Properties.Settings.Default.Save();

            beta = 0;
        }

        public void restart()
        {
            Process.Start(Application.ResourceAssembly.Location);
            this.Close();
        }

        public void notifcheck()
        {
            if (snozee == 0)
            {
                var task = c_web.EvaluateScriptAsync("document.getElementsByTagName('div')[2].textContent");

                task.ContinueWith(t =>
                {
                    if (!t.IsFaulted)
                    {
                        var response = t.Result;
                        if (response.Result.ToString() != "")
                        {
                            notisource = response.Result.ToString();
                            DateTime dt = DateTime.Now;
                            string validy = dt.ToString("h tt") + ": New " + notisource + " Notification";
                            if (prevnoti != validy)
                            {
                                System.Media.SoundPlayer player = new System.Media.SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + @"Resources\notification.wav");
                                player.Play();

                                prevnoti = dt.ToString("h tt") + ": New " + notisource + " Notification";

                                mNotiIcon.ToolTipText = dt.ToString("h:mm tt") + ": New " + notisource + " Notification";
                                lb_notif.Items.Add(dt.ToString("h:mm tt") + ": New " + notisource + " Notification");
                                mNotiIcon.ShowBalloonTip("New " + notisource + " Notification", "Click here to go to iCloud " + notisource, BalloonIcon.Info);

                                b_clearnoti.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());

                DelayAction(notitime, new Action(() => { notifcheck(); }));
            }
            else
            {
                DelayAction(notitime, new Action(() => { notifcheck(); }));
            }
        }

        public static void DelayAction(int millisecond, Action action)
        {
            var timer = new DispatcherTimer();
            timer.Tick += delegate
            {
                action.Invoke();
                timer.Stop();
            };

            timer.Interval = TimeSpan.FromMilliseconds(millisecond);
            timer.Start();
        }

        private void mNotiIcon_TrayBalloonTipClicked(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = modechange;

            string sourcenotif = "about:blank";
            switch (notisource)
            {
                case "Mail":
                    sourcenotif = "https://www.icloud.com/#mail";
                    break;
                case "Calendar":
                    sourcenotif = "https://www.icloud.com/#calendar";
                    break;
                case "Reminders":
                    sourcenotif = "https://www.icloud.com/#reminders";
                    break;
            }
            c_web.Load(sourcenotif);
        }

        private void tb_user_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                string passin = tb_user.Text;
                tb_user.Text = passin + "@icloud.com";
            }
        }

        private void b_save_Click(object sender, RoutedEventArgs e)
        {
            if (b_save.Content != "Reset Credential" && tb_user.Text == "" || b_save.Content != "Reset Credential" && tb_pass.Password == "")
            {
                fo_main.IsOpen = false;
                DialogManager.ShowMessageAsync(this, "Save Error - Missing Information", "Please set your username and password!", MessageDialogStyle.Affirmative);
            }
            else
            {
                pr_update.IsActive = true;

                if (tb_user.IsEnabled == false || b_save.IsEnabled == false)
                {
                    tb_user.Text = "";
                    tb_pass.Password = "";
                    b_save.Content = "Resetting Credential";

                    Properties.Settings.Default.user = tb_user.Text;
                    Properties.Settings.Default.pass = tb_pass.Password;
                    Properties.Settings.Default.Save();

                    b_save.IsEnabled = false;
                    tb_pass.IsEnabled = false;
                    tb_user.IsEnabled = false;

                    DelayAction(1000, new Action(() => { savedone(); }));
                }
                else
                {
                    b_save.Content = "Encrypting Credential";

                    EncryptText(tb_user.Text);
                    Properties.Settings.Default.user = encry;

                    EncryptText(tb_pass.Password);
                    Properties.Settings.Default.pass = encry;
                    Properties.Settings.Default.Save();

                    b_save.IsEnabled = false;
                    tb_pass.IsEnabled = false;
                    tb_user.IsEnabled = false;

                    DelayAction(5000, new Action(() => { savedone(); }));
                }
            }
        }

        public void savedone()
        {
            b_save.IsEnabled = true;

            if (tb_user.Text == "" || tb_pass.Password == "")
            {
                l_user.Content = "Username";
                tb_user.IsEnabled = true;
                l_pass.Content = "Password";
                tb_pass.IsEnabled = true;
                b_save.Content = "Save Credential";
                b_save.IsTabStop = true;
            }
            else
            {
                l_user.Content = "Username - Encrypted";
                tb_user.Text = Properties.Settings.Default.user;

                l_pass.Content = "Password - Encrypted";
                tb_pass.Password = Properties.Settings.Default.pass;

                b_save.Content = "Reset Credential";
                b_save.IsTabStop = false;
            }

            pr_update.IsActive = false;
        }

        public void EncryptText(string openText)
        {
            RC2CryptoServiceProvider rc2CSP = new RC2CryptoServiceProvider();
            ICryptoTransform encryptor = rc2CSP.CreateEncryptor(Convert.FromBase64String(c_key), Convert.FromBase64String(c_iv));
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    byte[] toEncrypt = Encoding.Unicode.GetBytes(openText);

                    csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
                    csEncrypt.FlushFinalBlock();

                    byte[] encrypted = msEncrypt.ToArray();

                    encry = Convert.ToBase64String(encrypted);
                }
            }
        }

        public void DecryptText(string encryptedText)
        {
            RC2CryptoServiceProvider rc2CSP = new RC2CryptoServiceProvider();
            ICryptoTransform decryptor = rc2CSP.CreateDecryptor(Convert.FromBase64String(c_key), Convert.FromBase64String(c_iv));
            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    List<Byte> bytes = new List<byte>();
                    int b;
                    do
                    {
                        b = csDecrypt.ReadByte();
                        if (b != -1)
                        {
                            bytes.Add(Convert.ToByte(b));
                        }

                    }
                    while (b != -1);

                    decry = Encoding.Unicode.GetString(bytes.ToArray());
                }
            }
        }

        public class TimedWebClient : WebClient
        {
            public int Timeout { get; set; }

            public TimedWebClient()
            {
                this.Timeout = 600000;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var objWebRequest = base.GetWebRequest(address);
                objWebRequest.Timeout = this.Timeout;
                return objWebRequest;
            }
        }

        private void ts_theme_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.theme = "Blue";
            Properties.Settings.Default.Save();

            var theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Blue"), ThemeManager.GetAppTheme("BaseLight"));
        }

        private void ts_theme_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.theme = "Amber";
            Properties.Settings.Default.Save();

            var theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Amber"), ThemeManager.GetAppTheme("BaseLight"));
        }

        private void lb_notif_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string selecitm = lb_notif.SelectedValue.ToString();
            string sourcenotif = "about:blank";

            if (selecitm.Contains("Mail"))
            {
                sourcenotif = "https://www.icloud.com/#mail";
            }
            if (selecitm.Contains("Calendar"))
            {
                sourcenotif = "https://www.icloud.com/#calendar";
            }
            if (selecitm.Contains("Reminders"))
            {
                sourcenotif = "https://www.icloud.com/#reminders";
            }
            c_web.Load(sourcenotif);

            prevnoti = "";
            lb_notif.Items.Remove(lb_notif.SelectedItem);
            fo_main.IsOpen = false;

            if (lb_notif.Items.Count == 0)
            {
                mNotiIcon.ToolTipText = "WinCloud Desktop";
                b_clearnoti.Visibility = Visibility.Collapsed;
            }
        }

        private async void b_web_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Project Page",
                NegativeButtonText = "Application Wiki",
                FirstAuxiliaryButtonText = "Submit An Issue",
                SecondAuxiliaryButtonText = "Cancel"
            };
            fo_main.IsOpen = false;
            var diagresponse = await DialogManager.ShowMessageAsync(this, "WinCloud to GitHub", "Where would you like to go?", MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary, mySettings);

            switch (diagresponse)
            {
                case MessageDialogResult.Affirmative:
                    Process.Start("https://github.com/QuantumVectors/WinCloud/");
                    break;

                case MessageDialogResult.Negative:
                    Process.Start("https://github.com/QuantumVectors/WinCloud/wiki");
                    break;

                case MessageDialogResult.FirstAuxiliary:
                    Process.Start("https://github.com/QuantumVectors/WinCloud/issues/new");
                    break;

                case MessageDialogResult.SecondAuxiliary:
                    break;
            }
        }

        public static async Task<MessageDialogResult> ShowMessage(string title, string message, MessageDialogStyle dialogStyle)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            return await metroWindow.ShowMessageAsync(title, message, dialogStyle, metroWindow.MetroDialogOptions);
        }

        private void b_web_MouseEnter(object sender, MouseEventArgs e)
        {
            b_setting.Fill = Brushes.WhiteSmoke;
        }

        private void b_web_MouseLeave(object sender, MouseEventArgs e)
        {
            b_setting.Fill = Brushes.White;
        }

        private void b_tray_MouseEnter(object sender, MouseEventArgs e)
        {
            b_tray.Fill = Brushes.WhiteSmoke;
        }

        private void b_tray_MouseLeave(object sender, MouseEventArgs e)
        {
            b_tray.Fill = Brushes.White;
        }

        private void b_snotray_MouseEnter(object sender, MouseEventArgs e)
        {
            b_snotray.Fill = Brushes.WhiteSmoke;
        }

        private void b_snotray_MouseLeave(object sender, MouseEventArgs e)
        {
            b_snotray.Fill = Brushes.White;
        }

        private void b_clearnoti_MouseEnter(object sender, MouseEventArgs e)
        {
            b_clearnoti.Fill = Brushes.WhiteSmoke;
        }

        private void b_clearnoti_MouseLeave(object sender, MouseEventArgs e)
        {
            b_clearnoti.Fill = Brushes.White;
        }

        private void b_refresh_MouseEnter(object sender, MouseEventArgs e)
        {
            b_refresh.Fill = Brushes.WhiteSmoke;
        }

        private void b_refresh_MouseLeave(object sender, MouseEventArgs e)
        {
            b_refresh.Fill = Brushes.White;
        }

        private void b_setting_MouseEnter(object sender, MouseEventArgs e)
        {
            b_setting.Fill = Brushes.WhiteSmoke;
        }

        private void b_setting_MouseLeave(object sender, MouseEventArgs e)
        {
            b_setting.Fill = Brushes.White;
        }
    }
}