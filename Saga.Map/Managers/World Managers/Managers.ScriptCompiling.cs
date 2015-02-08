using Microsoft.CSharp;
using Saga.Configuration;
using Saga.Map.Configuration;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;

namespace Saga.Managers
{
    public class ScriptCompiler : ManagerBase2
    {
        #region Ctor/Dtor

        public ScriptCompiler()
        {
        }

        #endregion Ctor/Dtor

        #region Internal Members

        //Settings
        internal static Assembly ScriptingAssembly;

        internal string Directory;

        #endregion Internal Members

        #region Protected Methods

        protected override void Initialize()
        {
            CoreService.OnProbeUnresolvedType += new UnresolvedType(CoreService_OnProbeUnresolvedType);
        }

        protected override void QuerySettings()
        {
            ScriptingSettings section = (ScriptingSettings)ConfigurationManager.GetSection("Saga.Manager.Scripting");
            if (section != null)
                Directory = Saga.Structures.Server.SecurePath(section.Directory);
            else
                Directory = "Scripting";
        }

        [EnvironmentPermissionAttribute(SecurityAction.Demand, Unrestricted = true)]
        protected override void Load()
        {
            WriteLine("ScriptCompiler", "Starting to compile scripting assembly: {0}", Directory);
            if (Directory != string.Empty)
                CompileFromDirectory(Directory, GetReferencedAssemblies());

            WriteLine("ScriptCompiler", "Starting to compile scripting assembly: {0}", ScriptingAssembly);
        }

        protected virtual IEnumerable<string> GetReferencedAssemblies()
        {
            try
            {
                ScriptingSettings section = (ScriptingSettings)ConfigurationManager.GetSection("Saga.Manager.Scripting");
                foreach (FactoryFileElement element in section.Assemblies)
                {
                    yield return element.Path;
                }
            }
            finally { }
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected virtual bool CompileFromDirectory(string directory, IEnumerable<string> ReferencedAssemblies)
        {
            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                string[] files = System.IO.Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
                ScriptingAssembly = CompileScript(files, provider, ReferencedAssemblies);
                if (ScriptingAssembly == null)
                    WriteWarning("ScriptCompiler", "scripting assembly compiled with errors");
                else
                    WriteWarning("ScriptCompiler", "scripting assembly was compiled");

                return true;
            }
            catch (DirectoryNotFoundException)
            {
                WriteError("ScriptCompiler", "Scripting directory was not found: {0}", directory);
                return false;
            }
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected virtual CompilerParameters getCompilerParameters(IEnumerable<string> ReferencedAssemblies)
        {
            WriteLine("ScriptCompiler", "Generate compile parameters");

            //CREATE PARAM OBJECT
            CompilerParameters parms = new CompilerParameters();

            //GET THE CURRENT ASSEMBLY AND PATH
            string file = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = Path.GetDirectoryName(file);
            string fname = Path.GetFileNameWithoutExtension(file);

            //CREATE A MD5 UNIQUE FILENAME TOKEN TO FIX A BUG. THE SCRIPTING ASSEMBLY CAN'T
            //HANDLE TO DOUBLE TOKEN ASSEMBLIES.
            MD5 md5 = MD5.Create();
            byte[] dec = Encoding.Unicode.GetBytes(fname);
            byte[] enc = md5.ComputeHash(dec);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < enc.Length; i++)
            {
                builder.AppendFormat("{0:X2}", enc[i]);
            }
            string assname = builder.ToString();

#if DEBUG

            //GENERAL SETTNGS
            parms.CompilerOptions = "/target:library";
            parms.GenerateExecutable = false;
            parms.GenerateInMemory = false;
            parms.OutputAssembly = assname + ".scripting";
            parms.IncludeDebugInformation = true;

#else

            //GENERAL SETTNGS
            parms.CompilerOptions = "/target:library /optimize";
            parms.GenerateExecutable = false;
            parms.GenerateInMemory = true;
            parms.IncludeDebugInformation = true;

#endif

            //ADD GENERAL ASSEMBLIES WE ALWAYS NEED
            parms.ReferencedAssemblies.Add(Path.Combine(path, "Saga.Shared.Definitions.dll"));
            parms.ReferencedAssemblies.Add("System.Data.dll");
            parms.ReferencedAssemblies.Add(file);

            //GENERATE OPTIONAL ASSEMBLIES PROVIDED BY THE USER
            foreach (string assemblyfile in ReferencedAssemblies)
            {
                parms.ReferencedAssemblies.Add(assemblyfile);
            }

            return parms;
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected Assembly CompileScript(string[] Source, CodeDomProvider Provider, IEnumerable<string> ReferencedAssemblies)
        {
            if (Source.Length == 0)
            {
                WriteInformation("ScriptCompiler", "No scripts to compile");
                return null;
            }

            CompilerResults results;
            CompilerParameters parms = getCompilerParameters(ReferencedAssemblies);
            results = Provider.CompileAssemblyFromFile(parms, Source);
            int tempCount = 0;
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                {
                    if (error.IsWarning == false)
                    {
                        //throw new CompileException(string.Format("Error {0} on file {1} on line {2}", error.ErrorText, error.FileName, error.Line))
                        WriteError("ScriptCompiler", "Error {0} on file {1} on line {2}", error.ErrorText, error.FileName, error.Line);
                        tempCount++;
                    }
                    else
                    {
                        WriteWarning("ScriptCompiler", "Error {0} on file {1} on line {2}", error.ErrorText, error.FileName, error.Line);
                    }
                }

                if (tempCount > 0)
                    return null;
            }
            else
            {
                WriteInformation("ScriptCompiler", "Scripts compiled no errors");
            }

            return results.CompiledAssembly;
        }

        #endregion Protected Methods

        #region Public Methods

        public bool TryFindType(string name, out object type)
        {
            return CoreService.TryFindType(name, out type);
        }

        public bool TryFindType(string name, out Type type)
        {
            object a;
            bool result = TryFindType(name, out a);
            if (a != null)
                type = a.GetType();
            else
                type = null;
            return result;
        }

        #endregion Public Methods

        #region Event Callbacks

        private bool CoreService_OnProbeUnresolvedType(string name, out Type type)
        {
            type = null;
            if (ScriptingAssembly != null)
            {
                type = ScriptingAssembly.GetType(name, false, false);
            }

            if (type == null)
            {
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                type = assembly.GetType(name, false, false);
            }

            return (type != null);
        }

        #endregion Event Callbacks

        #region Nested

        private class CompileException : Exception
        {
            public CompileException(string message)
                : base(message)
            {
            }

            public CompileException(string message, Exception innerexception)
                : base(message, innerexception)
            {
            }
        }

        #endregion Nested
    }
}