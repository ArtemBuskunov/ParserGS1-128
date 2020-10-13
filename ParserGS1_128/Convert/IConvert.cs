using System;
using System.Collections.Generic;
using System.Text;

namespace ParserGS1.Convert
{
    public interface IConvert
    {
        object SetValue(RuleKeyAI ruleKey, int A4);
        Type TypeValue { get; }
    }
}
