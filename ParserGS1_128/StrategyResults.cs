using System.Collections.Generic;

namespace ParserGS1
{
    public abstract class StrategyResults : IStrategyResults
    {
        public StrategyResults()
        {
            PrecedenceLevel = KeysAI.Length;
        }
        /// <summary>
        /// Набор обязательных полей в объекте
        /// </summary>
        public virtual string[] KeysAI { get; }
        public int PrecedenceLevel { get; private set; }

        public virtual bool ActionAfterCompletingFields() { return true; }

        public virtual object CompletingFields(string fullStrings, string[] codeStrings, Dictionary<string, RuleKeyAI> RuleKeys)
        { return null; }

        public virtual bool IsTypeResult(Dictionary<string, RuleKeyAI> RuleKeys)
        {
            bool i = true;
            foreach (string tag in KeysAI)
                i = i && RuleKeys.ContainsKey(tag);

            return i;
        }
    }
}
