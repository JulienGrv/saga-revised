using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Addition factory configuration.
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[<Saga.Factory.Addition reference="../Data/addition_reference.csv">
    /// <Files>
    ///     <add path="../Data/Addition_t.xml" format="text/xml" />
    /// </Files>
    /// </Saga.Factory.Addition>]]></code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class AdditionSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or sets a list of files which contains information about the additions
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Factory.Addition reference="../Data/addition_reference.csv">
        /// <Files>
        ///     <add path="../Data/Addition_t.xml" format="text/xml" />
        /// </Files>
        /// </Saga.Factory.Addition>]]></code>
        /// </example>
        /// <remarks>
        /// Additions are used by equipment, skills, potions and other items. This is a very
        /// important.
        /// </remarks>
        [ConfigurationProperty("Files", IsRequired = false)]
        public FactoryFileCollection FolderItems
        {
            get { return ((FactoryFileCollection)(base["Files"])); }
        }

        /// <summary>
        /// Get's or sets a file which holds a reference information
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Factory.Addition reference="../Data/addition_reference.csv">
        /// <Files>
        ///     <add path="../Data/Addition_t.xml" format="text/xml" />
        /// </Files>
        /// </Saga.Factory.Addition>]]></code>
        /// </example>
        /// <remarks>
        /// Each addition (called addition defines). Is a subfunction that globally creates the 'addition'.
        /// We associate each subfunction with their own function which matches the delegate AdditionHandler. Once
        /// we have collected each subfunction we compile the subfunction combination into 1 function using MSIL and
        /// Dynamic Method. This ensures we have the most optimized way for using additions and maintaining a flexible
        /// solution.
        /// </remarks>
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