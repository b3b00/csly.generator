// See https://aka.ms/new-console-template for more information

using csly.generator.tester.testerparser;

namespace csly.generator.tester;

public class Program {
	public static void Main(string[] args)
	{
		var instance = new TesterParser();
		var parser = new TesterParserMain(instance);
		var result = parser.Parse("A B C");
		if (result != null)
		{
			if (result.IsOk)
			{
				Console.WriteLine("OK");
				Console.WriteLine(result.Result);
			}
			else
			{
				Console.WriteLine("FAIL");
				foreach (var error in result.Errors)
				{
					Console.WriteLine(error.ErrorMessage);
				}
			}
			 
		}
	}
	
}

