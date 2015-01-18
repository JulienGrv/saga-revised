using System;
using System.Collections.Generic;
using System.Text;

namespace Saga
{
    public interface IHostPlugin
    {
        IHostPlugin Host { get; }
        List<IPlugin> HostedPlugins { get; }
        string Guid { get; }
        string Name { get; }        
        
    }
}
