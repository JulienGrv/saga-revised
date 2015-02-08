using System.Configuration;

namespace Saga.Configuration
{
    public class NetworkSettings : ManagerProviderBaseConfiguration
    {
        [ConfigurationProperty("Connections", IsRequired = false)]
        public NetworkFileCollection Connections
        {
            get { return ((NetworkFileCollection)(base["Connections"])); }
        }
    }
}