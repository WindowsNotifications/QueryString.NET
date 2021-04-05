﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.QueryStringDotNET
{
    internal static class ListExtensions
    {
        internal static bool RemoveFirstWhere<T>(this IList<T> list, Func<T, bool> shouldRemovePredicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (shouldRemovePredicate.Invoke(list[i]))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// A single query string parameter (name and value pair).
    /// </summary>
    public sealed class QueryStringParameter
    {
        private string _name;

        /// <summary>
        /// The name of the parameter. Cannot be null.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Name");

                _name = value;
            }
        }

        /// <summary>
        /// The value of the parameter (or null if there's no value).
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new query string parameter with the specified name and optional value.
        /// </summary>
        /// <param name="name">The name of the parameter. Cannot be null.</param>
        /// <param name="value">The optional value of the parameter.</param>
        internal QueryStringParameter(string name, string value = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
            Value = value;
        }
    }

    /// <summary>
    /// Specifies the separator to be used between query string parameters.
    /// </summary>
    public enum QueryStringSeparator
    {
        /// <summary>
        /// The default separator for query string parameters. Generated query string is like "a=&b=5".
        /// </summary>
        Ampersand,

        /// <summary>
        /// An alternative separator for query string parameters. Generated query string is like "a=1;b=5".
        /// </summary>
        Semicolon
    }

    /// <summary>
    /// A portable string serializer/deserializer for .NET.
    /// </summary>
    public class QueryString : IEnumerable<QueryStringParameter>, IEquatable<QueryString>
    {
        private Dictionary<string, List<string>> _dictionary = new Dictionary<string, List<string>>();

        /// <summary>
        /// Constructs a new empty query string.
        /// </summary>
        public QueryString()
        {
            // Nothing
        }

        /// <summary>
        /// Gets the first value of the first parameter with the matching name. Throws <see cref="KeyNotFoundException"/> if a parameter with a matching name could not be found. O(n) where n = Count of the current object.
        /// </summary>
        /// <param name="name">The parameter name to find.</param>
        /// <returns></returns>
        public string this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                string value;

                if (TryGetValue(name, out value))
                    return value;

                throw new KeyNotFoundException($"A parameter with name '{name}' could not be found.");
            }
        }

        /// <summary>
        /// Gets the first value of the first parameter with the matching name. If no parameter with a matching name exists, returns false.
        /// </summary>
        /// <param name="name">The parameter name to find.</param>
        /// <param name="value">The parameter's value will be written here once found.</param>
        /// <returns></returns>
        public bool TryGetValue(string name, out string value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            List<string> values;

            if (_dictionary.TryGetValue(name, out values))
            {
                value = values.First();
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Gets the values of the parameter with the matching name. If no parameter with a matching name exists, sets <paramref name="values"/> to null and returns false.
        /// </summary>
        /// <param name="name">The parameter name to find.</param>
        /// <param name="values">The parameter's values will be written here once found.</param>
        /// <returns></returns>
        public bool TryGetValues(string name, out string[] values)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            List<string> storedValues;

            if (_dictionary.TryGetValue(name, out storedValues))
            {
                values = storedValues.ToArray();
                return true;
            }

            values = null;
            return false;
        }

        /// <summary>
        /// Returns the count of parameters in the current query string.
        /// </summary>
        public int Count()
        {
            return _dictionary.Select(i => i.Value.Count).Sum();
        }
        
        /// <summary>
        /// Adds a query string parameter to the query string.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The optional value of the parameter.</param>
        public void Add(string name, string value = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            List<string> values;

            if (!_dictionary.TryGetValue(name, out values))
            {
                values = new List<string>();
                _dictionary[name] = values;
            }

            values.Add(value);
        }

        /// <summary>
        /// Sets a query string parameter. If there are existing parameters with the same name, they are removed.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The optional value of the parameter.</param>
        public void Set(string name, string value = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            _dictionary[name] = new List<string>()
            {
                value
            };
        }

        /// <summary>
        /// Determines if the query string contains at least one parameter with the specified name.
        /// </summary>
        /// <param name="name">The parameter name to look for.</param>
        /// <returns>True if the query string contains at least one parameter with the specified name, else false.</returns>
        public bool Contains(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return _dictionary.ContainsKey(name);
        }

        /// <summary>
        /// Determines if the query string contains a parameter with the specified name and value.
        /// </summary>
        /// <param name="name">The parameter name to look for.</param>
        /// <param name="value">The value to look for when the name has been matched.</param>
        /// <returns>True if the query string contains a parameter with the specified name and value, else false.</returns>
        public bool Contains(string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            List<string> values;

            return _dictionary.TryGetValue(name, out values) && values.Contains(value);
        }

        /// <summary>
        /// Removes the first parameter with the specified name.
        /// </summary>
        /// <param name="name">The name of parameter to remove.</param>
        /// <returns>True if the parameters were removed, else false.</returns>
        public bool Remove(string name)
        {
            List<string> values;

            if (_dictionary.TryGetValue(name, out values))
            {
                if (values.Count == 1)
                    _dictionary.Remove(name);
                else
                    values.RemoveAt(0);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all parameters with the specified name.
        /// </summary>
        /// <param name="name">The name of parameters to remove.</param>
        /// <returns>True if the parameters were removed, else false.</returns>
        public bool RemoveAll(string name)
        {
            return _dictionary.Remove(name);
        }

        /// <summary>
        /// Removes the first parameter with the specified name and value.
        /// </summary>
        /// <param name="name">The name of the parameter to remove.</param>
        /// <param name="value"></param>
        /// <returns>True if parameter was removed, else false.</returns>
        public bool Remove(string name, string value)
        {
            List<string> values;

            if (_dictionary.TryGetValue(name, out values))
            {
                if (values.RemoveFirstWhere(i => object.Equals(i, value)))
                {
                    // If removed last value, remove the key
                    if (values.Count == 0)
                        _dictionary.Remove(name);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all parameters with the specified name and value.
        /// </summary>
        /// <param name="name">The name of parameters to remove.</param>
        /// <param name="value">The value to match when deciding whether to remove.</param>
        /// <returns>The count of parameters removed.</returns>
        public int RemoveAll(string name, string value)
        {
            List<string> values;

            if (_dictionary.TryGetValue(name, out values))
            {
                int countRemoved = values.RemoveAll(i => object.Equals(i, value));

                // If removed last value, remove the key
                if (values.Count == 0)
                    _dictionary.Remove(name);

                return countRemoved;
            }

            return 0;
        }

        private static string UrlEncode(string str)
        {
            return Uri.EscapeDataString(str)

                // It incorrectly encodes spaces as %20, should use +
                .Replace("%20", "+");
        }

        private static string UrlDecode(string str)
        {
            // Doesn't handle decoding the +, so we manually do that
            return Uri.UnescapeDataString(str.Replace('+', ' '));
        }

        /// <summary>
        /// Parses a query string into a <see cref="QueryString"/> object. Keys/values are automatically URL decoded.
        /// </summary>
        /// <param name="queryString">The query string to deserialize. This should NOT have a leading ? character. Valid input would be something like "a=1&amp;b=5". URL decoding of keys/values is automatically performed. Also supports query strings that are serialized using ; instead of &amp;, like "a=1;b=5"</param>
        /// <returns></returns>
        public static QueryString Parse(string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
                return new QueryString();

            string[] pairs = queryString.Split('&', ';');

            QueryString answer = new QueryString();

            foreach (string pair in pairs)
            {
                string name;
                string value;

                int indexOfEquals = pair.IndexOf('=');

                if (indexOfEquals == -1)
                {
                    name = UrlDecode(pair);
                    value = null;
                }

                else
                {
                    name = UrlDecode(pair.Substring(0, indexOfEquals));
                    value = UrlDecode(pair.Substring(indexOfEquals + 1));
                }

                answer.Add(name, value);
            }

            return answer;
        }

        /// <summary>
        /// Serializes the key-value pairs into a query string, using the default &amp; separator. Produces something like "a=1&amp;b=5". URL encoding of keys/values is automatically performed. Null values are not written (only their key is written).
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(QueryStringSeparator.Ampersand);
        }

        private static string GetSeparatorString(QueryStringSeparator separator)
        {
            switch (separator)
            {
                case QueryStringSeparator.Ampersand:
                    return "&";

                case QueryStringSeparator.Semicolon:
                    return ";";

                default:
                    throw new NotImplementedException(separator.ToString());
            }
        }

        /// <summary>
        /// Serializes the key-value pairs into a query string, using the specified separator. URL encoding of keys/values is automatically performed. Null values are not written (only their key is written). If there are no parameters, an empty string will be returned.
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ToString(QueryStringSeparator separator)
        {
            return string.Join(GetSeparatorString(separator), this.Select(

                pair =>

                    // Key
                    UrlEncode(pair.Name) +

                    // Write value if not null
                    ((pair.Value == null) ? "" : ("=" + UrlEncode(pair.Value)))

                ));
        }

        /// <summary>
        /// Gets an enumerator to enumerate the query string parameters. Note that order of the parameters is NOT preserved.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<QueryStringParameter> GetEnumerator()
        {
            foreach (var pair in _dictionary)
            {
                foreach (var value in pair.Value)
                {
                    yield return new QueryStringParameter(pair.Key, value);
                }
            }
        }

        /// <summary>
        /// Gets an enumerator to enumerate the query string parameters.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines whether the current query string is equivalent to the provided query string.
        /// </summary>
        /// <param name="other">The query string to compare to.</param>
        /// <returns>Returns true if the query string has the exact same parameters as the current query string (order is irrelevant).</returns>
        public bool Equals(QueryString other)
        {
            return this.Equals(other, default(StringComparison), default(StringComparison));
        }

        public bool Equals(QueryString other, StringComparison nameComparisonType, StringComparison valueComparisonType)
        {
            // If they have a different count of keys
            if (_dictionary.Count != other._dictionary.Count)
                return false;
            
            // Go through each key from current object
            foreach (var param in _dictionary)
            {
                // Get values for this key
                List<string> thisValues = param.Value;
                
                List<string> otherValues;

                // If the other didn't have param name
                if (!other._dictionary.TryGetValue(param.Key, out otherValues))
                    return false;

                // If the count of values is different
                if (thisValues.Count != otherValues.Count)
                    return false;

                // Create copy of the other values list
                otherValues = new List<string>(otherValues);

                // And then remove matching values
                foreach (string thisVal in thisValues)
                {
                    // If we couldn't find matching to remove
                    if (!otherValues.RemoveFirstWhere(i => ValueEquals(thisVal, i, valueComparisonType)))
                        return false;
                }
            }

            // Otherwise they're equal, all matched!
            return true;
        }

        private bool ValueEquals(string value1, string value2, StringComparison comparisonType)
        {
            // If both are null, true
            if (value1 == null && value2 == null)
                return true;

            // If only one is null, and therefore the other initialized, then false
            if (value1 == null || value2 == null)
                return false;

            // Otherwise both are initialized, compare with equals
            return value1.Equals(value2, comparisonType);
        }
    }
}
