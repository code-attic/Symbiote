using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Eidetic
{
    public interface IRemember : IDisposable
    {
        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key">The identifier for the item to delete.</param>
        /// <returns>true if the item was successfully removed from the cache; false otherwise.</returns>
        bool Remove(string key);

        /// <summary>
        /// Retrieves the specified item from the cache.
        /// </summary>
        /// <param name="key">The identifier for the item to retrieve.</param>
        /// <returns>The retrieved item, or <value>null</value> if the key was not found.</returns>
        object Get(string key);

        /// <summary>
        /// Retrieves the specified item from the cache.
        /// </summary>
        /// <param name="key">The identifier for the item to retrieve.</param>
        /// <returns>The retrieved item, or <value>null</value> if the key was not found.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Increments the value of the specified key by the given amount. The operation is atomic and happens on the server.
        /// </summary>
        /// <param name="key">The identifier for the item to increment.</param>
        /// <param name="amount">The amount by which the client wants to increase the item.</param>
        /// <returns>The new value of the item or -1 if not found.</returns>
        /// <remarks>The item must be inserted into the cache before it can be changed. The item must be inserted as a <see cref="T:System.String"/>. The operation only works with <see cref="System.UInt32"/> values, so -1 always indicates that the item was not found.</remarks>
        long Increment(string key, uint amount);

        /// <summary>
        /// Increments the value of the specified key by the given amount. The operation is atomic and happens on the server.
        /// </summary>
        /// <param name="key">The identifier for the item to increment.</param>
        /// <param name="amount">The amount by which the client wants to decrease the item.</param>
        /// <returns>The new value of the item or -1 if not found.</returns>
        /// <remarks>The item must be inserted into the cache before it can be changed. The item must be inserted as a <see cref="T:System.String"/>. The operation only works with <see cref="System.UInt32"/> values, so -1 always indicates that the item was not found.</remarks>
        long Decrement(string key, uint amount);

        /// <summary>
        /// Inserts an item into the cache with a cache key to reference its location.
        /// </summary>
        /// <param name="mode">Defines how the item is stored in the cache.</param>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The object to be inserted into the cache.</param>
        /// <remarks>The item does not expire unless it is removed due memory pressure.</remarks>
        /// <returns>true if the item was successfully stored in the cache; false otherwise.</returns>
        bool Store(StoreMode mode, string key, object value);

        /// <summary>
        /// Inserts a range of bytes (usually memory area or serialized data) into the cache with a cache key to reference its location.
        /// </summary>
        /// <param name="mode">Defines how the item is stored in the cache.</param>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The data to be stored.</param>
        /// <param name="offset">A 32 bit integer that represents the index of the first byte to store.</param>
        /// <param name="length">A 32 bit integer that represents the number of bytes to store.</param>
        /// <remarks>The item does not expire unless it is removed due memory pressure.</remarks>
        /// <returns>true if the item was successfully stored in the cache; false otherwise.</returns>
        bool Store(StoreMode mode, string key, byte[] value, int offset, int length);

        /// <summary>
        /// Inserts an item into the cache with a cache key to reference its location.
        /// </summary>
        /// <param name="mode">Defines how the item is stored in the cache.</param>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The object to be inserted into the cache.</param>
        /// <param name="validFor">The interval after the item is invalidated in the cache.</param>
        /// <returns>true if the item was successfully stored in the cache; false otherwise.</returns>
        bool Store(StoreMode mode, string key, object value, TimeSpan validFor);

        /// <summary>
        /// Inserts an item into the cache with a cache key to reference its location.
        /// </summary>
        /// <param name="mode">Defines how the item is stored in the cache.</param>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The object to be inserted into the cache.</param>
        /// <param name="expiresAt">The time when the item is invalidated in the cache.</param>
        /// <returns>true if the item was successfully stored in the cache; false otherwise.</returns>
        bool Store(StoreMode mode, string key, object value, DateTime expiresAt);

        /// <summary>
        /// Inserts a range of bytes (usually memory area or serialized data) into the cache with a cache key to reference its location.
        /// </summary>
        /// <param name="mode">Defines how the item is stored in the cache.</param>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The data to be stored.</param>
        /// <param name="offset">A 32 bit integer that represents the index of the first byte to store.</param>
        /// <param name="length">A 32 bit integer that represents the number of bytes to store.</param>
        /// <param name="validFor">The interval after the item is invalidated in the cache.</param>
        /// <returns>true if the item was successfully stored in the cache; false otherwise.</returns>
        bool Store(StoreMode mode, string key, byte[] value, int offset, int length, TimeSpan validFor);

        /// <summary>
        /// Inserts a range of bytes (usually memory area or serialized data) into the cache with a cache key to reference its location.
        /// </summary>
        /// <param name="mode">Defines how the item is stored in the cache.</param>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The data to be stored.</param>
        /// <param name="offset">A 32 bit integer that represents the index of the first byte to store.</param>
        /// <param name="length">A 32 bit integer that represents the number of bytes to store.</param>
        /// <param name="expiresAt">The time when the item is invalidated in the cache.</param>
        /// <returns>true if the item was successfully stored in the cache; false otherwise.</returns>
        bool Store(StoreMode mode, string key, byte[] value, int offset, int length, DateTime expiresAt);

        /// <summary>
        /// Appends the data to the end of the specified item's data.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="data">The data to be stored.</param>
        /// <returns>true if the data was successfully stored; false otherwise.</returns>
        bool Append(string key, byte[] data);

        /// <summary>
        /// Inserts the data before the specified item's data.
        /// </summary>
        /// <returns>true if the data was successfully stored; false otherwise.</returns>
        bool Prepend(string key, byte[] data);

        /// <summary>
        /// Updates an item in the cache with a cache key to reference its location, but only if it has not been changed since the last retrieval. The invoker must pass in the value returned by <see cref="M:MultiGet"/> called "cas" value. If this value matches the server's value, the item will be updated; otherwise the update fails.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The object to be inserted into the cache.</param>
        /// <param name="cas">The unique value returned by <see cref="M:MultiGet"/>.</param>
        /// <remarks>The item does not expire unless it is removed due memory pressure.</remarks>
        bool CheckAndSet(string key, object value, ulong cas);

        /// <summary>
        /// Updates an item in the cache with a cache key to reference its location, but only if it has not been changed since the last retrieval. The invoker must pass in the value returned by <see cref="M:MultiGet"/> called "cas" value. If this value matches the server's value, the item will be updated; otherwise the update fails.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The data to be stored.</param>
        /// <param name="offset">A 32 bit integer that represents the index of the first byte to store.</param>
        /// <param name="length">A 32 bit integer that represents the number of bytes to store.</param>
        /// <param name="cas">The unique value returned by <see cref="M:MultiGet"/>.</param>
        /// <remarks>The item does not expire unless it is removed due memory pressure.</remarks>
        bool CheckAndSet(string key, byte[] value, int offset, int length, ulong cas);

        /// <summary>
        /// Updates an item in the cache with a cache key to reference its location, but only if it has not been changed since the last retrieval. The invoker must pass in the value returned by <see cref="M:MultiGet"/> called "cas" value. If this value matches the server's value, the item will be updated; otherwise the update fails.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The object to be inserted into the cache.</param>
        /// <param name="cas">The unique value returned by <see cref="M:MultiGet"/>.</param>
        /// <param name="validFor">The interval after the item is invalidated in the cache.</param>
        bool CheckAndSet(string key, object value, ulong cas, TimeSpan validFor);

        /// <summary>
        /// Updates an item in the cache with a cache key to reference its location, but only if it has not been changed since the last retrieval. The invoker must pass in the value returned by <see cref="M:MultiGet"/> called "cas" value. If this value matches the server's value, the item will be updated; otherwise the update fails.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The object to be inserted into the cache.</param>
        /// <param name="cas">The unique value returned by <see cref="M:MultiGet"/>.</param>
        /// <param name="expiresAt">The time when the item is invalidated in the cache.</param>
        bool CheckAndSet(string key, object value, ulong cas, DateTime expiresAt);

        /// <summary>
        /// Updates an item in the cache with a cache key to reference its location, but only if it has not been changed since the last retrieval. The invoker must pass in the value returned by <see cref="M:MultiGet"/> called "cas" value. If this value matches the server's value, the item will be updated; otherwise the update fails.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The data to be stored.</param>
        /// <param name="offset">A 32 bit integer that represents the index of the first byte to store.</param>
        /// <param name="length">A 32 bit integer that represents the number of bytes to store.</param>
        /// <param name="cas">The unique value returned by <see cref="M:MultiGet"/>.</param>
        /// <param name="validFor">The interval after the item is invalidated in the cache.</param>
        bool CheckAndSet(string key, byte[] value, int offset, int length, ulong cas, TimeSpan validFor);

        /// <summary>
        /// Updates an item in the cache with a cache key to reference its location, but only if it has not been changed since the last retrieval. The invoker must pass in the value returned by <see cref="M:MultiGet"/> called "cas" value. If this value matches the server's value, the item will be updated; otherwise the update fails.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The data to be stored.</param>
        /// <param name="offset">A 32 bit integer that represents the index of the first byte to store.</param>
        /// <param name="length">A 32 bit integer that represents the number of bytes to store.</param>
        /// <param name="cas">The unique value returned by <see cref="M:MultiGet"/>.</param>
        /// <param name="expiresAt">The time when the item is invalidated in the cache.</param>
        bool CheckAndSet(string key, byte[] value, int offset, int length, ulong cas, DateTime expiresAt);

        /// <summary>
        /// Removes all data from the cache.
        /// </summary>
        void FlushAll();

        /// <summary>
        /// Retrieves multiple items from the cache.
        /// </summary>
        /// <param name="keys">The list of identifiers for the items to retrieve.</param>
        /// <returns>a Dictionary holding all items indexed by their key.</returns>
        IDictionary<string, object> Get(IEnumerable<string> keys);

        /// <summary>
        /// Retrieves multiple items from the cache.
        /// </summary>
        /// <param name="keys">The list of identifiers for the items to retrieve.</param>
        /// <param name="casValues">The CAS values for the keys.</param>
        /// <returns>a Dictionary holding all items indexed by their key.</returns>
        IDictionary<string, object> Get(IEnumerable<string> keys, out IDictionary<string, ulong> casValues);

        /// <summary>
        /// Releases all resources allocated by this instance
        /// </summary>
        /// <remarks>Technically it's not really neccesary to call this, since the client does not create "really" disposable objects, so it's safe to assume that when the AppPool shuts down all resources will be released correctly and no handles or such will remain in the memory.</remarks>
        void Dispose();
    }
}
