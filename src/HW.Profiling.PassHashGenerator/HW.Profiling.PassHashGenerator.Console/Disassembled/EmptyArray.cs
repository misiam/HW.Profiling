﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Security.Cryptography
{
    internal static class EmptyArray<T>
    {
        public static readonly T[] Value = new T[0];
    }
}
