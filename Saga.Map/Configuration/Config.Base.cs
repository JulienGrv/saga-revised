using System.Configuration;

namespace Saga.Map.Configuration
{
    /// <summary>
    /// Default configuration element.
    /// </summary>
    [ConfigurationCollection(typeof(FactoryFileElement))]
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class FactoryFileCollection : KeyValueConfigurationCollection
    {
        public void Add(FactoryFileElement element)
        {
            BaseAdd(element, false);
        }

        /// <summary>
        /// Creates a new Element
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new FactoryFileElement();
        }

        /// <summary>
        /// Get/Sets the path as unique key
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FactoryFileElement)(element)).Path;
        }

        /// <summary>
        /// A collection of file elements
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public FactoryFileElement this[int idx]
        {
            get
            {
                return (FactoryFileElement)BaseGet(idx);
            }
        }
    }

    /// <summary>
    /// Default configuration element.
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    [ConfigurationCollection(typeof(NetworkFileCollection))]
    public class NetworkFileCollection : KeyValueConfigurationCollection
    {
        /// <summary>
        /// Creates a new Element
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new NetworkElement();
        }

        /// <summary>
        /// Get/Sets the path as unique key
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NetworkElement)(element)).Connection;
        }

        /// <summary>
        /// A collection of file elements
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public NetworkElement this[int idx]
        {
            get
            {
                return (NetworkElement)BaseGet(idx);
            }
        }

        /// <summary>
        /// A collection of file elements
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public new NetworkElement this[string idx]
        {
            get
            {
                return (NetworkElement)BaseGet(idx);
            }
            set
            {
                BaseAdd(value, false);
            }
        }
    }

    /// <summary>
    /// Configuration element speciafiing a file and it's fileformat.
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class FactoryFileElement : ConfigurationElement
    {
        public FactoryFileElement()
        {
        }

        public FactoryFileElement(string path)
        {
            this.Path = path;
        }

        public FactoryFileElement(string path, string format)
        {
            this.Path = path;
            this.Reader = format;
        }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        [ConfigurationProperty("path", IsKey = true, IsRequired = true)]
        public string Path
        {
            get
            {
                return ((string)(base["path"]));
            }
            set
            {
                base["path"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the fileformat.
        /// </summary>
        [ConfigurationProperty("format", DefaultValue = "text/csv", IsKey = false, IsRequired = false)]
        public string Reader
        {
            get
            {
                return ((string)(base["format"]));
            }
            set
            {
                base["format"] = value;
            }
        }
    }

    /// <summary>
    /// Configuration element speciafiing a file and it's fileformat.
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class NetworkElement : ConfigurationElement
    {
        public NetworkElement()
        {
        }

        public NetworkElement(string connection, string host, int port)
        {
            base["connection"] = connection;
            base["host"] = host;
            base["port"] = port;
        }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        [ConfigurationProperty("connection", IsKey = true, IsRequired = true)]
        public string Connection
        {
            get
            {
                return ((string)(base["connection"]));
            }
            set
            {
                base["connection"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the hosts address
        /// </summary>
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get
            {
                return ((string)(base["host"]));
            }
            set
            {
                base["host"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the port where to establish or connect the client to.
        /// </summary>
        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get
            {
                return ((int)(base["port"]));
            }
            set
            {
                base["port"] = value;
            }
        }
    }
}