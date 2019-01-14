using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class ExtensionMethods
{
    public static TSource[] ToArray_MY<TSource>(this IEnumerable<TSource> source, int count)
    {
        if (source == null) throw new ArgumentNullException("source");
        if (count < 0) throw new ArgumentOutOfRangeException("count");
        var array = new TSource[count];
        int i = 0;
        foreach (var item in source)
        {
            array[i++] = item;
        }
        return array;
    }
}
