using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows;
using Microsoft.Windows.Controls.Ribbon;
using TakeAsh;

namespace EnumHelperLib_Call {

    using NewLineCodesHelper = EnumHelper<Options.NewLineCodes>;
    using SeparatorsHelper = EnumHelper<Options.Separators>;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow {

        const string DefaultCultureName = "en-US";

        private Options _options;

        public Options Options {
            get { return _options ?? (_options = new Options()); }
            set { _options = value; }
        }

        public MainWindow() {
            InitializeComponent();

            // Insert code required on object creation below this point.
            comboBox_Language_GalleryCategory.ItemsSource = GetLocalizedResources();
            comboBox_Language_Gallery.SelectedValue = Thread.CurrentThread.CurrentCulture.Name;
            comboBox_NewLineCode_Gallery.SelectedValue = Options.NewLineCodes.CrLf;
            comboBox_Separator_Gallery.SelectedValue = Options.Separators.Tab;
        }

        private CultureInfo[] GetLocalizedResources() {
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(c => c.Name).ToArray();
            var ret = new List<CultureInfo>();
            ret.Add(new CultureInfo(DefaultCultureName));
            foreach (var directory in Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory)) {
                var name = Path.GetFileNameWithoutExtension(directory);
                if (Array.IndexOf(allCultures, name) < 0) {
                    continue;
                }
                ret.Add(new CultureInfo(name));
            }
            return ret.ToArray();
        }

        private void SetCurrentCulture() {
            var cultureName = comboBox_Language_Gallery.SelectedValue as string;
            if (String.IsNullOrEmpty(cultureName)) {
                return;
            }
            var culture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            NewLineCodesHelper.Init();
            initDialog();
        }

        private void initDialog() {
            var newLineCode = (Options.NewLineCodes)comboBox_NewLineCode_Gallery.SelectedValue;
            comboBox_NewLineCode_GalleryCategory.ItemsSource = NewLineCodesHelper.ValueDescriptionPairs;
            comboBox_NewLineCode_Gallery.SelectedValue = newLineCode;
            var separator = (Options.Separators)comboBox_Separator_Gallery.SelectedValue;
            comboBox_Separator_GalleryCategory.ItemsSource = SeparatorsHelper.ValueDescriptionPairs;
            comboBox_Separator_Gallery.SelectedValue = separator;
        }

        private void SetOption() {
            Options.NewLineCode = (Options.NewLineCodes)comboBox_NewLineCode_Gallery.SelectedValue;
            Options.Separator = (Options.Separators)comboBox_Separator_Gallery.SelectedValue;
            text_Result.Text = Options.ToXml();
        }

        private void comboBox_Language_Gallery_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            SetCurrentCulture();
        }

        private void comboBox_NewLineCode_Gallery_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            SetOption();
        }

        private void comboBox_Separator_Gallery_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            SetOption();
        }
    }
}
