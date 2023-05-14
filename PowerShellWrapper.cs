using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace PhlozLanguages
{
    class PowerShellWrapper
    {
        string filename = "";
        ArrayList data = new ArrayList();

        ~PowerShellWrapper()
        {

        }

        public string getVersion()
        {
            return "1.0.0";
        }

        public string getName()
        {
            return "PowerShell";
        }

        public short initialize(string code, out string CompilationOutput)
        {
            CompilationOutput = "";
            filename = code;


            data.Clear();
            return 0;
        }

        public void addVariable(string varname, string value)
        {
            data.Add("varname" + "=" + value + "\r\n");
        }

        public void dispose()
        {

        }

        public short execute(out string ExecutionOutput)
        {
            PowerShell psinstance = PowerShell.Create();
            psinstance.AddScript(filename);
            var results = psinstance.Invoke();
            ExecutionOutput = "";

            return 0;
        }
    }
}
