// Program to determine how much space would be taken up by a Sku->lastTimeUpdated 
// Used this SO article for getting the size approximation: https://stackoverflow.com/questions/207592/getting-the-size-of-a-field-in-bytes-with-c-sharp

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeOfSkuDictionary
{
    class Program
    {
        class SkuInfo
        {
            public DateTime lastCacheUpdateTime;
        }

        static void Main(string[] args)
        {
            const int entriesInSkuDict = 8000;

            long oldSize = GC.GetTotalMemory(false);

            // Create Dictionary that will hold Sku and last time the cache for that Sku was updated
            Dictionary<string, SkuInfo> skuDictionary;
            skuDictionary = new Dictionary<string, SkuInfo>();

            for (int i = 0; i < entriesInSkuDict; i++)
            {
                // Create SkuId
                string skuId = String.Format("ABC-{0}", i.ToString("D5"));

                // Create new datetime
                var skuInfo = new SkuInfo();
                skuInfo.lastCacheUpdateTime = DateTime.Now.ToUniversalTime();

                skuDictionary.Add(skuId, skuInfo);
            }

            long newSize = GC.GetTotalMemory(false);
            long sizeOfSkuDict = newSize - oldSize;
            Console.WriteLine("Total Memory Used by Dictionary: {0}", sizeOfSkuDict);
            Console.Read();
        }
    }
}
