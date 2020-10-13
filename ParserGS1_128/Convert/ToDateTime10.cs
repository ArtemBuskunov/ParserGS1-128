using System;
using System.Collections.Generic;
using System.Text;

namespace ParserGS1.Convert
{
    public class ToDateTime10 : IConvert
    {
        public object SetValue(RuleKeyAI ruleKey, int A4)
        {
            string Text = ruleKey.ToStringValue;
            //ГГММДД
            int yy = 0, mm = 0, dd = 0, yyyy = 0, MM = 0, HH = 0;
            Int32.TryParse(Text.Remove(2), out yy);
            Int32.TryParse(Text.Remove(0, 2).Remove(2), out mm);
            Int32.TryParse(Text.Remove(0, 4).Remove(2), out dd);
            Int32.TryParse(Text.Remove(0, 6).Remove(2), out HH);
            Int32.TryParse(Text.Remove(0, 8), out MM);
            yyyy = 2000 + yy;
            return new DateTime(yyyy, mm, dd, HH, MM, 00);
        }
        public Type TypeValue => typeof(DateTime);

    }

}
