using System;
using System.Collections.Generic;

public static class RangeExtensions
{
    public static IEnumerator<int> GetEnumerator(this Range r)
    {
        if (r.Start.IsFromEnd) throw new InvalidOperationException();
        if (r.End.IsFromEnd)
        {
            if (r.End.Value == 0)
            {
                for (int i = 0; ; i++)
                {
                    yield return i;
                }
            }
            throw new InvalidOperationException();
        }

        for (int i = r.Start.Value; i < r.End.Value; i++)
        {
            yield return i;
        }
    }

    public static IEnumerable<T> Select<T>(this Range r, Func<int, T> func)
    {
        foreach (var item in r)
        {
            yield return func.Invoke(item);
        }
    }
}