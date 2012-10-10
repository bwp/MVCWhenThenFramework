using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace MVCWhenThenFramework
{
    /// <summary>
    /// A class to simulate HttpSession
    /// </summary>
    public class MockHttpSession : HttpSessionStateBase
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public override object this[string name]
        {
            get { return _values.ContainsKey(name) ? _values[name] : null; }
            set { _values[name] = value; }
        }

        public override int CodePage
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override HttpSessionStateBase Contents
        {
            get { throw new NotImplementedException(); }
        }

        public override HttpCookieMode CookieMode
        {
            get { throw new NotImplementedException(); }
        }

        public override int Count
        {
            get { return _values.Count; }
        }

        public new IEnumerable<string> Keys
        {
            get { return _values.Keys; }
        }

        public Dictionary<string, object> UnderlyingStore
        {
            get { return _values; }
        }

        public override void Abandon()
        {
            _values.Clear();
        }

        public override void Add(string name, object value)
        {
            _values.Add(name, value);
        }

        public override void Clear()
        {
            _values.Clear();
        }

        public override void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return _values.Equals(obj);
        }

        public override IEnumerator GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public override int GetHashCode()
        {
            return (_values != null ? _values.GetHashCode() : 0);
        }

        public override void Remove(string name)
        { _values.Remove(name); }

        public override void RemoveAll()
        { _values.Clear(); }

        public override void RemoveAt(int index)
        { throw new NotImplementedException(); }

        public override string ToString()
        { return _values.ToString(); }

        public bool Equals(MockHttpSession other)
        {
            if (ReferenceEquals(null, other))
            { return false; }
            if (ReferenceEquals(this, other))
            { return true; }
            return Equals(other._values, _values);
        }
    }
}