using Saga.Map.Configuration;
using System.Configuration;
using System.Globalization;

namespace Saga.Configuration
{
    /// <summary>
    /// Network manager configuration
    /// </summary>
    /// <example>
    /// This is a example that shows how the element is configured
    /// <code><![CDATA[<Saga.Manager.NetworkSettings world="1" playerlimit="60" proof="c4ca4238a0b923820dcc509a6f75849b">
    /// <Connections>
    ///     <add connection="public" host="0.0.0.0" port="64001" />
    ///     <add connection="internal" host="127.0.0.1" port="64002" />
    /// </Connections>
    /// </Saga.Manager.NetworkSettings>
    /// ]]></code>
    /// </example>
    [System.Reflection.Obfuscation(Exclude = true, StripAfterObfuscation = true)]
    public class NetworkSettings : ManagerProviderBaseConfiguration
    {
        /// <summary>
        /// Get's or sets the adresses to configure for the network interface.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Manager.NetworkSettings world="1" playerlimit="60" proof="c4ca4238a0b923820dcc509a6f75849b">
        /// <Connections>
        ///     <add connection="public" host="0.0.0.0" port="64001" />
        ///     <add connection="internal" host="127.0.0.1" port="64002" />
        /// </Connections>
        /// </Saga.Manager.NetworkSettings>
        /// ]]></code>
        /// </example>
        /// <remarks>
        /// The public interface is for the connections that are established between gateway
        /// and map server this is binding interface which means that it will listen on the
        /// specified port for pending connections. The internal interface is a fixed established
        /// connection between map and authentication server.
        /// </remarks>
        [ConfigurationProperty("Connections", IsRequired = false)]
        public NetworkFileCollection Connections
        {
            get { return ((NetworkFileCollection)(base["Connections"])); }
        }

        /// <summary>
        /// Get's or sets the world id to loging with
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Manager.NetworkSettings world="1" playerlimit="60" proof="c4ca4238a0b923820dcc509a6f75849b">
        /// <Connections>
        ///     <add connection="public" host="0.0.0.0" port="64001" />
        ///     <add connection="internal" host="127.0.0.1" port="64002" />
        /// </Connections>
        /// </Saga.Manager.NetworkSettings>
        /// ]]></code>
        /// </example>
        [ConfigurationProperty("world", DefaultValue = 0, IsRequired = true)]
        public int WorldId
        {
            get
            {
                return (int)this["world"];
            }
            set
            {
                this["world"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Get's or sets the world maximum player limit.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Manager.NetworkSettings world="1" playerlimit="60" proof="c4ca4238a0b923820dcc509a6f75849b">
        /// <Connections>
        ///     <add connection="public" host="0.0.0.0" port="64001" />
        ///     <add connection="internal" host="127.0.0.1" port="64002" />
        /// </Connections>
        /// </Saga.Manager.NetworkSettings>
        /// ]]></code>
        /// </example>
        [ConfigurationProperty("playerlimit", DefaultValue = 50, IsRequired = false)]
        public int PlayerLimit
        {
            get
            {
                return (int)this["playerlimit"];
            }
            set
            {
                this["playerlimit"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Get's or sets the world required age to login.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Manager.NetworkSettings world="1" playerlimit="60" proof="c4ca4238a0b923820dcc509a6f75849b">
        /// <Connections>
        ///     <add connection="public" host="0.0.0.0" port="64001" />
        ///     <add connection="internal" host="127.0.0.1" port="64002" />
        /// </Connections>
        /// </Saga.Manager.NetworkSettings>
        /// ]]></code>
        /// </example>
        [ConfigurationProperty("requiredAge", DefaultValue = 0, IsRequired = false)]
        public int Agelimit
        {
            get
            {
                return (int)this["requiredAge"];
            }
            set
            {
                this["requiredAge"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Get's or sets the world proof to login.
        /// </summary>
        /// <example>
        /// This is a example that shows how the element is configured
        /// <code><![CDATA[<Saga.Manager.NetworkSettings world="1" playerlimit="60" proof="c4ca4238a0b923820dcc509a6f75849b">
        /// <Connections>
        ///     <add connection="public" host="0.0.0.0" port="64001" />
        ///     <add connection="internal" host="127.0.0.1" port="64002" />
        /// </Connections>
        /// </Saga.Manager.NetworkSettings>
        /// ]]></code>
        /// </example>
        /// <remarks>
        /// The proof is similair to password which is associated with the worldid. Using a proof concept it requirs
        /// you to know the credentionals and cannot simply state that worldid x is yours.
        /// </remarks>
        [ConfigurationProperty("proof", DefaultValue = "", IsRequired = true)]
        public string Proof
        {
            get
            {
                return (string)this["proof"];
            }
            set
            {
                this["proof"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}