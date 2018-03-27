using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xls2Lua
{
    class Program
    {
        private static string inDir;
        private static string outDir;
        private static readonly string configPath = "./config.ini";

        static void Main(string[] args)
        {
            ReadConfig();
            FileExporter.ExportAllLuaFile(inDir, outDir);
        }

        private static void ReadConfig()
        {
            StreamReader reader = new StreamReader(configPath, Encoding.UTF8);
            inDir = reader.ReadLine().Split(',')[1];
            inDir = Path.GetFullPath(inDir);
            outDir = reader.ReadLine().Split(',')[1];
            outDir = Path.GetFullPath(outDir);
            reader.Close();
        }
    }
}
