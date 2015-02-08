using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Quest manager configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <![CDATA[
    /// <code><Saga.Manager.Scripting directory="../Saga.Scripting">
    /// <Assemblies>
    ///     <add path="LuaInterface.dll" />
    ///     <add path="System.dll" />
    ///     <add path="System.Data.dll" />
    ///     <add path="System.Xml.dll" />
    ///     </Assemblies>
    /// </Saga.Manager.Scripting> ]]></code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class ScriptingSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or set's the directory that contains scripts to be compiled
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <![CDATA[
        /// <code><Saga.Manager.Scripting directory="../Saga.Scripting">
        /// <Assemblies>
        ///     <add path="LuaInterface.dll" />
        ///     <add path="System.dll" />
        ///     <add path="System.Data.dll" />
        ///     <add path="System.Xml.dll" />
        ///     </Assemblies>
        /// </Saga.Manager.Scripting> ]]></code>
        /// </example>
        /// <remarks>
        /// This scripting directory contains a bunch of .cs files which are assumed to be
        /// c-sharp (c#) code files. These files are compiled on runtime into a assembly which
        /// is stored on the disk as 'SAGA.SCRIPTING'.
        ///
        /// We store this assembly on the disk to ensure people can debug their skills/
        /// npc templates.
        /// </remarks>
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
        /// Get's or set's the assemblies that should be added as reference to the
        /// scripting section.
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// This is a example that shows how the element is configured
        /// <code><Saga.Manager.Scripting directory="../Saga.Scripting">
        /// <Assemblies>
        ///     <add path="LuaInterface.dll" />
        ///     <add path="System.dll" />
        ///     <add path="System.Data.dll" />
        ///     <add path="System.Xml.dll" />
        ///     </Assemblies>
        /// </Saga.Manager.Scripting> ]]></code>
        /// </example>
        /// <remarks>
        /// By default only the current calling assembly is added. This means that you
        /// have a complete empty compile if you don't add any references such as System.Data,
        /// System.Xml etc.
        /// </remarks>
        [ConfigurationProperty("Assemblies", IsRequired = false)]
        public FactoryFileCollection Assemblies
        {
            get { return ((FactoryFileCollection)(base["Assemblies"])); }
        }
    }
}