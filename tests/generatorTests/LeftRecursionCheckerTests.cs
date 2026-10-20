using System.Collections.Generic;
using System.Linq;
using csly.ebnf.builder;
using ebnf.grammar;
using csly.generator.sourceGenerator;
using NFluent;

namespace generatorTests;

public class LeftRecursionCheckerTests
{
    private static ParserModel Model(params Rule[] rules)
    {
        var model = new ParserModel("Parser", "Lexer", "string");
        foreach (var rule in rules)
        {
            model.AddRule(rule);
        }

        return model;
    }

    private static Rule Rule(string head, string methodName, params IClause[] clauses)
    {
        return new Rule(head, new List<IClause>(clauses), methodName) { NonTerminalName = head };
    }

    private static Rule Rule(string head, params IClause[] clauses)
    {
        return Rule(head, head, clauses);
    }

    private (bool found, List<List<string>> recursions) RunCheck(ParserModel model)
    {
        return LeftRecursionChecker.CheckLeftRecursion(model);
    }

    private static IEnumerable<string> Paths(List<List<string>> recursions)
    {
        return recursions.Select(r => string.Join(" > ", r));
    }

    #region direct left recursion

    [Fact]
    public void DirectLeftRecursionIsDetected()
    {
        // A : A 'a' | 'b'
        var model = Model(
            Rule("A", new NonTerminalClause("A"), new TerminalClause("'a'")),
            Rule("A", new TerminalClause("'b'")));

        var (found, recursions) = RunCheck(model);

        Check.That(found).IsTrue();
        Check.That(Paths(recursions)).ContainsExactly("A > A");
    }

    [Fact]
    public void DirectLeftRecursionThroughTerminalFirstThenNonTerminalIsNotDetected()
    {
        // A : 'a' A   (first clause is a terminal => not left recursive)
        var model = Model(
            Rule("A", new TerminalClause("'a'"), new NonTerminalClause("A")),
            Rule("A", new TerminalClause("'b'")));

        var (found, _) = RunCheck(model);

        Check.That(found).IsFalse();
    }

    #endregion

    #region indirect left recursion

    [Fact]
    public void IndirectLeftRecursionIsDetected()
    {
        // A : B ; B : A
        var model = Model(
            Rule("A", new NonTerminalClause("B")),
            Rule("B", new NonTerminalClause("A")));

        var (found, recursions) = RunCheck(model);

        Check.That(found).IsTrue();
        Check.That(Paths(recursions)).Contains("A > B > A");
        Check.That(Paths(recursions)).Contains("B > A > B");
    }

    [Fact]
    public void LongerIndirectLeftRecursionIsDetected()
    {
        // A : B ; B : C ; C : A
        var model = Model(
            Rule("A", new NonTerminalClause("B")),
            Rule("B", new NonTerminalClause("C")),
            Rule("C", new NonTerminalClause("A")));

        var (found, recursions) = RunCheck(model);

        Check.That(found).IsTrue();
        Check.That(Paths(recursions)).Contains("A > B > C > A");
    }

    [Fact]
    public void IndirectLeftRecursionWithIntermediateNonRecursiveBranchIsDetected()
    {
        // A : B | 'x' ; B : A
        var model = Model(
            Rule("A", new NonTerminalClause("B"), new TerminalClause("'x'")),
            Rule("B", new NonTerminalClause("A")));

        var (found, recursions) = RunCheck(model);

        Check.That(found).IsTrue();
        Check.That(Paths(recursions)).Contains("A > B > A");
    }

    #endregion

    #region nullable wrappers

    [Fact]
    public void LeftRecursionThroughOptionIsDetected()
    {
        // A : A? 'a'   (Option is nullable)
        var model = Model(
            Rule("A", new OptionClause(new NonTerminalClause("A")), new TerminalClause("'a'")),
            Rule("A", new TerminalClause("'b'")));

        var (found, recursions) = RunCheck(model);

        Check.That(found).IsTrue();
        Check.That(Paths(recursions)).Contains("A > A");
    }

    [Fact]
    public void LeftRecursionThroughZeroOrMoreIsDetected()
    {
        // A : A* 'b'   (ZeroOrMore is nullable)
        var model = Model(
            Rule("A", new ZeroOrMoreClause(new NonTerminalClause("A")), new TerminalClause("'b'")),
            Rule("A", new TerminalClause("'c'")));

        var (found, recursions) = RunCheck(model);

        Check.That(found).IsTrue();
        Check.That(Paths(recursions)).Contains("A > A");
    }

    [Fact]
    public void LeftRecursionThroughOneOrMoreIsDetected()
    {
        // A : A+ 'c'   (leading OneOrMore resolves to A => left recursive)
        var model = Model(
            Rule("A", new OneOrMoreClause(new NonTerminalClause("A")), new TerminalClause("'c'")),
            Rule("A", new TerminalClause("'d'")));

        var (found, recursions) = RunCheck(model);

        Check.That(found).IsTrue();
        Check.That(Paths(recursions)).Contains("A > A");
    }

    [Fact]
    public void NonNullableOneOrMoreBlocksTrailingRecursion()
    {
        // A : B+ A ; B : 'x'   (OneOrMore not nullable => walk stops before trailing A)
        var model = Model(
            Rule("A", new OneOrMoreClause(new NonTerminalClause("B")), new NonTerminalClause("A")),
            Rule("B", new TerminalClause("'x'")));

        var (found, _) = RunCheck(model);

        Check.That(found).IsFalse();
    }

