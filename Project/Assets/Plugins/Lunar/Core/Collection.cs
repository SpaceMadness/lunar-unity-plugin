using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;

namespace LunarPluginInternal
{
    static class Collection
    {
        public static void Each<T>(IList<T> list, EachDelegate<T> each)
        {
            foreach (T element in list)
            {
                each(element);
            }
        }

        public static void Each<T>(IList<T> list, EachIndexDelegate<T> each)
        {
            int index = 0;
            foreach (T element in list)
            {
                each(element, index++);
            }
        }

        public static OUT[] Map<IN, OUT>(IList<IN> list, MapDelegate<IN, OUT> map)
        {
            OUT[] result = new OUT[list.Count];

            int index = 0;
            foreach (IN element in list)
            {
                result[index++] = map(element);
            }
            return result;
        }

        public static OUT[] Map<IN, OUT>(IList<IN> list, MapIndexDelegate<IN, OUT> map)
        {
            OUT[] result = new OUT[list.Count];

            int index = 0;
            foreach (IN element in list)
            {
                result[index] = map(element, index);
                ++index;
            }
            return result;
        }
    }

    delegate void EachDelegate<T>(T element);
    delegate void EachIndexDelegate<T>(T element, int index);

    delegate OUT MapDelegate<IN, OUT>(IN element);
    delegate OUT MapIndexDelegate<IN, OUT>(IN element, int index);
}
