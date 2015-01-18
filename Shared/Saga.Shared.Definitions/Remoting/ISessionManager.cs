using System;
using System.Collections.Generic;
using System.Text;

namespace Saga.Shared.Definitions.Remoting
{
    interface ISessionManager
    {
        bool RequestSession(out int session);
        bool ReleaseSession(int session);
    }
}
