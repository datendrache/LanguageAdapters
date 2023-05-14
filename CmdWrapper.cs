using FatumCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhlozLanguages
{
    public class CmdWrapper : IntLanguage
    {
        string filename = "";
        ArrayList data = new ArrayList();
        EventHandler documentCallback = null;

        ~CmdWrapper()
        {

        }

        public string getVersion()
        {
            return "1.0.0";
        }

        public string getName()
        {
            return "CMD";
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
        public void addVariable(string varname, Tree value)
        {
            data.Add(TreeDataAccess.writeTreeToXMLString(value,"Metadata") + "\r\n");
        }

        public void dispose()
        {

        }

        public short execute(out string ExecutionOutput)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(filename);

            psi.RedirectStandardOutput = true;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;

            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(psi);
            StreamWriter infoToSend = proc.StandardInput;
            System.IO.StreamReader myOutput = proc.StandardOutput;

            foreach (string current in data)
            {
                infoToSend.WriteLine(current);
            }

            proc.WaitForExit();
            ExecutionOutput = myOutput.ReadToEnd();
            data.Clear();
            return 0;
        }

        public short document(out string ExecutionOutput)
        {
            return execute(out ExecutionOutput);
        }

        public void setCallback(EventHandler handler)
        {
            documentCallback += handler;
        }
    }
}
