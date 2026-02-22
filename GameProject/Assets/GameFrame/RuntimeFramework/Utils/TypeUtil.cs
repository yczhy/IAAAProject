using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    public class TypeUtil : MonoBehaviour
    {
        public static string GetName<T>()
        {
            Type type = typeof(T);
            if (type.IsGenericType)
            {
                return type.Name[..type.Name.LastIndexOf('`')];
            }

            return type.Name;
        }
    }
}

