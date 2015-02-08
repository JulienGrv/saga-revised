using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Default configuration element.
    /// </summary>
    [ConfigurationCollection(typeof(SingletonManagerElement))]
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class ManagerCollection : KeyValueConfigurationCollection
    {
        /// <summary>
        /// Creates a new Element
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new SingletonManagerElement();
        }

        /// <summary>
        /// Get/Sets the path as unique key
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SingletonManagerElement)(element)).Name;
        }

        /// <summary>
        /// A collection of file elements
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public SingletonManagerElement this[int idx]
        {
            get
            {
                return (SingletonManagerElement)BaseGet(idx);
            }
        }
    }

    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class SingletonManagerElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the path
        /// </summary>
        [ConfigurationProperty("type", IsKey = false, IsRequired = true)]
        public string Type
        {
            get
            {
                return ((string)(base["type"]));
            }
            set
            {
                base["type"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the fileformat.
        /// </summary>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return ((string)(base["name"]));
            }
            set
            {
                base["name"] = value;
            }
        }
    }
}