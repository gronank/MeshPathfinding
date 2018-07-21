using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapCreator
{
    /// <summary>
    /// Interaction logic for SliderElement.xaml
    /// </summary>
    public partial class SliderElement : UserControl
    {
        public string Label {
            get { return (String)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        public float Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public float Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public float Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public float Display
        {
            get {
                int display = (int)GetValue(DisplayProperty);
                if (display < 0) return (int)DependencyProperty.UnsetValue;
                else return display;
            }
            set { SetValue(DisplayProperty, value); }
        }
        public SliderElement()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public static readonly DependencyProperty LabelProperty =
    DependencyProperty.Register("Label", typeof(string),
      typeof(SliderElement), new PropertyMetadata(""));

        public static readonly DependencyProperty MaximumProperty =
    DependencyProperty.Register("Maximum", typeof(float),
      typeof(SliderElement), new PropertyMetadata(0f));

        public static readonly DependencyProperty MinimumProperty =
    DependencyProperty.Register("Minimum", typeof(float),
      typeof(SliderElement), new PropertyMetadata(0f));

        public static readonly DependencyProperty ValueProperty =
    DependencyProperty.Register("Value", typeof(float),
      typeof(SliderElement), new PropertyMetadata(0f));

        public static readonly DependencyProperty DisplayProperty =
    DependencyProperty.Register("Display", typeof(float),
      typeof(SliderElement), new PropertyMetadata(-1f));
    }

}
