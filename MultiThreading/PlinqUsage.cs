using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    public class PlinqUsage
    {
        public static void Call()
        {
            // Calculate prime numbers.
            IEnumerable<int> numbers = Enumerable.Range(3, 100000 - 3);
            var parallelQuery = from n in numbers.AsParallel()
                                where Enumerable.Range(2, (int)Math.Sqrt(n)).All(i => n % i > 0)
                                select n;
            int[] primes = parallelQuery.ToArray();



            var r = from site in new[] { "www.oreilly.com", "www.google.com", "stackoverflow.com" }
                    .AsParallel().WithDegreeOfParallelism(3)
                    let p = new Ping().Send(site)
                    select new
                    {
                        site,
                        Result = p.Status,
                        Time = p.RoundtripTime
                    };



            IEnumerable<int> million = Enumerable.Range(3, 1000000);
            var cancelSource = new CancellationTokenSource();
            var primeNumberQuery = from n in million.AsParallel().WithCancellation(cancelSource.Token)
                                   where Enumerable.Range(2, (int)Math.Sqrt(n)).All(i => n % i > 0)
                                   select n;
            new Thread(() =>
            {
                Thread.Sleep(100);
                cancelSource.Cancel();
            }
            ).Start();
            try
            {
                // Start query running:
                int[] p = primeNumberQuery.ToArray();
                // We'll never get here because the other thread will cancel us.
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Query canceled");
            }



            int[] nmb = { 3, 4, 5, 6, 7, 8, 9 };
            var pq = Partitioner.Create(nmb, true).AsParallel();


            int[] nn = { 2, 3, 4 };
            int sum = nn.Aggregate(0, (total, n) => total + n); // 9
            sum = nn.Select(n => n * n).Aggregate((total, n) => total + n);
        }
        public static void SpellChecker()
        {
            if (!File.Exists("WordLookup.txt"))
                new WebClient().DownloadFile("url", "WordLookup.txt");
            var wordLookup = new HashSet<string>(File.ReadAllLines("WordLookup.txt"), StringComparer.InvariantCultureIgnoreCase);
            string[] wordList = wordLookup.ToArray();

            var random = new Random();    
            string[] wordsToTest = Enumerable.Range(0, 1000000).Select(i => wordList[random.Next(0, wordList.Length)]).ToArray();

            var localRandom = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
            wordsToTest = Enumerable.Range(0, 1000000).AsParallel().Select(i => wordList[localRandom.Value.Next(0, wordList.Length)]).ToArray();


            //create errors
            wordsToTest[12345] = "vgtgkjn"; 
            wordsToTest[23456] = "bjhfh"; 

            var query = wordsToTest
                        .AsParallel()
                        .Select((word, index) => new IndexedWord { Word = word, Index = index })
                        .Where(iword => !wordLookup.Contains(iword.Word))
                        .OrderBy(iword => iword.Index);
            foreach (var mistake in query)
                Console.WriteLine(mistake.Word + " - index = " + mistake.Index);

        }

        public struct IndexedWord
        {
            public string Word;
            public int Index;
        }
    }
}
