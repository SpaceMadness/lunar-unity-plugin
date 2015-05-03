using System;
using System.Collections.Generic;

namespace LunarPluginInternal
{
    static class ListUtils
    {
        // not good at all
        public static void Sort<T>(IList<T> list, Comparison<T> comparation = null)
        {
            if (list is ReusableList<T>)
            {
                if (comparation != null)
                {
                    ((ReusableList<T>)list).Sort(comparation);
                }
                else
                {
                    ((ReusableList<T>)list).Sort();
                }
            }
            else if (list is List<T>)
            {
                if (comparation != null)
                {
                    ((List<T>)list).Sort(comparation);
                }
                else
                {
                    ((List<T>)list).Sort();
                }
            }
        }
    }
}

