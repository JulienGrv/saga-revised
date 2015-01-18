using System;
using System.Collections.Generic;
using System.Text;

namespace Saga.Data
{
    public interface IQueryProvider
    {
        QueryParameterCollection Parameters { get; }
        string CmdText { get; set; }
    }
}
