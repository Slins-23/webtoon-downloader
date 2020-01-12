using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Media;

namespace WToon
{
    public partial class MainWindow : Window
    {
        public static MainWindow AppWindow;

        public static bool cancel = false;
        public static string saveFolder;
        public static List<object> toons = new List<object>();

        public class Toon
        {
            public string Title { get; set; }
            public string Url { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            Verbose.Text = "";
            AppWindow = this;
        }



        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {

            if (App.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                App.Current.MainWindow.WindowState = WindowState.Normal;
            }

            else
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Minimize_MouseEnter(object sender, MouseEventArgs e)
        {
            //MOUSE ENTERED MINIMIZE
        }

        private void Minimize_MouseLeave(object sender, MouseEventArgs e)
        {
            //MOUSE LEFT MINIMIZE
        }

        private void Maximize_MouseEnter(object sender, MouseEventArgs e)
        {
            //MOUSE ENTERED MAXIMIZE
        }

        private void Maximize_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //MOUSE LEFT MAXIMIZE
        }

        private void Close_MouseEnter(object sender, MouseEventArgs e)
        {

            //var scale = new ScaleTransform() { ScaleX = 1.3, ScaleY = 1.3 };

            //var RTO = new Point() { X = 0.5, Y = 0.5 };

            //Close.RenderTransformOrigin = RTO;
            //Close.RenderTransform = scale;

            

            //MOUSE ENTERED CLOSE
            //var animation = new DoubleAnimation() { From = 1, To = 0.5, Duration = TimeSpan.FromMilliseconds(500) };
           // var scale = new DoubleAnimation() { From=30, }

            //Storyboard.SetTarget(animation, Close);
            //Storyboard.SetTargetProperty(animation, new PropertyPath(Button.OpacityProperty));
            //Storyboard.SetTargetProperty(animation, new PropertyPath(Button.HeightProperty));

            //var sb = new Storyboard();

            //sb.Children.Add(animation);

           // sb.Begin();
        }

        private void Close_MouseLeave(object sender, MouseEventArgs e)
        {
            var scale = new ScaleTransform() { ScaleX = 1, ScaleY = 1 };

            var RTO = new Point() { X = 0.5, Y = 0.5 };

            Close.RenderTransformOrigin = RTO;
            Close.RenderTransform = scale;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (!cancel)
            {
                StartBtn.IsEnabled = false;
                StopButton.IsEnabled = true;
                ClearQueue.IsEnabled = false;
                DeleteSelected.IsEnabled = false;
                Functionality.StartDownload();
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Verbose.Text += "Stopping...\n";

            if (!cancel)
            {
                StopButton.IsEnabled = false;
                cancel = true;
                ClearQueue.IsEnabled = true;
                DeleteSelected.IsEnabled = false;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

            if (ToonTitle.Text == "Enter title" || ToonUrl.Text == "Enter url")
            {
                Verbose.Text += "Please input a valid entry." + "\n";
                return;
            }

            if (ToonUrl.Text.Contains("https://www.webtoons.com"))
            {
                ToonUrl.Text = ToonUrl.Text.Replace("https://www.webtoons.com", "");
            }

            Toon tun = new Toon() { Title = ToonTitle.Text, Url = ToonUrl.Text };
            toons.Add(tun);
            QueueList.Items.Add(tun);

            if (!StartBtn.IsEnabled)
            {
                StartBtn.IsEnabled = true;
                HtmlCheckbox.IsEnabled = true;
                HtmlCheckbox.Foreground = new SolidColorBrush(Colors.White);
            }

            if (!ClearQueue.IsEnabled)
            {
                ClearQueue.IsEnabled = true;
            }

            ToonTitle.Text = "Enter title";
            ToonUrl.Text = "Enter url";
            ToonTitle.Foreground = new SolidColorBrush(Colors.DarkGray);
            ToonUrl.Foreground = new SolidColorBrush(Colors.DarkGray);
        }

        private void SaveDir_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog diag = new CommonOpenFileDialog();
            diag.IsFolderPicker = true;

            if (diag.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SaveDir.Text = diag.FileName + '\\';
                Properties.Settings.Default.folder = SaveDir.Text;
                Properties.Settings.Default.Save();
                saveFolder = diag.FileName + '\\';
                AddButton.IsEnabled = true;
                ToonTitle.IsEnabled = true;
                ToonUrl.IsEnabled = true;
            }
        }

        private void ToonTitle_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ToonTitle.Text == "Enter title")
            {
                ToonTitle.Text = "";
                ToonTitle.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private void ToonTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ToonTitle.Text))
            {
                ToonTitle.Text = "Enter title";
                ToonTitle.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
        }

        private void ToonUrl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ToonUrl.Text == "Enter url")
            {
                ToonUrl.Text = "";
                ToonUrl.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private void ToonUrl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ToonUrl.Text))
            {
                ToonUrl.Text = "Enter url";
                ToonUrl.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
        }

        private void ClearQueue_Click(object sender, RoutedEventArgs e)
        {
            QueueList.Items.Clear();
            toons.Clear();
            ClearQueue.IsEnabled = false;
        }

        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            toons.RemoveAt(QueueList.SelectedIndex);
            QueueList.Items.RemoveAt(QueueList.SelectedIndex);
            DeleteSelected.IsEnabled = false;
        }

        private void QueueList_Selected(object sender, RoutedEventArgs e)
        {
            DeleteSelected.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.folder != "")
            {
                SaveDir.Text = Properties.Settings.Default.folder;
                saveFolder = Properties.Settings.Default.folder;
                HtmlCheckbox.IsChecked = Properties.Settings.Default.HtmlChecked;

                AddButton.IsEnabled = true;
                ToonTitle.IsEnabled = true;
                ToonUrl.IsEnabled = true;
            }
        }

        private void HtmlCheckbox_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.HtmlChecked = HtmlCheckbox.IsChecked.Value;
            Properties.Settings.Default.Save();
        }
    }
}
