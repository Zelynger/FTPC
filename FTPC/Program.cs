using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;

/*
 Written by: Ankuda Dmitri
 Task: Write a console FTP-client.
 */

namespace FTPC
{
    class Program
    {

        public static string command = "", dir = "", conString = "";
        public static bool connectionFlag = false;
        public static FtpWebRequest ftpRequest = null;

        static void Main(string[] args)
        {

            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("║                  FTPC                  ║");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Type \"help\" for a list of commands.");
            Console.WriteLine();
            
            while (command != "exit")
            {
                command = Console.ReadLine();

                // swtich-structure for various commands specified in command list
                switch (command.Split(' ')[0])
                {
                    // Command list
                    case "help":
                        {
                            HelpCommand();
                            break;
                        }
                    // FTP-connection
                    case "connect":
                        {
                            ConnectionCommand(ftpRequest);
                            break;
                        }
                    // Subdirectories and files in current directory
                    case "sd":
                        {
                            ShowDirectoriesCommand(ftpRequest);
                            break;
                        }
                    // Directory changing 
                    case "cd":
                        {
                            ChangeDirectoryCommand(ftpRequest);
                            break;
                        }
                    // File downloading
                    case "df":
                        {
                            DownloadFileCommand(ftpRequest);
                            break;
                        }
                    case "exit":
                        {
                            break;
                        }
                    //If neither of commands is correct
                    default:
                        {
                            Console.WriteLine("Wrong command. Type \"help\" for a list of commands.");
                            break;
                        }
                }
            }
            try
            {
                ftpRequest.Abort();
            }
            catch
            {
            }
        }

        static void HelpCommand()
        {
            Console.WriteLine();
            Console.WriteLine("╔════════════════COMMANDS════════════════╗");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("║ help - list of commands                ║");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("║ connect <address> - connect to address ║");
            Console.WriteLine("║ sd - show subdirectories and files     ║");
            Console.WriteLine("║ cd <directory> - specified directory   ║");
            Console.WriteLine("║ cd .. - parent directory               ║");
            Console.WriteLine("║ df <file name> - download file         ║");
            Console.WriteLine("║ exit - close program                   ║");
            Console.WriteLine("║                                        ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine();
        }

        static void ConnectionCommand(FtpWebRequest fr)
        {
            connectionFlag = false;
            conString = "ftp://" + command.Split()[1];
            fr = (FtpWebRequest)WebRequest.Create(conString);
            dir = conString;
            //Connection checking
            try
            {
                fr.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)fr.GetResponse();
                connectionFlag = true;
                response.Close();
            }
            catch
            {
            }
            if (connectionFlag == true)
            {
                Console.WriteLine("Connected.");
            }
            else
            {
                Console.WriteLine("Connection failed.");
            }
            Console.WriteLine();

        }

        static void ShowDirectoriesCommand(FtpWebRequest fr)
        {
            if (connectionFlag == false)
            {
                Console.WriteLine("You need to connect first!");
            }
            else
            {
                fr = (FtpWebRequest)WebRequest.Create(dir);
                fr.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                FtpWebResponse response = (FtpWebResponse)fr.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                Console.WriteLine(reader.ReadToEnd());
                reader.Close();
                response.Close();
            }
        }

        static void ChangeDirectoryCommand(FtpWebRequest fr)
        {
            if (connectionFlag == false)
            {
                Console.WriteLine("You need to connect first!");
            }
            else
            {
                dir = dir + "/" + command.Split()[1];
                Console.WriteLine();
                fr = (FtpWebRequest)WebRequest.Create(dir);
            }
        }

        static void DownloadFileCommand(FtpWebRequest fr)
        {
            string fullFileName = dir + "/" + command.Split()[1];
            string savePath = "";

            Console.WriteLine();
            Console.WriteLine("Please enter a correct path to save the file.");
            savePath = Console.ReadLine();
            try
            {
                using (WebClient request = new WebClient())
                {
                    byte[] fileData = request.DownloadData(fullFileName);

                    using (FileStream file = File.Create(savePath + command.Split()[1]))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Download complete.");
                Console.WriteLine();
            }
            catch
            {
                Console.WriteLine();
                Console.WriteLine("There is no file with such name.");
                Console.WriteLine();
            }
        }
    }
}
