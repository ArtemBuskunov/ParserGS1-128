using System;
using System.Collections.Generic;
using System.Text;

namespace ParserGS1.Convert
{
    public class ToDefault : IConvert
    {
        public object SetValue(RuleKeyAI ruleKey, int parament) { return ruleKey.ToStringValue; }
        public Type TypeValue => typeof(string);

    }
}
