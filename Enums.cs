using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StripSymmetry
{
    internal abstract class Enums<U> where U : class
    {
        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <typeparam name="EnumType">The type of the enumeration.</typeparam>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        /// <exception cref="System.ArgumentException">value is either an empty string or only contains white space.-or- value is a name, but not one of the named constants defined for the enumeration.</exception>
        /// <exception cref="System.OverflowException">value is outside the range of the underlying type of EnumType</exception>
        /// <returns>An object of type enumType whose value is represented by value.</returns>
        static public EnumType Parse<EnumType>(string value) where EnumType : U
        {
            return (EnumType)Enum.Parse(typeof(EnumType), value);
        }
    }

    abstract class Enums : Enums<Enum>
    {
    }

    static class ExtensionMethods
    {
        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> list)
        {
            var enumerator = list.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var curr = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    yield return curr;
                    curr = enumerator.Current;
                }
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> list, int n)
        {
            var enumerator = list.GetEnumerator();
            var buffer = new T[n];
            int idx = 0;
            while (enumerator.MoveNext() && idx < n)
            {
                buffer[idx] = enumerator.Current;
                idx++;
            }
            idx = 0;
            do
            {
                yield return buffer[idx];
                buffer[idx] = enumerator.Current;
                idx++;
                if (idx >= n)
                {
                    idx = 0;
                }
            }
            while (enumerator.MoveNext());
        }
    }
}
