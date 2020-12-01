using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordCounter.Business
{
    public class CounterManager: ICounterManager
    {
        private readonly ConcurrentDictionary<string,int> _words;
        private readonly ConcurrentDictionary<int, int> _threadSentences;
        private ConcurrentQueue<string>[] _wordQueues;
        private string _fileName;
        private int _threadCount;
        private readonly ITextManager _textManager;

        public CounterManager(ITextManager textManager)
        {
            _textManager = textManager;
            _words = new ConcurrentDictionary<string, int>();
            _threadSentences = new ConcurrentDictionary<int, int>();
        }
        /// <summary>
        /// Sets needed parameters
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="threadCount"></param>
        public void Prepare(string fileName, int threadCount)
        {
            _fileName = fileName;
            _threadCount = threadCount;
            _wordQueues = Enumerable.Range(0, threadCount).Select(x => new ConcurrentQueue<string>()).ToArray();
        }
        /// <summary>
        /// Main method
        /// </summary>
        public void Run()
        {
            var sentences = _textManager.GetSentences(_fileName);

            for (var i = 0; i < sentences.Length; i++)
            {
                var index = i % _threadCount;
                AddQueue(index, sentences[i]);
            }

            Task[] workers = new Task[_threadCount];

            for (int i = 0; i < _threadCount; ++i)
            {
                int workerId = i;
                Task task = new Task(() => Worker(workerId));
                workers[i] = task;
                task.Start();
                AddThreadToDic(workerId);
            }

            Task.WaitAll(workers);
        }

        /// <summary>
        /// Returns word dictionary that ordered desc by word counts. 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,int> GetTextWords()
        {
            return _words.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }
        /// <summary>
        /// Returns thread dictionary with sentence count as value.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int,int> GetThreadSentences()
        {
            return new Dictionary<int, int>(_threadSentences);
        }
        /// <summary>
        /// Get sentences from thread queue and find words.
        /// </summary>
        /// <param name="workerId"></param>
        private void Worker(int workerId)
        {
            foreach (var sentence in _wordQueues[workerId])
            {
                var words = _textManager.GetWords(sentence);
                AddWords(words);
                AddThreadSentence(workerId);
            }
        }
        /// <summary>
        /// Add sentence to threads queue.
        /// </summary>
        /// <param name="indexQueue"></param>
        /// <param name="sentence"></param>
        private void AddQueue(int indexQueue, string sentence)
        {
            _wordQueues[indexQueue].Enqueue(sentence);
        }
       /// <summary>
       /// Add words to word dictionary
       /// </summary>
       /// <param name="sentencesWords"></param>
        private void AddWords(string[] sentencesWords)
        {
            foreach (var word in sentencesWords.Where(x => x != null))
            {
                if (_words.ContainsKey(word))
                {
                    _words[word]++;
                }
                else
                {
                    //_words object is concurrent but there is if else condition and all of the condition is not thread safe
                    //so to keep thread safe added bottom logic.
                    if (!_words.TryAdd(word, 1))
                    {
                        _words[word]++;
                    }
                }
            }
        }
        /// <summary>
        /// Increases threads sentence count
        /// </summary>
        /// <param name="threadId"></param>
        private void AddThreadSentence(int threadId)
        {
            _threadSentences[threadId]++;
        }
        /// <summary>
        /// Adds thread to dictionary 
        /// </summary>
        /// <param name="threadId"></param>
        private void AddThreadToDic(int threadId)
        {
            if (!_threadSentences.ContainsKey(threadId))
                _threadSentences.TryAdd(threadId, 0);
        }
    }
}
