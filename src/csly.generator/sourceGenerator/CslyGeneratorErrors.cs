namespace csly.generator.sourceGenerator;

public static class CslyGeneratorErrors
{
    public const string NOT_PARTIAL = "CSLY0000";
    public const string MISSING_INHERITANCE = "CSLY0001";
    public const string LEXER_NOT_FOUND = "CSLY0002";
    public const string PARSER_NOT_FOUND = "CSLY0003";

    

    public const string LEXER_GENERATION_FAILED = "CSLY0004";
    public const string PARSER_GENERATION_FAILED = "CSLY0005";

    public const string NO_PARSER_GENERATOR_FOUND = "CSLY0006";

    public const string LEFT_RECURSION_DETECTED = "CSLY0007";
    
    public const string TOO_MANY_CONTEXT_TYPES = "CSLY0008";
    
    public const string PARAMETER_TYPE_ERROR = "CSLY0009";
}