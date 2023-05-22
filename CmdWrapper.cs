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

using Proliferation.Fatum;
using System.Collections;

namespace Proliferation.LanguageAdapters
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
            data.Add(TreeDataAccess.WriteTreeToXmlString(value,"Metadata") + "\r\n");
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
