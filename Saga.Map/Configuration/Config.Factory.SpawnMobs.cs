using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Multi-world spawn configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[
    /// <Saga.Factory.SpawnMultiWorldObjects type="Saga.Map.Plugins.dll, Saga.Map.Plugins.MultifileSpawnMultiWorldObjects">
    /// <Files>
    ///     <add path="../Data/mob-spawns/" format="text/csv" />
    /// </Files>
    /// </Saga.Factory.SpawnMultiWorldObjects>]]></code>
    /// </example>
    public class SpawnMultiWorldObjectSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or sets a list of files containing spawninformaton for mobs
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[
        /// <Saga.Factory.SpawnMultiWorldObjects type="Saga.Map.Plugins.dll, Saga.Map.Plugins.MultifileSpawnMultiWorldObjects">
        /// <Files>
        ///     <add path="../Data/mob-spawns/" format="text/csv" />
        /// </Files>
        /// </Saga.Factory.SpawnMultiWorldObjects>]]></code>
        /// </example>
        [ConfigurationProperty("Files", IsRequired = false)]
        public FactoryFileCollection FolderItems
        {
            get { return ((FactoryFileCollection)(base["Files"])); }
        }
    }
}