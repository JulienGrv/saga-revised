using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Mob-template factory configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[<Saga.Factory.SpawnTemplate>
    /// <Files>
    ///     <add path="../Data/npc_templates.csv" format="text/csv" />
    /// </Files>
    /// </Saga.Factory.SpawnTemplate>]]></code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class SpawntemplateSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or sets a list of files containing template information for mobs
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Factory.SpawnTemplate>
        /// <Files>
        ///     <add path="../Data/npc_templates.csv" format="text/csv" />
        /// </Files>
        /// </Saga.Factory.SpawnTemplate>]]></code>
        /// </example>
        [ConfigurationProperty("Files", IsRequired = false)]
        public FactoryFileCollection FolderItems
        {
            get { return ((FactoryFileCollection)(base["Files"])); }
        }
    }
}