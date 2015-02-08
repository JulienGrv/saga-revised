using System.Configuration;

namespace Saga.Configuration
{
    public class DatabaseSettings : ManagerProviderBaseConfiguration
    {
        [ConfigurationProperty("provider", IsRequired = true)]
        public string DbType
        {
            get
            {
                return (string)this["provider"];
            }
            set
            {
                this["provider"] = value.ToString();
            }
        }

        [ConfigurationProperty("host", DefaultValue = "127.0.0.1", IsRequired = false)]
        public string Host
        {
            get
            {
                return (string)this["host"];
            }
            set
            {
                this["host"] = value.ToString();
            }
        }

        [ConfigurationProperty("username", IsRequired = true)]
        public string Username
        {
            get
            {
                return (string)this["username"];
            }
            set
            {
                this["username"] = value.ToString();
            }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                this["password"] = value.ToString();
            }
        }

        [ConfigurationProperty("port", IsRequired = true)]
        public uint Port
        {
            get
            {
                return (uint)this["port"];
            }
            set
            {
                this["port"] = value.ToString();
            }
        }

        [IntegerValidator(MinValue = 1, MaxValue = 5)]
        [ConfigurationProperty("pool", IsRequired = false, DefaultValue = 1)]
        public int PooledConnections
        {
            get
            {
                return (int)this["pool"];
            }
            set
            {
                this["pool"] = value.ToString();
            }
        }

        [ConfigurationProperty("database", IsRequired = true)]
        public string Database
        {
            get
            {
                return (string)this["database"];
            }
            set
            {
                this["database"] = value.ToString();
            }
        }
    }
}