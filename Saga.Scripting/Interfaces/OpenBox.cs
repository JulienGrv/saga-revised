using System;
using System.Collections.Generic;
using System.Text;
using Saga.PrimaryTypes;
using Saga.Shared.Definitions;

namespace Saga.Scripting.Interfaces
{
    public interface OpenBox : ILootable
    {
        void OnOpenBox(Character sender);
    }
}
