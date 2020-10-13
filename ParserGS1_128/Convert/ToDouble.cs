using System;
using System.Collections.Generic;
using System.Text;

namespace ParserGS1.Convert
{
    public class ToDouble : IConvert
    {
        public object SetValue(RuleKeyAI ruleKey, int A4)
        {
            string Text = ruleKey.ToStringValue;
            if (A4 > 0 && Text.Length > 0)
            {
                int count = Text.Length + 1;
                int i = Text.Length - A4;
                Text = Text.Remove(i) + "," + Text.Remove(0, i);
            }
            double d = 0;
            Double.TryParse(Text, out d);
            return d;
        }
        public Type TypeValue => typeof(Double);

    }

}
