using ParserGS1.Convert;
using ParserGS1.StrategyKeyAI;
using System;
using System.Collections.Generic;

namespace ParserGS1
{
    /// <summary> Класс тега</summary>
    public class RuleKeyAI
    {
        public RuleKeyAI(string _KeyAI, string DescriptionEn, string DescriptionRu, string Title)
            : this(_KeyAI, 0, typeof(VariableLengthStrategy), new ToDefault(), DescriptionEn, DescriptionRu, Title) { }
        public RuleKeyAI(string _KeyAI, int _fieldLength, string DescriptionEn, string DescriptionRu, string Title)
            : this(_KeyAI, _fieldLength, typeof(FixedLengthStrategy), new ToDefault(), DescriptionEn, DescriptionRu, Title) { }
        public RuleKeyAI(string _KeyAI, int _fieldLength, IConvert _Convert, string DescriptionEn, string DescriptionRu, string Title)
            : this(_KeyAI, _fieldLength, typeof(FixedLengthStrategy), _Convert, DescriptionEn, DescriptionRu, Title) { }
        public RuleKeyAI(int _KeyLength_Whole, string _KeyAI, IConvert _Convert, string DescriptionEn, string DescriptionRu, string Title)
            : this(_KeyLength_Whole, _KeyAI, 0, typeof(VariableLengthStrategy), _Convert, DescriptionEn, DescriptionRu, Title) { }
        public RuleKeyAI(int _KeyLength_Whole, string _KeyAI, int _fieldLength, IConvert _Convert, string DescriptionEn, string DescriptionRu,
            string Title)
            : this(_KeyLength_Whole, _KeyAI, _fieldLength, typeof(FixedLengthStrategy), _Convert, DescriptionEn, DescriptionRu, Title) { }

        protected RuleKeyAI(string _KeyAI, int _fieldLength, Type _typeTextParsingStrategy, IConvert _Convert, string _DescriptionEn, string _DescriptionRu, string _Title)
        {
            DescriptionEn = _DescriptionEn;
            DescriptionRu = _DescriptionRu;
            Title = _Title;
            iConvert = _Convert;
            KeyAI = _KeyAI;
            ParsingStrategy = (TextParsingStrategy)Activator.CreateInstance(_typeTextParsingStrategy, new object[] { _fieldLength, _KeyAI });
        }
        protected RuleKeyAI(int _KeyLength_Whole, string _KeyAI, int _fieldLength, Type _typeTextParsingStrategy, IConvert _Convert, string _DescriptionEn, string _DescriptionRu, string _Title)
        {
            DescriptionEn = _DescriptionEn;
            DescriptionRu = _DescriptionRu;
            Title = _Title;
            iConvert = _Convert;
            KeyAI = _KeyAI;
            ParsingStrategy = (TextParsingStrategy)Activator.CreateInstance(_typeTextParsingStrategy, new object[] { _KeyLength_Whole, _fieldLength, _KeyAI });
        }


        private protected IConvert iConvert;
        private protected TextParsingStrategy ParsingStrategy;
        
        /// <summary> Строка после удаления данных относящихся к тегу </summary>
        internal string TextAfterProcessing => ParsingStrategy == null ? "" : ParsingStrategy.TextAfterProcessing;

        /// <summary> Значение тега </summary>
        public object Value => iConvert.SetValue(this, ParsingStrategy.A4);
        /// <summary> Значение тега в типе string </summary>
        public string ToStringValue { get; set; }


        /// <summary> идентификаторы применения из словарь Code-128 </summary>
        public string KeyAI { get; protected set; }
        /// <summary> Описание идентификаторы  </summary>
        public string DescriptionEn { get; protected set; }
        /// <summary> Описание идентификаторы  </summary>
        public string DescriptionRu { get; protected set; }
        /// <summary> Название идентификаторы  </summary>
        public string Title { get; protected set; }
        /// <summary> Тип возвращаемого значения  </summary>
        public Type TypeValue => iConvert.TypeValue;

        public override string ToString() => $"AI {KeyAI}: {iConvert.SetValue(this, ParsingStrategy.A4).ToString()} ({DescriptionRu})";

        /// <summary> Выполнить проверки и выделение значение для тега   </summary>
        /// <param name="Text">Текст для проверки и обработки</param>
        /// <param name="HandlerFields">Словарь обнаруженных тегов и значений</param>
        public virtual void Set(string Text, ref Dictionary<string, RuleKeyAI> HandlerFields)
        {
           
            ParsingStrategy.GetValue(Text);
            if (ParsingStrategy.IsKey)
            {
                ToStringValue = ParsingStrategy.ResultText;
                HandlerFields.Add(KeyAI, this);
            }
        }
    }
}
