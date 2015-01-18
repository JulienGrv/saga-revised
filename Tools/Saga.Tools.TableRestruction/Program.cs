using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Saga.Tools.TableRestruction
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) return;
            string filename = (args.Length == 1) ? args[0] : string.Empty;

            if (File.Exists(filename))
                 Process(filename);
             else if (Directory.Exists(filename))
                foreach (string a in Directory.GetFiles(filename))
                     Process(a);
        }

        static void Process(string filename)
        {
            if (Path.GetFileName(filename).ToLowerInvariant() == "saga_backup.sql") return;

            string content = string.Empty;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fs, true))
            {
                string file = reader.ReadToEnd();

                Regex[] regex = new Regex[4];
                regex[0] = new Regex("SET[ \\t]+\\@saved_cs_client[ \\t]+=[ \\t]+\\@\\@character_set_client;", RegexOptions.CultureInvariant);
                regex[1] = new Regex("SET[ \\t]+character_set_client[ \\t]+=[ \\t]+.*?;", RegexOptions.CultureInvariant);
                regex[2] = new Regex("SET[ \\t]+character_set_client[ \\t]+=[ \\t]+\\@saved_cs_client;", RegexOptions.CultureInvariant);
                regex[3] = new Regex("CREATE[ \\t]+TABLE[ \\t]+(IF[ \\t]+]+NOT[ \\t]+EXISTS)?", RegexOptions.CultureInvariant);

                file = regex[0].Replace(file, "");
                file = regex[1].Replace(file, "");
                file = regex[2].Replace(file, "");
                file = regex[3].Replace(file, "CREATE TABLE IF NOT EXISTS ");
                content = file;

                reader.Close();
                fs.Close();
            }

            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            using (StreamWriter writter = new StreamWriter(fs, Encoding.Default))
            {
                writter.Write(content);
                writter.Flush();
                writter.Close();
                fs.Close();
            }
        }
    }
}
