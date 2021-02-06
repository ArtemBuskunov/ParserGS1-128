# Parser GS1-128

Библиотека для разбора строки, полученной со сканера, в соответствии с словарем Code-128 (GS1-128).

Библиотека была создана для сканера обозначенных маркировок шин

### Использование:

1. Получение полей AI из строки с GS = ASCII 29 и FNC1 = ASCII 232
```C#
        ParserGS1.Parser parser = new ParserGS1.Parser();
        void scan(string text, object obj)
        {
            parser.SetText(text);
            (obj as I01).GTIN = (string)parser["01"].Value;
            (obj as I21).SN = (string)parser["21"].Value;
            (obj as I8008).DateTimeOfProduction = (DateTime)parser["8008"].Value;
        }
        interface I01
        {
            string GTIN { get; set; }
        }
        interface I21 
        {
            string SN { get; set; }
        }
        interface I8008 
        {
            DateTime DateTimeOfProduction { get; set; }
        }
```
2. Получение полей AI из строки со своими обозначениями для GS и FNC1
```C#
        ParserGS1.Parser parser = new ParserGS1.Parser();
        ParserGS1.Parser.SetGroutSeperatorUser(@"\GS1", "{FNC1}");

            parser.SetText(@"{FNC1}0104660068212768310200110121zWyn8vjJ2rp0u\GS18008990101111230");
            (obj as I01).GTIN = (string)parser["01"].Value; // Value == "046600682127683102001101"
            (obj as I21).SN = (string)parser["21"].Value; // Value =="zWyn8vjJ2rp0u"
            (obj as I8008).DateTimeOfProduction = (DateTime)parser["8008"].Value; // Value == new DateTime(1999, 01, 01, 11, 12, 30)
            string actual = parser["8008"].ToStringValue; // ToStringValue == "990101111230"
              
```
![Диаграмма классов](http://service-debug.ru/Content/ParserGS1_128_1.png)
![Диаграмма последовательности](http://service-debug.ru/Content/ParserGS1_128_2.bmp)