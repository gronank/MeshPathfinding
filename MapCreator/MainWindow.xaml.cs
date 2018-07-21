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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //map generation properties
        int blocks;

        float forestSmallScale;
        float forestLargeScale;
        float townSmallScale;
        float townLargeScale;

        float forestProportion;
        float townProportion;
        //road network properties
        float minTownSize;
        float maxHighwayRange;

        float minVillageSize;
        float maxRoadRangeToTown;
        float maxRoadRangeToVillage;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new OptionsDataContext();
        }
        void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {

        }


    }
}
