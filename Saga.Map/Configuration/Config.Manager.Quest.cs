using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Quest manager configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[<Saga.Manager.Quest
    /// directory="../Quests/"
    /// sdirectory="../Data/Scenario.Quests/"
    /// provider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.LuaQuestProvider"
    /// scenarioprovider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.ScenarioLuaQuest"
    /// />]]></code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class QuestSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or set's the directory to which contains all personal and offcial
        /// quests
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Manager.Quest
        /// directory="../Quests/"
        /// sdirectory="../Data/Scenario.Quests/"
        /// provider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.LuaQuestProvider"
        /// scenarioprovider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.ScenarioLuaQuest"
        /// />]]></code>
        /// </example>
        [ConfigurationProperty("directory", DefaultValue = "Quest", IsRequired = false)]
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
        /// Get's or set's the directory to which contains all scenario
        /// quests
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Manager.Quest
        /// directory="../Quests/"
        /// sdirectory="../Data/Scenario.Quests/"
        /// provider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.LuaQuestProvider"
        /// scenarioprovider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.ScenarioLuaQuest"
        /// />]]></code>
        /// </example>
        [ConfigurationProperty("sdirectory", DefaultValue = "Scenario.Quests", IsRequired = false)]
        public string SDirectory
        {
            get
            {
                return (string)this["sdirectory"];
            }
            set
            {
                this["sdirectory"] = value.ToString();
            }
        }

        /// <summary>
        /// Get's or set's the provider to use for official and personal quests.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Manager.Quest
        /// directory="../Quests/"
        /// sdirectory="../Data/Scenario.Quests/"
        /// provider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.LuaQuestProvider"
        /// scenarioprovider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.ScenarioLuaQuest"
        /// />]]></code>
        /// </example>
        /// <remarks>
        /// By settings this configuration option you can force to use provider from a remote plugin.
        /// Please speciafy the name of the assembly for example 'Saga.Map.Data.LuaQuest.dll' followed by
        /// a comma and a space then the fullname of the class.
        /// </remarks>
        [ConfigurationProperty("provider", IsRequired = true)]
        public string Provider
        {
            get
            {
                return (string)this["provider"];
            }
            set
            {
                this["provider"] = value.ToString();
            }
        }

        /// <summary>
        /// Get's or set's the provider to use for scenario quests.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Manager.Quest
        /// directory="../Quests/"
        /// sdirectory="../Data/Scenario.Quests/"
        /// provider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.LuaQuestProvider"
        /// scenarioprovider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.ScenarioLuaQuest"
        /// />]]></code>
        /// </example>
        /// <remarks>
        /// By settings this configuration option you can force to use provider from a remote plugin.
        /// Please speciafy the name of the assembly for example 'Saga.Map.Data.LuaQuest.dll' followed by
        /// a comma and a space then the fullname of the class.
        /// </remarks>
        [ConfigurationProperty("scenarioprovider", IsRequired = true)]
        public string ScenarioProvider
        {
            get
            {
                return (string)this["scenarioprovider"];
            }
            set
            {
                this["scenarioprovider"] = value.ToString();
            }
        }
    }
}