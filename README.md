# NFastTag

NFastTag is a .NET port of Mark Watson's FastTag Part of Speech Tagger which was itself based on Eric Brill's trained rule set and English lexicon.

Licensed under LGPL3 or Apache 2 licenses

The code is set up to read the file lexicon.txt.

ACKNOWLEDGMENTS:
----------------

- Eric Brill for his lexicon and trained rule set:   http://www.cs.jhu.edu/~brill/

- Medpost team for their tagging lexicon:            http://mmtx.nlm.nih.gov/MedPost_SKR.shtml

- Brant Chee for bug reports and bug fixes

TAG DEFINITIONS:
----------------

<pre>
	
CC Coord Conjuncn           and,but,or
CD Cardinal number          one,two
DT Determiner               the,some
EX Existential there        there
FW Foreign Word             mon dieu
IN Preposition              of,in,by
JJ Adjective                big
JJR Adj., comparative       bigger
JJS Adj., superlative       biggest
LS List item marker         1,One
MD Modal                    can,should
NN Noun, sing. or mass      dog
NNP Proper noun, sing.      Edinburgh
NNPS Proper noun, plural    Smiths
NNS Noun, plural            dogs
POS Possessive ending       �s
PDT Predeterminer           all, both
PP$ Possessive pronoun      my,one�s
PRP Personal pronoun         I,you,she
RB Adverb                   quickly
RBR Adverb, comparative     faster
RBS Adverb, superlative     fastest
RP Particle                 up,off
SYM Symbol                  +,%,&
TO �to�                     to
UH Interjection             oh, oops
URL url                     http://www.google.com/
VB verb, base form          eat
VBD verb, past tense        ate
VBG verb, gerund            eating
VBN verb, past part         eaten
VBP Verb, present           eat
VBZ Verb, present           eats
WDT Wh-determiner           which,that
WP Wh pronoun               who,what
WP$ Possessive-Wh           whose
WRB Wh-adverb               how,where

</pre>

## Usage
```csharp
    // read the english lexicon data
    var lexicon = File.ReadAllText("Grammar\\lexicon.txt");
    // run the sample loop
    var ft = new FastTag(lexicon);
    var tagResult = ft.Tag(@"The truth is revealed in what we do, not in what we think.");
    foreach (var ftr in tagResult)
    {
        var message = string.Format(@"[{0} {1}]", ftr.Word, ftr.PosTag);
        Console.WriteLine(message);
    }
```
