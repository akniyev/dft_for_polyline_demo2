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

namespace dft_polyline_demo2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        void enableDisableControls()
        {
            if (radPolyNodesRandom.IsChecked == true)
            {
                txtMaxDelta.IsEnabled = true;
                comboNodesFunction.IsEnabled = false;
                btnNodesFunctionSave.IsEnabled = false;
            }
            else
            {
                txtMaxDelta.IsEnabled = false;
                comboNodesFunction.IsEnabled = true;
                btnNodesFunctionSave.IsEnabled = true;
            }

            if (radPolyValuesRandom.IsChecked == true)
            {
                txtPolyMinValue.IsEnabled = true;
                txtPolyMaxValue.IsEnabled = true;
                comboValuesFunction.IsEnabled = false;
                btnValuesFunctionSave.IsEnabled = false;
            }
            else
            {
                txtPolyMinValue.IsEnabled = false;
                txtPolyMaxValue.IsEnabled = false;
                comboValuesFunction.IsEnabled = true;
                btnValuesFunctionSave.IsEnabled = true;
            }
        }

        bool noErrors()
        {
            var errorMessages = new Tuple<string,int>[]
            {
                new Tuple<string, int>("m - integer, m >= 1", 0),
                new Tuple<string, int>("h = 0 || h >= 2pi/m", 0),
                new Tuple<string, int>("min_x, min_y - double, min_x < min_y", 0),
                new Tuple<string, int>("Nodes should be in ascending order", 0),
                new Tuple<string, int>("Wrong nodes function", 0),
                new Tuple<string, int>("Wrong values function", 0),
                new Tuple<string, int>("2n <= N in FT", 0),
                new Tuple<string, int>("2n <= N in DFT", 0),
            };

            return true;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void radPolyNodesRandom_Click(object sender, RoutedEventArgs e)
        {
            enableDisableControls();
        }

        private void radPolyNodesFunction_Click(object sender, RoutedEventArgs e)
        {
            enableDisableControls();
        }

        private void radPolyValuesRandom_Click(object sender, RoutedEventArgs e)
        {
            enableDisableControls();
        }

        private void radPolyValuesFunction_Click(object sender, RoutedEventArgs e)
        {
            enableDisableControls();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            enableDisableControls();
        }
    }
}
