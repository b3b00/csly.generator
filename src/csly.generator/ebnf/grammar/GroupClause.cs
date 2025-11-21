using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;
using System;
using ebnf.grammar;

namespace ebnf.grammar
{

    public sealed class GroupClause : IClause
    {
        public GroupClause(IClause clause)
        {
            Clauses = new List<IClause> { clause };
        }


        public List<IClause> Clauses { get; set; }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [ExcludeFromCodeCoverage]
        public bool MayBeEmpty()
        {
            return true;
        }

        public void AddRange(GroupClause clauses)
        {
            Clauses.AddRange(clauses.Clauses);
        }

        public string Dump()
        {
            return $"( {string.Join(" ", Clauses.Select(x => x.Dump()))} )";
        }
    }
}