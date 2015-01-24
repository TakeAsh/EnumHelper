using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace TakeAsh {
    public class XmlHelper<T> {
        static private XmlSerializer _serializer = new XmlSerializer(typeof(T));
        static private XmlSerializerNamespaces _blankNameSpace = new XmlSerializerNamespaces();
        static private XmlWriterSettings _settings = new XmlWriterSettings() {
            Indent = true,
            //IndentChars = ("  "),
            Encoding = Encoding.UTF8,
        };
        static private Regex _regXmlHeader = new Regex(
            @"^(?<head1><\?xml\s+version=""[^""]+""\s+encoding="")[^""]+(?<head2>""\s*\?>)"
        );

        static XmlHelper() {
            _blankNameSpace.Add("", "");
        }

        static public T importFile(string fileName) {
            if (!File.Exists(fileName)) {
                Debug.Print("XmlHelper.importFile: Not Exist : " + fileName);
                return default(T);
            }
            try {
                using (var stream = new FileStream(fileName, FileMode.Open)) {
                    return (T)_serializer.Deserialize(stream);
                }
            }
            catch (Exception ex) {
                Debug.Print(ex.Message);
            }
            return default(T);
        }

        static public bool exportFile(string fileName, T obj) {
            try {
                using (var writer = XmlWriter.Create(fileName, _settings)) {
                    _serializer.Serialize(writer, obj, _blankNameSpace);
                }
                return true;
            }
            catch (Exception ex) {
                Debug.Print(ex.Message);
            }
            return false;
        }

        static public T convertFromString(string text) {
            try {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text ?? ""))) {
                    return (T)_serializer.Deserialize(stream);
                }
            }
            catch (Exception ex) {
                Debug.Print(ex.Message);
            }
            return default(T);
        }

        static public string convertToString(T obj) {
            try {
                var sb = new StringBuilder();
                using (var writer = XmlWriter.Create(sb, _settings)) {
                    _serializer.Serialize(writer, obj, _blankNameSpace);
                    return _regXmlHeader.Replace(
                        sb.ToString(),
                        (Match m) => m.Groups["head1"].Value + "utf-8" + m.Groups["head2"].Value
                    );
                }
            }
            catch (Exception ex) {
                Debug.Print(ex.Message);
            }
            return null;
        }

        static public T readElement(XmlReader reader) {
            return (T)_serializer.Deserialize(reader);
        }

        static public void writeElement(XmlWriter writer, T value) {
            _serializer.Serialize(writer, value, _blankNameSpace);
        }
    }
}
