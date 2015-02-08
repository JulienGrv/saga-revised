using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Event factory configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[<Saga.Factory.Events provider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.EventInfo">
    /// <Files>
    ///     <add path="../Data/eventlist.csv" format="text/csv" />
    /// </Files>
    /// </Saga.Factory.Events>]]></code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class EventSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or sets a list of files containing the experience information
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Factory.Events provider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.EventInfo">
        /// <Files>
        ///     <add path="../Data/eventlist.csv" format="text/csv" />
        /// </Files>
        /// </Saga.Factory.Events>]]></code>
        /// </example>
        [ConfigurationProperty("Files", IsRequired = false)]
        public FactoryFileCollection FolderItems
        {
            get { return ((FactoryFileCollection)(base["Files"])); }
        }

        /// <summary>
        /// Get's or sets the plugin provider to use
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Factory.Events provider="Saga.Map.Data.LuaQuest.dll, Saga.Map.Data.LuaQuest.EventInfo">
        /// <Files>
        ///     <add path="../Data/eventlist.csv" format="text/csv" />
        /// </Files>
        /// </Saga.Factory.Events>]]></code>
        /// </example>
        /// <remarks>
        /// By settings this configuration option you can force to use provider from a remote plugin.
        /// Please speciafy the name of the assembly for example 'Saga.Map.Data.LuaQuest.dll' followed by
        /// a comma and a space then the fullname of the class.
        /// </remarks>
        [ConfigurationProperty("provider", IsRequired = false)]
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
    }
}