using System.Configuration;

namespace Saga.Configuration
{
    public class ManagerProviderBaseConfiguration : ConfigurationSection
    {
        /// <summary>
        /// Get's or sets a list of files containing spawninformaton for mobs
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
        [ConfigurationProperty("type", IsRequired = false)]
        public string DerivedType
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = value.ToString();
            }
        }
    }
}