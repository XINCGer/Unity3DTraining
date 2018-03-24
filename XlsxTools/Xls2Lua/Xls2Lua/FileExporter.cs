using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xls2Lua
{
    /// <summary>
    /// 负责最终文件的输出保存等操作类
    /// </summary>
    public class FileExporter
    {
        private string intDir = "";
        private string outDir = "";

        /// <summary>
        /// 清空某个DIR下的内容
        /// </summary>
        /// <param name="dir"></param>
        public static void ClearDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }
            Console.WriteLine("清空目录：" + dir);
            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();

            foreach (var info in fileSystemInfos)
            {
                if (info is DirectoryInfo)
                {
                    DirectoryInfo subDir = new DirectoryInfo(info.FullName);
                    try
                    {
                        subDir.Delete(true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("警告：目录删除失败 " + e.Message);
                    }
                }
                else
                {
                    try
                    {
                        File.Delete(info.FullName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("警告：文件删除失败 " + e.Message);
                    }
                }
            }
        }
    }
}
