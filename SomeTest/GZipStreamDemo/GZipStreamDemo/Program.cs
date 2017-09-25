using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipStreamDemo
{
    /// <summary>
    /// 利用GZipStream来解压缩操作
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("文件路径：");
            string path = Console.ReadLine();
            Console.WriteLine("1:压缩；2：解压 输入操作：");
            string op = Console.ReadLine();

            FileInfo fileInfo = new FileInfo(path);
            if (op == "1")
            {
                CompressFile(fileInfo);
            }
            else
            {
                DecompressFile(fileInfo);
            }
        }

        /// <summary>
        /// 对单个文件进行压缩
        /// </summary>
        /// <param name="fileInfo"></param>
        public static void CompressFile(FileInfo fileInfo)
        {
            using (FileStream inStream = fileInfo.OpenRead())
            {
                if ((File.GetAttributes(fileInfo.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden &
                    fileInfo.Extension != ".gz")
                {
                    using (FileStream outStream = File.Create(fileInfo.FullName + ".gz"))
                    {
                        using (GZipStream compress = new GZipStream(outStream, CompressionMode.Compress))
                        {
                            inStream.CopyTo(compress);
                            Console.WriteLine("Compress {0} from {1} to {2} bytes.", fileInfo.FullName, fileInfo.Length, outStream.Length);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 对单个文件进行解压缩
        /// </summary>
        /// <param name="fileInfo"></param>
        public static void DecompressFile(FileInfo fileInfo)
        {
            using (FileStream inStream = fileInfo.OpenRead())
            {
                string fileName = fileInfo.FullName;
                string originName = fileName.Substring(0, fileName.Length - fileInfo.Extension.Length);

                using (FileStream ouStream = File.Create(originName))
                {
                    using (GZipStream decompress = new GZipStream(inStream, CompressionMode.Decompress))
                    {
                        decompress.CopyTo(ouStream);
                        Console.WriteLine("Decompress: {0}", fileInfo.FullName);
                    }
                }
            }
        }
    }
}
