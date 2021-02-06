using ParserGS1.Convert;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParserGS1
{
    /// <summary> Парсер строки, советующей требованиям GS1</summary>
    public class Parser : INotifyPropertyChanged
    {
        private static char groutSeperator = (char)29;
        private static char startCodeC = (char)232;
        private static string groutSeperatorUser = "";
        private static string startCodeUser = "]C1";

        private RuleKeyAI[] ruleTags;
        private string[] ItemsCodeWithoutGS;
        private Dictionary<string, RuleKeyAI> AIFields;
        private ResultFactory ResultFactory;
        public event PropertyChangedEventHandler PropertyChanged;

        public RuleKeyAI this[string key] => AIFields[key];


        /// <summary> Парсер строки, советующей требованиям GS1 </summary>
        public Parser() : this(new IStrategyResults[] { }, new DefaultResult()) { }
        /// <summary> Парсер строки, советующей требованиям GS1 </summary>
        /// <param name="StrategyResults">Массив стратегий для объектов-результатов</param>
        public Parser(IStrategyResults[] StrategyResults) : this(StrategyResults, new DefaultResult()) { }
        /// <summary> Парсер строки, советующей требованиям GS1 </summary>
        /// <param name="StrategyResults">Массив стратегий для объектов-результатов</param>
        /// <param name="defaultParseType">Объект-результат используемый если другие варианты не подходят</param>
        public Parser(IStrategyResults[] StrategyResults, DefaultResult defaultParseType)
        {
            setRuleTags();
            IStrategyResults[] StrategyResults2 = StrategyResults.ToList().OrderBy(p => p.PrecedenceLevel).ToArray();
            ResultFactory = new ResultFactory(StrategyResults2, defaultParseType);
        }


        /// <summary> Объект-результат после парсинга </summary>
        public IStrategyResults Result { get { return ResultFactory.Result; } }
        /// <summary> Полученые значения </summary>
        public Dictionary<string, RuleKeyAI> ValuesAI => AIFields;
        /// <summary> идентификаторы применения, по которым проводится проверка при анализе строки </summary>
        public RuleKeyAI[] RuleTags
        {
            get { return ruleTags; }
            set { ruleTags = value.ToList().OrderByDescending(p => p.KeyAI.Length).ToArray(); }
        }

        public static string GroutSeperatorUser => groutSeperatorUser;

        /// <summary> Установить строку для анализа и разбора на поля </summary>
        /// <param name="CodeWithGS">Строка, удовлетворяющая требованиям GS1 </param>
        public void SetText(string CodeWithGS)
        {
            string first = CodeWithGS;
            ItemsCodeWithoutGS = CodeWithGS.Split(new char[] { groutSeperator });

            if (CodeWithGS.Remove(startCodeUser.Length) == startCodeUser) CodeWithGS = CodeWithGS.Remove(0, startCodeUser.Length);
            else if (CodeWithGS[0] == startCodeC) CodeWithGS = CodeWithGS.Remove(0, 1);
            else if (CodeWithGS[0] == groutSeperator) CodeWithGS = CodeWithGS.Remove(0, 1);
            else if (CodeWithGS.Remove(GroutSeperatorUser.Length) == GroutSeperatorUser) CodeWithGS = CodeWithGS.Remove(0, GroutSeperatorUser.Length);



            int length, length2 = CodeWithGS.Length;
            AIFields = new Dictionary<string, RuleKeyAI>();
            do
            {
                length = length2;
                foreach (RuleKeyAI HandlerField in RuleTags)
                {
                    HandlerField.Set(CodeWithGS, ref AIFields);
                    CodeWithGS = HandlerField.TextAfterProcessing;
                }
                length2 = CodeWithGS.Length;
            } while (CodeWithGS.Length > 0 && length > length2);
            ResultFactory.Create(first, ItemsCodeWithoutGS, AIFields);

        }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        { if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        /// <summary> Установить свои символы начального (вместо FNC1) и разделительного (вместо GS) </summary>
        /// <param name="_GroutSeperatorUser">Разделительный символ вместо  GS</param>
        /// <param name="_StartCodeUser">Начальный символ вместо FNC1</param>
        public static void SetGroutSeperatorUser(string _GroutSeperatorUser, string _StartCodeUser)
        {
            Parser.groutSeperatorUser = _GroutSeperatorUser;
            Parser.startCodeUser = string.IsNullOrWhiteSpace(_StartCodeUser) ?"]C1":_StartCodeUser;
        }
        private void setRuleTags()
        {
            RuleTags = new RuleKeyAI[]
            {
            new RuleKeyAI("8200","Url","Url","Url"),
            new RuleKeyAI("00",18,"Identification of a logistic unit","Серийный грузовой контейнерный код","SSCC"),
            new RuleKeyAI("01",14,"Identification of a trade item","Идентификационный номер единицы товара","GTIN"),
            new RuleKeyAI("02",14,"Identification of trade items contained in a logistic unit","GTIN торговых единиц, содержащихся в грузе","CONTENT"),
            new RuleKeyAI("10","Batch or lot number","Номер лота (партии, группы, пакета)","BATCH/LOT"),
            new RuleKeyAI("11",6, new ToDate1(),"Production date","Дата выработки","PROD DATE"),
            new RuleKeyAI("12",6, new ToDate1(),"Due date for amount on payment slip","Дата платежа","DUE DATE"),
            new RuleKeyAI("13",6, new ToDate1(),"Packaging date","Дата упаковки","PACK DATE"),
            new RuleKeyAI("15",6, new ToDate1(),"Best before date","Минимальный срок годности","BEST BEFORE or SELL BY"),
            new RuleKeyAI("17",6, new ToDate1(),"Expiration date","Максимальный срок годности","USE BY / EXPIRY"),
           
            //------------------------------------------------------------------
            new RuleKeyAI("20",2,"Internal product variant","Разновидность продукта","VARIANT"),
            new RuleKeyAI("21","Serial number","Серийный номер","SERIAL"),
            new RuleKeyAI("22","Consumer product variant","Вспомогательные данные специальных фармацевтических продуктов","QTY/DATE/BATCH"),
            new RuleKeyAI(3, "23", new ToDouble(),"", "Номер лота (переходный)", "BATCH/LOT"),
            new RuleKeyAI("240","Additional product identification assigned by the manufacturer","Дополнительная идентификация продукта, присваиваемая производителем","ADDITIONAL ID"),
            new RuleKeyAI("241","Customer part number","Номер покупателя","CUST.PART №"),
            new RuleKeyAI("242","Made-to-Order variation number","",""),
            new RuleKeyAI("243","Packaging component number","",""),
            new RuleKeyAI("250","Secondary serial number","Дополнительный серийный номер","SECONDARY SERIAL"),
            new RuleKeyAI("251","Reference to source entity","Ссылка на источник","REF TO SOURCE"),
            new RuleKeyAI("253","Global Document Type Identifier","",""),
            new RuleKeyAI("254","GLN extension component","",""),
            new RuleKeyAI("255","Global Coupon Number","",""),

            //------------------------------------------------------------------
            new RuleKeyAI("30","Variable count of items","Переменное количество","VAR.COUNT"),
            new RuleKeyAI(4, "310",6, new ToDouble(),"Net weight, Kilograms","Вес нетто в килограммах",""),
            new RuleKeyAI(4, "311",6, new ToDouble(),"Length or first dimension, Metres","Длина/1-е измерение в метрах",""),
            new RuleKeyAI(4, "312",6, new ToDouble(),"Width, diameter, or second dimension,  Metres","Ширина/Диаметр/2-е измерение в метрах ",""),
            new RuleKeyAI(4, "313",6, new ToDouble(),"Depth, thickness, height, or third dimension, Metres","Глубина/Толщина/Высота/3-е измерение в метрах",""),
            new RuleKeyAI(4, "314",6, new ToDouble(),"Area, Square metres","Площадь в квадратных метрах",""),
            new RuleKeyAI(4, "315",6, new ToDouble(),"Net volume, Litres","Объем нетто в литрах",""),
            new RuleKeyAI(4, "316",6, new ToDouble(),"Net volume, Cubic metres","Объем нетто в кубических метрах",""),
            new RuleKeyAI(4, "320",6, new ToDouble(),"Net weight, Pounds","Вес нетто в фунтах",""),
            new RuleKeyAI(4, "321",6, new ToDouble(),"Length or first dimension, Inches","Длина/1-е измерение в дюймах",""),
            new RuleKeyAI(4, "322",6, new ToDouble(),"Length or first dimension, Feet","Длина/1-е измерение в футах ",""),
            new RuleKeyAI(4, "323",6, new ToDouble(),"Length or first dimension, Yards","Длина/1-е измерение в ярдах",""),
            new RuleKeyAI(4, "324",6, new ToDouble(),"Width, diameter, or second dimension, Inches","Ширина/Диаметр/2-е измерение в дюймах",""),
            new RuleKeyAI(4, "325",6, new ToDouble(),"Width, diameter, or second dimension, Feet","Ширина/Диаметр/2-е измерение в футах",""),
            new RuleKeyAI(4, "326",6, new ToDouble(),"Width, diameter, or second dimension, Yards","Ширина/Диаметр/2-е измерение в ярдах",""),
            new RuleKeyAI(4, "327",6, new ToDouble(),"Depth, thickness, height, or third dimension, Inches","Глубина/Толщина/Высота/3-е измерение в дюймах",""),
            new RuleKeyAI(4, "328",6, new ToDouble(),"Depth, thickness, height, or third dimension, Feet","Глубина/Толщина/Высота/3-е измерение в футах",""),
            new RuleKeyAI(4, "329",6, new ToDouble(),"Depth, thickness, height, or third dimension, Yards","Глубина/Толщина/Высота/3-е измерение в ярдах",""),
            
            //------------------------------------------------------------------
            new RuleKeyAI(4, "330",6, new ToDouble(),"Logistic weight, Kilograms","Вес брутто контейнера в килограммах",""),
            new RuleKeyAI(4, "331",6, new ToDouble(),"Length or first dimension, Metres","Длина/1-е измерение контейнера в метрах",""),
            new RuleKeyAI(4, "332",6, new ToDouble(),"Width, diameter, or second dimension, Metres","Ширина/Диаметр/2-е измерение контейнера в метрах",""),
            new RuleKeyAI(4, "333",6, new ToDouble(),"Depth, thickness, height, or third dimension, Metres","Глубина/Толщина/Высота/3-е измерение контейнера в метрах ",""),
            new RuleKeyAI(4, "334",6, new ToDouble(),"Area Square, metres","Площадь контейнера",""),
            new RuleKeyAI(4, "335",6, new ToDouble(),"Logistic volume, Litres","Объем брутто контейнера в литрах",""),
            new RuleKeyAI(4, "336",6, new ToDouble(),"Logistic volume Cubic, metres","Объем брутто контейнера в кубических метрах",""),
            
            //------------------------------------------------------------------
            new RuleKeyAI(4, "340",6, new ToDouble(),"Logistic weight Pounds","Вес брутто контейнера в фунтах",""),
            new RuleKeyAI(4, "341",6, new ToDouble(),"Length or first dimension Inches","Длина/1-е измерение контейнера в дюймах",""),
            new RuleKeyAI(4, "342",6, new ToDouble(),"Length or first dimension Feet","Длина/1-е измерение контейнера в футах",""),
            new RuleKeyAI(4, "343",6, new ToDouble(),"Length or first dimension Yards","Длина/1-е измерение контейнера в ярдах ",""),
            new RuleKeyAI(4, "344",6, new ToDouble(),"Width, diameter, or second dimension Inches","Ширина/Диаметр/2-е измерение контейнера в дюймах",""),
            new RuleKeyAI(4, "345",6, new ToDouble(),"Width, diameter, or second dimension Feet","Ширина/Диаметр/2-е измерение контейнера в футах",""),
            new RuleKeyAI(4, "346",6, new ToDouble(),"Width, diameter, or second dimension Yards","Ширина/Диаметр/2-е измерение контейнера в ярдах",""),
            new RuleKeyAI(4, "347",6, new ToDouble(),"Depth, thickness, height, or third dimension Inches","Глубина/Толщина/Высота/3-е измерение контейнера в дюймах",""),
            new RuleKeyAI(4, "348",6, new ToDouble(),"Depth, thickness, height, or third dimension Feet","Глубина/Толщина/Высота/3-е измерение контейнера в футах",""),
            new RuleKeyAI(4, "349",6, new ToDouble(),"Depth, thickness, height, or third dimension Yards","Глубина/Толщина/Высота/3-е измерение контейнера в ярдах",""),
            
            //------------------------------------------------------------------
            new RuleKeyAI(4, "350",6, new ToDouble(),"Area, Square inches","Площадь в квадратных дюймах",""),
            new RuleKeyAI(4, "351",6, new ToDouble(),"Area, Square feet","Площадь в квадратных футах",""),
            new RuleKeyAI(4, "352",6, new ToDouble(),"Area, Square yards","Площадь в квадратных ярдах",""),
            new RuleKeyAI(4, "353",6, new ToDouble(),"Area Square inches","Площадь контейнера в квадратных дюймах",""),
            new RuleKeyAI(4, "354",6, new ToDouble(),"Area Square feet","Площадь контейнера в квадратных футах ",""),
            new RuleKeyAI(4, "355",6, new ToDouble(),"Area Square yards","Площадь контейнера в квадратных ярдах",""),
            new RuleKeyAI(4, "356",6, new ToDouble(),"Net weight Troy ounces","Вес нетто в тройских унциях",""),
            new RuleKeyAI(4, "357",6, new ToDouble(),"Net weight (or volume) Ounces","Вес/объем в унциях",""),
            
            //------------------------------------------------------------------
            new RuleKeyAI(4, "360",6, new ToDouble(),"Net volume, Quarts","Объем в квартах",""),
            new RuleKeyAI(4, "361",6, new ToDouble(),"Net volume, Gallons (U.S.)","Объем в галлонах",""),
            new RuleKeyAI(4, "362",6, new ToDouble(),"Logistic volume Quarts","Объем брутто контейнера в квартах",""),
            new RuleKeyAI(4, "363",6, new ToDouble(),"Logistic volume Gallons (U.S.)","Объем брутто контейнера в американских галлонах",""),
            new RuleKeyAI(4, "364",6, new ToDouble(),"Net volume, Cubic inches","Объем в кубических дюймах",""),
            new RuleKeyAI(4, "365",6, new ToDouble(),"Net volume, Cubic feet","Объем в кубических футах",""),
            new RuleKeyAI(4, "366",6, new ToDouble(),"Net volume, Cubic yards","Объем в кубических ярдах",""),
            new RuleKeyAI(4, "367",6, new ToDouble(),"Logistic volume Cubic inches","Объем брутто контейнера в кубических дюймах",""),
            new RuleKeyAI(4, "368",6, new ToDouble(),"Logistic volume Cubic feet","Объем брутто контейнера в кубических футах",""),
            new RuleKeyAI(4, "369",6, new ToDouble(),"Logistic volume Cubic yards","Объем брутто контейнера в кубических ярдах",""),
            
            //------------------------------------------------------------------
            new RuleKeyAI("37","","Количество содержащихся торговых единиц",""),
            new RuleKeyAI(4, "390", new ToDouble(),"Amount payable or coupon value","Сумма к оплате в местной валюте",""),
            new RuleKeyAI(4, "391", new ToDouble(),"Amount payable and ISO currency code","Сумма к оплате с кодом валюты ISO",""),
            new RuleKeyAI(4, "392", new ToDouble(),"","Сумма к оплате за одно место в местной валюте",""),
            new RuleKeyAI(4, "393", new ToDouble(),"Amount payable for a variable measure trade item and ISO currency code","Сумма к оплате за одно место с кодом валюты ISO","PRICE"),
            new RuleKeyAI(4, "394", new ToDouble(),"Percentage discount of a coupon","",""),
                        
            //------------------------------------------------------------------
            new RuleKeyAI("400","Customer’s purchase order number","Номер заказа клиента",""),
            new RuleKeyAI("401","Global Identification Number for Consignment","Глобальный идентификатор номера груза ","GINC"),
            new RuleKeyAI("402",17,"Global Shipment Identification Number","Глобальный идентификационный номер отправления","GSIN"),
            new RuleKeyAI("403","Routing code","Код маршрута",""),
            new RuleKeyAI("410",13,"Ship to - Deliver to Global Location Number","Отгрузить в/доставить до местонахождения",""),
            new RuleKeyAI("411",13,"Bill to - Invoice to Global Location Number","Код местонахождения для выставления счета/инвойса",""),
            new RuleKeyAI("412",13,"Purchased from Global Location Number","Код местонахождения совершения покупки",""),
            new RuleKeyAI("413",13,"Ship for - Deliver for - Forward to Global Location Number","Отгрузить из/Доставить из/Переслать в местонахождение",""),
            new RuleKeyAI("414",13,"Identification of a physical location - Global Location Number","Идентификатор физического местонахождения",""),
            new RuleKeyAI("415",13,"Global Location Number of the invoicing party","",""),
            new RuleKeyAI("416",13,"GLN of the production or service location","",""),
            new RuleKeyAI("417",13,"Party GLN","",""),
           
            //------------------------------------------------------------------            
            new RuleKeyAI("420","Ship to - Deliver to postal code within a single postal authority","Отгрузить в/доставить до почтового индекса (в рамках одного почтового оператора)",""),
            new RuleKeyAI("421","Ship to - Deliver to postal code with three-digit ISO country code","Отгрузить в/доставить до почтового индекса (код страны по ISO) ",""),
            new RuleKeyAI("422",3,"Country of origin of a trade item","Страна происхождения (код страны по ISO) ",""),
            new RuleKeyAI("423","Country of initial processing","Страна или страны начала обработки",""),
            new RuleKeyAI("424",3,"Country of processing","Страна обработки",""),
            new RuleKeyAI("425",15,"Country of disassembly","Страна разборки",""),
            new RuleKeyAI("426",3,"Country covering full process chain","Страна полного цикла обработки",""),
            new RuleKeyAI("427","Country subdivision of origin code for a trade item","",""),
            new RuleKeyAI("7001",13,"NATO Stock Number","Инвентаризационный номер NATO",""),
            new RuleKeyAI("7002","UN/ECE meat carcasses and cuts classification","UN-/ECE-классификация туш и отрубов",""),
            new RuleKeyAI("7003",10,"Expiration date and time","Дата и время истечения строка годности",""),
            new RuleKeyAI("7004","Active potency","Потенциал действия",""),

            //------------------------------------------------------------------            
            new RuleKeyAI("8001",14,"Roll products - width, length, core diameter, direction, splices","Характеристики рулона: ширина/длина/диаметр сердечника/направление/сращения ",""),
            new RuleKeyAI("8002","Cellular mobile telephone identifier:","Идентификатор сотового телефона",""),
            new RuleKeyAI("8003","Global Returnable Asset Identifier (GRAI)","Глобальный номер оборотной тары",""),
            new RuleKeyAI("8004","Global Individual Asset Identifier (GIAI)","Глобальный номер индивидуального имущества",""),
            new RuleKeyAI("8005",6,"Price per unit of measure","Цена единицы измерения товара",""),
            new RuleKeyAI("8006",18,"Identification of an individual trade item piece","Идентификация компонент торговой единицы",""),
            new RuleKeyAI("8007","International Bank Account Number (IBAN)","Международный номер банковского счета",""),
            new RuleKeyAI("8008",12, new ToDateTime121(),"Date and time of production","Дата и время производства",""),
            new RuleKeyAI("8018",18,"Global Service Relation Number (GSRN)","Глобальный номер для услуг",""),
            new RuleKeyAI("8020","Service Relation Instance Number (SRIN)","Ссылочный номер платежного требования",""),
            new RuleKeyAI("8100",6,"","Расширенный код купона – NSC + код предложения",""),
            new RuleKeyAI("8101",10,"","Расширенный код купона – NSC + код предложения + конец кода предложения",""),
            new RuleKeyAI("8102",2,"","Расширенный код купона – NSC",""),
            new RuleKeyAI("8110","Coupon code identification for use in North America","",""),
            new RuleKeyAI("8200","Extended packaging URL","",""),
            new RuleKeyAI("90","Information mutually agreed between trading partners","Информация по согласованию между торговыми партнерами (включая FACT Dls)",""),
            
            //------------------------------------------------------------------            
            new RuleKeyAI("91","Key Check","Ключ проверки","Key Check"),
            new RuleKeyAI("92","Code Check","Код проверки,","Code Check"),
            new RuleKeyAI("93","","",""),
            new RuleKeyAI("94","","",""),
            new RuleKeyAI("95","","",""),
            new RuleKeyAI("96","","",""),
            new RuleKeyAI("97","","",""),
            new RuleKeyAI("98","","",""),
            new RuleKeyAI("99","","","")
            };
        }
    }
}