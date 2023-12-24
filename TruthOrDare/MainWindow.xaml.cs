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
using System.Speech.Synthesis;

namespace TruthOrDare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        public bool isCountDown = false;
        public bool isTimerBlinking = false;
        public bool _13 = true;
        public bool _18 = false;
        public bool autoplayVar = false;

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

        public string GetUrl(string url) 
        {
            string myUrl = url;
            Random random = new Random();
            if (_13 && _18)
            {
                int n = random.Next(0, 3);
                switch (n)
                {
                    case 0:
                        myUrl = url + "?rating=r";
                        break;
                    case 1:
                        myUrl = url + "?rating=pg13";
                        break;
                    case 2:
                        myUrl = url + "?rating=pg";
                        break;
                }
            }
            else if (_13 == true && _18 == false)
            {
                int n = random.Next(0, 2);
                switch (n)
                {
                    case 0:
                        myUrl = url + "?rating=pg13";
                        break;
                    case 1:
                        myUrl = url + "?rating=pg";
                        break;
                }
            }
            else if (_13 == false && _18 == true)
            {
                int n = random.Next(0, 2);
                switch (n)
                {
                    case 0:
                        myUrl = url + "?rating=r";
                        break;
                    case 1:
                        myUrl = url + "?rating=pg";
                        break;
                }
            }
            else 
            {
                myUrl = url + "?rating=pg";
            }
            return myUrl;
        }

        public void CharacterControl() 
        {
            if (mainText.FontSize > 30) 
            {
                mainText.FontSize = 30;
            }
            if (mainText.Text.Length > 160)
            {
                mainText.FontSize = 16;
            }
            else if (mainText.Text.Length > 61)
            {
                mainText.FontSize = 22;
            }
            else if (mainText.Text.Length <= 61) 
            {
                mainText.FontSize = 30;
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

        public void ShowCopyText() 
        {
            if (copied.Visibility == Visibility.Visible) 
            {
                return;
            }
            Thread thread = new Thread(ShowCopyTextThread);
            thread.Start();
        } 

        public void ShowCopyTextThread() 
        {
            this.Dispatcher.Invoke(() =>
            {
                copied.Visibility = Visibility.Visible;
            }
            );
            Thread.Sleep(2000);
            this.Dispatcher.Invoke(() =>
            {
                copied.Visibility = Visibility.Hidden;
            }
            ); 
        }

        private void Copy(object sender, RoutedEventArgs e) 
        {
            Clipboard.SetText(mainText.Text);
            ShowCopyText();
        }

        private void DayMode(object sender, RoutedEventArgs e) 
        {
            bgimg.Visibility = System.Windows.Visibility.Hidden;
            Canvas.SetTop(snowOverlay, 183);
        }

        private void NightMode(object sender, RoutedEventArgs e)
        {
            bgimg.Visibility = System.Windows.Visibility.Visible;
            Canvas.SetTop(snowOverlay, 225);
        }

        private void checkedd(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Name == "autoplay") 
            {
                autoplayVar = true;
                return;
            }

            if ((sender as CheckBox).Name == "m18")
            {
                _18 = true;
            }
            else if ((sender as CheckBox).Name == "m13") 
            {
                _13 = true;
            }
        }
        private void uncheckedd(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Name == "autoplay")
            {
                autoplayVar = false;
                return;
            }

            if ((sender as CheckBox).Name == "m18")
            {
                _18 = false;
            }
            else if ((sender as CheckBox).Name == "m13")
            {
                _13 = false;
            }
        }

        public void PlayTTS(string text) 
        {
            Thread thread = new Thread(()=>TTSThread(text));
            thread.Start();
        }
        public void TTSThread(string text) 
        {
            SpeechSynthesizer ss = new SpeechSynthesizer();
            ss.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
            ss.Speak(text);
        }
        private void AudioPlayClick(object sender, RoutedEventArgs e) 
        {
            PlayTTS(mainText.Text);
        }

        private void TruthClick(object sender, RoutedEventArgs e) 
        {
            if (isCountDown) 
            {
                TimerBlinkRed();
                return;
            }
            string content = Get(GetUrl("https://api.truthordarebot.xyz/v1/truth"));
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
                        if (autoplayVar)
                        {
                            PlayTTS(mainText.Text);
                        }
                        CharacterControl();
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
            string content = Get(GetUrl("https://api.truthordarebot.xyz/v1/dare"));
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
                        if (autoplayVar) 
                        {
                            PlayTTS(mainText.Text);
                        }
                        BeginCountDown();
                        CharacterControl();
                        break;
                    }
                }
            }
        }
    }
}
