namespace ParserGS1.StrategyKeyAI
{
    /// <summary> Обработчик строк для тегов с переменой длиной данных </summary>
    internal class VariableLengthStrategy : TextParsingStrategy
    {
        /// <summary> Стратегия обработчик строк для тегов с переменой длиной данных </summary>
        /// <param name="FieldLength">Фиксирования длина данных с тегом, имеет значение при использовании FixedLength</param>
        /// <param name="keyAI">идентификатор применения</param>
        public VariableLengthStrategy(int FieldLength, string KeyAI) : base(FieldLength, KeyAI) { }
        /// <summary>
        /// Стратегия обработчик строк для тегов с переменой длиной данных
        /// </summary>
        /// <param name="_KeyLength_Whole">Полная длина идентификатора применения</param>
        /// <param name="FieldLength">Фиксирования длина данных с тегом, имеет значение при использовании FixedLength</param>
        /// <param name="keyAI">идентификатор применения</param>
        public VariableLengthStrategy(int _KeyLength_Whole, int FieldLength, string KeyAI) : base(_KeyLength_Whole, FieldLength, KeyAI) { }
        protected override bool getValue(string Text)
        {
            Text = Text.Remove(0, KeyLength_Whole);
            int length =
             ParserGS1.Parser.GroutSeperatorUser == "" ? Text.IndexOf((char)29)
             : Text.IndexOf(ParserGS1.Parser.GroutSeperatorUser);

            if (length == -1)
            {
                ResultText = Text;
                TextAfterProcessing = "";
            }
            else
            {
                ResultText = Text.Remove(length);
                TextAfterProcessing = ParserGS1.Parser.GroutSeperatorUser == "" ? Text.Remove(0, length + 1)
                     : Text.Remove(0, length + ParserGS1.Parser.GroutSeperatorUser.Length);

            }
            return true;
        }
    }

}
