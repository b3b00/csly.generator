using Xunit;
using csly.whileLang;
using csly.whileLang.model;
using NFluent;
using csly.whileLang.interpreter;
using csly.whileLang.compiler;



namespace ParserTests.samples;

[CollectionDefinition("while", DisableParallelization = true)]
public class WhileTests
{



    public WhileParserGenericMain BuildParser()
    {
        WhileParserGeneric instance = new WhileParserGeneric();

        var parser = new WhileParserGenericMain(instance);
        return parser;

    }


    [Fact]
    public void TestAssignAdd()
    {
        var parser = BuildParser();
        var result = parser.Parse("(a:=1+1)");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();

        Check.That(result.Result).IsInstanceOf<SequenceStatement>();
        var seq = result.Result as SequenceStatement;
        Check.That(seq.Get(0)).IsInstanceOf<AssignStatement>();
        var assign = seq.Get(0) as AssignStatement;
        Check.That(assign.VariableName).IsEqualTo("a");
        var val = assign.Value;
        Check.That(val).IsInstanceOf<BinaryOperation>();
        var bin = val as BinaryOperation;
        Check.That(bin.Operator).IsEqualTo(BinaryOperator.ADD);
        Check.That((bin.Left as IntegerConstant)?.Value).IsEqualTo(1);
        Check.That((bin.Right as IntegerConstant)?.Value).IsEqualTo(1);
    }

    [Fact]
    public void TestBuildParser()
    {
        var parser = BuildParser();
        Check.That(parser).IsNotNull();
    }

    [Fact]
    public void TestCounterProgram()
    {
        var parser = BuildParser();
        var result = parser.Parse("(a:=0; while a < 10 do (print a; a := a +1 ))");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
    }

    [Fact]
    public void TestCounterProgramExec()
    {
        var parser = BuildParser();
        var result = parser.Parse("(a:=0; while a < 10 do (print a; a := a +1 ))");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        var interpreter = new Interpreter();
        var context = interpreter.Interprete(result.Result, true);
        Check.That(context.variables).CountIs(1);
        var a = context.GetVariable("a");
        Check.That(a).IsNotNull();
        Check.That(a.ValueType).IsEqualTo(WhileType.INT);
        Check.That(a.IntValue).IsEqualTo(10);

    }

    [Fact]
    public void TestFactorialProgramExec()
    {
        var program = @"
(
    r:=1;
    i:=1;
    while i < 11 do 
    ( 
    r := r * i;
    print r;
    print i;
    i := i + 1 )
)";
        var parser = BuildParser();
        var result = parser.Parse(program);
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        var interpreter = new Interpreter();
        var context = interpreter.Interprete(result.Result, true);
        Check.That(context.variables).CountIs(2);
        var i = context.GetVariable("i");
        Check.That(i).IsNotNull();
        Check.That(i.ValueType).IsEqualTo(WhileType.INT);
        Check.That(i.IntValue).IsEqualTo(11);
        var r = context.GetVariable("r");
        Check.That(r).IsNotNull();
        Check.That(r.ValueType).IsEqualTo(WhileType.INT);
        Check.That(r.IntValue).IsEqualTo(3628800);
    }




    [Fact]
    public void TestIfThenElse()
    {
        var parser = BuildParser();
        var result = parser.Parse("if true then (a := \"hello\") else (b := \"world\")");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsInstanceOf<SequenceStatement>();
        var seq = result.Result as SequenceStatement;
        Check.That(seq.Get(0)).IsInstanceOf<IfStatement>();
        var si = seq.Get(0) as IfStatement;
        var cond = si.Condition;
        Check.That(cond).IsInstanceOf<BoolConstant>();
        Check.That((cond as BoolConstant).Value).IsTrue();
        var s = si.ThenStmt;

        Check.That(si.ThenStmt).IsInstanceOf<SequenceStatement>();
        var thenBlock = si.ThenStmt as SequenceStatement;
        Check.That(thenBlock).CountIs(1);
        Check.That(thenBlock.Get(0)).IsInstanceOf<AssignStatement>();
        var thenAssign = thenBlock.Get(0) as AssignStatement;
        Check.That(thenAssign.VariableName).IsEqualTo("a");
        Check.That(thenAssign.Value).IsInstanceOf<StringConstant>();
        Check.That((thenAssign.Value as StringConstant).Value).IsEqualTo("hello");

        Check.That(si.ElseStmt).IsInstanceOf<SequenceStatement>();
        var elseBlock = si.ElseStmt as SequenceStatement;
        Check.That(elseBlock).CountIs(1);
        Check.That(elseBlock.Get(0)).IsInstanceOf<AssignStatement>();
        var elseAssign = elseBlock.Get(0) as AssignStatement;
        Check.That(elseAssign.VariableName).IsEqualTo("b");
        Check.That(elseAssign.Value).IsInstanceOf<StringConstant>();
        Check.That((elseAssign.Value as StringConstant).Value).IsEqualTo("world");
    }

