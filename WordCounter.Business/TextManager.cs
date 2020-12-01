using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WordCounter.Business
{
    public class TextManager: ITextManager
    {
        /// <summary>
        /// Return all sentences from text file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string[] GetSentences(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException();
            }
            var text = File.ReadAllText(fileName);
            return Regex.Split(text, @"(?<=[\.!\?])\s+");
        }
        /// <summary>
        /// Return all words from sentence
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public string[] GetWords(string sentence)
        {
            var matches = Regex.Matches(sentence, @"\b[\w']*\b");
            var sentencesWords = from m in matches.Cast<Match>()
                where !string.IsNullOrEmpty(m.Value)
                select m.Value;
            return sentencesWords.ToArray();
        }
    }
}
