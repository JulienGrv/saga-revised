using System;
using System.Collections.Generic;
using System.Text;

namespace Saga
{
    /// <summary>
    /// List of strings delimited by a ',' as
    /// delimited character.
    /// </summary>
    public class CommaDelimitedString
    {
        #region Private Members

        /// <summary>
        /// Contains a list of delimited strings
        /// </summary>
        private string[] _Fields;

        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Gets the delimited string
        /// </summary>
        /// <param name="index">Index of the string to access</param>
        /// <returns>Delimited string</returns>
        public string this[int index]
        {
            get
            {
                return _Fields[index];
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Parses a delimited string to be seperated
        /// </summary>
        /// <param name="value">String to seperated</param>
        /// <returns>Returns a delimited string</returns>
        public static CommaDelimitedString Parse(string value)
        {
            List<String> stringlist = new List<string>();
            StringBuilder builder = new StringBuilder();
            bool NextSpecial = false;
            bool QuoteOpen = false;

            for (int i = 0; i < value.Length; i++)
            {
                char current = value[i];
                if (NextSpecial == false)
                {
                    switch (current)
                    {
                        case ',':
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
                        case ',':
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

            CommaDelimitedString delimitedString = new CommaDelimitedString();
            delimitedString._Fields = stringlist.ToArray();
            return delimitedString;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string a in this._Fields)
                sb.AppendLine(a);
            return sb.ToString();
        }

        #endregion Public Methods
    }
}