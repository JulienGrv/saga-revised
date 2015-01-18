using System;
using System.Collections.Generic;
using System.Text;
using LuaInterface;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;

namespace Saga.Tools.DBInstaller
{
    class Program
    {

        static Lua myLua = new Lua();
        static void Main(string[] args)
        {            

            myLua.NewTable("DBInstaller");
            myLua.RegisterFunction("print", null, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
            myLua.RegisterFunction("prompt", null, typeof(Program).GetMethod("Prompt"));
            myLua.RegisterFunction("promptstring", null, typeof(Program).GetMethod("PromptString"));
            myLua.RegisterFunction("doproccess", null, typeof(Program).GetMethod("DoProcess"));
            


            if (args.Length == 0)
            {                                               
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    myLua.DoFile(dialog.FileName);
                }
            }
            else if (args.Length == 1)
            {
                myLua.DoFile(args[0]);
            }


            Console.WriteLine("");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

        }


        public static string PromptString(string question)
        {
            Console.WriteLine(question);
            return Console.ReadLine();
        }

        public static char Prompt()
        {            
            return Console.ReadKey(true).KeyChar;
        }

        public static void DoProcess(string exe, string parameters)
        {
            try
            {

                int l1 = parameters.LastIndexOf('>');
                int l2 = parameters.LastIndexOf('<');

                string filename = "";

                if (l1 == -1 && l2 == -1)
                {
                    parameters = parameters;
                }
                else if (l1 > -1)
                {
                    filename = parameters.Substring(l1 + 1).Trim();
                    parameters = parameters.Substring(0, l1).Trim();
                }
                else if (l2 > -1)
                {
                    filename = parameters.Substring(l2 + 1).Trim();
                    parameters = parameters.Substring(0, l2).Trim();
                }

                Process myProcess = new Process();
                myProcess.StartInfo.FileName = exe;
                myProcess.StartInfo.Arguments = parameters;
                myProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                myProcess.StartInfo.RedirectStandardError = true;
                myProcess.StartInfo.RedirectStandardInput = true;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.Start();


                if (l1 == -1 && l2 == -1)
                {
                    myProcess.WaitForExit();
                }
                if (l1 > -1)
                {
                    using (StreamWriter wr = new StreamWriter(filename))
                    {
                        while (myProcess.HasExited == false)
                        {
                            while (myProcess.StandardOutput.Peek() > -1)
                            {
                                wr.WriteLine(myProcess.StandardOutput.ReadLine());
                            }
                            myProcess.StandardOutput.DiscardBufferedData();
                        }
                        myProcess.WaitForExit();
                    }
                }
                else if (l2 > -1)
                {
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        while (sr.Peek() > -1)
                        {
                            myProcess.StandardInput.WriteLine(sr.ReadLine());
                        }
                        myProcess.StandardInput.WriteLine("\\q");
                        myProcess.WaitForExit();
                    }
                }

                
            }
            catch (Win32Exception e)
            {
                Console.WriteLine("Filename: {0}", Path.Combine(Environment.CurrentDirectory, Path.GetFullPath(exe)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
