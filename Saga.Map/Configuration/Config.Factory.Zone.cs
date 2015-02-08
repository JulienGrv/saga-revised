using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Zone factory configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[
    /// <Saga.Factory.Zones directory="../Data/heightmaps">
    /// <Files>
    ///     <add path="../Data/zone_data.csv" format="text/csv" />
    /// </Files>
    ///</Saga.Factory.Zones>]]></code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class ZoneSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or sets the directory containing the heightmaps
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[
        /// <Saga.Factory.Zones directory="../Data/heightmaps">
        /// <Files>
        ///     <add path="../Data/zone_data.csv" format="text/csv" />
        /// </Files>
        ///</Saga.Factory.Zones>]]></code>
        /// </example>
        [ConfigurationProperty("directory", DefaultValue = "Heightmaps", IsRequired = false)]
        public string Directory
        {
            get
            {
                return (string)this["directory"];
            }
            set
            {
                this["directory"] = value.ToString();
            }
        }

        /// <summary>
        /// Get's or sets a list of files containing zone information
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[
        /// <Saga.Factory.Zones directory="../Data/heightmaps">
        /// <Files>
        ///     <add path="../Data/zone_data.csv" format="text/csv" />
        /// </Files>
        ///</Saga.Factory.Zones>]]></code>
        /// </example>
        [ConfigurationProperty("Files", IsRequired = false)]
        public FactoryFileCollection FolderItems
        {
            get { return ((FactoryFileCollection)(base["Files"])); }
        }
    }
}