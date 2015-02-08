using System.Configuration;

namespace Saga.Configuration
{
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class ServerVars : ConfigurationSection
    {
        [ConfigurationProperty("datadir", DefaultValue = "", IsRequired = true)]
        public string DataDirectory
        {
            get
            {
                return (string)this["datadir"];
            }
            set
            {
                this["datadir"] = value.ToString();
            }
        }
    }

    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class ConsoleVars : ConfigurationSection
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

        [ConfigurationProperty("outputcommand", IsRequired = false)]
        public bool CanOutputCommand
        {
            get
            {
                return (bool)this["outputcommand"];
            }
            set
            {
                this["outputcommand"] = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}