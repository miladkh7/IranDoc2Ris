using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
namespace IranDoc2Ris
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(80, 20);
            do
            {

                UInt16 uintCont = 0;
                UInt16 i;
                string readPath;
                UInt16 isReadExist = 1;
                bool isWriteExit = false;
                bool exitProgram = false;
                do
                {

                    if (isReadExist == 0) Console.WriteLine("File does not exist.");
                    Console.WriteLine("Enter The path Or darg and drop txt Irandoc Refrence File (for menu enter 0)");
                    isReadExist = 0;
                    readPath = Console.ReadLine();
                    if (readPath == "0") {
                        Console.WriteLine("1:About\n2:Help\n3:contiue\n4:exit");
                        string ans = Console.ReadLine();
                        if (ans == "1")
                        {
                            Console.WriteLine("\n\nIntroduction");
                            Console.WriteLine("This Program Wirtten By Milad Khaleghi \nFor Get Ris Information From IranDoc");
                            Console.WriteLine("Milad_khaleghi@live.com");
                            Console.WriteLine("\nSource and Demos");
                            Console.WriteLine("The project and demos are available on GitHub");
                            Console.WriteLine("\nLicense");
                            Console.WriteLine("source code , is licensed under The Microsoft Reciprocal License(Ms - RL)\n\n");
                        }

                        if (ans == "2") Console.WriteLine();
                        if (ans == "3") continue;
                        if (ans == "4") { exitProgram = true;break; }
                    }
                    
               } while (!File.Exists(readPath));
                if (exitProgram) break;
                string savePath = readPath.Replace(".txt", ".Ris");
                do
                {
                    if (isWriteExit)
                    {
                        Console.WriteLine("There is {0}\n1:New Name\n2:OverRide", savePath);
                        string answer = Console.ReadLine();
                        if (answer == "1")
                        {
                            Console.WriteLine("Enter New Name");
                            savePath = Path.GetDirectoryName(readPath) + Console.ReadLine() + ".ris";
                        }
                        if (answer == "2") break;
                    }
                    isWriteExit = true;
                } while (File.Exists(savePath));
                Console.WriteLine("Export File");
                Console.WriteLine(savePath);
                //string savePath = @"E:\MY free projecet\Text Mining2\sample.ris";

                //// #it worked But
                ////foreach (string line in File.ReadAllLines(path))
                ////{
                ////    MessageBox.Show(line);

                ////    string[] parts = line.Split(',');
                ////    foreach (string part in parts)
                ////    {

                ////        //MessageBox.Show(part);
                ////    }
                ////    i++; // For demonstration.
                ////}
                ArrayList Publishes = new ArrayList();
                FileStream s = new FileStream(readPath, FileMode.Open);
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                string line;
                while (reader.EndOfStream != true)
                {
                    uintCont++;
                    line = reader.ReadLine();
                    i = 0;
                    Publish currentPublish = new Publish();
                    while (line != "﻿" && line != null)
                    {
                        i++;
                        if (i == 1)
                        {
                            currentPublish.title = line;
                            currentPublish.title = line.Replace("اویرایش شده", "");
                        }

                        if (line.Contains("پایان‌نامه "))
                        {
                            currentPublish.AddField("TY", " THES"); //#OK
                            currentPublish.AddField("T1", currentPublish.title);
                            string[] tokens = line.Split('.');
                            string unversity = tokens[1].Replace("دولتی - وزارت علوم، تحقیقات، و فناوری", "");
                            currentPublish.AddField("CY", tokens[1].Replace("دولتی - وزارت علوم، تحقیقات، و فناوری", "")); //#OK Publication Place
                            currentPublish.AddField("Y1", tokens[2]); //#OK its for date of publish
                        }

                        if (line.Contains("موضوع"))
                        {
                            //string[] parts = line.Split(':');
                            //string[] parts3 = parts[1].Split('>');
                            //foreach (string part in parts3)
                            //{
                            //    string str = part + "﻿";
                            //    //MessageBox.Show(str);
                            //}
                        }
                        if (line.Contains("استاد راهنما"))
                        {
                            string[] parts = line.Split('|');
                            foreach (string part in parts)
                            {
                                string[] token = part.Split(':');
                                if (token[0].Contains("نویسنده") || token[0].Contains("پدیدآور") || token[0].Contains("دانشجو"))
                                {
                                    token[1] = token[1].Replace("دسترسی به فایل تمام‌متن پیشینه‌هایی (رکوردهایی) که نشانه «پی.دی.اف", "");
                                    token[1] = token[1].Replace("» Pdf_note ندارند، شدنی نیست.", "");
                                    token[1] = token[1].Replace(".", "");
                                    token[1] = token[1].Replace(" دریافت فایل", "");
                                    token[1] = token[1].Replace("Pdf", "");
                                    currentPublish.AddField("A1", token[1]);
                                }
                                if (token[0].Contains("راهنما")) currentPublish.AddField("A2", token[1]);
                                if (token[0].Contains("مشاور")) currentPublish.AddField("A3", token[1]);
                            }
                            line = reader.ReadLine();
                            if (line.Contains("دسترسی")) line = reader.ReadLine();
                            currentPublish.AddField("N2", line); //Titile
                        }
                        if (line.Contains("نمایه"))
                        {
                            line = reader.ReadLine();
                            string[] parts = line.Split('|');
                            foreach (string part in parts)
                            {
                                string[] tokens = part.Split(new[] { "Rss" }, StringSplitOptions.None);
                                currentPublish.AddField("KW", tokens[0]);
                            }
                        }
                        if (line.Contains("Comments") || line.Contains("Like") || line.Contains("Folder_heart") || line.Contains("Printer") || line.Contains("Email_attach"))
                        {
                        }

                        line = reader.ReadLine();
                    }
                    currentPublish.AddField("ER", "");
                    Publishes.Add(currentPublish);
                }
                reader.Close();
                s.Close();
                FileStream w = new FileStream(savePath, FileMode.Create);
                StreamWriter writer = new StreamWriter(w, Encoding.UTF8);
                foreach (Publish Item in Publishes)
                {
                    string str;
                    foreach (IranDoc2Ris.Publish.Field itemm in Item.Feilds)
                    {
                        str = itemm.tag.ToString() + "  - " + itemm.value.ToString();
                        writer.WriteLine(str);
                    }
                }
                writer.Close();
                w.Close();
                Console.WriteLine("TotlaExport:{0}", uintCont.ToString());
            } while (true);
        }
    }
    public class Publish
    {
        public struct Field
        {
            public string tag;
            public string value;
        }
        public string title;
        public ArrayList Feilds = new ArrayList();


        public Publish(){Feilds.Clear();}

        public void AddField(string taag, string vaalue)
        {
            Field newTag;
            newTag.tag = taag;
            newTag.value = vaalue;
            Feilds.Add(newTag);
        }
    }
}

 

    