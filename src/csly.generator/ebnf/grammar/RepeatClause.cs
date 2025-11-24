using System;
using System.Diagnostics.CodeAnalysis;

namespace ebnf.grammar
{
    public sealed class RepeatClause : ManyClause
    {
        public int MinRepetitionCount { get; set; }
        
        public int MaxRepetitionCount { get; set; }
        
        bool IsRangeRepetition => MaxRepetitionCount !=  MinRepetitionCount;
        public RepeatClause(IClause clause, int minNumber, int maxNumber)
        {
            manyClause = clause;
            MinRepetitionCount = minNumber; 
            MaxRepetitionCount = maxNumber;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
           
                return $"{manyClause}{DumpRange()}";
           
        }

        public string DumpRange()
        {
            if (IsRangeRepetition)
            {
                return $"{{{MinRepetitionCount}-{MaxRepetitionCount}}}";
            }

            return $"{{{MinRepetitionCount}}}";
        }



        public override bool MayBeEmpty()
        {
            return MinRepetitionCount == 0;
        }

        public override string Dump()
        {
            return $"{manyClause.Dump()}{DumpRange()}";
        }
    }
}