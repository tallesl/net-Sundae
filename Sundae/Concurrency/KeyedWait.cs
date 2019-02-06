namespace Sundae
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    // One thread blocks calling 'Get', another unblocks calling 'Set'.
    // This class enables the 'request-response' mechanism of IQ stanzas to be offered synchronously:
    // TKey should be the IQ stanza id and TValue the stanza returned by the server.
    internal class KeyedWait<TKey, TValue> : IDisposable
    {
        // TKeys pending to be set (pending 'Set' call).
        private readonly IDictionary<TKey, AutoResetEvent> _pendingSet = new Dictionary<TKey, AutoResetEvent>();

        // TValues pending to be get (pending blocking 'Get' call).
        private readonly IDictionary<TKey, TValue> _pendingGet = new Dictionary<TKey, TValue>();

        // Can't get and set a TKey at the same time.
        private readonly object _lock = new object();

        private volatile bool _disposed = false;

        private Exception _innerDisposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            lock (_lock)
            {
                Parallel.ForEach(_pendingSet, kvp => kvp.Value.Set());
            }
        }

        internal void Dispose(Exception innerException)
        {
            _innerDisposed = innerException;
            Dispose();
        }

        // One thread calls 'Get', blocking until a TValue is produced.
        // A TKey is given as a reference of the pending blocking call.
        internal Task<TValue> Get(TKey key, int? timeout = null)
        {
            CheckDisposed();

            return Task.Run(
                () =>
                {
                    var signal = new AutoResetEvent(false);

                    lock (_lock)
                    {
                        if (_pendingSet.ContainsKey(key))
                            throw new InvalidOperationException("There's already a waiting call for this response.");

                        _pendingSet.Add(key, signal);
                    }

                    if (timeout.HasValue)
                        signal.WaitOne(timeout.Value);
                    else
                        signal.WaitOne();

                    return Pop(_pendingGet, key);
                }
            );
        }

        // Another thread calls 'Set' unblocking the pending 'Get' call.
        // The TKey of the blocking call waiting to be release and the desired TValue are given.
        internal bool Set(TKey key, TValue value)
        {
            CheckDisposed();

            lock (_lock)
            {
                if (!_pendingSet.ContainsKey(key))
                    return false;

                _pendingGet.Add(key, value);
                Pop(_pendingSet, key).Set();

                return true;
            }
        }

        // Not a stack, but I like the name.
        private TTValue Pop<TTKey, TTValue>(IDictionary<TTKey, TTValue> dict, TTKey key)
        {
            // This check makes the pending blocking calls end with the exception when the object is disposed.
            CheckDisposed();

            var value = dict[key];

            dict.Remove(key);

            return value;
        }

        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("The XMPP connection was disposed.", _innerDisposed);
        }
    }
}