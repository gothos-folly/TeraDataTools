using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GothosDC.LowLevel
{
   public static class Helpers
    {
       public static Dictionary<TKey, TValue> CollectionToDictionary<T,TKey,TValue>(this ICollection<T> collection, Func<T,TKey> keySelector,Func<T,TValue> valueSelector)
       {
           var result = new Dictionary<TKey, TValue>(collection.Count);
           foreach (var item in collection)
           {
               result.Add(keySelector(item), valueSelector(item));
           }
           return result;
       }
    }
}
