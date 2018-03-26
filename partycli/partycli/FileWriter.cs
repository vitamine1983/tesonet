using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace partycli
{
    class FileWriter
    {
        public string local_storage_file = "server_storage.txt";
        public static bool WriteLog(string strMessage)
        {
            string log_file_name = "log_file.txt";
            try
            {
                FileStream fs = 
                    new FileStream(string.Format("{0}\\{1}", Path.GetTempPath(), log_file_name), 
                    FileMode.Append, 
                    FileAccess.Write);

                StreamWriter sw = new StreamWriter((Stream)fs);
                sw.WriteLine(strMessage);
                sw.Close();

                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to namage file {0}", ex.ToString());
                return false;
            }
        }

        public static void LocalStorageWrite(string file_name, string data)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string directory = Path.GetDirectoryName(path);
            FileStream fs =
               new FileStream(string.Format("{0}\\{1}", directory, file_name),
                FileMode.Create,
                FileAccess.Write);

            StreamWriter sw = new StreamWriter((Stream)fs);
            sw.WriteLine(data);
            sw.Close();

            fs.Close();
        }
        public static string LocalStorageRead(string file_name)
        {
            try
            {
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string directory = Path.GetDirectoryName(path);

                return File.ReadAllText(string.Format("{0}\\{1}", directory, file_name));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to read file {0}", ex.ToString());
                return "";
            }
        }
    }
}
