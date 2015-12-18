using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GothosDC;

namespace DataTools
{
    class Utils
    {
        public static string RootDirectory;

        public static string CacheDirectory;

        public static string DataDirectory;

        public static string OutputDirectory { get; private set; }

        public static string GetOutput(string name)
        {
            var result = Path.Combine(OutputDirectory, name);
            Directory.CreateDirectory(Path.GetDirectoryName(result));
            return result;
        }

        public static void Init()
        {
            RootDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
            CacheDirectory = Path.GetFullPath(RootDirectory + @"\..\..\..\cache");
            DataDirectory = Path.GetFullPath(RootDirectory + @"\..\..\..\data");

            if (!Directory.Exists(CacheDirectory))
                Directory.CreateDirectory(CacheDirectory);

            ConsoleManager.Show();
            var dcPath = GetDataCenterPath();
            Console.WriteLine("Loading datacenter...");
            var start = DateTime.UtcNow;
            DCTools.DCT.DataCenter = DataCenter.Load(dcPath);
            Console.WriteLine("DataCenter loaded in {0:f1}s", (DateTime.UtcNow - start).TotalSeconds);

            OutputDirectory = "data/";
            Directory.CreateDirectory(OutputDirectory);
        }

        public static string GetDataCenterPath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DataCenters|DataCenter*.unpacked;DataCenter*.bin|All files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                return openFileDialog.FileName;
            throw new Exception("Cancelled");
        }

        public static string LoadPage(string url)
        {
            string cacheFileName = CacheDirectory + "/" +
                                   url.Substring(url.IndexOf("://", StringComparison.Ordinal) + 3).Replace('/', '_') +
                                   ".txt";

            if (File.Exists(cacheFileName))
            {
                using (FileStream fs = File.OpenRead(cacheFileName))
                {
                    using (TextReader reader = new StreamReader(fs))
                    {
                        Console.WriteLine("Loaded cached {0}", url);
                        return reader.ReadToEnd();
                    }
                }
            }

            Console.WriteLine("Loading {0}", url);

            for (int i = 0; i < 2; i++)
            {
                try
                {
                    WebClient client = new WebClient
                                           {
                                               Encoding = Encoding.UTF8,
                                               CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable)
                                           };


                    client.Headers.Add("user-agent",
                                       "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.186 Safari/535.1");

                    string result;
                    using (Stream data = client.OpenRead(url))
                    {
                        if (data == null)
                            continue;

                        using (StreamReader reader = new StreamReader(data, client.Encoding))
                        {
                            result = reader.ReadToEnd();
                        }
                    }

                    Console.WriteLine("Loaded {0}", url);

                    using (FileStream fs = File.Create(cacheFileName))
                    {
                        using (TextWriter writer = new StreamWriter(fs))
                        {
                            writer.Write(result);
                        }
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Can't load {0}", url);
                    Thread.Sleep(1000);
                }
            }

            return "";
        }
    }
}