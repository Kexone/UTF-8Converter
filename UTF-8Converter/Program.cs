using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace UTF_8Converter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PrintMessage(ReadFromFile(DirSearch(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))));
            Console.ReadKey();
        }

        private static List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir, "*.srt"))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }
            return files;
        }

        private static List<String> ReadFromFile(List<String> subs)
        {
            foreach (var subFile in subs)
            {
                StringBuilder builder = new StringBuilder();
                using (StreamReader reader = new StreamReader(subFile, Encoding.UTF8, true))
                {
                    builder.Append(reader.ReadToEnd());
                    reader.Close();
                }
                File.Delete(subFile);
                using (StreamWriter file = new StreamWriter(subFile))
               {
                    file.WriteLine((builder.ToString()));
                    builder.Clear();
                    file.Close();
               }
            }
            return subs;
        }

        private static void PrintMessage(List<String> subs)
        {
            foreach (var subFile in subs)
            {
                using (StreamReader reader = new StreamReader(subFile))
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("Converted sucessfully");
                    Console.ResetColor();
                    Console.WriteLine(" \n{0}, coding: {1} \n", subFile, reader.CurrentEncoding.BodyName);
                }
            }
            Console.WriteLine("Completed");
        }
    }
}
