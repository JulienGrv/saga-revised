using System;
using System.Collections.Generic;

namespace Saga.Data
{
    /// <summary>
    /// Parameter collection passed on to our database settings to query
    /// the database.
    /// </summary>
    public class QueryParameterCollection : IEnumerable<KeyValuePair<string, object>>
    {
        #region Private Members

        /// <summary>
        /// Dictionairy containing all parameters.
        /// </summary>
        private Dictionary<string, object> dict
            = new Dictionary<string, object>();

        #endregion Private Members

        #region Public  Methods

        /// <summary>
        /// Adds a parameter to the parameter collection
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        public void AddWithValue(string name, object value)
        {
            if (name == null) throw new NullReferenceException("Parameter name cannot be null");
            if (value != null)
                dict[name] = value;
            else
                dict.Remove(name);
        }

        #endregion Public  Methods

        #region IEnumerable<KeyValuePair<string,object>> Members

        /// <summary>
        /// Gets a list of parameters
        /// </summary>
        /// <returns>Paramater iteratation</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (KeyValuePair<string, object> c in this.dict)
            {
                yield return c;
            }
        }

        #endregion IEnumerable<KeyValuePair<string,object>> Members

        #region IEnumerable Members

        /// <summary>
        /// Gets a list of parameters
        /// </summary>
        /// <returns>Paramater iteratation</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}