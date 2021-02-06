using System;

namespace ParserGS1.Convert
{
    public interface IConvert
    {
        object SetValue(RuleKeyAI ruleKey, int A4);
        Type TypeValue { get; }
    }
}
