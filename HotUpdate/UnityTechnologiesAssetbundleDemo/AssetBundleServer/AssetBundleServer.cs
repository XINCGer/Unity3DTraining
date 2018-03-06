using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace AssetBundleServer
{
    class MainClass
    {
        public static void WatchDog(object processID)
        {
            Console.WriteLine("Watching parent processID: {0}!", processID);
            Process masterProcess = Process.GetProcessById((int)processID);
            while (masterProcess == null || !masterProcess.HasExited)
            {
                Thread.Sleep(1000);
            }

            Console.WriteLine("Exiting because parent process has exited!");
            Environment.Exit(0);
        }

        public static void Main(string[] args)
        {
            int parentProcessID;
            string basePath = "";

            // For testing purposes...
            if (args.Length == 0)
            {
                Console.WriteLine("No commandline arguments, harcoded debug mode...");
                parentProcessID = 0;
            }
            else
            {
                basePath = args[0];
            }

            if (args.Length >= 2)
            {
                parentProcessID = int.Parse(args[1]);
            }
            else
            {
                parentProcessID = 0;
            }
            // Automatically quit bundle server when Unity exits
            if (parentProcessID != 0)
            {
                Thread thread = new Thread(WatchDog);
                thread.Start(parentProcessID);
            }

            bool detailedLogging = false;
            int port = 7888;

            Console.WriteLine("Starting up asset bundle server.", port);
            Console.WriteLine("Port: {0}", port);
            Console.WriteLine("Directory: {0}", basePath);

            HttpListener listener = new HttpListener();

            /*
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName ());
            foreach (IPAddress ip in host.AddressList)
            {
                //if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    Console.WriteLine(ip.AddressFamily.ToString() + " - " + ip.ToString());
                }
            }
            */


            listener.Prefixes.Add(string.Format("http://*:{0}/", port));
            listener.Start();

            while (true)
            {
                Console.WriteLine("Waiting for request...");

                HttpListenerContext context = listener.GetContext();

                WriteFile(context, basePath, detailedLogging);
            }
        }

        static void WriteFile(HttpListenerContext ctx, string basePath, bool detailedLogging)
        {
            HttpListenerRequest request = ctx.Request;
            string rawUrl = request.RawUrl;
            string path = basePath + rawUrl;

            if (detailedLogging)
                Console.WriteLine("Requesting file: '{0}'. Relative url: {1} Full url: '{2} AssetBundleDirectory: '{3}''", path, request.RawUrl, request.Url, basePath);
            else
                Console.Write("Requesting file: '{0}' ... ", request.RawUrl);

            var response = ctx.Response;
            try
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    string filename = Path.GetFileName(path);
                    //response is HttpListenerContext.Response...
                    response.ContentLength64 = fs.Length;
                    response.SendChunked = false;
                    response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                    response.AddHeader("Content-disposition", "attachment; filename=" + filename);

                    byte[] buffer = new byte[64 * 1024];
                    int read;
                    using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
                    {
                        while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, read);
                            bw.Flush(); //seems to have no effect
                        }

                        bw.Close();
                    }

                    Console.WriteLine("completed.");
                    //				response.StatusCode = (int)HttpStatusCode.OK;
                    //				response.StatusDescription = "OK";
                    response.OutputStream.Close();
                    response.Close();
                }
            }
            catch (System.Exception exc)
            {
                Console.WriteLine(" failed.");
                Console.WriteLine("Requested file failed: '{0}'. Relative url: {1} Full url: '{2} AssetBundleDirectory: '{3}''", path, request.RawUrl, request.Url, basePath);
                Console.WriteLine("Exception {0}: {1}'", exc.GetType(), exc.Message);
                response.Abort();
            }
        }
    }
}
