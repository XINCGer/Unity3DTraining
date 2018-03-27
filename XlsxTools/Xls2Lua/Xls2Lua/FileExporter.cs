using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpJExcel.Jxl;

namespace Xls2Lua
{
    /// <summary>
    /// 负责最终文件的输出保存等操作类
    /// </summary>
    public class FileExporter
    {

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

        /// <summary>
        /// 导出所有的Excel配置到对应的lua文件中
        /// </summary>
        /// <param name="inDir"></param>
        /// <param name="outDir"></param>
        public static void ExportAllLuaFile(string inDir, string outDir)
        {
            ClearDirectory(outDir);
            List<string> allXlsList = Directory.GetFiles(inDir, "*.xls", SearchOption.AllDirectories).ToList();
            Console.WriteLine("开始转表...");
            foreach (var curXlsName in allXlsList)
            {
                ExportSingleLuaFile(curXlsName, outDir);
            }
            Console.WriteLine("按任意键继续...");
            Console.ReadKey();
        }

        public static void ExportSingleLuaFile(string xlsName, string outDir)
        {
            if (".xls" != Path.GetExtension(xlsName).ToLower())
            {
                return;
            }

            Console.WriteLine(Path.GetFileName(xlsName));

            //打开文件流
            FileStream fs = null;
            try
            {
                fs = File.Open(xlsName, FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            if (null == fs) return;
            //读取xls文件
            Workbook book = Workbook.getWorkbook(fs);
            fs.Close();
            //循环处理sheet
            foreach (var sheet in book.getSheets())
            {
                string sheetName = XlsTransfer.GetSheetName(sheet);
                if (string.IsNullOrEmpty(sheetName)) continue;
                sheetName = sheetName.Substring(1, sheetName.Length - 1);
                Console.WriteLine("Sheet:" + sheetName);
                string outPath = Path.Combine(outDir, sheetName + ".lua");
                string content = XlsTransfer.GenLuaFile(sheet);
                if (!string.IsNullOrEmpty(content))
                {
                    File.WriteAllText(outPath, content);
                }
            }
        }
    }
}
