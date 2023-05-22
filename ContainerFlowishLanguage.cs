//   Language Adapters -- Allows for multiple embedded languages
//
//   Copyright (C) 2003-2023 Eric Knight
//   This software is distributed under the GNU Public v3 License
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.

//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.

//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections;
using Proliferation.Flowish;
using Proliferation.Fatum;

namespace Proliferation.LanguageAdapters
{
    public class ContainerFlowish : IntLanguage
    {
        IntFunction MainFunction = null;
        IntFunction DocumentFunction = null;

        IntRuntime Runtime = null;
        ArrayList varStack = null;

        EventHandler documentcallback = null;

        ~ContainerFlowish()
        {
            MainFunction = null;
            Runtime = null;
            if (varStack!=null)
            {
                varStack.Clear();
            }
            varStack = null;
        }

        public string getVersion()
        {
            return "1.0.0";
        }

        public string getName()
        {
            return "Flowish";
        }

        public short initialize(string code, out string CompilationOutput)
        {
            IntError errorCode = new IntError(null, -1);
            if (varStack!=null)
            {
                varStack.Clear();
            }
            varStack = new ArrayList();
            Runtime = CreateFlowishRuntime(new CallbackEventHandler(callBackHandler), code, out errorCode, out CompilationOutput);
            if (errorCode==null)
            {
                MainFunction = IntFunction.locateFunction("main", Runtime.programAssembly.Functions);
                DocumentFunction = IntFunction.locateFunction("document", Runtime.programAssembly.Functions);

                return 0;
            }
            else
            {
                return errorCode.ErrorType;
            }
        }

        private void callBackHandler(Object o, CallbackEventArgs callbackEvent)
        {
            if (documentcallback!=null)
            {
                documentcallback.Invoke(callbackEvent.Message, new EventArgs());
            }
            else
            {
                ArrayList bitbucket = (ArrayList)o;
                bitbucket.Clear();
            }
        }

        public void setCallback(EventHandler handler)
        {
            documentcallback += handler;
        }

        public void addVariable(string varname, string value)
        {
            IntToken msgToken = new IntToken(varname, IntConstants.STRING);
            msgToken.variableReference = new VarString();
            msgToken.variableReference.setValue(value);
            varStack.Add(msgToken);
        }

        public void addVariable(string varname, Tree value)
        {
            IntToken msgToken = new IntToken(varname, IntConstants.Tree);
            msgToken.variableReference = new VarTree();
            msgToken.variableReference.setValue(value);
            varStack.Add(msgToken);
        }

        public void dispose()
        {
            MainFunction = null;
            Runtime = null;
            if (varStack != null)
            {
                varStack.Clear();
            }
            varStack = null;
        }

        public short execute(out string ExecutionOutput)
        {
            Runtime.lineNumber = 0;

            // Run "Main" code

            ArrayList opStack = new ArrayList();
            ArrayList tmpVarStack = new ArrayList();

            IntError errormsg = Interpreter.processRPN(Runtime, tmpVarStack, MainFunction, varStack, null, opStack);

            // Cleanup

            foreach (IntToken current in varStack)
            {
                if (current.array != null) current.array.Clear();
            }
            varStack.Clear();

            foreach (IntToken current in opStack)
            {
                if (current.array != null) current.array.Clear();
            }
            opStack.Clear();

            foreach (IntToken current in tmpVarStack)
            {
                if (current.array != null) current.array.Clear();
            }
            tmpVarStack.Clear();

            ExecutionOutput = "";

            foreach (string current in Runtime.Output)
            {
                ExecutionOutput += current;
            }
            
            if (errormsg!=null)
            {
                return errormsg.ErrorType;
            }
            else
            {
                return 0;
            }
        }

        private IntRuntime CreateFlowishRuntime(CallbackEventHandler M, string code, out IntError errorCode, out string CompilationOutput)
        {
            IntRuntime runtime = new IntRuntime();
            string compilerDetails = "";

            // Compile code

            ArrayList parsedCode = Interpreter.Compile(code);

            int linecount = 1;

            foreach (ArrayList line in parsedCode)
            {
                foreach (IntToken current in line)
                {
                    compilerDetails += current.linenumber.ToString() + "v," + current.text + ":" + current.tokentype.ToString() + "\r\n";
                }
                linecount++;
            }

            ArrayList sections = Interpreter.divideSections(parsedCode);
            IntAssembly newAssembly = new IntAssembly();

            for (int i = 0; i < sections.Count; i++)
            {
                ArrayList currentFunction = (ArrayList)sections[i];

                if (i == sections.Count - 1)
                {
                    IntFunction newFunction = new IntFunction("main", null, currentFunction);
                    newFunction.updateParameters();
                    newAssembly.Functions.Add(newFunction);
                }
                else
                {
                    IntFunction newFunction = new IntFunction(currentFunction, false);
                    newFunction.updateParameters();
                    newAssembly.Functions.Add(newFunction);
                }
            }

            IntAssembly.assignFunctions(newAssembly);
            errorCode = Interpreter.Compile(newAssembly);

            runtime.Output = new ArrayList();
            runtime.programAssembly = newAssembly;
            runtime.OnDocument += M;

            CompilationOutput = compilerDetails;
            return runtime;
        }

        public short document(out string ExecutionOutput)
        {
            Runtime.lineNumber = 0;

            // Run "Main" code

            ArrayList opStack = new ArrayList();
            ArrayList tmpVarStack = new ArrayList();

            IntError errormsg = Interpreter.processRPN(Runtime, tmpVarStack, DocumentFunction, varStack, null, opStack);

            // Cleanup

            foreach (IntToken current in varStack)
            {
                if (current.array != null) current.array.Clear();
            }
            varStack.Clear();

            foreach (IntToken current in opStack)
            {
                if (current.array != null) current.array.Clear();
            }
            opStack.Clear();

            foreach (IntToken current in tmpVarStack)
            {
                if (current.array != null) current.array.Clear();
            }
            tmpVarStack.Clear();

            ExecutionOutput = "";

            foreach (string current in Runtime.Output)
            {
                ExecutionOutput += current;
            }

            if (errormsg != null)
            {
                return errormsg.ErrorType;
            }
            else
            {
                return 0;
            }
        }
    }
}
