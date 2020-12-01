using System;
using System.IO;
using System.Linq;
using WordCounter.Business;

namespace WordCounter
{
    public class CounterApplication
    {
        private readonly ICounterManager _counterManager;
        public CounterApplication(ICounterManager counterManager)
        {
            _counterManager = counterManager;
        }
        public void Run()
        {
            var fileNotExists = true;
            var fullFileName = string.Empty;
            var threadCount = 5;
            var isThreadCountWrongInput = true;

            while (fileNotExists)
            {
                Console.WriteLine("File full name:");
                fullFileName = Console.ReadLine();
                fileNotExists = !File.Exists(fullFileName);
            }

            while (isThreadCountWrongInput)
            {
                Console.WriteLine($"Default thread count is {threadCount}, set different or Enter!");
                var count = Console.ReadLine();
                if (count == string.Empty)
                {
                    break;
                }
                isThreadCountWrongInput = !Int16.TryParse(count, out _);
                if (!isThreadCountWrongInput)
                {
                    threadCount = Convert.ToInt16(count);
                }
            }

            _counterManager.Prepare(fullFileName,threadCount);
            _counterManager.Run();

            var threadSentences = _counterManager.GetThreadSentences();
            var words = _counterManager.GetTextWords();
            var totalWords = words.Sum(x => x.Value);
            var totalSentences = threadSentences.Sum(x => x.Value);

            Console.WriteLine($"Sentence Count:{totalSentences}");
            Console.WriteLine($"Avg.Word Count:{(float)totalWords / (float)totalSentences}");
            Console.WriteLine("Thread counts:");
            foreach (var thread in threadSentences)
            {
                Console.WriteLine($"     ThreadId={thread.Key}, Count={thread.Value}");
            }

            foreach (var word in words)
            {
                Console.WriteLine($"{word.Key} {word.Value}");
            }
            Console.ReadKey();
        }
    }
}
