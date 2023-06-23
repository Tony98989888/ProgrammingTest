using System.Collections.Generic;

public static class CollectionHelper
{
    public static void AddItem<K, V>(this SerializableDictionary<K, List<V>> serializableDic, K key, V value)
    {
        if (serializableDic.ContainsKey(key))
        {
            serializableDic[key].Add(value);
            return;
        }
        serializableDic.Add(key, new List<V>() { value });
    }
}
