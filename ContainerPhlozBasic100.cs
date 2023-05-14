using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using FatumCore;

namespace PhlozLanguages
{
    public class ContainerPhlozBasic100 : IntLanguage
    {
        IntFunction MainFunction = null;
        IntFunction DocumentFunction = null;

        IntRuntime Runtime = null;
        ArrayList varStack = null;

        EventHandler documentcallback = null;

        ~ContainerPhlozBasic100()
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
            return "PhlozBasic";
        }

        public short initialize(string code, out string CompilationOutput)
        {
            IntError errorCode = new IntError(null, -1);
            if (varStack!=null)
            {
                varStack.Clear();
            }
            varStack = new ArrayList();
            Runtime = CreatePhlozBasic100Runtime(new CallbackEventHandler(callBackHandler), code, out errorCode, out CompilationOutput);
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

        private IntRuntime CreatePhlozBasic100Runtime(CallbackEventHandler M, string code, out IntError errorCode, out string CompilationOutput)
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
