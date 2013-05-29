namespace NFastTag.PosTag
{
    /// <summary>
    /// Represent POS result for a word
    /// </summary>
    public class FastTagResult
    {
        /// <summary>
        /// The word used for tagging
        /// </summary>

        public string Word { get; private set; }

        /// <summary>
        /// The assigned tag
        /// </summary>

        public string PosTag { get; private set; }

        /// <summary>
        /// CTOR

        /// </summary>
        /// <param name="word"></param>
        /// <param name="pTag"></param>

        public FastTagResult(string word, string pTag)
        {
            Word = word;
            PosTag = pTag;
        }
    }
}