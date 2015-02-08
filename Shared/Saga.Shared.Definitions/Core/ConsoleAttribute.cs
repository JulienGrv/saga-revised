using System;

namespace Saga.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleAttribute : Attribute
    {
        #region Private Members

        private string name;
        private string description;
        private string syntax;
        private int gmlevel;

        #endregion Private Members

        #region Public Properties

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
        }

        public string Syntax
        {
            get
            {
                return syntax;
            }
        }

        public int GmLevel
        {
            get
            {
                return gmlevel;
            }
        }

        #endregion Public Properties

        #region Constructor / Deconstructor

        public ConsoleAttribute(string name)
        {
            this.name = name;
            this.description = string.Empty;
            this.syntax = string.Empty;
        }

        public ConsoleAttribute(string name, string description)
        {
            this.name = name;
            this.description = description;
            this.syntax = string.Empty;
        }

        public ConsoleAttribute(string name, string description, string syntax)
        {
            this.name = name;
            this.description = description;
            this.syntax = syntax;
        }

        public ConsoleAttribute(int gmlevel, string name)
        {
            this.gmlevel = gmlevel;
            this.name = name;
            this.description = string.Empty;
            this.syntax = string.Empty;
        }

        public ConsoleAttribute(int gmlevel, string name, string description)
        {
            this.gmlevel = gmlevel;
            this.name = name;
            this.description = description;
            this.syntax = string.Empty;
        }

        public ConsoleAttribute(int gmlevel, string name, string description, string syntax)
        {
            this.gmlevel = gmlevel;
            this.name = name;
            this.description = description;
            this.syntax = syntax;
        }

        #endregion Constructor / Deconstructor
    }
}