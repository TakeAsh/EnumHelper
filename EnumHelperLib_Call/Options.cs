using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using TakeAsh;

namespace EnumHelperLib_Call {

    using NewLineCodesHelper = EnumHelper<Options.NewLineCodes>;
    using SeparatorsHelper = EnumHelper<Options.Separators>;

    public class Options {

        [TypeConverter(typeof(EnumTypeConverter<NewLineCodes>))]
        public enum NewLineCodes {
            [ExtraProperties("Entity:'\n', Escaped:'\\x22\\u0027\t'")]
            Lf = 1,

            [ExtraProperties("Entity : \"\r\"Escaped : '\\x0027\\x0022'")]
            [Description("[A] Mac(CR)")]
            Cr = 2,

            [ExtraProperties("Entity:\t'\r\n';;;Escaped:\t\"\\x3042\"")] // U+3042 あ
            [Description("[A] Windows(CR+LF)")]
            CrLf = 4,

            [ExtraProperties("Entity:\n\t'\n\r'\nEscaped:\n\t'\\uD842\\uDFB7'")] // U+00020BB7 𠮷
            LfCr = 8,
        }

        [TypeConverter(typeof(EnumTypeConverter<Separators>))]
        public enum Separators {
            [Description("Tab(\\t)")]
            Tab,
            [Description("Space( )")]
            Space,
            [Description("Comma(,)")]
            Comma,
            [Description("Semicolon(;)")]
            Semicolon,
        }

        [XmlAttribute]
        public NewLineCodes NewLineCode { get; set; }

        [XmlAttribute]
        public Separators Separator { get; set; }

        public string ToXml() {
            return XmlHelper<Options>.convertToString(this);
        }
    }
}
