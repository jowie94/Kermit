﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal
{
    internal class GlobalScope : BaseScope
    {
        internal GlobalScope() : base(null)
        {
            ScopeName = "global";
        }
    }
}