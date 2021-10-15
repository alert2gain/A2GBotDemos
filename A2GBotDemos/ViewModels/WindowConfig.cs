using System.ComponentModel;

namespace A2GBotDemos.ViewModels
{
    /// <summary>
    /// Class that represents the App Configuration that can be set to test the connectivity to A2G.IO
    /// </summary>
    public class WindowConfig : INotifyPropertyChanged
    {
        /// <summary>
        /// API Key
        /// </summary>
        private string apiKey;

        /// <summary>
        /// Bot Activation Key
        /// </summary>
        private string aKey;

        /// <summary>
        /// InputStream Key
        /// </summary>
        private string iKey;

        /// <summary>
        /// JSON Body
        /// </summary>
        private string jsonCfg;

        /// <summary>
        /// API Key
        /// </summary>
        public string ApiKey
        {
            get { return apiKey; }
            set
            {
                if (apiKey != value)
                {
                    apiKey = value;
                    RaisePropertyChanged("ApiKey");
                }
            }
        }

        /// <summary>
        /// Bot Activation Key
        /// </summary>
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

        /// <summary>
        /// InputStream Key
        /// </summary>
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

        /// <summary>
        /// JSON Body
        /// </summary>
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

        /// <summary>
        /// Property changes handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the update of a property in the Window Config
        /// </summary>
        /// <param name="property">Property that must be updated</param>
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

}
