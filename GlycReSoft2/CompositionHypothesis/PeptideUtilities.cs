using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycReSoft.CompositionHypothesis
{
    public static class PeptideUtilities
    {
        enum SequonParserState
        {
            Seeking,
            FoundAsparagine,
            FoundFollowingNonProline,
        }
        /// <summary>
        /// Counts the number of N-Glycosylation Sequons in the sequence
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static int CountNGlycanSequons(String sequence)
        {
            SequonParserState state = SequonParserState.Seeking;
            int numSequons = 0;
            int position = 0;
            int lastAsparaginePosition = 0;
            char currentChar;
            while (position < sequence.Length)
            {
                currentChar = sequence[position];
                if (state == SequonParserState.Seeking)
                {
                    if (currentChar == 'N')
                    {
                        lastAsparaginePosition = position;
                        state = SequonParserState.FoundAsparagine;
                    }
                }
                else if (state == SequonParserState.FoundAsparagine)
                {
                    if (currentChar != 'P')
                    {
                        state = SequonParserState.FoundFollowingNonProline;
                    }
                    else
                    {
                        state = SequonParserState.Seeking;
                    }
                }
                else if (state == SequonParserState.FoundFollowingNonProline)
                {
                    if (currentChar == 'S' || currentChar == 'T')
                    {
                        state = SequonParserState.Seeking;
                        numSequons++;
                        position = lastAsparaginePosition;
                    }
                    else
                    {
                        state = SequonParserState.Seeking;
                    }
                }
                position++;
            }
            return numSequons;
        }
        public static int CountNGlycanSequons(MSDigestPeptide sequence)
        {
            return CountNGlycanSequons(sequence.Sequence);
        }
    }
}
