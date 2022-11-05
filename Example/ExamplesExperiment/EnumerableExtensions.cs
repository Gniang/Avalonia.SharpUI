using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

public static class EnumerableExtensions
{
    public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> sourceItems, IEnumerable<T> items)
    {
        var itProperty = typeof(ObservableCollection<T>).GetProperty("Items", BindingFlags.NonPublic | BindingFlags.Instance);
        var colResetMethod = typeof(ObservableCollection<T>).GetMethod("OnCollectionReset", BindingFlags.NonPublic | BindingFlags.Instance);

        if (itProperty?.GetValue(sourceItems) is IList<T> list)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
            colResetMethod?.Invoke(sourceItems, null);
        }
        return sourceItems;
    }

    /// <summary>
    /// Generates a sequence that contains one repeated value.
    /// </summary>
    /// <param name="element">The value to be repeated.</param>
    /// <param name="count">The number of times to repeat the value in the generated sequence.</param>
    /// <exception cref="ArgumentOutOfRangeException">count is less than 0.</exception>
    /// <typeparam name="T"></typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains a repeated value.</returns>
    public static IEnumerable<T> Repeat<T>(this T element, int count)
    {
        return Enumerable.Repeat(element, count);
    }

    /// <summary>
    /// Concatenates the members of a collection, using the specified separator between each member.
    /// </summary>
    /// <param name="values">A collection that contains the objects to concatenate.</param>
    /// <param name="sepalater">The string to use as a separator. separator is included in the returned string only if values has more than one element.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>A string that consists of the elements of values delimited by the separator string. -or- System.String.Empty if values has no elements.</returns>
    public static string JoinString<T>(this IEnumerable<T> values, string sepalater)
    {
        return string.Join(sepalater, values);
    }

    /// <summary>
    /// Concatenate and flatten multipe <see cref="IEnumerable{T}"/> or <see cref="{T}"/> objects.
    /// </summary>
    /// <param name="enumerableItems"></param>
    /// <exception cref="System.ArgumentException">enumerableItems contains other than <see cref="{T}"/> or <see cref="IEnumerable{T}"/> </exception>
    /// <typeparam name="T"></typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> that concatenated and flatted input items.</returns>
    public static T[] ToArrayFlat<T>(params object[] enumerableItems)
    {
        return ConcatFlat<T>(enumerableItems).ToArray();
    }

    /// <summary>
    /// Concatenate and flatten multipe <see cref="IEnumerable{T}"/> or <see cref="{T}"/> objects.
    /// </summary>
    /// <param name="enumerableItems"></param>
    /// <exception cref="System.ArgumentException">enumerableItems contains other than <see cref="{T}"/> or <see cref="IEnumerable{T}"/> </exception>
    /// <typeparam name="T"></typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> that concatenated and flatted input items.</returns>
    public static IEnumerable<T> ConcatFlat<T>(params object[] enumerableItems)
    {
        return enumerableItems.SelectMany(x =>
        {
            return x switch
            {
                IEnumerable<T> a => a,
                T c => ToEnumeralbe(c),
                _ => throw new ArgumentException($"type miss match. expect type is [{typeof(T).FullName}]. actual type is [{x.GetType().FullName}]"),
            };
        });
    }

    /// <summary>
    /// Concatenate and flatten multipe Enumerable objects.
    /// <code>
    /// Concat( new [] {1, 2}, new {] {3, 4} new [] {5, 6} ) => [ 1, 2, 3, 4, 5, 6 ]
    /// </code>
    /// </summary>
    /// <param name="enumerableItems">Enumerable objects.</param>
    /// <return>An <see cref="IEnumerable{T}"/> that concatenated and flatted input items.</return>
    public static IEnumerable<T> ConcatFlat<T>(params IEnumerable<T>[] enumerableItems)
    {
        return enumerableItems.SelectMany(x => x);
    }

    /// <summary>
    /// Concatenate single object to Enumerable objects.
    /// </summary>
    /// <param name="source">Single object.</param>
    /// <param name="values">Enumerable objects.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that concatenated input items.</returns>
    public static IEnumerable<T> Concat<T>(this T source, IEnumerable<T> values)
    {
        return ToEnumeralbe(source).Concat(values);
    }

    /// <summary>
    /// Concatinate Enumerable objects to single object.
    /// </summary>
    /// <param name="source">Enumerable objects.</param>
    /// <param name="value">Single object.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> that concatenated input items.</returns>
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T value)
    {
        return source.Concat(ToEnumeralbe(value));
    }

    public static IEnumerable<T> ToEnumeralbe<T>(this T source)
    {
        yield return source;
    }
}