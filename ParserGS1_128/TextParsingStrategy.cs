using System;

namespace ParserGS1
{

    /// <summary> Абстрактный класс стратегии обработки строки тега:
    /// <para>-	Проверка соответствия тегу</para>
    /// <para>-	Выделения значения, относящегося к тегу</para>
    /// <para>-	Удаления данных из строки относящихся к тегу</para>
    /// </summary>
    internal abstract class TextParsingStrategy
    {
        protected string KeyAI;
        protected int fieldLength;

        /// <summary> Стратегия обработки строки для тега </summary>
        /// <param name="_fieldLength">Фиксирования длина данных с тегом, имеет значение при использовании FixedLength</param>
        /// <param name="_keyAI">идентификатор применения</param>
        public TextParsingStrategy(int _fieldLength, string _keyAI)
        {
            KeyAI = _keyAI;
            KeyLength_Wanted = KeyAI.Length;
            KeyLength_Whole = KeyAI.Length;
            fieldLength = _fieldLength;
        }
        /// <summary> Стратегия обработки строки для тега </summary>
        /// <param name="_KeyLength_Whole">Полная длина идентификатора применения</param>
        /// <param name="_fieldLength">Фиксирования длина данных с тегом, имеет значение при использовании FixedLength</param>
        /// <param name="_keyAI">идентификатор применения</param>
        public TextParsingStrategy(int _KeyLength_Whole, int _fieldLength, string _keyAI)
        {
            KeyAI = _keyAI;
            KeyLength_Wanted = KeyAI.Length;
            KeyLength_Whole = _KeyLength_Whole;
            fieldLength = _fieldLength;
        }



        /// <summary> Длина искомой части идентификатора применения </summary>
        public int KeyLength_Wanted { get; private set; }
        /// <summary> Полная длина идентификатора применения </summary>
        public int KeyLength_Whole { get; private set; }
        public int A4 { get; private set; }

        /// <summary> Длина значения в строке для тега, имеет значение при использовании FixedLength </summary>
        public int FieldLength { get; private set; }
        /// <summary> Значение тега в виде строки </summary>
        public string ResultText { get; set; }
        /// <summary> Строка после удаления данных относящихся к тегу </summary>
        public string TextAfterProcessing { get; set; }
        /// <summary>  Тег обнаружен </summary>
        public bool IsKey { get; set; }


        /// <summary> Получаем значение из строки </summary>
        /// <param name="Text">Строка из кода для обработки</param>
        public virtual void GetValue(string Text)
        {
            TextAfterProcessing = Text;
            ResultText = "";
            string key = Text.Length > KeyLength_Whole ? Text.Remove(KeyLength_Wanted) : "";
            IsKey = KeyAI == key;
            if (IsKey)
            {
                int i = 0, Delta = KeyLength_Whole - KeyLength_Wanted;
                if (Delta > 0)
                {
                    string st = Text.Remove(KeyLength_Wanted + Delta).Remove(0, KeyLength_Wanted);
                    Int32.TryParse(st, out i);
                }
                A4 = i;
                FieldLength = fieldLength;
                getValue(Text);
            }
        }
        protected virtual bool getValue(string Text) { return false; }
    }
}