    [Fact]
    public void TestInfiniteWhile()
    {
        var parser = BuildParser();
        var result = parser.Parse("while true do (skip)");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();

        Check.That(result.Result).IsInstanceOf<SequenceStatement>();
        var seq = result.Result as SequenceStatement;
        Check.That(seq.Get(0)).IsInstanceOf<WhileStatement>();
        var whil = seq.Get(0) as WhileStatement;
        var cond = whil.Condition;
        Check.That(cond).IsInstanceOf<BoolConstant>();
        Check.That((cond as BoolConstant).Value).IsTrue();
        var s = whil.BlockStmt;
        Check.That(whil.BlockStmt).IsInstanceOf<SequenceStatement>();
        var seqBlock = whil.BlockStmt as SequenceStatement;
        Check.That(seqBlock).CountIs(1);
        Check.That(seqBlock.Get(0)).IsInstanceOf<SkipStatement>();
    }

    [Fact]
    public void TestPrintBoolExpression()
    {
        var parser = BuildParser();
        var result = parser.Parse("print true and false");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();

        Check.That(result.Result).IsInstanceOf<SequenceStatement>();
        var seq = result.Result as SequenceStatement;
        Check.That(seq.Get(0)).IsInstanceOf<PrintStatement>();
        var print = seq.Get(0) as PrintStatement;
        var expr = print.Value;
        Check.That(expr).IsInstanceOf<BinaryOperation>();
        var bin = expr as BinaryOperation;
        Check.That(bin.Operator).IsEqualTo(BinaryOperator.AND);
        Check.That(bin.Left).IsInstanceOf<BoolConstant>();
        Check.That(bin.Right).IsInstanceOf<BoolConstant>();
        Check.That((bin.Left as BoolConstant).Value).IsTrue();
        Check.That((bin.Right as BoolConstant).Value).IsFalse();
    }


    [Fact]
    public void TestSkip()
    {
        var parser = BuildParser();
        var result = parser.Parse("skip");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();

        Check.That(result.Result).IsInstanceOf<SequenceStatement>();
        var seq = result.Result as SequenceStatement;
        Check.That(seq.Get(0)).IsInstanceOf<SkipStatement>();
    }

    [Fact]
    public void TestSkipAssignSequence()
    {
        var parser = BuildParser();
        var result = parser.Parse("(a:=1; b:=2; c:=3)");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsInstanceOf<SequenceStatement>();
        var seq = result.Result as SequenceStatement;
        Check.That(seq).CountIs(3);

        (string name, int value)[] values = new[] { ("a", 1), ("b", 2), ("c", 3) };

        Check.That(seq.Cast<AssignStatement>().Extracting(x => (x.VariableName, (x.Value as IntegerConstant).Value))).ContainsExactly(values);
    }


    [Fact]
    public void TestSkipSkipSequence()
    {
        var parser = BuildParser();
        var result = parser.Parse("(skip; skip; skip)");
        Check.That(result).IsNotNull();
        Check.That(result.IsOk).IsTrue();
        Check.That(result.Result).IsInstanceOf<SequenceStatement>();
        var seq = result.Result as SequenceStatement;
        Check.That(seq).CountIs(3);
        Check.That(seq.Get(0)).IsInstanceOf<SkipStatement>();
        Check.That(seq.Get(1)).IsInstanceOf<SkipStatement>();
        Check.That(seq.Get(2)).IsInstanceOf<SkipStatement>();
    }
}
