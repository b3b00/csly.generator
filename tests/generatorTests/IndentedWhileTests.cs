using Xunit;
using csly.indented.whileLang;
using csly.indented.whileLang.compiler;
using csly.indented.whileLang.model;
using NFluent;
using csly.indented.whileLang.interpreter;
using csly.indented.whileLang.interpreter;
using csly.indentedWhile.models;
using csly.indentedWhile.whileLang;


namespace ParserTests.samples;

[CollectionDefinition("while", DisableParallelization = true)]
public class IndentedWhileTests
{



    public IndentedWhileParserGenericMain BuildParser()
    {
        IndentedWhileParserGeneric instance = new IndentedWhileParserGeneric();

        var parser = new IndentedWhileParserGenericMain(instance);
        return parser;

    }


    [Fact]
    public void TestAssignAdd()
    {
        var parser = BuildParser();
        
        var result = parser.Parse("a:=1+1");
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
        public void TestCounterProgram()
        {
            var parser = BuildParser();
            string program = @"
a:=0 
while a < 10 do 
    print a
    a := a +1
";
            var result = parser.Parse(program);
            Check.That(result.IsOk).IsTrue();
            Check.That(result.Result).IsNotNull();
        }

        [Fact]
        public void TestCounterProgramExec()
        {
            var parser = BuildParser();
            string program = @"
a:=0 
while a < 10 do 
    print a
    a := a +1
";
            var result = parser.Parse(program);
            Check.That(result.IsOk).IsTrue();
            Check.That(result.Result).IsNotNull();
            var interpreter = new Interpreter();
            var context = interpreter.Interprete(result.Result, true);
            Check.That(context.variables).CountIs(1);
            var a = context.GetVariable("a");
            Check.That(a.Value).IsEqualTo(10);
            Check.That(a.ValueType).IsEqualTo(WhileType.INT);
        }

        [Fact]
        public void TestFactorialProgramExec()
        {
            var program = @"
# TestFactorialProgramExec
r:=1
i:=1
while i < 11 do 
    r := r * i
    print r
    print i
    i := i + 1 
";
            var parser = BuildParser();
            var result = parser.Parse(program);
            Check.That(result.IsOk).IsTrue();
            Check.That(result.Result).IsNotNull();
            var interpreter = new Interpreter();
            var context = interpreter.Interprete(result.Result, true);
            Check.That(context.variables).CountIs(2);
            CheckvariableInt(context,"i",11);
            CheckvariableInt(context,"r",3628800);
        }

        private static void CheckvariableInt(InterpreterContext context, string variableName, int expectedValue)
        {
            var i = context.GetVariable(variableName);
            Check.That(i.Value).IsEqualTo(expectedValue);
            Check.That(i.ValueType).IsEqualTo(WhileType.INT);
        }
        
        private static void CheckvariableBool(InterpreterContext context, string variableName, bool expectedValue)
        {
            var i = context.GetVariable(variableName);
            Check.That(i.Value).IsEqualTo(expectedValue);
            Check.That(i.ValueType).IsEqualTo(WhileType.BOOL);
        }
        
        private static void CheckvariableString(InterpreterContext context, string variableName, string expectedValue)
        {
            var i = context.GetVariable(variableName);
            Check.That(i.Value).IsEqualTo(expectedValue);
            Check.That(i.ValueType).IsEqualTo(WhileType.STRING);
        }


        

        [Fact]
        public void TestIfThenElse()
        {
            var parser = BuildParser();
            var program = @"
# TestIfThenElse
if true then
    a := $""hello""
else
    b := $""world""
";
            var result = parser.Parse(program);
            Check.That(result.IsOk).IsTrue();
            Check.That(result.Result).IsNotNull();
            

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
            var fstring = thenAssign.Value as StringConstant;
            Check.That(fstring).IsNotNull();
            Check.That(fstring.Value).IsEqualTo("hello");

            Check.That(si.ElseStmt).IsInstanceOf<SequenceStatement>();
            var elseBlock = si.ElseStmt as SequenceStatement;
            Check.That(elseBlock).CountIs(1);
            Check.That(elseBlock.Get(0)).IsInstanceOf<AssignStatement>();
            var elseAssign = elseBlock.Get(0) as AssignStatement;
            Check.That(elseAssign.VariableName).IsEqualTo("b");
            Check.That(elseAssign.Value).IsInstanceOf<StringConstant>();
            fstring = elseAssign.Value as StringConstant;
            Check.That(fstring).IsNotNull();
            Check.That(fstring.Value).IsEqualTo("world");
        }

        [Fact]
        public void TestNestedIfThenElse()
        {
            var program = @"
# TestIfThenElse
a := -111
if true then
    if true then
        a := 1
    else
        a := 2
else
    a := 3
    b := $""world""
return a
";
            // var compiler = new IndentedWhileCompiler();
            // var func = compiler.CompileToFunction(program,true);
            // Check.That(func).IsNotNull();
            // var f = func();
            // Check.That(f).IsEqualTo(1);
        }


        [Fact]
        public void TestFString()
        {
            var parser = BuildParser();
            var program = @"
# fstring
v1 := 48
v2 := 152
b := true
fstring := $""v1 :> {v1} < v2 :> {v2} < v3 :> {v1+v2} <  v4 :>{$""hello,"".$"" world""}< v5 :>{(? b -> $""true"" | $""false"")}< - end""
print fstring
return 100
";
            
            var result = parser.Parse(program);
            Check.That(result.Result).IsNotNull();
            Check.That(result.IsOk).IsTrue();
            Check.That(result.Result).IsNotNull();
            Check.That(result.Result).IsInstanceOf<SequenceStatement>();
            SequenceStatement seq = result.Result as SequenceStatement;
            Check.That(seq.Count).IsEqualTo(6);
            var fstringAssign = seq.Get(3) as AssignStatement;
            Check.That(fstringAssign).IsNotNull();
            Check.That(fstringAssign.VariableName).IsEqualTo("fstring");
            Check.That(fstringAssign.Value).IsInstanceOf<BinaryOperation>();
            var fString = fstringAssign.Value as BinaryOperation;
            Check.That(fString.Operator).IsEqualTo(BinaryOperator.CONCAT);
            var interpreter = new Interpreter();
            var context = interpreter.Interprete(result.Result, true);
            Check.That(context.variables).CountIs(4);
            CheckvariableInt(context,"v1", 48);
            CheckvariableInt(context, "v2", 152);
            CheckvariableBool(context,"b", true);
            string expected = "v1 :> 48 < v2 :> 152 < v3 :> 200 <  v4 :>hello, world< v5 :>true< - end";
            CheckvariableString(context,"fstring", expected);
            
            // TODO 
            // var compiler = new IndentedWhileCompiler();
            // var func = compiler.CompileToFunction(program,true);
            // Check.That(func).IsNotNull();
            // Printer.Clear();
            // var f = func();
            // Check.That(Printer.lines).CountIs(1);
            // Check.That(Printer.lines[0]).IsEqualTo(expected);
            
        }

        [Fact]
        public void TestInfiniteWhile()
        {
            var parser = BuildParser();
            var program = @"
# infinite loop
while true do
    skip
";
            var result = parser.Parse(program);
            Check.That(result.IsOk).IsTrue();
            Check.That(result.Result).IsNotNull();
            
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
            Check.That(result.IsOk).IsTrue();
            Check.That(result.Result).IsNotNull();

            Check.That(result.Result).IsInstanceOf<SequenceStatement>();
            var seq = result.Result as SequenceStatement;
            Check.That(seq.Get(0)).IsInstanceOf<PrintStatement>();
            var print = seq.Get(0) as PrintStatement;
            var expr = print.Value;
            Check.That(expr).IsInstanceOf<BinaryOperation>();
            var bin = expr as BinaryOperation;
            Check.That(bin.Operator).IsEqualTo(BinaryOperator.AND);
            Check.That((bin.Left as BoolConstant)?.Value).IsEqualTo(true);
            Check.That((bin.Right as BoolConstant)?.Value).IsEqualTo(false);
        }


        [Fact]
        public void TestSkip()
        {
            var parser = BuildParser();
            var result = parser.Parse("skip");
            Check.That(result.Result).IsNotNull();
            Check.That(result.IsOk).IsTrue();

            Check.That(result.Result).IsInstanceOf<SequenceStatement>();
            var seq = result.Result as SequenceStatement;
            Check.That(seq.Get(0)).IsInstanceOf<SkipStatement>();
        }

        [Fact]
        public void TestSkipAssignSequence()
        {
            var parser = BuildParser();
            var program = @"a:=1
b:=2
c:=3";
            var result = parser.Parse(program);
            Check.That(result.Result).IsNotNull();
            Check.That(result.IsOk).IsTrue();
            Check.That(result.Result).IsInstanceOf<SequenceStatement>();
            var seq = result.Result as SequenceStatement;
            Check.That(seq).CountIs(3);



            (string name, int value)[] values = new []{ ("a",1), ("b",2), ("c",3) };

            Check.That(seq.Cast<AssignStatement>().Extracting(x => (x.VariableName,(x.Value as IntegerConstant).Value))).ContainsExactly(values);
            
        }


        [Fact]
        public void TestSkipSkipSkipSequence()
        {
            var parser = BuildParser();
            var result = parser.Parse(@"
skip
skip
skip");
            Check.That(result.Result).IsNotNull();
            Check.That(result.IsOk).IsTrue();
            Check.That(result.Result).IsInstanceOf<SequenceStatement>();
            var seq = result.Result as SequenceStatement;
            Check.That(seq).CountIs(3);
            Check.That(seq.Get(0)).IsInstanceOf<SkipStatement>();
            Check.That(seq.Get(1)).IsInstanceOf<SkipStatement>();
            Check.That(seq.Get(2)).IsInstanceOf<SkipStatement>();
        }


        [Fact]
        public void TestIndentationError()
        {
            var parser = BuildParser();
            var result = parser.Parse(@"
# infinite loop
while true do
    skip
  skip");
            Check.That(result.IsError).IsTrue();
            Check.That(result.Errors).CountIs(1);
            var error = result.Errors.First();
            Check.That(error.ErrorType).IsEqualTo(ErrorType.IndentationError);
            Check.That(error.Line).IsEqualTo(3);
            Check.That(error.ErrorMessage).Contains("Indentation error");
            
            result = parser.Parse(@"
# infinite loop
while true do
    skip
skip");
            Check.That(result.IsOk).IsTrue();
           // TODO : more tests?
        }
        
        [Fact]
        public void TestEmptyLines()
        {
            var parser = BuildParser();
            var program = @"
# infinite loop
i:= 2

while true do

    skip
";
            var result = parser.Parse(program);
            Check.That(result.IsOk).IsTrue();
            Check.That(result.Result).IsNotNull();
        }
        
        [Fact]
        public void TestMissingUIndents()
        {
            var parser = BuildParser();
            var program = @"
if true then
        if false then
            x := 28";
            var result = parser.Parse(program);
            var root = result.Result;
            Check.That(root).IsInstanceOf<SequenceStatement>();
            var seq = result.Result as SequenceStatement;
            var dump = seq.Dump("  ");
            Check.That(seq.Statements).CountIs(1);
            Check.That(seq.Statements[0]).IsInstanceOf<IfStatement>();
            Check.That(result.IsOk).IsTrue();
            Check.That(result.Result).IsNotNull();
        }
        
        [Fact]
        public void TestIssue413_incompleteProgram()
        {
            var parser = BuildParser();
            var program = @"
if 
";
            var result = parser.Parse(program);
            
            Check.That(result.IsOk).IsFalse();
            Check.That(result.Errors).CountIs(2);
            var error = result.Errors.First();
            Check.That(error.ErrorType).IsEqualTo(ErrorType.UnexpectedEOS);
            var unexpectedEosError = error as UnexpectedTokenSyntaxError<IndentedWhileTokenGeneric>;
            Check.That(unexpectedEosError).IsNotNull();
            Check.That(unexpectedEosError.ExpectedTokens.Extracting(x => x.TokenId)).IsEquivalentTo(
                IndentedWhileTokenGeneric.MINUS,IndentedWhileTokenGeneric.NOT, IndentedWhileTokenGeneric.QUESTION,
                IndentedWhileTokenGeneric.OPEN_PAREN,IndentedWhileTokenGeneric.OPEN_FSTRING,IndentedWhileTokenGeneric.TRUE,
                IndentedWhileTokenGeneric.INT,IndentedWhileTokenGeneric.FALSE,IndentedWhileTokenGeneric.IDENTIFIER);
        }
        
        [Fact]
        public void TestIndentationError_emptyIndentLine()
        {
            
            var parser = BuildParser();
            var lexer = parser.Lexer;
            

            var program = @"
if true then

        if false then
            x := 28";
            var lexed = lexer.Scan(program);
            Check.That(lexed.IsOk).IsTrue();
            var mainTokens = lexed.Tokens;
            Check.That(mainTokens).Not.IsEmpty();
            Check.That(mainTokens.Last().IsEOS).IsTrue();
            var lastToken = mainTokens[mainTokens.Count - 2];
            Check.That(lastToken).IsNotNull();
            Check.That(lastToken.IsUnIndent)
                .IsTrue();
        }
}
