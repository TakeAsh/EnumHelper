using System;
using System.Windows;
using System.Windows.Controls;

namespace TakeAsh {

    /// <summary>
    /// ComboBox Extension Methods
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>[c# - How can I make a WPF combo box have the width of its widest element in XAML? - Stack Overflow](http://stackoverflow.com/questions/1034505/)</item>
    /// </list>
    /// </remarks>
    static public class ComboBoxExtensionMethods {

        /// <summary>
        /// Returns maximum item width of combobox
        /// </summary>
        /// <param name="comboBox">ComboBox to be measured width</param>
        /// <returns>Maximum item width</returns>
        static public double GetMaxItemWidth(this ComboBox comboBox) {
            if (comboBox.Items.Count == 0) {
                return SystemParameters.VerticalScrollBarWidth;
            }
            var displayMemberInfo = comboBox.Items[0].GetType().GetProperty(comboBox.DisplayMemberPath);
            var getDisplayMember = displayMemberInfo != null ?
                (Func<object, object>)(item => displayMemberInfo.GetValue(item, null)) :
                (Func<object, object>)(item => item);
            var width = 0.0;
            foreach (var item in comboBox.Items) {
                var comboBoxItem = comboBox.ItemContainerGenerator.ContainerFromItem(item) as ComboBoxItem ??
                    new ComboBoxItem() { Content = getDisplayMember(item) };
                comboBoxItem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (width < comboBoxItem.DesiredSize.Width) {
                    width = comboBoxItem.DesiredSize.Width;
                }
            }
            return width + SystemParameters.VerticalScrollBarWidth;
        }

        /// <summary>
        /// Set ComboBox width to maximum item width
        /// </summary>
        /// <param name="comboBox">ComboBox to be adjusted</param>
        static public void AdjustMaxItemWidth(this ComboBox comboBox) {
            comboBox.Width = comboBox.GetMaxItemWidth();
        }
    }
}
