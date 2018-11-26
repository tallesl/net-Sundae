using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System;

public static class BlockingCollection
{
    public static IEnumerable<T> TakeAll<T>(this BlockingCollection<T> pending, int timeout = 300)
    {
        var failed = false;

        for (;;)
        {
            T element;

            var anything = pending.TryTake(out element, timeout);

            if (anything)
                yield return element;

            else if (!failed)
                failed = true;

            else
                break;
        }
    }
}