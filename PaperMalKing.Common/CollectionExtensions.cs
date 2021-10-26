﻿#region LICENSE

// PaperMalKing.
// Copyright (C) 2021 N0D4N
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace PaperMalKing.Common
{
    public static class CollectionExtensions
    {
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            using var provider = new RNGCryptoServiceProvider();
            var n = list.Count;
            Span<byte> box = stackalloc byte[sizeof(int)];
            while (n > 1)
            {
                provider.GetBytes(box);
                var bit = BitConverter.ToInt32(box);
                var k = Math.Abs(bit) % n;
                n--;
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static (IReadOnlyList<T> AddedValues, IReadOnlyList<T> RemovedValues) GetDifference<T>(
            this IReadOnlyList<T> original, IReadOnlyList<T> resulting) where T : IEquatable<T>
        {
            var originalHs = new HashSet<T>(original);
            var resultingHs = new HashSet<T>(resulting);
            originalHs.ExceptWith(resulting);
            resultingHs.ExceptWith(original);
            var added = resultingHs.ToArray() ?? Array.Empty<T>();
            var removed = originalHs.ToArray() ?? Array.Empty<T>();
            return (added, removed);
        }

        public static T[] ForEach<T>(this T[] array, Action<T> action)
        {
            Array.ForEach(array, action);
            return array;
        }

		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) where TKey : IComparable<TKey> =>
            source.Aggregate((accumulator, next) => selector(accumulator).CompareTo(selector(next)) < 0 ? next : accumulator);

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) where TKey : IComparable<TKey> =>
            source.Aggregate((accumulator, next) => selector(accumulator).CompareTo(selector(next)) > 0 ? next : accumulator);
    }
}