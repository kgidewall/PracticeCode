using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// https://stackoverflow.com/questions/16195994/can-you-call-wait-on-a-task-multiple-times
// https://stackoverflow.com/questions/6123406/waitall-vs-whenall

namespace AsyncAwait
{
    class Program
    {
        static public Dictionary<String, Task> myTask = new Dictionary<String, Task>();

        static void Main(string[] args)
        {
            int waitTimeMs = 5000;
            Console.WriteLine("Calling MyThreadCallAsync for thread [{0}]", 0);
            MyThreadCallAsync(waitTimeMs, 0);
            Thread.Sleep(1000);

            Console.WriteLine("Calling MyThreadCallAsync for thread [{0}]", 1);
            MyThreadCallAsync(waitTimeMs, 1);
            Thread.Sleep(1000);

            Console.WriteLine("Calling MyThreadCallAsync for thread [{0}]", 2);
            MyThreadCallAsync(waitTimeMs, 2);
            Thread.Sleep(1000);

            Console.WriteLine("Calling MyThreadCallAsync for thread [{0}]", 3);
            MyThreadCallAsync(waitTimeMs, 3).Wait();

            Console.WriteLine("Finished all threads");

            //GetWebPage();
            //Console.WriteLine("Program Complete!");

            Console.Write("Press any key to exit");
            Console.Read();
        }

        static async Task<int> MyThreadCallAsync(int waitMs, int threadNum)
        {
            if (!myTask.ContainsKey("sku1"))
            {
                Console.WriteLine("Kicking off async task for thread [{0}] at time [{1}]", threadNum, DateTime.Now);
                myTask["sku1"] = Task.Run<int>(() =>
                {
                    Console.WriteLine("Starting wait of {0} ms", waitMs);
                    Thread.Sleep(waitMs);
                    Console.WriteLine("Completed wait of {0} ms", waitMs);
                    return 1;
                });
                Console.WriteLine("Completed kicking off asynctask for thread [{0}] at time [{1}]", threadNum, DateTime.Now);
            }
            else
            {
                Console.WriteLine("Waiting on task for thread [{0}] at time [{1}]", threadNum, DateTime.Now);
                await Task.WhenAll(myTask["sku1"]);
                Console.WriteLine("Finished waiting on task for thread [{0}] at time [{1}]", threadNum, DateTime.Now);
            }

            return waitMs;
        }

/// ///////////////////// Old code below this. Please ignore ////////////////////////////////////
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

            return content;
        }


        /*
        // Working on my own threads
        MyThreadCall(int waitMS)
        {
            MyThreadCallAsync(int waitMs);
        }

        static async Task<int> MyThreadCallAsync(int waitMs)
        {
            var task = await MakeMyOwnThread(5000);
            return task;
        }

        static Task<int> MakeMyOwnThread(int waitMs)
        {
            return Task.Run<int>(() => 
            {
                Task.Delay(waitMs);
                return 1;
            });
        }
        */

    }
}
