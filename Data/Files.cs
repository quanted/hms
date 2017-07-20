using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Files
    {
        /// <summary>
        /// Reads the contents of a space delimited file and populates the specified Application variable.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public static Dictionary<string, string> FileToDictionary(string fileName)
        {
            Dictionary<string, string> fileValues = new Dictionary<string, string>();
            foreach (string line in File.ReadLines(fileName))
            {
                string[] lineValues = line.Split(' ');
                if (!fileValues.ContainsKey(lineValues[0]) && lineValues.Length > 1)
                {
                    fileValues.Add(lineValues[0], lineValues[1]);
                }
            }
            return fileValues;
        }

    }
}
