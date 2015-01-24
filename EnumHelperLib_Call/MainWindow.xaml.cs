using System;
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

        private Options _options;

        public Options Options {
            get { return _options ?? (_options = new Options()); }
            set { _options = value; }
        }

        public MainWindow() {
            InitializeComponent();

            // Insert code required on object creation below this point.
            comboBox_NewLineCode_GalleryCategory.ItemsSource = NewLineCodesHelper.ValueDescriptionPairs;
            comboBox_NewLineCode_Gallery.SelectedValue = Options.NewLineCodes.CrLf;
            comboBox_Separator_GalleryCategory.ItemsSource = SeparatorsHelper.ValueDescriptionPairs;
            comboBox_Separator_Gallery.SelectedValue = Options.Separators.Tab;
        }

        private void SetOption() {
            Options.NewLineCode = (Options.NewLineCodes)comboBox_NewLineCode_Gallery.SelectedValue;
            Options.Separator = (Options.Separators)comboBox_Separator_Gallery.SelectedValue;
            text_Result.Text = Options.ToXml();
        }

        private void comboBox_NewLineCode_Gallery_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            SetOption();
        }

        private void comboBox_Separator_Gallery_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            SetOption();
        }
    }
}
