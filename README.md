# EnumHelper&lt;TEnum&gt; Class
- It helps binding an enum with description to `ComboBox`.
- Enum item description come from:
  1. localized resource (EnumType_ItemName)
  1. default resource (EnumType_ItemName)
  1. `Description` attribute (System.ComponentModel)
  1. Enum item name
- Not only `ValueDescriptionPairs` but also `EnumTypeConverter<TEnum>` can be used when the UIElement support `TypeConverter`.
- `ExtraProperties` attribute attaches a `Dictionary<string, string>`.
- `GetExtraProperty` extension method returns the value from the dictionary.

## Usage Sample
```csharp
using System.ComponentModel;
using TakeAsh;

using NewLineCodesHelper = EnumHelper<NewLineCodes>;

[TypeConverter(typeof(EnumTypeConverter<NewLineCodes>))]
public enum NewLineCodes {
  [ExtraProperties("Entity:'\n', SecondKey:'Not Used'")]
  [Description("Unix(LF)")]
  Lf,
  [ExtraProperties("Entity : \"\r\"")]
  [Description("Mac(CR)")]
  Cr,
  [ExtraProperties("Entity:\t'\r\n'")]
  [Description("Windows(CR+LF)")]
  CrLf,
}

// Resource
string NewLineCodes_Lf = "Unix(LF) [Localized]";
string NewLineCodes_Cr = "Mac(CR) [Localized]";
string NewLineCodes_CrLf = "Windows(CR+LF) [Localized]";
//

// using ValueDescriptionPairs
comboBox_NewLineCode_GalleryCategory.ItemsSource = NewLineCodesHelper.ValueDescriptionPairs;
comboBox_NewLineCode_GalleryCategory.DisplayMemberPath = "Value";
comboBox_NewLineCode_Gallery.SelectedValuePath = "Key";
comboBox_NewLineCode_Gallery.SelectedValue = NewLineCodes.CrLf;

var newLineCode = NewLineCodesHelper.Cast(comboBox_NewLineCode_Gallery.SelectedValue);
var desc1 = newLineCode.ToDescription();
var desc2 = NewLineCodesHelper.ValueDescriptionDictionary[newLineCode];
var newLineEntity = newLineCode.GetExtraProperty("Entity");

var newLineEntities = NewLineCodesHelper.GetAllExtraProperties("Entity");

// using TypeConverter
comboBox_NewLineCode2_GalleryCategory.ItemsSource = NewLineCodesHelper.Values;
comboBox_NewLineCode2_Gallery.SelectedItem = NewLineCodes.CrLf;

var newLineCode2 = NewLineCodesHelper.Cast(comboBox_NewLineCode2_Gallery.SelectedItem);
```

## Properties and Methods
### EnumHelper&lt;TEnum&gt; Class Properties
- string[] Descriptions [get]
- string[] Names [get]
- Dictionary&lt;TEnum, string&gt; ValueDescriptionDictionary [get]
- KeyValuePair&lt;TEnum, string&gt;[] ValueDescriptionPairs [get]
- TEnum[] Values [get]

### EnumHelper&lt;TEnum&gt; Class Methods
- TEnum Cast(object obj)
- string[] GetAllExtraProperties(string key)
- string GetAssemblyName()
- TEnum GetValueFromDescription(string description)
- TEnum GetValueFromName(string name)
- Nullable&lt;TEnum&gt; GetValueNullableFromDescription(string description)
- Nullable&lt;TEnum&gt; GetValueNullableFromName(string name)
- void Init()
- bool IsDefined(object value)
- string ToDescription(TEnum en)
- bool TryParse(string value, out TEnum result)

### EnumExtensionMethods Class Methods
- string GetAssemblyName(this Enum en)
- string GetExtraProperty(this Enum en, string key)
- string ToDescription(this Enum en)

## Link
- [C# 3.0 : using extension methods for enum ToString - I know the answer (it's 42) - MSDN Blogs](http://blogs.msdn.com/b/abhinaba/archive/2005/10/21/483337.aspx)
- [C# の enum に関連する小技。 - Qiita](http://qiita.com/hugo-sb/items/38fe86a09e8e0865d471)
- [[C#] 何故 enum に拘りたくなるのか？ | Moonmile Solutions Blog](http://www.moonmile.net/blog/archives/3666)
- [c# - Localizing enum descriptions attributes - Stack Overflow](http://stackoverflow.com/questions/569298/)
- [unit testing - .NET NUnit test - Assembly.GetEntryAssembly() is null - Stack Overflow](http://stackoverflow.com/questions/4337201/)
- [c# - Data bind enum properties to grid and display description - Stack Overflow](http://stackoverflow.com/questions/1540103/)
