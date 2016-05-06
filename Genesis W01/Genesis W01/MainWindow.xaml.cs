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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Genesis_W01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        char hd;
        int td=30;
        Bitmap bt = new Bitmap(480, 480);
        int height = 460, width = 460, tw, th;
        int mid = 200;
        int cout = 0;
        String dt_arduino;
        // String[] test = { "12","0","9","w","12","0","9","w","12","0","9","w","12","0","9","w"};
        String[] test = new string[1000];
        String[] dt_pre_processed, dt_processed;
        String[] final_data;
        int d1, d2, d3, heading;
        SerialPort sp = new SerialPort();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(" Developed for Autonomous and Manual Navigation Monitoring for UGV Genesis A01 \n Made in India, by Indian and for Indian \n Avirup Basu, 2016");
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            //plot_map();
            sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        private void manual_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("It will be activated soon!");
        }

        private void autonomous_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("It will be activated soon!");
        }

        private void manual_ctrl_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void manual_ctrl_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            plot_map();
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String portName = comno.Text;
                sp.PortName = portName;
                //sp.BaudRate = 9600;
                sp.Open();
                connect.Content = "Connected";
                sp.DtrEnable = true;
                //sp.RtsEnable = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Please give a valid port number or check your connection");
            }
        }

        private void disconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.Close();
                connect.Content = "Connect";
            }
            catch (Exception)
            {

                MessageBox.Show("You are not connected yet! First connect.");
            }

        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            dt_arduino = sp.ReadTo("arg");
            Dispatcher.Invoke((Action)(() => getData()));
        }

        private void getData()
        {
            int dist_front = 0, dist_right = 0, dist_left = 0;
            try
            {
                dt_pre_processed = dt_arduino.Split('+');
                dist_front = int.Parse(dt_pre_processed[1]);
                dist_left = int.Parse(dt_pre_processed[2]);
                dist_right = int.Parse(dt_pre_processed[3]);
                if(dist_front<=td)
                {
                    front.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                }
                else
                {
                    front.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
                }
                if (dist_right<= td)
                {
                    right.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                }
                else
                {
                    right.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
                }
                if (dist_left <= td)
                {
                    left.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                }
                else
                {
                    left.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
                }
                String dt1 = dt_pre_processed[2];
                String dt2 = dt_pre_processed[3];
                //  d3 = Int32.Parse(dt_pre_processed[3]);
                heading = Int32.Parse(dt_pre_processed[4]);
                RotateTransform rotateTransform = new RotateTransform(heading);
                needle.RenderTransform = rotateTransform;
                heading_number.Text = heading.ToString();
                if ((heading <= 30&&heading>=0) || (heading >= 330&&heading<=359))
                    hd = 'n';
                if (heading <= 320 && heading >= 250)
                    hd = 'w';
                if (heading <= 200 && heading >= 160)
                    hd = 'e';
                test[cout] = dt1;
                test[cout+1] = dt2;
                test[cout+2] = hd.ToString();
                hhd.Text = hd.ToString();
                cout +=3;
                dd1.Text = cout.ToString();
                dd2.Text = dt2;
            }
            catch (Exception)
            {
                MessageBox.Show("Some error occured! Sorry");
                //throw;
            }
        }
        private ImageSource Bitmap2ImageSource(Bitmap bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                MemoryStream ms = new MemoryStream();
                bitmapImage.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                bi.Freeze();
                return bi;
            }
        }
        private void plot_map()
        {
            int i = 0;
            height = height * 2;
            for (i=0;i<cout-2;i+=3)
            {
                d1 = Int32.Parse(test[i]);
                d2 = Int32.Parse(test[i+1]);
                if (d1 > td)
                    d1 = 0;
                if (d2 > td)
                    d2 = 0;
                String hdd = test[i+2];
                hd = hdd[0];
                if (hd == 'n')
                {
                    bt.SetPixel((width / 2) - d1, height / 2, System.Drawing.Color.White);
                    bt.SetPixel((width / 2) + d2, height / 2, System.Drawing.Color.White);
                    bt.SetPixel(width / 2, height / 2, System.Drawing.Color.Red);
                    height = height - 10;
                }
                if (hd == 'e')
                {
                    bt.SetPixel(width / 2, (height / 2) - d1, System.Drawing.Color.White);
                    bt.SetPixel(width / 2, (height / 2) + d2, System.Drawing.Color.White);
                    bt.SetPixel(width / 2, height / 2, System.Drawing.Color.Red);
                    width = width + 10;
                }
                if (hd == 'w')
                {
                    bt.SetPixel(width / 2, (height / 2) + d1, System.Drawing.Color.White);
                    bt.SetPixel(width / 2, (height / 2) - d2, System.Drawing.Color.White);
                    bt.SetPixel(width / 2, height / 2, System.Drawing.Color.Red);
                    width = width - 10;
                }
                if (hd == 's')
                {
                    bt.SetPixel((width / 2) - d1, height / 2, System.Drawing.Color.White);
                    bt.SetPixel((width / 2) + d2, height / 2, System.Drawing.Color.White);
                    bt.SetPixel(width / 2, height / 2, System.Drawing.Color.Red);
                    height = height + 10;
                }

                //RotateTransform rotateTransform = new RotateTransform(heading);
                //bt.RotateFlip(RotateFlipType.Rotate180FlipY);
                bt.RotateFlip(RotateFlipType.Rotate180FlipXY);
                ImageSource img = Bitmap2ImageSource(bt);
                imBox.Source = img;
            }
        }
    }
}
