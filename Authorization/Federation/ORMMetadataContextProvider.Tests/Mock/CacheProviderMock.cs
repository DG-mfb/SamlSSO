using System;
using System.Collections.Generic;
using Kernel.Cache;

namespace ORMMetadataContextProvider.Tests.Mock
{
    internal class CacheProviderMock : ICacheProvider
    {
        private IDictionary<string, object> _cache = new Dictionary<string, object>();
        public event EventHandler WrittenTo;
        public event EventHandler ReadFrom;

        public void Clear()
        {
            this._cache.Clear();
        }

        public bool Contains(string key)
        {
            return this._cache.ContainsKey(key);
        }

        public object Delete(string key)
        {
            if (this.Contains(key))
                this._cache.Remove(key);
            return null;
        }

        public object Get(string key)
        {
            if (this.Contains(key))
            {
                this.OnRead();
                return this._cache[key];
            }
            return null;
        }

        public T Get<T>(string key)
        {
            if (this.Contains(key))
            {
                this.OnRead();
                return (T)this._cache[key];
            }
            return default(T);
        }

        public void Initialise()
        {
            throw new NotImplementedException();
        }

        public void Post(string key, object value)
        {
            this.Post(key, value, null);
        }

        public void Post(string key, object value, ICacheItemPolicy policy)
        {
            if (this.Contains(key))
                throw new InvalidOperationException(String.Format("Key: {0} already exists", key));
            this.Put(key, value);
        }

        public void Put(string key, object value)
        {
            this.Put(key, value, null);
        }

        public void Put(string key, object value, ICacheItemPolicy policy)
        {
            this._cache[key] = value;
        }

        public IDictionary<string, T> TypeOf<T>()
        {
            throw new NotImplementedException();
        }
        protected virtual void OnRead()
        {
            if (this.ReadFrom == null)
                return;

            this.ReadFrom(this, new EventArgs());
        }
    }
}