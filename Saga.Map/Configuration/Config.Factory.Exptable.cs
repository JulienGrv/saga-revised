using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Status by level factory configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[<Saga.Factory.StatusByLevel>
    /// <Files>
    ///     <add path="../Data/experience.csv" format="text/csv" />
    /// </Files>
    /// </Saga.Factory.StatusByLevel>]]></code>
    /// </example>
    /// <remarks>
    /// This modifies the character experience droprate. Setting this to a value
    /// above one will make the cexp be based on xx times the normal cexp.
    /// </remarks>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class StatusByLevelSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or sets a the cexp modifier.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Factory.StatusByLevel>
        /// <Files>
        ///     <add path="../Data/experience.csv" format="text/csv" />
        /// </Files>
        /// </Saga.Factory.StatusByLevel>]]></code>
        /// </example>
        /// <remarks>
        /// This modifies the character experience droprate. Setting this to a value
        /// above one will make the cexp be based on xx times the normal cexp.
        /// </remarks>
        [ConfigurationProperty("cexp", DefaultValue = 1, IsRequired = false)]
        [IntegerValidator(MinValue = 1, MaxValue = 20, ExcludeRange = false)]
        public int Cexp
        {
            get
            {
                return (int)this["cexp"];
            }
            set
            {
                this["cexp"] = value;
            }
        }

        /// <summary>
        /// Get's or sets a the jexp modifier.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Factory.StatusByLevel>
        /// <Files>
        ///     <add path="../Data/experience.csv" format="text/csv" />
        /// </Files>
        /// </Saga.Factory.StatusByLevel>]]></code>
        /// </example>
        /// <remarks>
        /// This modifies the job experience droprate. Setting this to a value
        /// above one will make the jexp be based on xx times the normal jexp.
        /// </remarks>
        [ConfigurationProperty("jexp", DefaultValue = 1, IsRequired = false)]
        [IntegerValidator(MinValue = 1, MaxValue = 20, ExcludeRange = false)]
        public int Jexp
        {
            get
            {
                return (int)this["jexp"];
            }
            set
            {
                this["jexp"] = value;
            }
        }

        /// <summary>
        /// Get's or sets a the wexp modifier.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Factory.StatusByLevel>
        /// <Files>
        ///     <add path="../Data/experience.csv" format="text/csv" />
        /// </Files>
        /// </Saga.Factory.StatusByLevel>]]></code>
        /// </example>
        /// <remarks>
        /// This modifies the weapon experience droprate. Setting this to a value
        /// above one will make the wexp be based on xx times the normal wexp.
        /// </remarks>
        [ConfigurationProperty("wexp", DefaultValue = 1, IsRequired = false)]
        [IntegerValidator(MinValue = 1, MaxValue = 20, ExcludeRange = false)]
        public int Wexp
        {
            get
            {
                return (int)this["wexp"];
            }
            set
            {
                this["wexp"] = value;
            }
        }

        /// <summary>
        /// Get's or sets a the drate modifier.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Factory.StatusByLevel>
        /// <Files>
        ///     <add path="../Data/experience.csv" format="text/csv" />
        /// </Files>
        /// </Saga.Factory.StatusByLevel>]]></code>
        /// </example>
        /// <remarks>
        /// This modifies the item droprate. Setting this to a value
        /// above one will make the drop rate be based on xx times the normal droprates.
        /// </remarks>
        [ConfigurationProperty("drate", DefaultValue = 1, IsRequired = false)]
        [IntegerValidator(MinValue = 1, MaxValue = 20, ExcludeRange = false)]
        public int Droprate
        {
            get
            {
                return (int)this["drate"];
            }
            set
            {
                this["drate"] = value;
            }
        }

        /// <summary>
        /// Get's or sets a list of files containing information per level
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Factory.StatusByLevel>
        /// <Files>
        ///     <add path="../Data/experience.csv" format="text/csv" />
        /// </Files>
        /// </Saga.Factory.StatusByLevel>]]></code>
        /// </example>
        [ConfigurationProperty("Files", IsRequired = false)]
        public FactoryFileCollection FolderItems
        {
            get { return ((FactoryFileCollection)(base["Files"])); }
        }
    }
}