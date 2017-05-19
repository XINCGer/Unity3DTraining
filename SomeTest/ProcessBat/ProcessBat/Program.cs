using System;
using System.Diagnostics;

namespace ProcessBat
{
    /// <summary>
    /// C#程序执行外部exe、bat文件
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            // 如果要运行时隐藏dos窗口，需使用下面的代码

            //proc.StartInfo.UseShellExecute = false;
            //proc.StartInfo.CreateNoWindow = true;

            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.FileName = @"./test.bat";

                //proc.StartInfo.Arguments = string.Format("10");
                //proc.StartInfo.CreateNoWindow = false;

                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }
        }
    }
}
