using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Spell/Skill factory configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code>
    /// <![CDATA[
    /// <Saga.Factory.Spells reference="../Data/skill_reference.csv">
    /// <Files>
    ///     <add path="../Data/spell_data.xml" format="text/xml" />
    ///     </Files>
    ///     </Saga.Factory.Spells>
    /// ]]>
    /// </code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class SpellSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or set's a list of items that contain skill information.
        /// </summary>
        /// <remarks>
        /// This contains a list of items that contain all the skill information. It can be
        /// a single file or multiple files as desired. The skillinformation should include
        /// information such as: skillid, skilltype, maximumexperience, maximumgrowlevel,
        /// minimumrange, maximumrange.
        /// </remarks>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code>
        /// <![CDATA[
        /// <Saga.Factory.Spells reference="../Data/skill_reference.csv">
        /// <Files>
        ///     <add path="../Data/spell_data.xml" format="text/xml" />
        ///     </Files>
        ///     </Saga.Factory.Spells>
        /// ]]>
        /// </code>
        /// </example>
        [ConfigurationProperty("Files", IsRequired = false)]
        public FactoryFileCollection FolderItems
        {
            get { return ((FactoryFileCollection)(base["Files"])); }
        }

        /// <summary>
        /// Get's or set's the skill reference filename
        /// </summary>
        /// <remarks>
        /// This file contains a link association between skillid and their associated managed
        /// function. For every skill referenced in this file it searches for the assigned function
        /// where the function has to match SkillHandler delegate.
        /// </remarks>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code>
        /// <![CDATA[
        /// <Saga.Factory.Spells reference="../Data/skill_reference.csv">
        /// <Files>
        ///     <add path="../Data/spell_data.xml" format="text/xml" />
        ///     </Files>
        ///     </Saga.Factory.Spells>
        /// ]]>
        /// </code>
        /// </example>
        [ConfigurationProperty("reference", DefaultValue = "", IsRequired = true)]
        public string Reference
        {
            get
            {
                return (string)this["reference"];
            }
            set
            {
                this["reference"] = value.ToString();
            }
        }
    }
}