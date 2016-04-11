﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public interface IParserErrorReporter
    {
        void ReportError(string msg);
    }
}
