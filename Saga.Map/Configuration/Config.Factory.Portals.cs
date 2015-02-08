using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Portal factory configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[
    /// <Saga.Factory.Portals>
    /// <Files>
    ///     <add path="../Data/portal_data.csv" format="text/csv" />
    /// </Files>
    /// </Saga.Factory.Portals>]]></code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class PortalSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or sets a list of files containing portal information
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[
        /// <Saga.Factory.Portals>
        /// <Files>
        ///     <add path="../Data/portal_data.csv" format="text/csv" />
        /// </Files>
        /// </Saga.Factory.Portals>]]></code>
        /// </example>
        [ConfigurationProperty("Files", IsRequired = false)]
        public FactoryFileCollection FolderItems
        {
            get { return ((FactoryFileCollection)(base["Files"])); }
        }
    }
}