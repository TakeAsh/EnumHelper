using System;

namespace TakeAsh {

    static public class AttributeHelper<TAttr>
        where TAttr : Attribute {

        /// <summary>
        /// Returns TAttr attribute of the object.
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>TAttr attribute</returns>
        static public TAttr GetAttribute(object obj) {
            var memInfos = obj.GetType().GetMember(obj.ToString());
            if (memInfos == null || memInfos.Length == 0) {
                return default(TAttr);
            }
            var attrs = memInfos[0].GetCustomAttributes(typeof(TAttr), false);
            if (attrs == null || attrs.Length == 0) {
                return default(TAttr);
            }
            return (TAttr)attrs[0];
        }
    }
}
