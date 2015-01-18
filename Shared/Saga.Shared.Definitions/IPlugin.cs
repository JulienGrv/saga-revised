using System;
using System.Collections.Generic;
using System.Text;

namespace Saga
{
    public interface IPlugin
    {
        void LoadPlugin(IHostPlugin host);
        void UnloadPlugin(IHostPlugin host);
    }
}
