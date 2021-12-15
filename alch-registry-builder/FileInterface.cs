using System;
using System.Collections.Generic;
using System.IO;

namespace Mutagen.alch_registry_builder
{
    public class FileInterface
    {
        public FileInterface(string filepath)
        {
            File = new StreamWriter(filepath);
        }

        public StreamWriter File;

        public void WriteLine(string data)
        {
            Console.WriteLine();
            File.WriteLineAsync(data);
        }
        // Write a list of strings to file, each element is one line.
        public void Write(ref List<string> data)
        {
            foreach (var line in data)
            {
                File.WriteLine(line);
                Console.WriteLine(line);
            }
        }
    }
}