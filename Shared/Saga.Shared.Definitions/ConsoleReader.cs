using Saga.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Saga.Shared.Definitions
{
    public class SagaTestException : Exception { }

    public delegate void ConsoleCommandHandler(string[] args);

    public class ConsoleReader
    {
        #region Members

        public event EventHandler Initialize;

        private string _Title = "Server instance";
        private SortedDictionary<string, ConsoleCommand> callback = new SortedDictionary<string, ConsoleCommand>();

        #endregion Members

        #region Constructor / Decontructor

        public ConsoleReader()
        {
            try
            {
                Register(new ConsoleCommandHandler(Clear));
                Register(new ConsoleCommandHandler(Help));
            }
            catch (SystemException e)
            {
                System.Diagnostics.Trace.TraceError(e.Message);
            }
        }

        #endregion Constructor / Decontructor

        #region Public Properties

        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                if (value == null)
                    _Title = string.Empty;
                else
                    _Title = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        [DebuggerNonUserCode()]
        private ConsoleCommand CreateConsoleCommand(ConsoleAttribute attribute, ConsoleCommandHandler handler)
        {
            ConsoleCommand scommand = new ConsoleCommand();
            if (attribute != null)
            {
                scommand.description = attribute.Description;
                scommand.syntax = attribute.Syntax;
                scommand.mingmlevel = attribute.GmLevel;
                scommand.handler = handler;
            }

            return scommand;
        }

        [DebuggerNonUserCode()]
        public void Register(ConsoleCommandHandler handler)
        {
            ConsoleCommand scommand;
            ConsoleAttribute[] attribute = handler.Method.GetCustomAttributes(typeof(ConsoleAttribute), true) as ConsoleAttribute[];
            if (attribute != null && attribute.Length > 0)
            {
                scommand = CreateConsoleCommand(attribute[0], handler);
                callback.Add(attribute[0].Name.ToUpperInvariant(), scommand);
            }
            else
            {
                throw new SystemException("Cannot register command: no information found");
            }
        }

        [DebuggerNonUserCode()]
        public void Register(string command, ConsoleCommandHandler handler)
        {
            ConsoleCommand scommand;
            ConsoleAttribute[] attribute = handler.Method.GetCustomAttributes(typeof(ConsoleAttribute), true) as ConsoleAttribute[];
            if (attribute != null && attribute.Length > 0)
            {
                scommand = CreateConsoleCommand(attribute[0], handler);
                callback.Add(command.ToUpperInvariant(), scommand);
            }
            else
            {
                throw new SystemException("Cannot register command: no information found");
            }
        }

        [DebuggerNonUserCode()]
        public void Register(string command, string description, ConsoleCommandHandler handler)
        {
            ConsoleCommand scommand;
            ConsoleAttribute[] attribute = handler.Method.GetCustomAttributes(typeof(ConsoleAttribute), true) as ConsoleAttribute[];
            if (attribute != null && attribute.Length > 0)
            {
                scommand = CreateConsoleCommand(attribute[0], handler);
                scommand.description = (description == null) ? string.Empty : description;
                callback.Add(command.ToUpperInvariant(), scommand);
            }
            else
            {
                throw new SystemException("Cannot register command: no information found");
            }
        }

        [DebuggerNonUserCode()]
        public void Register(string command, string description, string syntax, ConsoleCommandHandler handler)
        {
            ConsoleCommand scommand;
            ConsoleAttribute[] attribute = handler.Method.GetCustomAttributes(typeof(ConsoleAttribute), true) as ConsoleAttribute[];
            if (attribute != null && attribute.Length > 0)
            {
                scommand = CreateConsoleCommand(attribute[0], handler);
                scommand.description = (description == null) ? string.Empty : description;
                scommand.syntax = (syntax == null) ? string.Empty : syntax;
                callback.Add(command.ToUpperInvariant(), scommand);
            }
            else
            {
                throw new SystemException("Cannot register command: no information found");
            }
        }

        public void Start()
        {
            //Clear(null);
            if (Initialize != null) Initialize.Invoke(this, EventArgs.Empty);
            Read();
        }

        #endregion Public Methods

        #region Private Methods

        [DebuggerNonUserCode()]
        private string[] DelemitedString(string mystring)
        {
            List<String> stringlist = new List<string>();
            StringBuilder builder = new StringBuilder();
            bool NextSpecial = false;
            bool QuoteOpen = false;

            for (int i = 0; i < mystring.Length; i++)
            {
                char current = mystring[i];
                if (NextSpecial == false)
                {
                    switch (current)
                    {
                        case ' ':
                            if (QuoteOpen == false)
                            {
                                stringlist.Add(builder.ToString());
                                builder = new StringBuilder();
                            }
                            else
                            {
                                builder.Append(current);
                            }
                            break;

                        case '\\':
                            NextSpecial = true;
                            break;

                        case '"':
                            QuoteOpen ^= true;
                            break;

                        default:
                            builder.Append(current);
                            break;
                    }
                }
                else
                {
                    NextSpecial = false;
                    switch (current)
                    {
                        case ' ':
                            if (QuoteOpen == false)
                            {
                                stringlist.Add(builder.ToString());
                                builder = new StringBuilder();
                            }
                            else
                            {
                                builder.Append(current);
                            }
                            break;

                        case '"':
                            builder.Append(current);
                            break;

                        default:
                            builder.Append('\\');
                            builder.Append(current);
                            break;
                    }
                }
            }
            stringlist.Add(builder.ToString());
            return stringlist.ToArray();
        }

        [DebuggerNonUserCode()]
        private void Read()
        {
            try
            {
                while (Environment.HasShutdownStarted == false)
                {
                    string command = Console.ReadLine();
                    DoString(command);
                }
            }
            catch (Exception e)
            {
                if (e is SagaTestException)
                    throw e;
                //Excpeption don't process
            }
        }

        [DebuggerNonUserCode()]
        private void DoString(string command)
        {
            ConsoleCommand handler;
            string[] commands = DelemitedString(command);
            if (command.Length > 0 && commands.Length > 0)
                if (!callback.TryGetValue(commands[0].ToUpperInvariant(), out handler))
                {
                    Console.WriteLine("Command does not exists");
                }
                else
                {
                    try
                    {
                        if (handler.handler != null)
                            handler.handler.Invoke(commands);
                    }
                    catch (Exception e)
                    {
                        if (e is SagaTestException)
                            throw e;
                        Console.WriteLine("Command did not execute it had errors");
                        System.Diagnostics.Trace.TraceError(e.ToString());
                    }
                }
        }

        #endregion Private Methods

        #region Nested Classes/Structures

        protected class ConsoleCommand
        {
            public string description;
            public string syntax;
            public int mingmlevel;
            public ConsoleCommandHandler handler;
        }

        #endregion Nested Classes/Structures

        #region Built-in

        [ConsoleAttribute("Clear", "Clears the console")]
        public void Clear(string[] args)
        {
            Console.Clear();
            Console.Write(Resource.AsciiHeader);
            Console.WriteLine(Title);
        }

        [ConsoleAttribute("Help", "Lists all available commands")]
        public void Help(string[] args)
        {
            if (args.Length == 1)
            {
                Console.WriteLine();
                Console.WriteLine("List of available commands:");
                foreach (string a in callback.Keys)
                {
                    Console.WriteLine("   {0}", a.ToLower());
                }
            }
            else if (args.Length == 2)
            {
                ConsoleCommand command;
                if (callback.TryGetValue(args[1].Trim(' ', '\0').ToUpperInvariant(), out command))
                {
                    Console.WriteLine();
                    Console.WriteLine("Name:            {0}", args[1].ToLower());
                    Console.WriteLine("Description:     {0}", command.description);
                    Console.WriteLine("Syntax:          {0}", command.syntax);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Command not found");
                }
            }
            else
            {
                Console.WriteLine("Invalid argument for comamnd");
            }
        }

        #endregion Built-in
    }
}