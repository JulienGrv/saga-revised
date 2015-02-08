using Saga.Map.Configuration;
using System.Configuration;

namespace Saga.Configuration
{
    /// <summary>
    /// Item factory configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[
    /// <Saga.Factory.Items>
    /// <Files>
    ///     <add path="../Data/item_data.xml" format="text/xml" />
    /// </Files>
    /// </Saga.Factory.Items>]]></code>
    /// </example>
    public class ItemSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or sets a list of files containing item information
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[
        /// <Saga.Factory.Items>
        /// <Files>
        ///     <add path="../Data/item_data.xml" format="text/xml" />
        /// </Files>
        /// </Saga.Factory.Items>]]></code>
        /// </example>
        [ConfigurationProperty("Files", IsRequired = false)]
        public FactoryFileCollection FolderItems
        {
            get { return ((FactoryFileCollection)(base["Files"])); }
        }
    }
}