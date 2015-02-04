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
        static void Main(string[] args)
        {
            string command = "", dir = "", conString = "";
            bool connectionFlag = false;
            FtpWebRequest ftpRequest = null;

            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║                  FTPC                  ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Type \"help\" for a list of commands.");
            Console.WriteLine();
            //Mark for returning after input
            Choice:
            command = Console.ReadLine();
            // swtich-structure for various commands specified in command list
            switch (command.Split(' ')[0])                                                          
            {
                // Command list
                case "help":
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
                        goto Choice;
                    }
                // FTP-connection
                case "connect":
                    {
                        connectionFlag = false;
                        conString = "ftp://" + command.Split()[1];
                        ftpRequest = (FtpWebRequest)WebRequest.Create(conString);
                        dir = conString;
                        //Connection checking
                        try
                        {
                            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                            FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
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
                        goto Choice;
                    }
                // Subdirectories and files in current directory
                case "sd":
                    {
                        if (connectionFlag == false)
                        {
                            Console.WriteLine("You need to connect first!");
                        }
                        else
                        {
                            ftpRequest = (FtpWebRequest)WebRequest.Create(dir);
                            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                            FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                            Stream responseStream = response.GetResponseStream();
                            StreamReader reader = new StreamReader(responseStream);
                            Console.WriteLine(reader.ReadToEnd());
                            reader.Close();
                            response.Close();
                        }
                        goto Choice;
                    }
                // Directory changing 
                case "cd":
                    {
                        if (connectionFlag == false)
                        {
                            Console.WriteLine("You need to connect first!");
                        }
                        else
                        {
                            dir = dir + "/" + command.Split()[1];
                            Console.WriteLine();
                            ftpRequest = (FtpWebRequest)WebRequest.Create(dir);
                        }
                        goto Choice;
                    }
                // File downloading
                case "df":
                    {
                        
                        string fullFileName = dir +"/"+ command.Split()[1];
                        string savePath = @"C:\";
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
                            Console.WriteLine("Download complete.");
                            Console.WriteLine();
                        }
                        catch 
                        {
                            Console.WriteLine();
                            Console.WriteLine("There is no file with such name.");
                        }
                        goto Choice;
                    }
                // Program closing
                case "exit":
                    {
                        try
                        {
                            ftpRequest.Abort();
                        }
                        catch
                        {
                        }
                        break;
                    }
                //If neither of commands is correct
                default:
                    {
                        Console.WriteLine("Wrong command. Type \"help\" for a list of commands.");
                        goto Choice;
                    }
            }
        }
    }
}
