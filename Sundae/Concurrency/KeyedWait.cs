namespace Sundae
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    internal class KeyedWait<TKey, TValue>
    {
        private readonly IDictionary<TKey, AutoResetEvent> _pendingSet = new Dictionary<TKey, AutoResetEvent>();

        private readonly IDictionary<TKey, TValue> _pendingGet = new Dictionary<TKey, TValue>();

        private readonly object _lock = new object();

        internal bool Set(TKey key, TValue value)
        {
            lock (_lock)
            {
                if (!_pendingSet.ContainsKey(key))
                    return false;

                _pendingGet.Add(key, value);
                Pop(_pendingSet, key).Set();

                return true;
            }
        }

        internal Task<TValue> Get(TKey key, int? millisecondsTimeout = null)
        {
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

                    if (millisecondsTimeout.HasValue)
                        signal.WaitOne(millisecondsTimeout.Value);
                    else
                        signal.WaitOne();

                    return Pop(_pendingGet, key);
                }
            );
        }

        private TTValue Pop<TTKey, TTValue>(IDictionary<TTKey, TTValue> dict, TTKey key)
        {
            var value = dict[key];

            dict.Remove(key);

            return value;
        }
    }
}