    [Fact]
    public void NullableNonRecursiveChainIsNotFlagged()
    {
        // A : B* ; B : 'x'   (B* nullable but B does not refer back to A)
        var model = Model(
            Rule("A", new ZeroOrMoreClause(new NonTerminalClause("B"))),
            Rule("B", new TerminalClause("'x'")));

        var (found, _) = RunCheck(model);

        Check.That(found).IsFalse();
    }

    #endregion

    #region group and choice clauses

    [Fact]
    public void LeftRecursionThroughGroupClauseIsDetected()
    {
        // A : (A) 'a'   (GroupClause's first clause is A)
        var model = Model(
            Rule("A", new GroupClause(new NonTerminalClause("A")), new TerminalClause("'a'")),
            Rule("A", new TerminalClause("'b'")));

        var (found, recursions) = RunCheck(model);

        Check.That(found).IsTrue();
        Check.That(Paths(recursions)).Contains("A > A");
    }

    [Fact]
    public void LeftRecursionThroughNonTerminalChoiceIsDetected()
    {
        // A : (A | B) 'a'   (all choices non-terminal => lefts include A and B)
        var model = Model(
            Rule("A", new ChoiceClause(new List<IClause> { new NonTerminalClause("A"), new NonTerminalClause("B") }), new TerminalClause("'a'")),
            Rule("A", new TerminalClause("'b'")),
            Rule("B", new TerminalClause("'x'")));

        var (found, recursions) = RunCheck(model);

        Check.That(found).IsTrue();
        Check.That(Paths(recursions)).Contains("A > A");
    }

    [Fact]
    public void MixedTerminalAndNonTerminalChoiceIsNotTreatedAsStartingWithNonTerminal()
    {
        // A : (A | 'x') 'a'   (mixed choice => IsNonTerminalChoice is false => no lefts)
        var model = Model(
            Rule("A", new ChoiceClause(new List<IClause> { new NonTerminalClause("A"), new TerminalClause("'x'") }), new TerminalClause("'a'")),
            Rule("A", new TerminalClause("'b'")));

        var (found, _) = RunCheck(model);

        Check.That(found).IsFalse();
    }

    [Fact]
    public void TerminalChoiceIsNotLeftRecursive()
    {
        // A : ('a' | 'b')   (terminal choice => no left non-terminal)
        var model = Model(
            Rule("A", new ChoiceClause(new List<IClause> { new TerminalClause("'a'"), new TerminalClause("'b'") })));

        var (found, _) = RunCheck(model);

        Check.That(found).IsFalse();
    }

    #endregion

    #region missing / empty

    [Fact]
    public void ReferenceToMissingNonTerminalDoesNotCrashAndIsNotRecursive()
    {
        // A : Missing 'a'   (Missing is not a declared non-terminal)
        var model = Model(
            Rule("A", new NonTerminalClause("Missing"), new TerminalClause("'a'")),
            Rule("A", new TerminalClause("'b'")));

        var (found, _) = RunCheck(model);

        Check.That(found).IsFalse();
    }

    [Fact]
    public void RuleWithNoClausesIsNotRecursive()
    {
        var model = Model(
            Rule("A"));

        var (found, _) = RunCheck(model);

        Check.That(found).IsFalse();
    }

    [Fact]
    public void NonRecursiveGrammarIsNotFlagged()
    {
        // A : 'a' B ; B : 'b'
        var model = Model(
            Rule("A", new TerminalClause("'a'"), new NonTerminalClause("B")),
            Rule("B", new TerminalClause("'b'")));

        var (found, _) = RunCheck(model);

        Check.That(found).IsFalse();
    }

    #endregion

    #region multiple rules / mixed

    [Fact]
    public void OneRecursiveRuleAmongManyIsFlagged()
    {
        // A : 'a' | A 'b'   (only the second rule is left recursive)
        var model = Model(
            Rule("A", new TerminalClause("'a'")),
            Rule("A", new NonTerminalClause("A"), new TerminalClause("'b'")));

        var (found, recursions) = RunCheck(model);

        Check.That(found).IsTrue();
        Check.That(Paths(recursions)).Contains("A > A");
    }

    [Fact]
    public void MultipleIndependentNonTerminalsOneRecursiveIsFlagged()
    {
        // A : 'a' ; B : B 'b'
        var model = Model(
            Rule("A", new TerminalClause("'a'")),
            Rule("B", new NonTerminalClause("B"), new TerminalClause("'b'")));

        var (found, recursions) = RunCheck(model);

        Check.That(found).IsTrue();
        Check.That(Paths(recursions)).Contains("B > B");
    }

    [Fact]
    public void EmptyModelIsNotRecursive()
    {
        var model = Model();

        var (found, _) = RunCheck(model);

        Check.That(found).IsFalse();
    }

    #endregion

    #region nullable intermediate non-terminal

    [Fact]
    public void LeftRecursionThroughNullableIntermediateNonTerminalIsDetected()
    {
        // A : B A ; B : ε   (B nullable => A effectively starts with A, direct self-recursion)
        var bClause = new NonTerminalClause("B");
        bClause.SetMayBeEmpty(true);
        var model = Model(
            Rule("A", bClause, new NonTerminalClause("A")),
            Rule("B"));

        var (found, recursions) = RunCheck(model);

        Check.That(found).IsTrue();
        Check.That(Paths(recursions)).Contains("A > A");
    }

    #endregion
}
