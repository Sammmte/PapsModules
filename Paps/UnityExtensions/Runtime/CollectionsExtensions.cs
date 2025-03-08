using System;
using System.Collections.Generic;
using System.Linq;

namespace Paps.UnityExtensions
{
    public static class CollectionsExtensions
    {
        public static TOutput[] ToArray<TSource, TOutput>(this IEnumerable<TSource> enumerable, Func<TSource, TOutput> selector)
        {
            var array = new TOutput[enumerable.Count()];
            
            for(int i = 0; i < array.Length; i++)
                array[i] = selector(enumerable.ElementAt(i));

            return array;
        }
        
        public static TOutput[] ToArray<TSource, TOutput>(this TSource[] array, Func<TSource, TOutput> selector)
        {
            var arrayOutput = new TOutput[array.Length];
            
            for(int i = 0; i < arrayOutput.Length; i++)
                arrayOutput[i] = selector(array[i]);

            return arrayOutput;
        }
        
        public static TOutput[] ToArray<TSource, TOutput>(this List<TSource> list, Func<TSource, TOutput> selector)
        {
            var arrayOutput = new TOutput[list.Count];
            
            for(int i = 0; i < arrayOutput.Length; i++)
                arrayOutput[i] = selector(list[i]);

            return arrayOutput;
        }
    }
}