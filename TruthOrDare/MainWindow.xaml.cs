using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using Newtonsoft.Json; 

namespace TruthOrDare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isCountDown = false;
        public bool isTimerBlinking = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        
        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void BeginCountDown() 
        {
            Thread thread = new Thread(CountDown);
            thread.Start();
            isCountDown = true;
            Timer.Foreground = Brushes.Black;
        }

        public void CountDown() 
        {
            for (int i = 12; i >= 0; i--) 
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (i >= 10)
                    {
                        Timer.Content = i;
                    }
                    else 
                    {
                        Timer.Content = "0" + i;
                    }
                }
                );
                Thread.Sleep(1000);
            }
            this.Dispatcher.Invoke(() => { Timer.Foreground = Brushes.Gray; });
            isCountDown = false;
        }

        public void TimerBlinkRed() 
        {
            if (isTimerBlinking) 
            {
                return;
            }
            Thread thread = new Thread(TimerBlinkRedThread);
            thread.Start();
            isTimerBlinking = true;
        }

        public void TimerBlinkRedThread() 
        {
            this.Dispatcher.Invoke(() =>
            {
                Timer.Foreground = Brushes.Red;
            }
            );
            Thread.Sleep(500);
            isTimerBlinking = false;
            this.Dispatcher.Invoke(() =>
            {
                Timer.Foreground = Brushes.Black;
            }
            );
        }

        private void TruthClick(object sender, RoutedEventArgs e) 
        {
            if (isCountDown) 
            {
                TimerBlinkRed();
                return;
            }
            string content = Get("https://api.truthordarebot.xyz/v1/truth");
            //string truthText = JsonConvert.DeserializeObject<string>(content);
            JsonTextReader reader = new JsonTextReader(new StringReader(content));
            while (reader.Read()) 
            {
                if (reader.Value != null) 
                {
                    if (reader.Value.Equals("question"))
                    {
                        reader.Read();
                        mainText.Text = (string)reader.Value;
                        BeginCountDown();
                        break;
                    }
                }
            }
        }
        private void DareClick(object sender, RoutedEventArgs e) 
        {
            if (isCountDown)
            {
                TimerBlinkRed();
                return;
            }
            string content = Get("https://api.truthordarebot.xyz/v1/dare");
            //string truthText = JsonConvert.DeserializeObject<string>(content);
            JsonTextReader reader = new JsonTextReader(new StringReader(content));
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    if (reader.Value.Equals("question"))
                    {
                        reader.Read();
                        mainText.Text = (string)reader.Value;
                        BeginCountDown();
                        break;
                    }
                }
            }
        }
    }
}
