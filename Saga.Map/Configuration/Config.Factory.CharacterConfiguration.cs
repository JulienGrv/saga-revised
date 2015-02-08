using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// CharacterConfiguration factory configuration element.
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[<Saga.Factory.CharacterConfiguration>
    /// <Files>
    ///     <add path="../Data/character-template.csv" format="text/csv" />
    /// </Files>
    /// </Saga.Factory.CharacterConfiguration> ]]></code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class CharacterConfigurationSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or sets a list of files containing the character base stats
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Factory.CharacterConfiguration>
        /// <Files>
        ///     <add path="../Data/character-template.csv" format="text/csv" />
        /// </Files>
        /// </Saga.Factory.CharacterConfiguration> ]]></code>
        /// </example>
        [ConfigurationProperty("Files", IsRequired = false)]
        public FactoryFileCollection FolderItems
        {
            get { return ((FactoryFileCollection)(base["Files"])); }
        }
    }
}