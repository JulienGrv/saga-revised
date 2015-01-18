using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Saga.Map;

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
