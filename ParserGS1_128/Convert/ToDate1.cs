using System;

namespace ParserGS1.Convert
{
    internal class ToDate1 : IConvert
    {
        public object SetValue(RuleKeyAI ruleKey, int A4)
        {
            string Text = ruleKey.ToStringValue;
            //ГГММДД
            int yy = 0, mm = 0, dd = 0, yyyy = 0;
            Int32.TryParse(Text.Remove(2), out yy);
            Int32.TryParse(Text.Remove(0, 2).Remove(2), out mm);
            Int32.TryParse(Text.Remove(0, 4), out dd);

            if (yy < 0) yyyy = 2000 - yy;
            else if (yy < 51) yyyy = 2000 + yy;
            else yyyy = 1900 + yy;

            return new DateTime(yyyy, mm, dd);
        }
        public Type TypeValue => typeof(DateTime);

    }
}
