namespace WordCounter.Business
{
    public interface ITextManager
    {
        string[] GetSentences(string fileName);
        string[] GetWords(string sentence);
    }
}
