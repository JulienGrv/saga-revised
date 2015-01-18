using System;
using System.Collections.Generic;
using System.Text;
using Saga.PrimaryTypes;

namespace Saga.Shared.Definitions
{
    public interface OpenBook 
    {
        void OnOpenBook(Character sender);
    }
}
