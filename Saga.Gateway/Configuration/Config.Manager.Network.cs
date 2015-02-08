using System.Configuration;

namespace Saga.Configuration
{
    internal class NetworkSettings : ConfigurationSection
    {
        [ConfigurationProperty("Connections", IsRequired = false)]
        public NetworkFileCollection Connections
        {
            get { return ((NetworkFileCollection)(base["Connections"])); }
        }

        [ConfigurationProperty("CRCKEY", IsRequired = true)]
        public string Crckey
        {
            get
            {
                return ((string)(base["CRCKEY"]));
            }
            set
            {
                base["CRCKEY"] = value;
            }
        }

        [ConfigurationProperty("GUID", IsRequired = true)]
        public string Guidkey
        {
            get
            {
                return ((string)(base["GUID"]));
            }
            set
            {
                base["GUID"] = value;
            }
        }
    }
}