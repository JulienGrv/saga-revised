using System;

namespace Saga.Map.Client
{
    partial class Client
    {
        public void OnAddShortcut()
        {
            Console.WriteLine("OnAddShortcut");
        }

        public void OnDelShortcut()
        {
            Console.WriteLine("OnDelShortcut");
        }
    }
}