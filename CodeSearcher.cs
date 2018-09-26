using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;



namespace Pat
{
    public class CodeSearcher
    {
        public string directory;
        public List<string> filenames;

        /// <summary>
        /// search all files in user provided path for user value matches found in data set
        /// </summary>
        public void searchCode ()
        {
            DirectoryInfo dirinfo = new DirectoryInfo(directory);
            FileInfo[] files = dirinfo.GetFiles("*", SearchOption.AllDirectories);
            filenames = new List<string>();

            foreach (FileInfo i in files)
            {         
                try
                {
                    string[] lines = File.ReadAllLines(i.FullName.ToString());
                    int lineNumber = 1;
                    foreach (string line in lines)
                    {
                        if (line.Contains("MEPE"))
                            filenames.Add(i.FullName.ToString() + i + "Line: " + lineNumber + " " + line.ToString() + Environment.NewLine);
                        lineNumber++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }                
            }           
         }
    }
}
