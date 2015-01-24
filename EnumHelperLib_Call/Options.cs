using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using TakeAsh;

namespace EnumHelperLib_Call {

    using NewLineCodesHelper = EnumHelper<Options.NewLineCodes>;
    using SeparatorsHelper = EnumHelper<Options.Separators>;

    public class Options {

        public enum NewLineCodes {
            [Description("Unix(LF)")]
            Lf,
            [Description("Mac(CR)")]
            Cr,
            [Description("Windows(CR+LF)")]
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

        [XmlIgnore]
        public NewLineCodes NewLineCode { get; set; }

        [XmlAttribute("NewLineCode")]
        public string NewLineCodeAttribute {
            get { return NewLineCode.ToString(); }
            set { NewLineCode = NewLineCodesHelper.GetValueFromName(value); }
        }

        [XmlIgnore]
        public Separators Separator { get; set; }

        [XmlAttribute("Separator")]
        public string SeparatorAttribute {
            get { return Separator.ToString(); }
            set { Separator = SeparatorsHelper.GetValueFromName(value); }
        }

        public string ToXml() {
            return XmlHelper<Options>.convertToString(this);
        }
    }
}
