using System.Collections.Generic;

namespace ParserGS1
{
    /// <summary>
    /// Фабрика результатов сканирования, подбирает объект и инициализирует его по полученному словарю тегов
    /// </summary>
    internal class ResultFactory
    {
        /// <summary>
        /// Фабрика результатов сканирования, подбирает объект и инициализирует его по полученному словарю тегов
        /// </summary>
        /// <param name="StrategyResults">Массив стратегий для объектов-результатов</param>
        /// <param name="defaultParseType">Объект-результат используемый если другие варианты не подходят</param>
        public ResultFactory(IStrategyResults[] StrategyResults, DefaultResult defaultParseType)
        {
            IStrategyResults = StrategyResults;
            DefaultParseType = defaultParseType;
            CurrentParseType = null;
        }
        private IStrategyResults CurrentParseType;
        private DefaultResult DefaultParseType;
        private IStrategyResults[] IStrategyResults;


        public bool Create(string fullStrings, string[] codeStrings, Dictionary<string, RuleKeyAI> RuleKeys)
        {
            CurrentParseType = null;
            foreach (var ParseType in IStrategyResults)
            {
                if (ParseType.IsTypeResult(RuleKeys))
                {
                    CurrentParseType = ParseType;
                    CurrentParseType.CompletingFields(fullStrings, codeStrings, RuleKeys);
                }
            }

            if (CurrentParseType == null)
            {
                string text = string.IsNullOrWhiteSpace(fullStrings)?"": fullStrings + "\n";
                foreach (var aHandler in RuleKeys)
                { text = text + aHandler.Value.ToString() + "\n"; }
                DefaultParseType.Text = text;
                CurrentParseType = DefaultParseType;
            }
            else
            {
                CurrentParseType.ActionAfterCompletingFields();
            }

            return true;
        }

        /// <summary>
        /// Полученный объект-результат после обработки
        /// </summary>
        public IStrategyResults Result => CurrentParseType;
    }

}
