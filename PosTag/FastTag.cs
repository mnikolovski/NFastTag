using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NFastTag.PosTag
{
    /// <summary>
    /// .NET port of the mark-watson FastTag_v2
    /// </summary>
    public class FastTag
    {
        /// <summary>
        /// Internal word lexicon where the word/pos tags are stored
        /// </summary>
        private readonly Dictionary<string, String[]> _lexicon = new Dictionary<string, String[]>();

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="learningData"></param>
        public FastTag(string learningData)
        {
            using (var sr = new StringReader(learningData))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    ParseLine(line);
                }
            }
        }

        /// <summary>
        /// Checks if the provided word exist in the imported lexicon
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool WordInLexicon(string word)
        {
            return _lexicon.ContainsKey(word) || _lexicon.ContainsKey(word.ToLower());
        }

        /// <summary>
        /// Assigns parts of speech to each word
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public IList<FastTagResult> Tag(IList<string> words)
        {
            if (words == null || words.Count == 0) return new List<FastTagResult>();
            
            var result = new List<FastTagResult>();
            var pTags = GetPosTagsFor(words);
            // Apply transformational rules
            for (int i = 0; i < words.Count; i++)
            {
                string word = words[i];
                string pTag = pTags[i];
                //  rule 1: DT, {VBD | VBP} --> DT, NN		
                if (i > 0 && string.Equals(pTags[i - 1], "DT"))
                {
                    if (string.Equals(pTag, "VBD") || string.Equals(pTag, "VBP") || string.Equals(pTag, "VB"))
                    {
                        pTag = "NN";
                    }
                }
                // rule 2: convert a noun to a number (CD) if "." appears in the word
                if (pTag.StartsWith("N"))
                {
                    Single s;
                    if (word.IndexOf(".", StringComparison.CurrentCultureIgnoreCase) > -1 || Single.TryParse(word, out s))
                    {
                        pTag = "CD";
                    }
                }
                // rule 3: convert a noun to a past participle if words.get(i) ends with "ed"
                if (pTag.StartsWith("N") && word.EndsWith("ed"))
                {
                    pTag = "VBN";
                }
                // rule 4: convert any type to adverb if it ends in "ly";
                if (word.EndsWith("ly"))
                {
                    pTag = "RB";
                }
                // rule 5: convert a common noun (NN or NNS) to a adjective if it ends with "al"
                if (pTag.StartsWith("NN") && word.EndsWith("al"))
                {
                    pTag = "JJ";
                }
                // rule 6: convert a noun to a verb if the preceeding work is "would"
                if (i > 0 && pTag.StartsWith("NN") && string.Equals(words[i - 1], "would"))
                {
                    pTag = "VB";
                }
                // rule 7: if a word has been categorized as a common noun and it ends with "s",
                //         then set its type to plural common noun (NNS)
                if (string.Equals(pTag, "NN") && word.EndsWith("s"))
                {
                    pTag = "NNS";
                }
                // rule 8: convert a common noun to a present participle verb (i.e., a gerand)
                if (pTag.StartsWith("NN") && word.EndsWith("ing"))
                {
                    pTag = "VBG";
                }

                result.Add(new FastTagResult(word, pTag));
            }
            return result;
        }

        /// <summary>
        /// Assigns parts of speech to a sentence
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public IList<FastTagResult> Tag(string sentence)
        {
            if (string.IsNullOrEmpty(sentence)) return new List<FastTagResult>();
            var sentenceWords = sentence.Split(' ');
            return Tag(sentenceWords);
        }

        /// <summary>
        /// Retrieve the pos tags from the lexicon for the provided word list
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        private IList<string> GetPosTagsFor(IList<string> words)
        {
            IList<string> ret = new List<string>(words.Count);
            for (int i = 0, size = words.Count; i < size; i++)
            {
                var word = RemoveSpecialCharacters(words[i]);
                if (string.IsNullOrEmpty(word))
                {
                    ret.Add(@"");
                    continue;
                }

                string[] ss;
                _lexicon.TryGetValue(word, out ss);
                // 1/22/2002 mod (from Lisp code): if not in hash, try lower case:
                if (ss == null)
                {
                    _lexicon.TryGetValue(word.ToLower(), out ss);
                }
                if (ss == null && word.Length == 1)
                {
                    ret.Add(word + "^");
                }
                else if (ss == null)
                {
                    ret.Add("NN");
                }
                else
                {
                    ret.Add(ss[0]);
                }
            }

            return ret;
        }

        /// <summary>
        /// Clears special chars from start and end of the word
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string RemoveSpecialCharacters(string str)
        {
            if (str.Length == 1) return str;
            var rpl = Regex.Replace(str, "^[^A-Za-z0-9]+|[^A-Za-z0-9]+$", string.Empty);
            return rpl;
        }

        /// <summary>
        /// Parse a line into word and part of speech tags
        /// </summary>
        /// <param name="line"></param>
        private void ParseLine(string line)
        {
            var ss = line.Split(' ');
            var word = ss[0];
            _lexicon[word] = ss.Skip(1).ToArray();
        }
    }
}