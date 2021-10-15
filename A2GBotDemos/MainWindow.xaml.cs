using Newtonsoft.Json;

using RestSharp;

using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace A2GBotDemos
{
    public class WindowConfig : INotifyPropertyChanged
    {
        private string apiKey;
        private string aKey;
        private string iKey;
        private string jsonCfg;
        
        public string ApiKey
        {
            get { return apiKey; }
            set {
                if (apiKey != value)
                {
                    apiKey = value;
                    RaisePropertyChanged("ApiKey");
                }
            }
        }

        public string AKey
        {
            get { return aKey; }
            set
            {
                if (aKey != value)
                {
                    aKey = value;
                    RaisePropertyChanged("AKey");
                }
            }
        }

        public string IKey
        {
            get { return iKey; }
            set
            {
                if (iKey != value)
                {
                    iKey = value;
                    RaisePropertyChanged("IKey");
                }
            }
        }

        public string JsonCfg
        {
            get { return jsonCfg; }
            set
            {
                if (jsonCfg != value)
                {
                    jsonCfg = value;
                    RaisePropertyChanged("JsonCfg");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RestClient _client;

        string activationUrl = "/triggeralert/alert";
        string streamUrl = "/v1/production/inputstream";

        public MainWindow()
        {            
            InitializeComponent();  
            _client = new RestClient("https://listen.a2g.io");
        }

        private void TriggerBotManual(object sender, RoutedEventArgs e)
        {
            var dataContext = this.DataContext as WindowConfig;
            dynamic payload = new
            {
                AKEY = dataContext.AKey,
                Payload = dataContext.JsonCfg
            };

            RestRequest request = new RestRequest(activationUrl, Method.POST);
            request.AddHeader("X-API-KEY", dataContext.ApiKey);
            request.AddJsonBody(payload);

            var response = _client.Execute(request);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {                
                OutputInfo.Text += $"\nBot activated successfully, message: {response.Content}";
            }
            else
            {
                OutputInfo.Text += $"\nBot activation has failed, message: {response.Content}";
            }
        }

        private void GeneratePatternPayload(object sender, RoutedEventArgs e)
        {
            var dataContext = this.DataContext as WindowConfig;
            dynamic payload = new
            {
                IKEY = dataContext.IKey,
                Data = dataContext.JsonCfg
            };

            RestRequest request = new RestRequest(streamUrl, Method.POST);
            request.AddHeader("X-API-KEY", dataContext.ApiKey);
            request.AddJsonBody(payload);

            var response = _client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                OutputInfo.Text += $"\nData Sent successfully, message: {response.Content}";
            }
            else
            {
                OutputInfo.Text += $"\nData Transmission has failed, message: {response.Content}";
            }
        }

        private void GenerateAnomalyPayloadNormal(object sender, RoutedEventArgs e)
        {
            var dataContext = this.DataContext as WindowConfig;
            Task.Factory.StartNew(() => { GenerateNormalPayload(dataContext); });
        }

        private void GenerateNormalPayload(WindowConfig dataContext)
        {
            Random r = new Random();

            // We generate 256 data ticks

            for (var i = 1; i <= 256; i++)
            {
                dynamic dataInfo = new
                {
                    Sensor = "Sensor 1",
                    Temperature = r.Next(30, 35),
                    Pressure = r.Next(80, 90),
                    MeasuredOn = DateTime.UtcNow.ToString("s")
                };

                dynamic payload = new
                {
                    IKEY = dataContext.IKey,
                    Data = JsonConvert.SerializeObject(dataInfo)
                };

                RestRequest request = new RestRequest(streamUrl, Method.POST);
                request.AddHeader("X-API-KEY", dataContext.ApiKey);
                request.AddJsonBody(payload);

                var response = _client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string entry = $"\nSent payload N°{ i }, message: { response.Content }";
                    Dispatcher.BeginInvoke((Action)(() => OutputInfo.Text += entry));
                }
                else
                {
                    string entry = $"\nPayload N°{i} Tx has failed, message: {response.Content}";
                    Dispatcher.BeginInvoke((Action)(() => OutputInfo.Text += entry));
                }
                Thread.Sleep(1);
            }
        }

        private void GenerateAnomalyPayloadActivation(object sender, RoutedEventArgs e)
        {
            // We generate 256 data ticks, tick N°200 will be abnormal
            var dataContext = this.DataContext as WindowConfig;
            Task.Factory.StartNew(() => { GenData(dataContext); });
        }

        private void GenData(WindowConfig dataContext)
        {
            Random r = new Random();
            for (var i = 1; i <= 256; i++)
            {
                dynamic dataInfo = new
                {
                    Sensor = "Sensor 1",
                    Temperature = (i < 200 || i > 201) ? r.Next(30, 35) : r.Next(100, 110),
                    Pressure = r.Next(80, 90),
                    MeasuredOn = DateTime.UtcNow.ToString("s")
                };

                dynamic payload = new
                {
                    IKEY = dataContext.IKey,
                    Data = JsonConvert.SerializeObject(dataInfo)
                };

                RestRequest request = new RestRequest(streamUrl, Method.POST);
                request.AddHeader("X-API-KEY", dataContext.ApiKey);
                request.AddJsonBody(payload);

                var response = _client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string entry = $"\nSent payload N°{ i }, message: { response.Content }";
                    Dispatcher.BeginInvoke((Action)(() => OutputInfo.Text += entry));
                }
                else
                {
                    string entry = $"\nPayload N°{i} Tx has failed, message: {response.Content}";
                    Dispatcher.BeginInvoke((Action)(() => OutputInfo.Text += entry));
                }

                Thread.Sleep(1);
            }
        }        

        private void InputSample_LostFocus(object sender, RoutedEventArgs e)
        {
            string data = InputSample.Text;
            try
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(data);
                InputSample.Text = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            catch (Exception ex)
            {
                OutputInfo.Text += $"\nInvalid JSON on Input Sample";
            }
        }
    }
}