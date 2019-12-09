using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace http_status_code
{
    public class HttpHeadersHelper
    {
        public static void CheckOwaspRecHeader(HttpResponseMessage httpResponseMessage)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Output.WriteLine(string.Format("[{0}] {1} ", DateTime.Now, string.Format("Checking  OWASP recommended headers [STARTS {0}]",httpResponseMessage.RequestMessage.RequestUri.ToString())));
            HttpHeaders headers = httpResponseMessage.Headers;

            //check x-xss-protection:
            if (headers.TryGetValues("x-xss-protection", out IEnumerable<string> xss))
            {
                if (xss.First().Trim().StartsWith("1;mode=block"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ + ] (X-XSS-Protection) Cross-Site Scripting Protection is enforced.", xss.First()));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ - ] Server does not enforce Cross-Site Scripting Protection.\nThe X-XSS-Protection Header setting is either inadequate or missing.\nClient may be vulnerable to Cross-Site Scripting Attacks. ", string.Join(" ", xss)));
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output.WriteLine(string.Format("{0} [HEADER {1}] ", "[ - ] Server does not enforce Cross-Site Scripting Protection.\nThe X-XSS-Protection Header setting is either inadequate or missing.\nClient may be vulnerable to Cross-Site Scripting Attacks.", getHeaderValue(xss)));
            }
            // check x-frame-options:
            if (headers.TryGetValues("x-frame-options", out IEnumerable<string> xframe))
            {
                if (xframe.First().Trim().Contains("deny") || xframe.First().Trim().Contains("sameorigin"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ + ] (X-Frame-Options) Cross-Frame Scripting Protection is enforced.", string.Join(" ", xframe)));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ - ] Server does not enforce Cross-Frame Scripting Protection.\nThe X-Frame-Options Header setting is either inadequate or missing.\nClient may be vulnerable to Click-Jacking Attacks. ", string.Join(" ", xframe)));
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output.WriteLine(string.Format("{0} [HEADER {1}] ", "[ - ] Server does not enforce Cross-Frame Scripting Protection.\nThe X-Frame-Options Header setting is either inadequate or missing.\nClient may be vulnerable to Click-Jacking Attacks. ", getHeaderValue(xframe)));
            }
            //check x-content-type-options:
            if (headers.TryGetValues("x-content-type-options", out IEnumerable<string> xcontent))
            {
                if (xcontent.First() == "nosniff")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ + ] (X-Content-Type-Options) MIME-Sniffing Protection is enforced.", string.Join(" ", xcontent)));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ - ] Server does not enforce MIME-Sniffing Protection.\nThe X-Content-Type-Options Header setting is either inadequate or missing.\nClient may be vulnerable to MIME-Sniffing Attacks.", string.Join(" ", xcontent)));
                }

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output.WriteLine(string.Format("{0} [HEADER {1}] ", "[ - ] Server does not enforce MIME-Sniffing Protection.\nThe X-Content-Type-Options Header setting is either inadequate or missing.\nClient may be vulnerable to MIME-Sniffing Attacks.", getHeaderValue(xcontent)));
            }
            //check strict-transport-security:
            if (headers.TryGetValues("strict-transport-security", out IEnumerable<string> sts))
            {
                if (sts != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ + ] (Strict-Transport-Security) HTTP over TLS/SSL is enforced.", getHeaderValue( sts)));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ - ] Server does not enforce HTTP over TLS/SSL Connections.\nThe Strict-Transport-Security Header setting is either inadequate or missing.\nClient may be vulnerable to Session Information Leakage. ", getHeaderValue( sts)));
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output.WriteLine(string.Format("{0} [HEADER {1}] ", "[ - ] Server does not enforce HTTP over TLS/SSL Connections.\nThe Strict-Transport-Security Header setting is either inadequate or missing.\nClient may be vulnerable to Session Information Leakage. ", getHeaderValue(sts)));
            }
            //check content-security-policy:
            if (headers.TryGetValues("content-security-policy", out IEnumerable<string> csp))
            {
                if (csp != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ + ] (Content-Security-Policy) Content Security Policy is enforced.", getHeaderValue( csp)));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ - ] Server does not enforce a Content Security Policy.\nThe Content-Security-Policy Header setting is either inadequate or missing.\nClient may be vulnerable to Cross-Site Scripting and Injection Attacks.", getHeaderValue(csp)));
                }

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output.WriteLine(string.Format("{0} [HEADER {1}] ", "[ - ] Server does not enforce a Content Security Policy.\nThe Content-Security-Policy Header setting is either inadequate or missing.\nClient may be vulnerable to Cross-Site Scripting and Injection Attacks.", getHeaderValue(csp)));
            }
            //check x-content-security-policy:
            if (headers.TryGetValues("content-security-policy", out IEnumerable<string> xcsp))
            {
                if (xcsp != null)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Output.WriteLine("Deprecated :");
                    if (!headers.TryGetValues("content-security-policy", out IEnumerable<string> cspp))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Output.WriteLine("[ ~ ] (X-Content-Security-Policy) Content Security Policy is enforced. (SWITCH TO STANDARD HTTP HEADER: \'Content-Security-Policy\')\n\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Output.WriteLine("[ ~ ] (X-Content-Security-Policy) Content Security Policy is enforced. (DROP DEPRECATED HEADER: \'X-Content-Security-Policy\')\n\n");
                    }
                }
            }
            //check x-webkit-csp:
            if (headers.TryGetValues("content-security-policy", out IEnumerable<string> xwc))
            {
                if (xwc != null)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Output.WriteLine("Deprecated");
                    if (!headers.TryGetValues("content-security-policy", out IEnumerable<string> csspp))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Output.WriteLine("[ ~ ] (X-Webkit-CSP) Content Security Policy is enforced. (SWITCH TO STANDARD HTTP HEADER: \'Content-Security-Policy\')\n\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Output.WriteLine("[ ~ ] (X-Webkit-CSP) Content Security Policy is enforced. (DROP DEPRECATED HEADER: \'X-Webkit-CSP\')\n\n");
                    }
                }
            }
            //check access-control-allow-origin:
            if (headers.TryGetValues("access-control-allow-origin", out IEnumerable<string> acao))
            {
                if (acao != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ + ] (Access-Control-Allow-Origin) Access Control Policies are enforced. ", string.Join(" ", acao)));
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output.WriteLine(string.Format("{0} [HEADER {1}] ", "[ - ] Server does not enforce an Access Control Policy.\nThe Access-Control-Allow-Origin Header setting is either inadequate or missing.\nClient may be vulnerable to Cross-Domain Scripting Attacks.", getHeaderValue(acao)));
            }
            //check x-download-options:
            if (headers.TryGetValues("x-download-options", out IEnumerable<string> xdo))
            {
                if (xdo != null && xdo.First().Trim() == "noopen")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ + ] (X-Download-Options) File Download and Open Restriction Policies are enforced. ", getHeaderValue(xdo)));
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output.WriteLine(string.Format("{0} [HEADER {1}] ", "[ - ] Server does not enforce a File Download and Open Policy.\nThe X-Download-Options Header setting is either inadequate or missing.\nClient may be vulnerable to Browser File Execution Attacks.", getHeaderValue(xdo)));
            }
            //check cache-control:
            if (headers.TryGetValues("cache-control", out IEnumerable<string> cc))
            {
                if (cc != null && cc.First().Trim().StartsWith("noopen") || cc.First().Trim().StartsWith("no-cache"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ + ] (Cache-control) Private Caching or No-Cache is enforced.", string.Join(" ", cc)));
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output.WriteLine(string.Format("{0} [HEADER {1}] ", "[ - ] Server does not enforce a Content Caching Policy.\nThe Cache-control Header setting is either inadequate or missing.\nClient may be vulnerable to Content Caching Attacks. ", getHeaderValue(cc)));
            }
            //check x-permitted-cross-domain-policies:
            if (headers.TryGetValues("'X-Permitted-Cross-Domain-Policies", out IEnumerable<string> xpcdp))
            {
                if (xpcdp != null && xpcdp.First().Trim().StartsWith("master-only") || xpcdp.First().Trim().StartsWith("none"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Output.WriteLine(string.Format("{0} [VALUES {1}] ", "[ + ] (X-Permitted-Cross-Domain-Policies) X-Permitted-Cross-Domain-Policies are enforced. ", getHeaderValue(xpcdp)));
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Output.WriteLine(string.Format("{0} [HEADER {1}] ", "[ - ] Server does not enforce a X-Permitted-Cross-Domain-Policies.\nThe Cross-Domain Meta Policy Header setting is either inadequate or missing.\nClient may be vulnerable to Cross-Protocol-Scripting Attacks. ", getHeaderValue(xpcdp)));
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Output.WriteLine(string.Format("[{0}] {1} ", DateTime.Now, "Checking  OWASP recommended headers [ENDS]"));
        }


        private static string getHeaderValue(IEnumerable<string> header)
        {
            if (header != null && header.Count() > 0)
            {
                return string.Join(" ", header);
            }
            return "MISSING";
        }
    }
}
