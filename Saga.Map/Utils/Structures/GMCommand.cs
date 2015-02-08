using System;
using System.Text.RegularExpressions;

namespace Saga.Shared.Definitions
{
    public struct GMCommand
    {
        public string Command;
        public string Arguments;

        public GMCommand(string command, string arguments)
        {
            this.Command = command;
            this.Arguments = arguments;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GmAttribute : Attribute
    {
        public GmAttribute(string name, byte gmlevel, string regex)
        {
            this._name = name;
            this._gmlevel = gmlevel;
            this._regex = new Regex(regex);
        }

        public GmAttribute(string name, byte gmlevel, string regex, RegexOptions options)
        {
            this._name = name;
            this._gmlevel = gmlevel;
            this._regex = new Regex(regex, options);
        }

        private Regex _regex;
        private byte _gmlevel;
        private string _name;

        public byte Gmlevel
        {
            get
            {
                return _gmlevel;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public Regex Regex
        {
            get
            {
                return _regex;
            }
        }
    }
}