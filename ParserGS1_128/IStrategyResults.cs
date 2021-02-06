using System.Collections.Generic;

namespace ParserGS1
{
    /// <summary> Стратегия для объектов-результатов </summary>
    public interface IStrategyResults
    {
        /// <summary> Приоритет выбора объекта для случаев со схожими полями, чем выше число, тем выше приоритет формирования </summary>
        int PrecedenceLevel { get; }
        /// <summary> Проверить на соответствие типу </summary>
        /// <param name="RuleKeys">набор полученных полей из кода</param>
        bool IsTypeResult(Dictionary<string, RuleKeyAI> RuleKeys);

        /// <summary> Заполнить поля объекта</summary>
        /// <param name="fullStrings">Полная строка из кода, без преобразования</param>
        /// <param name="codeStrings">Подстроки </param>
        /// <param name="RuleKeys">набор полученных полей из кода</param>
        object CompletingFields(string fullStrings, string[] codeStrings, Dictionary<string, RuleKeyAI> RuleKeys);

        /// <summary> Действие после заполнения полей, если объект соответствует </summary>
        bool ActionAfterCompletingFields();
    }
}
