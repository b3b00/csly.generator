
using json;
using json.jsonparser;

public class Program
{
    public static void Main(string[] args)
    {
        string j = Console.ReadLine();
        var parser = new JsonParserMain(new JsonParser(), useMemoization: true);
        var r = parser.Parse(j);
        if (r.IsError)
        {
            foreach (var error in r.Errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            
        }
        else
        {
            Console.WriteLine("ok");
        }
    }
}


