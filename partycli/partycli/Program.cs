using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;

using System.IO;

namespace partycli
{
    class Program
    {
        static string local_storage_fn = "local_storage.txt";
        static string auth_url = "http://playground.tesonet.lt/v1/tokens";
        static string server_list_url = "http://playground.tesonet.lt/v1/servers";

        static void Main(string[] args)
        {
            Console.WriteLine("Enter values to arguments {0}", args.Length);
//            string token = GetToken(auth_url, Properties.Settings.Default.auth_user, Properties.Settings.Default.auth_psw);
//            GetServers(server_list_url, token);

            if (args.Length == 0)
            {
                FileWriter.WriteLog(string.Format("Enter values to arguments {0}\r\n", args.Length));
                return;
            }
            else
            {
                //  Make processing there
                if (args[0] == "config")
                {
                    if (args.Length != 5)
                    {
                        Console.WriteLine("Wrong numer of config values");
                        FileWriter.WriteLog(string.Format("config wrong number of config values\r\n", args.Length));
                        return;
                    }

                    // partycli.exe config --username "YOUR USERNAME"--password "YOUR PASSWORD"
                    Console.WriteLine("Configuration entered");
                    if (args[1] == "--username")
                    {
                        Properties.Settings.Default.auth_user = args[2];
                        Console.WriteLine("User name entered");
                        //  Parse user password
                        if (args[3] == "--password")
                        {
                            Properties.Settings.Default.auth_psw = args[4];
                            Console.WriteLine("Password entered");
                        }
                    }
                    FileWriter.WriteLog("Authorisation changed\r\n");
                    Properties.Settings.Default.Save();
                }
                else if (args[0] == "config?")
                {
                    Console.Write("User:\t");
                    Console.WriteLine(Properties.Settings.Default.auth_user);
                    Console.Write("Password:\t");
                    Console.WriteLine(Properties.Settings.Default.auth_psw);
                }
                if (args[0] == "server_list")
                {
                    //  partycli.exe server_list
                    if (args.Length == 1)
                    {
                        string token = GetToken(auth_url, Properties.Settings.Default.auth_user, Properties.Settings.Default.auth_psw);
                        GetServers(server_list_url, token);
                    }
                    else if (args.Length == 2)
                    {
                        if (args[1] == "--local")
                        {
                            //  Get server list from local storage
                            var servers_list = FileWriter.LocalStorageRead(local_storage_fn);
                            try
                            {
                                var servers = JsonConvert.DeserializeObject<List<Servers>>(servers_list);
                                DisplayServers(ConsoleColor.DarkYellow, servers);
                            }
                            catch (Exception ex)
                            {
                                FileWriter.WriteLog(string.Format("Enter values to arguments {0}", ex.ToString()));
                                return;
                            }
                        }
                    }
                    else
                    {
                        FileWriter.WriteLog(string.Format("server_list wrong parameters\r\n", args.Length));
                        Console.WriteLine("Wrong parameters");
                    }
                }
            }
        }

        static string GetToken(string url, string UserName, string Password)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                User user = new User();
                user.username = UserName;
                user.password = Password;
                string json = JsonConvert.SerializeObject(user);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result;
            Tokens server_token;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
                server_token = JsonConvert.DeserializeObject<Tokens>(result);
            }
            return server_token.token;
        }
        static void GetServers(string url, string token)
        {
            //  Get servers part
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + token;
                string servers_list = wc.DownloadString(server_list_url);
                FileWriter.LocalStorageWrite(local_storage_fn, servers_list);

                var servers = JsonConvert.DeserializeObject<List<Servers>>(servers_list);
                DisplayServers(ConsoleColor.Blue, servers);
            }
        }
        static void DisplayServers(ConsoleColor color, List<Servers> serv)
        {
            ConsoleColor prev_col = Console.BackgroundColor;
            Console.BackgroundColor = color;
            Console.WriteLine("Number of servers {0}", serv.Count);
            
            foreach (Servers server in serv)
            {
                Console.WriteLine("Distance [{0, 10}] | Server [{1, 30}]",
                    server.distance,
                    server.name
                    );
            }
            Console.BackgroundColor = prev_col;
        }
    }
}
