using System.Collections.Generic;

namespace WordCounter.Business
{
    public interface ICounterManager
    {
        void Prepare(string fileName, int threadCount);
        Dictionary<string, int> GetTextWords();
        Dictionary<int, int> GetThreadSentences();
        void Run();
    }
}
