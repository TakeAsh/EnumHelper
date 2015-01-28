using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using TakeAsh;

namespace EnumHelperLib_Call {

    using NewLineCodesHelper = EnumHelper<Options.NewLineCodes>;
    using SeparatorsHelper = EnumHelper<Options.Separators>;

    public class Options {

        public enum NewLineCodes {
            Lf,
            Cr,
            CrLf,
        }

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
