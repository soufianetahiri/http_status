using http_status_code;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Http_Status_Code
{
    class Program
    {
        const int MF_BYCOMMAND = 0x00000000;
        const int SC_MINIMIZE = 0xF020;
        const int SC_MAXIMIZE = 0xF030;
        const int SC_SIZE = 0xF000;
        static bool isScanning = true;
        static string userInput;
        static Uri uriResult;
        static HttpClient client = new HttpClient();
        static string getting = "=========GETting Started========";

        public static string Usage { get; } = "Usage : http_status URL | http_status file.txt \n\r if running just type in the url or the path to the file (with an url by line) to check";

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
            string sb = "__________________________________________________________________________________________________" +
                "  ___ ___________________________________    ________________________________________ ___  _________" +
                " /   |   \\__    ___/\\__    ___/\\______   \\  /   _____/\\__    ___/  _  \\__    ___/    |   \\/   _____/" +
                "/    ~    \\|    |     |    |    |     ___/  \\_____  \\   |    | /  /_\\  \\|    |  |    |   /\\_____  \\ " +
                "\\    Y    /|    |     |    |    |    |      /        \\  |    |/    |    \\    |  |    |  / /        \\" +
                " \\___|_  / |____|     |____|    |____|     /_______  /  |____|\\____|__  /____|  |______/ /_______  /" +
                "       \\/                                          \\/                 \\/                         \\/ ";
            Output.Write(sb);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            string auth = "v0.1 by Soufiane Tahiri";
            Console.WriteLine(string.Format("{0," + ((Console.WindowWidth / 2) + (auth.Length / 2)) + "}", auth));
            Console.WriteLine("\n\r");
            Console.WriteLine("\n\r");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            while (isScanning)
            {
                CheckArgsAsync(args, userInput).Wait();
                userInput = Console.ReadLine();
            }
        }

        private static async System.Threading.Tasks.Task<bool> CheckArgsAsync(string[] args, string userin = "")
        {
            try
            {
                if (args == null || args.Length != 1 && string.IsNullOrEmpty(userInput))
                {
                    WriteUsage();
                    return false;
                }
                if (args.Length > 0 || !string.IsNullOrEmpty(userin))
                {

                    if (userInput == "exit")
                    {
                        isScanning = false;
                    }
                    if (userInput.Contains(".txt") && !File.Exists(userInput))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Output.WriteLine(string.Format("{0} does not exists.", userInput));
                        return false;
                    }
                    else if (File.Exists(userInput))
                    {
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
                        Console.WriteLine("\n\r");
                        string LogDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                        Console.WriteLine(string.Format("Output saved to {0}", LogDirPath));
                    }
                    if (ValidHttpURL(userInput))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("\n\r");
                        Console.WriteLine("\n\r");
                        Output.WriteLine(string.Format("{0," + ((Console.WindowWidth / 2) + (getting.Length / 2)) + "}", getting));
                        Output.WriteLine("\n\r");
                        Call(uriResult.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
        private static void Call(string url)
        {
            CallTheHostAsync(url);
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
        private static async System.Threading.Tasks.Task CallTheHostAsync(string uri)
        {
        
            HttpResponseMessage checkingResponse = await client.GetAsync(uri).ConfigureAwait(true ); ;
            if (checkingResponse.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {

                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            Output.WriteLine(string.Format("[{0}] {1} {2} - {3}", DateTime.Now, checkingResponse.ReasonPhrase, (int)checkingResponse.StatusCode, uri));
            System.Threading.Thread.Sleep(50);
        }
        private static void TryHttpAndHttps(string url)
        {
            url = url.Replace("http://", "https://");
            CallTheHostAsync(url);
        }
    }
}
