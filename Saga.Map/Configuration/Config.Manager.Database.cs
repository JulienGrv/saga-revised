using System.Configuration;
using System.Globalization;

namespace Saga.Configuration
{
    /// <summary>
    /// Database manager configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code>
    /// <![CDATA[<Saga.Manager.Database
    ///     username="root"
    ///     password="root"
    ///     port="3306"
    ///     database="saga1"
    ///     provider="Saga.Map.Data.Mysql.dll, Saga.Map.Data.Mysql.MysqlProvider"
    /// />]]></code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class DatabaseSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or set's the provider to for the database management.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code>
        /// <![CDATA[<Saga.Manager.Database
        ///     username="root"
        ///     password="root"
        ///     port="3306"
        ///     database="saga1"
        ///     provider="Saga.Map.Data.Mysql.dll, Saga.Map.Data.Mysql.MysqlProvider"
        /// />]]></code>
        /// </example>
        /// <remarks>
        /// By settings this configuration option you can force to use provider from a remote plugin.
        /// Please speciafy the name of the assembly for example 'Saga.Map.Data.Mysql.dll' followed by
        /// a comma and a space then the fullname of the class.
        /// </remarks>
        [ConfigurationProperty("provider", IsRequired = true)]
        public string DBType
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

        /// <summary>
        /// Get's or set's the host to connect to.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code>
        /// <![CDATA[<Saga.Manager.Database
        ///     username="root"
        ///     password="root"
        ///     port="3306"
        ///     database="saga1"
        ///     provider="Saga.Map.Data.Mysql.dll, Saga.Map.Data.Mysql.MysqlProvider"
        /// />]]></code>
        /// </example>
        /// <remarks>
        /// This value is parsed to the plugin in the on connect interface.
        /// </remarks>
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

        /// <summary>
        /// Get's or set's the username to authenticate with.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code>
        /// <![CDATA[<Saga.Manager.Database
        ///     username="root"
        ///     password="root"
        ///     port="3306"
        ///     database="saga1"
        ///     provider="Saga.Map.Data.Mysql.dll, Saga.Map.Data.Mysql.MysqlProvider"
        /// />]]></code>
        /// </example>
        /// <remarks>
        /// This value is parsed to the plugin in the on connect interface.
        /// </remarks>
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

        /// <summary>
        /// Get's or set's the password to authenticate with.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code>
        /// <![CDATA[<Saga.Manager.Database
        ///     username="root"
        ///     password="root"
        ///     port="3306"
        ///     database="saga1"
        ///     provider="Saga.Map.Data.Mysql.dll, Saga.Map.Data.Mysql.MysqlProvider"
        /// />]]></code>
        /// </example>
        /// <remarks>
        /// This value is parsed to the plugin in the on connect interface.
        /// </remarks>
        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                this["password"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Get's or set's the port to connect to
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code>
        /// <![CDATA[<Saga.Manager.Database
        ///     username="root"
        ///     password="root"
        ///     port="3306"
        ///     database="saga1"
        ///     provider="Saga.Map.Data.Mysql.dll, Saga.Map.Data.Mysql.MysqlProvider"
        /// />]]></code>
        /// </example>
        /// <remarks>
        /// This value is parsed to the plugin in the on connect interface.
        /// </remarks>
        [ConfigurationProperty("port", IsRequired = true)]
        public uint Port
        {
            get
            {
                return (uint)this["port"];
            }
            set
            {
                this["port"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Get's or set's the number of maximum database pooled connections.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code>
        /// <![CDATA[<Saga.Manager.Database
        ///     username="root"
        ///     password="root"
        ///     port="3306"
        ///     database="saga1"
        ///     provider="Saga.Map.Data.Mysql.dll, Saga.Map.Data.Mysql.MysqlProvider"
        /// />]]></code>
        /// </example>
        /// <remarks>
        /// An pooled connection can be used as an backup connection in case the primary
        /// connection is in use with an long running query. An recommended value for map
        /// servers is 3 pooled connections.
        /// </remarks>
        [IntegerValidator(MinValue = 1, MaxValue = 5)]
        [ConfigurationProperty("pool", IsRequired = false, DefaultValue = 3)]
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

        /// <summary>
        /// Get's or set's the database to connect to
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code>
        /// <![CDATA[<Saga.Manager.Database
        ///     username="root"
        ///     password="root"
        ///     port="3306"
        ///     database="saga1"
        ///     provider="Saga.Map.Data.Mysql.dll, Saga.Map.Data.Mysql.MysqlProvider"
        /// />]]></code>
        /// </example>
        /// <remarks>
        /// This value is parsed to the plugin in the on connect interface.
        /// </remarks>
        [ConfigurationProperty("database", IsRequired = true)]
        public string Database
        {
            get
            {
                return (string)this["database"];
            }
            set
            {
                this["database"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}