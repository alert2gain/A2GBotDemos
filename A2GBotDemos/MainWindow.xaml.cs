using A2GBotDemos.ViewModels;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace A2GBotDemos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// RestSharp client that connects to A2G.IO
        /// </summary>
        RestClient _client;

        /// <summary>
        /// Edge bot activation URL
        /// </summary>
        string activationUrl = "/triggeralert/alert";

        /// <summary>
        /// InputStream Streaming data URL
        /// </summary>
        string streamUrl = "/v1/production/inputstream";

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainWindow()
        {            
            InitializeComponent();  
            _client = new RestClient("https://listen.a2g.io");
        }

        /// <summary>
        /// Triggers an activation of an Edge Bot using Window Parameters (VM)
        /// </summary>
        /// <param name="sender">WPF element that triggered the event</param>
        /// <param name="e">Event information</param>
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

        /// <summary>
        /// Sends a payload to an InputStream using the using Window Parameters (VM)
        /// </summary>
        /// <param name="sender">WPF element that triggered the event</param>
        /// <param name="e">Event information</param>
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

        /// <summary>
        /// Sends a random payload to an InputStream with a fixed schema using the Window Parameters (VM)
        /// </summary>
        /// <param name="sender">WPF element that triggered the event</param>
        /// <param name="e">Event information</param>
        private void GenerateAnomalyPayloadNormal(object sender, RoutedEventArgs e)
        {
            var dataContext = this.DataContext as WindowConfig;
            Task.Factory.StartNew(() => { GenerateNormalPayload(dataContext); });
        }

        /// <summary>
        /// Generates a normal payload with a Fixed Schema on Code (for Anomaly Detection)
        /// </summary>
        /// <param name="windowCfg">Window Configuration</param>
        private void GenerateNormalPayload(WindowConfig windowCfg)
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
                    IKEY = windowCfg.IKey,
                    Data = JsonConvert.SerializeObject(dataInfo)
                };

                RestRequest request = new RestRequest(streamUrl, Method.POST);
                request.AddHeader("X-API-KEY", windowCfg.ApiKey);
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

        /// <summary>
        /// Generates an abnormal payload (on iteration 200) with a Fixed Schema on Code (for Anomaly Detection)
        /// </summary>
        /// <param name="sender">WPF element that triggered the event</param>
        /// <param name="e">Event information</param>
        private void GenerateAnomalyPayloadActivation(object sender, RoutedEventArgs e)
        {
            // We generate 256 data ticks, tick N°200 will be abnormal
            var dataContext = this.DataContext as WindowConfig;
            Task.Factory.StartNew(() => { GenData(dataContext); });
        }

        /// <summary>
        /// Generates a abnormal payload with a Fixed Schema on Code (for Anomaly Detection)
        /// </summary>
        /// <param name="windowCfg"></param>
        private void GenData(WindowConfig windowCfg)
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
                    IKEY = windowCfg.IKey,
                    Data = JsonConvert.SerializeObject(dataInfo)
                };

                RestRequest request = new RestRequest(streamUrl, Method.POST);
                request.AddHeader("X-API-KEY", windowCfg.ApiKey);
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

        /// <summary>
        /// Formats the JSON input TextBox once Focus is lost.
        /// </summary>
        /// <param name="sender">WPF element that triggered the event</param>
        /// <param name="e">Event information</param>
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