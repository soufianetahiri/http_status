using http_status_code;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Http_Status_Code
{
    class Program
    {
        const int MF_BYCOMMAND = 0x00000000;
        const int SC_MINIMIZE = 0xF020;
        const int SC_MAXIMIZE = 0xF030;
        const int SC_SIZE = 0xF000;
        private const string V = "  ___ ___________________________________    ________________________________________ ___  _________";
        private const string V1 = " /   |   \\__    ___/\\__    ___/\\______   \\  /   _____/\\__    ___/  _  \\__    ___/    |   \\/   _____/";
        private const string V2 = "/    ~    \\|    |     |    |    |     ___/  \\_____  \\   |    | /  /_\\  \\|    |  |    |   /\\_____  \\ ";
        private const string V3 = "\\    Y    /|    |     |    |    |    |      /        \\  |    |/    |    \\    |  |    |  / /        \\";
        private const string V4 = " \\___|_  / |____|     |____|    |____|     /_______  /  |____|\\____|__  /____|  |______/ /_______  /";
        private const string V5 = "       \\/                                          \\/                 \\/                         \\/ ";
        static bool isScanning = true;
        static string userInput;
        private static readonly string noChecks = "--ignore-checks";
        static Uri uriResult;
        private static bool ignoreChecks = false;
        static string getting = "¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤_GET_ing STARTS_¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤";
        static string gettinge = "¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤_GET_ing ENDS_¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤¤";
        private static readonly string[] owaspHeaders = { "HTTP Strict Transport Security", "Public Key Pinning Extension for HTTP", "X-Frame-Options", "X-XSS-Protection", "X-Content-Type-Options", "Content-Security-Policy", "X-Permitted-Cross-Domain-Policies", "Referrer-Policy", "Expect-CT", "Feature-Policy" };
        public static string Usage { get; } = "Usage : http_status URL | http_status file.txt (--ignore-checks ...to ignore checks :)) \n\r if running just type in the url or the path to the file (with an url by line) to check. \n\r HTTP Header seucrity is checked when single URL scan";

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        static void Main(string[] args)
        {

            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetWindowSize(100, 50);
            Output.WriteLine(V);
            Output.WriteLine(V1);
            Output.WriteLine(V2);
            Output.WriteLine(V3);
            Output.WriteLine(V4);
            Output.WriteLine(V5);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            string auth = "v0.1 by Soufiane Tahiri";
            Output.WriteLine(string.Format("{0," + ((Console.WindowWidth / 2) + (auth.Length / 2)) + "}", auth));

            Output.WriteLine("\n\r");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            while (isScanning)
            {
                CheckArgs(args, userInput);
                userInput = Console.ReadLine();
            }

        }

        private static void CheckArgs(string[] args, string userin = "")
        {
            try
            {
                if (args == null || args.Length != 1 && string.IsNullOrEmpty(userInput))
                {
                    WriteUsage();

                }
                if (args.Length > 0 || !string.IsNullOrEmpty(userin))
                {
                    try
                    {
                        if (userInput.Split(' ')[1].ToString() == noChecks)
                        {
                            ignoreChecks = true;
                            userInput = userInput.Split(' ')[0];
                        }
                    }
                    catch (Exception)
                    {
                    }
                    if (userInput == "exit")
                    {
                        isScanning = false;
                    }
                    else
                    if (userInput.Contains(".txt") && !File.Exists(userInput))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Output.WriteLine(string.Format("{0} does not exists.", userInput));

                    }
                    else if (File.Exists(userInput))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Output.WriteLine(string.Format("{0," + ((Console.WindowWidth / 2) + (getting.Length / 2)) + "}", getting));
                        Output.WriteLine("\n\r");
                        StreamReader file = new StreamReader(userInput);
                        string line;
                        while ((line = file.ReadLine()) != null)
                        {
                            if (ValidHttpURL(line))
                            {
                                Call(uriResult.ToString());
                                System.Threading.Thread.Sleep(50);
                            }
                        }
                        System.Threading.Thread.Sleep(50);
                        Output.WriteLine("\n\r");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Output.WriteLine(string.Format("{0," + ((Console.WindowWidth / 2) + (gettinge.Length / 2)) + "}", gettinge));
                        Console.ForegroundColor = ConsoleColor.Green;
                        string LogDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                        Output.WriteLine(string.Format("Output saved to {0}", LogDirPath));
                        isScanning = false;
                    }
                    else
                    if (ValidHttpURL(userInput))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("\n\r");
                        Console.WriteLine("\n\r");
                        Output.WriteLine(string.Format("{0," + ((Console.WindowWidth / 2) + (getting.Length / 2)) + "}", getting));
                        Output.WriteLine("\n\r");
                        Call(uriResult.ToString());
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Output.WriteLine(string.Format("{0," + ((Console.WindowWidth / 2) + (gettinge.Length / 2)) + "}", gettinge));
                        Console.ForegroundColor = ConsoleColor.Green;
                        string LogDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                        Console.WriteLine(string.Format("Output saved to {0}", LogDirPath));
                        isScanning = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        private static void Call(string url)
        {
            AsyncHelper.RunSync(() => CallTheHostAsync(url));
            TryHttpAndHttps(url);
        }
        public static bool ValidHttpURL(string s)
        {

            if (!Regex.IsMatch(s, @"^https?:\/\/", RegexOptions.IgnoreCase))
            {
                s = "http://" + s;
            }

            if (Uri.TryCreate(s, UriKind.Absolute, out uriResult))
            {
                return (uriResult.Scheme == Uri.UriSchemeHttp ||
                        uriResult.Scheme == Uri.UriSchemeHttps);
            }

            return false;
        }
        private static void WriteUsage()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(Usage);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

        }
        private static async Task CallTheHostAsync(string uri)
        {
            try
            {
                HttpClient client = new HttpClient();
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                client.Timeout = TimeSpan.FromMilliseconds(3000);
                HttpResponseMessage checkingResponse = await client.GetAsync(uri);
                if (checkingResponse.IsSuccessStatusCode)
                {
                    if (!ignoreChecks)
                    {
                        Task t = Task.Factory.StartNew(() => HttpHeadersHelper.CheckOwaspRecHeader(checkingResponse));
                        t.Wait();
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }
                Output.WriteLine(string.Format("[{0}] {1} {2} - {3}", DateTime.Now, checkingResponse.ReasonPhrase, (int)checkingResponse.StatusCode, uri));
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Output.WriteLine(string.Format("[{0}] {1} - {2}", DateTime.Now, "TimeOut or SSLError", uri));

            }

        }

        private static void TryHttpAndHttps(string url)
        {
            if (url.ToLower().StartsWith("https://"))
            {
                url = url.Replace("https://", "http://");
            }
            else if (url.ToLower().StartsWith("http://"))
            {
                url = url.Replace("http://", "https://");
            }

            AsyncHelper.RunSync(() => CallTheHostAsync(url));
        }


    }
}
