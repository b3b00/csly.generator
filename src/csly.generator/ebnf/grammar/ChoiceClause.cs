
using System.Collections.Generic;
using System.Linq;

namespace ebnf.grammar
{
    public sealed class ChoiceClause : AbstractClause
    {

        public bool IsDiscarded { get; set; } = false;
        public bool IsTerminalChoice => Choices.Select(c => c is TerminalClause).Aggregate((x, y) => x && y);
        public bool IsNonTerminalChoice => Choices.Select(c => c is NonTerminalClause).Aggregate((x, y) => x && y);
            
        public  List<IClause> Choices { get; }
        public ChoiceClause(IClause clause)
        {
            Choices = new List<IClause> {clause};
        }
        
        public ChoiceClause(List<IClause> choices)
        {
            Choices = choices;
        }
        
        public ChoiceClause(IClause choice, List<IClause> choices) : this(choice)
        {
            Choices.AddRange(choices);
        }


        public override bool MayBeEmpty()
        {
            return false;
        }
       

        private bool Equals(ChoiceClause other)
        {
            if (other.Choices.Count != Choices.Count)
            {
                return false;
            }

            if (other.IsTerminalChoice != IsTerminalChoice)
            {
                return false;
            }

            return other.Choices.TrueForAll(x => Choices.Exists(y => y.Equals(x)));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ChoiceClause)obj);
        }

        public override string Dump()
        {
            return $"[{string.Join(" | ", Choices.Select(c => c.Dump()))}]";
        }
    }
}