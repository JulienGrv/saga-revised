using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// World spawn configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[
    /// <Saga.Factory.SpawnWorldObjects type="Saga.Map.Plugins.dll, Saga.Map.Plugins.MultifileSpawnWorldObjects">
    /// <Files>
    ///     <add path="../Data/npc-spawns/" format="text/csv" />
    ///     <add path="../Data/item-spawns/" format="text/csv" />
    ///  </Files>
    /// </Saga.Factory.SpawnWorldObjects>]]></code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class SpawnWorldObjectSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or sets a list of files containing spawninformaton for npc's and mapitems
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[
        /// <Saga.Factory.SpawnWorldObjects type="Saga.Map.Plugins.dll, Saga.Map.Plugins.MultifileSpawnWorldObjects">
        /// <Files>
        ///     <add path="../Data/npc-spawns/" format="text/csv" />
        ///     <add path="../Data/item-spawns/" format="text/csv" />
        ///  </Files>
        /// </Saga.Factory.SpawnWorldObjects>]]></code>
        /// </example>
        [ConfigurationProperty("Files", IsRequired = false)]
        public FactoryFileCollection FolderItems
        {
            get { return ((FactoryFileCollection)(base["Files"])); }
        }
    }
}