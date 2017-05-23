using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace UTF_8Converter
{
    internal class Program
    {
        private static int counter;
        private static int bad;
        private static String type = "srt";
        private static Stopwatch stopwatch = new Stopwatch();
        static void Main(string[] args)
        {
            Console.Write("Choose type subtitles: \n 1. .srt  2. .sub \nDefault is .srt\nYour choose:");
            try
            {
                switch (int.Parse(Console.ReadLine()))
                {
                    case 1:
                        type = "srt";
                        break;
                    case 2:
                        type = "sub";
                        break;
                    default:
                        Console.WriteLine("Bad type.\n");
                        break;
                }
            }
            catch
            {
                Console.WriteLine("Type .srt selected.\n");
            }
            Console.WriteLine("\nChoose directory (for actual type 'd') :");
            var path = Console.ReadLine();
            if (path == "d")
            {
               path = Assembly.GetEntryAssembly().Location;
            }
            Stopwatch.StartNew();  
            PrintMessage(ReadFromFile(DirSearch(Path.GetDirectoryName(path))));
            Console.ReadKey();
        }

        private static List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir, "*."+type))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return files;
        }

        private static List<String> ReadFromFile(List<String> subs)
        {
            var files = 0;
            stopwatch.Start();
            foreach (var subFile in subs)
            {
                Console.Write(" \n---Converted {0} of {1} files---", files++, subs.Count);
                using (StreamReader sr = new StreamReader(subFile, GetEncoding(subFile), false))
                {
                    using (StreamWriter sw = new StreamWriter(subFile + "tmp", false, Encoding.UTF8))
                    {

                        int charsRead;
                        char[] buffer = new char[128*1024];
                        while ((charsRead = sr.ReadBlock(buffer, 0, buffer.Length)) > 0)
                        {
                            sw.Write(buffer, 0, charsRead);
                        }
                    }
                }
                try
                {
                    File.Delete(subFile);
                    File.Move(subFile + "tmp", subFile);
                    counter++;    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    bad++;
                }
                finally
                {
                    File.Delete(subFile + "tmp");
                }
                   Console.Clear();
            }
            stopwatch.Stop();
            return subs;
        }

        private static void PrintMessage(List<String> subs)
        {
            if (bad == 0)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("\nConverted sucessfully");
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nCompleted with errors!");
            }
            Console.ResetColor();
            Console.WriteLine("{0} files in {1}s {2}ms, errors: {3} ", counter, stopwatch.Elapsed.Seconds, stopwatch.ElapsedMilliseconds,bad);
        }

        private static Encoding GetEncoding(string fileName)
        {
            Encoding encoding;
            using (StreamReader reader = new StreamReader(fileName, Encoding.Default, true))
            {
                reader.Peek();
                encoding = reader.CurrentEncoding;
            }
            return encoding;
            
        }
    }
}
