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
        
        public static List<TOutput> ToList<TSource, TOutput>(this IEnumerable<TSource> enumerable, Func<TSource, TOutput> selector)
        {
            var list = new List<TOutput>(enumerable.Count());
            
            for(int i = 0; i < enumerable.Count(); i++)
                list.Add(selector(enumerable.ElementAt(i)));

            return list;
        }
        
        public static List<TOutput> ToList<TSource, TOutput>(this TSource[] array, Func<TSource, TOutput> selector)
        {
            var listOutput = new List<TOutput>(array.Length);
            
            for(int i = 0; i < array.Length; i++)
                listOutput.Add(selector(array[i]));

            return listOutput;
        }
        
        public static List<TOutput> ToList<TSource, TOutput>(this List<TSource> listInput, Func<TSource, TOutput> selector)
        {
            var listOutput = new List<TOutput>(listInput.Count);
            
            for(int i = 0; i < listInput.Count; i++)
                listOutput.Add(selector(listInput[i]));

            return listOutput;
        }
    }
}