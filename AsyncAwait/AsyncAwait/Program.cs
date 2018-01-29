using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwait
{
    class Program
    {
        static void Main(string[] args)
        {
            GetWebPage();
            Console.WriteLine("Program Complete!");
            Console.Read();
        }

        static void GetWebPage()
        {
            // Call without waiting
            var webPageContents = GetWebPageAsync("http://msn.com");
            Console.WriteLine("After calling Async method #1");

            // Call with waiting
            var webPageContents2 = GetWebPageAsync("http://google.com").ConfigureAwait(false);
            Console.WriteLine("After calling Async method #2");
            webPageContents2.GetAwaiter().GetResult();
            Console.WriteLine("After waiting for async method #2 to finish");
        }

        static async Task<string> GetWebPageAsync(string urlString)
        {
            var client = new HttpClient();

            //Await for the HttpResponseMessage in this thread, but control is immediately returned to the calling function
            Console.WriteLine("Requesting results results for: " + urlString);
            HttpResponseMessage response = await client.GetAsync(urlString);
            Console.WriteLine("Finished awaiting client for: " + urlString);

            // Await on the string content in this thread, but control is immediately returned to the calling function
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Finished awaiting content for: " + urlString);

            Console.WriteLine("Finished getting results for: " + urlString);
            return content;
        }

    }
}
