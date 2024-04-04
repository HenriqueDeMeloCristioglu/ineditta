using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.BuildingBlocks.Core.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this T[] source, int chunkSize)
        {
            for (int i = 0; i < source.Length; i += chunkSize)
            {
                yield return source.Skip(i).Take(chunkSize);
            }
        }
    }
}
