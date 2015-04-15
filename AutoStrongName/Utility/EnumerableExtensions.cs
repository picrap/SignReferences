#region AutoStrongName
// AutoStrongName
// An automatic tool to presign unsigned dependencies
// https://github.com/picrap/AutoStrongName
#endregion

namespace AutoStrongName.Utility
{
    using System;
    using System.Collections.Generic;

    internal static class EnumerableExtensions
    {
        public static void ForAll<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
        {
            foreach (var item in items)
                action(item);
        }
    }
}
