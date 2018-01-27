using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Concurrency
{
    class Program
    {
        static void Main(string[] args)
        {

            //DateTime t1 = DateTime.Now;
            //PrintPrimaryNumbers();
            //var ts1 = DateTime.Now.Subtract(t1);
            //Console.WriteLine("Finished Sync and started Async");
            var t2 = DateTime.Now;
            PrintPrimeNumberRanges();
            var ts2 = DateTime.Now.Subtract(t2);

            //Console.WriteLine(string.Format("It took {0} for the sync call and {1} for the Async one", ts1, ts2));
            Console.WriteLine("Press Enter to terminate!!");
            Console.ReadLine();
        }

        static void PrintPrimeNumberRanges()
        {
            for (int i = 0; i < 5; i++)
            {
                PrintPrimaryNumbersAsync(i);
                Console.WriteLine("Iteration [{0}] kicked off asynchronously", i);
            }
        }

        private static async void PrintPrimaryNumbersAsync(int i)
        {
            var result = await getPrimesAsync(i * 100000 + 1, 100000);
            Console.WriteLine("Iteration [{0}] completed asynchronously. Printing Results...", i);
            result.ToList().ForEach(x => Console.WriteLine(string.Format("ASYNC: Prime Number: {0}", x)));
        }

        public static Task<IEnumerable<int>> getPrimesAsync(int min, int count)
        {
            return Task.Run(() => Enumerable.Range(min, count).Where
            (n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i =>
              n % i > 0)));
        }

        /*
        private static void PrintPrimaryNumbers()
        {
            for (int i = 0; i < 10; i++)
                getPrimes(i * 10 + 1, 10)
                    .ToList().
                    ForEach(x => Console.WriteLine(string.Format("SYNC: Prime Number: {0}", x)));
        }

        public static int getPrimeCount(int min, int count)
        {
            return ParallelEnumerable.Range(min, count).Count(n=> 
                Enumerable.Range(2,(int)Math.Sqrt(n)-1).All(i=>
                n%i>0));
        }
        */

        public static IEnumerable<int> getPrimes(int min, int count)
        {
            return Enumerable.Range(min, count).Where
              (n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i =>
                n % i > 0));
        }

    }
}
