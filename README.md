# Enum Helper class
- It helps:
  - binding an enum with description to ComboBox
  - XML serializing an enum as attribute

## Usage Sample
### Definition
```csharp
using System.ComponentModel;
using TakeAsh;

using NewLineCodesHelper = EnumHelper<NewLineCodes>;

public enum NewLineCodes {
  [Description("Unix(LF)")]
  Lf,
  [Description("Mac(CR)")]
  Cr,
  [Description("Windows(CR+LF)")]
  CrLf,
}
```

### Binding an enum with description to ComboBox
```csharp
comboBox_NewLineCode_GalleryCategory.ItemsSource = NewLineCodesHelper.ValueDescriptionPairs;
comboBox_NewLineCode_Gallery.SelectedValuePath="Value";
comboBox_NewLineCode_Gallery.SelectedValue = NewLineCodes.CrLf;

var newLineCode = (NewLineCodes)comboBox_NewLineCode_Gallery.SelectedValue;
```

### XML serializing an enum as attribute
```csharp
using System.Xml;
using System.Xml.Serialization;

[XmlIgnore]
public NewLineCodes NewLineCode { get; set; }

[XmlAttribute("NewLineCode")]
public string NewLineCodeAttribute {
  get { return NewLineCode.ToString(); }
  set { NewLineCode = NewLineCodesHelper.GetValueFromName(value); }
}
```

## Public Member Functions
- TEnum GetValueFromName(string name)
- TEnum GetValueFromDescription(string description)
- bool IsDefined(int value)

## Public Properties
- TEnum[] Values [get]
- string[] Names [get]
- string[] Descriptions [get]
- ValueDescriptionPair[] ValueDescriptionPairs [get]

## Link
- [C# 3.0 : using extension methods for enum ToString - I know the answer (it's 42) - MSDN Blogs](http://blogs.msdn.com/b/abhinaba/archive/2005/10/21/483337.aspx)
- [C# の enum に関連する小技。 - Qiita](http://qiita.com/hugo-sb/items/38fe86a09e8e0865d471)
- [[C#] 何故 enum に拘りたくなるのか？ | Moonmile Solutions Blog](http://www.moonmile.net/blog/archives/3666)
