//   Phloz
//   Copyright (C) 2006-2019 by Eric Knight

using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using Basic;
using FatumCore;

namespace PhlozLanguages
{
    public interface IntLanguage
    {
        string getVersion();
        string getName();
        short initialize(string code, out string CompilationOutput);
        void addVariable(string varname, string value);
        void addVariable(string varname, Tree value);
        void dispose();
        short execute(out string ExecutionOutput);
        short document(out string ExecutionOutput);
        void setCallback(EventHandler handler);
    }
}
