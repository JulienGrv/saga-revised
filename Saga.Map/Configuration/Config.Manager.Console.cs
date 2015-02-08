using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class ConsoleSettings : ManagerProviderBaseConfiguration
    {
        [ConfigurationProperty("commandprefix", IsRequired = true)]
        public string CommandPrefix
        {
            get
            {
                return (string)this["commandprefix"];
            }
            set
            {
                this["commandprefix"] = value.ToString();
            }
        }

        [ConfigurationProperty("outputcommand", DefaultValue = true, IsRequired = false)]
        public bool CanOutputCommand
        {
            get
            {
                return (bool)this["outputcommand"];
            }
            set
            {
                this["outputcommand"] = value.ToString();
            }
        }

        [ConfigurationProperty("Commands", IsRequired = false)]
        public FactoryFileCollection Commands
        {
            get { return ((FactoryFileCollection)(base["Commands"])); }
        }

        [ConfigurationProperty("GmCommands", IsRequired = false)]
        public FactoryFileCollection GmCommands
        {
            get { return ((FactoryFileCollection)(base["GmCommands"])); }
        }
    }
}