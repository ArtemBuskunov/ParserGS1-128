using System;

namespace ParserGS1.Convert
{
    internal class ToDefault : IConvert
    {
        public object SetValue(RuleKeyAI ruleKey, int parament) { return ruleKey.ToStringValue; }
        public Type TypeValue => typeof(string);

    }
}
