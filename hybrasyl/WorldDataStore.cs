﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Hybrasyl
{
    public class WorldDataStore
    {
        private ConcurrentDictionary<Type, ConcurrentDictionary<string, dynamic>> _dataStore;
        private ConcurrentDictionary<Type, ConcurrentDictionary<dynamic, dynamic>> _index;

        /// <summary>
        /// Constructor, takes no arguments.
        /// </summary>
        public WorldDataStore()
        {
            _dataStore = new ConcurrentDictionary<Type, ConcurrentDictionary<string, dynamic>>();
            _index = new ConcurrentDictionary<Type, ConcurrentDictionary<dynamic, dynamic>>();
        }

        /// <summary>
        /// Get a substore for a given type T.
        /// </summary>
        /// <typeparam name="T">The type to fetch</typeparam>
        /// <returns></returns>
        private ConcurrentDictionary<string, dynamic> GetSubStore<T>()
        {
            if (_dataStore.ContainsKey(typeof(T)))
            {
                return _dataStore[typeof(T)];
            }
            _dataStore.TryAdd(typeof(T), new ConcurrentDictionary<string, dynamic>());
            return _dataStore[typeof(T)];
        }

        /// <summary>
        /// Get a subindex for the given type T.
        /// </summary>
        /// <typeparam name="T">The type to fetch</typeparam>
        /// <returns></returns>
        private ConcurrentDictionary<dynamic, dynamic> GetSubIndex<T>()
        {
            if (_index.ContainsKey(typeof(T)))
            {
                return _index[typeof(T)];
            }
            _index.TryAdd(typeof(T), new ConcurrentDictionary<dynamic, dynamic>());
            return _index[typeof(T)];
        }

        /// <summary>
        /// Given a type and a key, return the typed object matching the key, or a default value.
        /// </summary>
        /// <typeparam name="T">The type to be returned</typeparam>
        /// <param name="key">The key for the object</param>
        /// <returns></returns>
        public T Get<T>(dynamic key)
        {
            if (_dataStore.ContainsKey(typeof(T)))
            {
                return (T) _dataStore[typeof(T)][key.ToString()];
            }
            return default(T);
        }

        /// <summary>
        /// Given a type and a key, return the typed object matching the key in the subindex,
        /// or a default value.
        /// </summary>
        /// <typeparam name="T">The type to be returned</typeparam>
        /// <param name="key">The index key for the object</param>
        /// <returns>Found object</returns>

        public T GetByIndex<T>(dynamic key)
        {
            if (_index.ContainsKey(typeof(T)))
            {
                return (T) _index[typeof(T)][key];
            }
            return default(T);
        }

        /// <summary>
        /// Try to find a typed value in the store given a key.
        /// </summary>
        /// <typeparam name="T">The type to be returned</typeparam>
        /// <param name="key">The key</param>
        /// <param name="tresult">The out parameter which will contain the object, if found</param>
        /// <returns>True or false depending on whether or not item was found</returns>
        public bool TryGetValue<T>(dynamic key, out T tresult)
        {
            tresult = default(T);
            var sub = GetSubStore<T>();
            if (!sub.ContainsKey(key.ToString())) return false;
            tresult = (T) sub[key.ToString()];
            return true;
        }

        /// <summary>
        /// Try to find a typed value in the store given an index key.
        /// </summary>
        /// <typeparam name="T">The type to be returned</typeparam>
        /// <param name="key">The index key</param>
        /// <param name="tresult">The out parameter which will contain the object, if found</param>
        /// <returns>True or false depending on whether or not item was found</returns>
        public bool TryGetValueByIndex<T>(dynamic key, out T tresult)
        {
            tresult = default(T);
            var sub = GetSubIndex<T>();
            if (!sub.ContainsKey(key)) return false;
            tresult = (T)sub[key];
            return true;
        }

        /// <summary>
        /// Store an object in the datastore with the given key.
        /// </summary>
        /// <typeparam name="T">The type to be stored</typeparam>
        /// <param name="key">The key to be used for the object</param>
        /// <param name="value">The actual object to be stored</param>
        /// <returns>Boolean indicating success</returns>
        public bool Set<T>(dynamic key, T value) => GetSubStore<T>().TryAdd(key.ToString(), value);

        /// <summary>
        /// Store an object in the datastore with the given key and index key.
        /// </summary>
        /// <typeparam name="T">The type to be stored</typeparam>
        /// <param name="key">The key for the object</param>
        /// <param name="value">The actual object to be stored</param>
        /// <param name="index">The index key for the object</param>
        /// <returns>Boolean indicating success</returns>
        public bool SetWithIndex<T>(dynamic key, T value, dynamic index) => GetSubStore<T>().TryAdd(key.ToString(), value) &&
            GetSubIndex<T>().TryAdd(index, value);
   

        /// <summary>
        /// Returns all the objects contained in the datastore of the specified type's substore.
        /// </summary>
        /// <typeparam name="T">The type to be returned.</typeparam>
        /// <returns></returns>      
        public IEnumerable<T> Values<T>() => GetSubStore<T>().Values.Cast<T>();

        /// <summary>
        /// Returns all the keys contained in the datastore for the specified type's substore.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<string> Keys<T>() => GetSubStore<T>().Keys;

        /// <summary>
        /// Checks to see whether a key exists in the datastore for a given type.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <param name="key">The key to check</param>
        /// <returns>Boolean indicating whether or not the key exists</returns>
        public bool ContainsKey<T>(dynamic key) => GetSubStore<T>().ContainsKey(key.ToString());

        /// <summary>
        /// Return a count of typed objects in the datastore.
        /// </summary>
        /// <typeparam name="T">The type for which to produce a count</typeparam>
        /// <returns>Integer number of objects</returns>
        public int Count<T>() => GetSubStore<T>().Count;

        /// <summary>
        /// Get an IDictionary which will only contain values of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <returns>IDictionary of objects of the specified type.</returns>
        public IDictionary<string, T> GetDictionary<T>() => (IDictionary<string,T>) _dataStore[typeof(T)];

        /// <summary>
        /// Remove an object from the datastore.
        /// </summary>
        /// <typeparam name="T">The type of the object to remove</typeparam>
        /// <param name="key">The key corresponding to the object to be removed</param>
        /// <returns></returns>
        public bool Remove<T>(dynamic key)
        {
            dynamic ignored;
            return GetSubStore<T>().TryRemove(key, out ignored);
        }

    }
}
