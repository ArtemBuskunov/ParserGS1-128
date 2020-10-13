namespace ParserGS1.StrategyKeyAI
{
    /// <summary>
    /// Обработчик строк для тегов с постоянной длиной данных
    /// </summary>
    internal class FixedLengthStrategy : TextParsingStrategy
    {
        /// <summary>
        /// Стратегия обработчик строк для тегов с постоянной длиной данных
        /// </summary>
        /// <param name="FieldLength">Фиксирования длина данных с тегом, имеет значение при использовании FixedLength</param>
        /// <param name="keyAI">идентификатор применения</param>
        public FixedLengthStrategy(int FieldLength, string KeyAI) : base(FieldLength, KeyAI) { }

        /// <summary>
        /// Стратегия обработчик строк для тегов с постоянной длиной данных
        /// </summary>
        /// <param name="_KeyLength_Whole">Полная длина идентификатора применения</param>
        /// <param name="FieldLength">Фиксирования длина данных с тегом, имеет значение при использовании FixedLength</param>
        /// <param name="keyAI">идентификатор применения</param>
        public FixedLengthStrategy(int _KeyLength_Whole, int FieldLength, string KeyAI) : base(_KeyLength_Whole, FieldLength, KeyAI) { }


        protected override bool getValue(string StringWithGS)
        {
            StringWithGS = StringWithGS.Remove(0, KeyLength_Whole);
            if (StringWithGS.Length > FieldLength)
            {
                ResultText = StringWithGS.Remove(FieldLength);
                TextAfterProcessing = StringWithGS.Remove(0, FieldLength);
            }
            else if (StringWithGS.Length == FieldLength)
            {
                ResultText = StringWithGS;
                TextAfterProcessing = "";
            }
            else
            {
                TextAfterProcessing = StringWithGS;
                IsKey = false;
                return false;
            }
            return true;
        }
    }
}
