using System.Collections.Generic;

namespace ParserGS1
{
    /// <summary>
    /// Объект-результат используемый если другие варианты не подходят
    /// </summary>
    public class DefaultResult : IStrategyResults
    {
        public DefaultResult() { }
        public bool IsTypeResult(Dictionary<string, RuleKeyAI> RuleKeys) => false;
        public string Text { get; set; }

        public int PrecedenceLevel => 0;

        public object CompletingFields(string fullStrings, string[] codeStrings, Dictionary<string, RuleKeyAI> RuleKeys) => this;
        public bool ActionAfterCompletingFields() => true;
    }
}
