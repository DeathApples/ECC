using System.Windows;
using System.Windows.Controls;

namespace ECC.GUI.UserControls
{
    /// <summary>
    /// Логика взаимодействия для NumberBox.xaml
    /// </summary>
    public partial class NumberBox : UserControl
    {
        public string ParameterName
        {
            get { return (string)GetValue(ParameterNameProperty); }
            set { SetValue(ParameterNameProperty, value); }
        }

        public static readonly DependencyProperty ParameterNameProperty =
            DependencyProperty.Register("ParameterName", typeof(string), typeof(NumberBox), new PropertyMetadata(string.Empty));

        public string Parameter
        {
            get { return (string)GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register("Parameter", typeof(string), typeof(NumberBox), 
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public NumberBox()
        {
            InitializeComponent();
        }
    }
}
