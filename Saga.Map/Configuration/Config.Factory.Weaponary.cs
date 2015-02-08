using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Weaponary factory configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[<Saga.Factory.Weaponary>
    /// <Files>
    ///     <add path="../Data/weapon_data.csv" format="text/csv" />
    /// </Files>
    /// </Saga.Factory.Weaponary>
    /// ]]></code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class WeaponarySettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or sets a list of files containing weaponary information
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Factory.Weaponary>
        /// <Files>
        ///     <add path="../Data/weapon_data.csv" format="text/csv" />
        /// </Files>
        /// </Saga.Factory.Weaponary>
        /// ]]></code>
        /// </example>
        [ConfigurationProperty("Files", IsRequired = false)]
        public FactoryFileCollection FolderItems
        {
            get { return ((FactoryFileCollection)(base["Files"])); }
        }
    }
}