using System;
using System.Collections.Generic;
using System.IO;

namespace Mutagen.alch_registry_builder
{
    public class FileInterface
    {
        public FileInterface(string filepath, bool printToConsole = false)
        {
            if (File.Exists(filepath))
                File.Delete(filepath);
            FileHandle = new StreamWriter(filepath, true);
            PrintToConsole = printToConsole;
        }

        public StreamWriter FileHandle;
        public bool PrintToConsole;

        public void WriteLine(string data)
        {
            FileHandle.WriteLine(data);
            if (PrintToConsole)
                Console.WriteLine(data);
        }
        // Write a list of strings to file, each element is one line.
        public void Write(List<string> data)
        {
            foreach (var line in data)
                WriteLine(line);
        }

        public void Append(List<string> data)
        {
            foreach (var line in data)
                WriteLine(line);
        }

        public void Close()
        {
            FileHandle.Close();
        }
    }
}