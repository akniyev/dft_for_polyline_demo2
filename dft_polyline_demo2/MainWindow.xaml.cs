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
using DiscreteFunctions;
using DiscreteFunctionsPlots;
using GraphBuilders;
using NCalc;
using static System.Math;
using Expression = NCalc.Expression;

namespace dft_polyline_demo2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double[] polylineNodes;
        double[] polylineValues;
        double[] alpha;
        double[] beta;
        int m;

        GraphBuilder2DForm gb_grid = new GraphBuilder2DForm();
        Plot2D plot_grid = new Plot2D("Nodes");

        GraphBuilder2DForm gb_functions = new GraphBuilder2DForm(2);
        Plot2D plot_polyline = new Plot2D("Polyline");
        Plot2D plot_ft = new Plot2D("Partial sum of Fourier series");
        Plot2D plot_dft = new Plot2D("Partial sum of DFT series");

        Plot2D plotFtDiff = new Plot2D("Fourier trasnsform diff");
        Plot2D plotDftDiff = new Plot2D("Discrete Fourier trasnsform diff");
        Plot2D plotEst = new Plot2D("User estimate");

        void enableDisableControls()
        {
            if (radPolyNodesRandom.IsChecked == true)
            {
                txt_m.IsEnabled = true;
                txtMaxDelta.IsEnabled = false;
                comboNodesFunction.IsEnabled = false;
                btnNodesFunctionSave.IsEnabled = false;
            }
            else if (radPolyNodesFunction.IsChecked == true)
            {
                txt_m.IsEnabled = true;
                txtMaxDelta.IsEnabled = false;
                comboNodesFunction.IsEnabled = true;
                btnNodesFunctionSave.IsEnabled = true;
            }
            else
            {
                txt_m.IsEnabled = false;
                txtMaxDelta.IsEnabled = true;
                comboNodesFunction.IsEnabled = false;
                btnNodesFunctionSave.IsEnabled = false;
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

            comboEstFunction.IsEnabled = cbDrawEstimate.IsChecked == true ? true : false; 
            btnEstFunctionSave.IsEnabled = cbDrawEstimate.IsChecked == true ? true : false;
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

            gb_grid.GraphBuilder.DrawPlot<Steema.TeeChart.Styles.Points>(plot_grid);
            gb_grid.Show();

            gb_functions.GraphBuilder.DrawPlot(plot_polyline);
            gb_functions.GraphBuilder.DrawPlot(plot_ft);
            gb_functions.GraphBuilder.DrawPlot(plot_dft);

            gb_functions.GetGraphBuilder(1).DrawPlot(plotFtDiff);
            gb_functions.GetGraphBuilder(1).DrawPlot(plotDftDiff);
            gb_functions.GetGraphBuilder(1).DrawPlot(plotEst);
            gb_functions.Show();

        }

        private void btnGenerateGrid_Click(object sender, RoutedEventArgs e)
        {
            if (!noErrors()) return;

            if (radPolyNodesRandom.IsChecked == true)
            {
                string sm = txt_m.Text;
                sm = sm.Replace('.', ',');
                m = int.Parse(sm);
                polylineNodes = new double[m + 1];
                var r = new Random();
                for (int i = 1; i < m; i++)
                {
                    polylineNodes[i] = r.NextDouble()*2*PI;
                }
                polylineNodes[0] = 0;
                polylineNodes[m] = 2*PI;

                polylineNodes = polylineNodes.OrderBy(x => x).ToArray();
            }
            else if (radPolyNodesRandomMaxH.IsChecked == true)
            {
                var max_delta = double.Parse(txtMaxDelta.Text);
                var nodes = new List<double>();
                var sum = 0d;
                var r = new Random();
                nodes.Add(0d);
                while (2*PI - sum > max_delta)
                {
                    var a = r.NextDouble()*max_delta;
                    sum += a;
                    nodes.Add(sum);
                }
                nodes.Add(2*PI);
                polylineNodes = nodes.ToArray();
                txt_m.Text = nodes.Count.ToString();
                m = nodes.Count;
            } else if (radPolyNodesFunction.IsChecked == true)
            {
                var exprString = comboNodesFunction.Text.Split(new string[] {"//"}, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                m = int.Parse(txt_m.Text);
                polylineNodes = new double[m + 1];
                var ex = new Expression(exprString);
                ex.Parameters["m"] = m;
                ex.Parameters["PI"] = PI;
                for (int j = 0; j < m + 1; j++)
                {
                    ex.Parameters["j"] = j;
                    polylineNodes[j] = (double)ex.Evaluate();
                }
            }

            var nodesDf = new DiscreteFunction2D(polylineNodes, new double[polylineNodes.Length]);
            plot_grid.DiscreteFunction = nodesDf;
            plot_grid.Refresh();
        }

        private void btnGeneratePolyline_Copy_Click(object sender, RoutedEventArgs e)
        {
            polylineValues = new double[polylineNodes.Length];

            if (radPolyValuesRandom.IsChecked == true)
            {
                double max = double.Parse(txtPolyMaxValue.Text);
                double min = double.Parse(txtPolyMinValue.Text);
                var r = new Random();

                for (int i = 0; i < polylineNodes.Length - 1; i++)
                {
                    double x = polylineNodes[i];
                    double y = r.NextDouble() * (max - min) + min;

                    polylineValues[i] = y;
                }

                polylineValues[polylineValues.Length - 1] = polylineValues[0];
            }
            else if (radPolyValuesFunction.IsChecked == true)
            {
                var exprString = comboValuesFunction.Text.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                var m = int.Parse(txt_m.Text);
                var ex = new Expression(exprString);
                ex.Parameters["m"] = m;
                ex.Parameters["PI"] = PI;
                for (int i = 0; i < polylineNodes.Length; i++)
                {
                    double x = polylineNodes[i];
                    ex.Parameters["x"] = x;
                    double y = (double)ex.Evaluate();

                    polylineValues[i] = y;
                }
            }

            calculateAlphasAndBetas();

            var polylineDf = new DiscreteFunction2D(polylineNodes, polylineValues);
            plot_polyline.DiscreteFunction = polylineDf;
            plot_polyline.Refresh();
        }

        void calculateAlphasAndBetas()
        {
            alpha = new double[polylineNodes.Length - 1];
            beta = new double[polylineNodes.Length - 1];
            for (int i = 0; i < alpha.Length; i++)
            {
                double x1 = polylineNodes[i];
                double x2 = polylineNodes[i + 1];
                double y1 = polylineValues[i];
                double y2 = polylineValues[i + 1];

                alpha[i] = (y2 - y1)/(x2 - x1);
                beta[i] = y1 - alpha[i] * x1;
            }
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

        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            enableDisableControls();
        }

        double polylineValue(double x)
        {
            int i = 0;
            var xs = polylineNodes;

            //Находим с помощью бинарного поиска, в каком отрезке лежит наша точка
            if (xs.Length > 2)
            {
                int left = 0;
                int right = xs.Length;
                i = (left + right)/2;
                while (!(xs[i] <= x && xs[i + 1] >= x))
                {
                    if (x < xs[i])
                    {
                        right = i;
                    }
                    else
                    {
                        left = i;
                    }
                    i = (left + right)/2;
                }
            }


            return alpha[i] * x + beta[i];
        }

        private void btnFouriesTransform_Click(object sender, RoutedEventArgs e)
        {
            //Checking Fourier series
            //Вычисляем частичную сумму ряда Фурье
            int Dens = int.Parse(txtFt_N.Text);
            int n_fourier = int.Parse(txtFt_n.Text);

            double[] x_fourier = new double[Dens + 1];
            double[] y_fourier = new double[Dens + 1];
            double[] ksi = polylineNodes;

            for (int i = 0; i <= Dens; i++)
            {

                double x = 2.0 * Math.PI / Dens * i;
                x_fourier[i] = x;

                double y = 0;

                //Здесь начинается новый способ вычисления (выведенный по формулам)
                //Новый способ тоже работает, как и старый
                double a0 = 0;
                for (int j = 0; j < m; j++)
                {
                    a0 += alpha[j] / 2.0 * (ksi[j + 1] * ksi[j + 1] - ksi[j] * ksi[j]) + beta[j] * (ksi[j + 1] - ksi[j]);

                }
                a0 *= 1.0 / (2.0 * Math.PI);
                y += a0;

                double sum_k = 0;
                for (int k = 1; k <= n_fourier; k++)
                {
                    double sum_j = 0;
                    for (int j = 0; j < m; j++)
                    {
                        sum_j += alpha[j] * (Math.Cos(k * x) * (Math.Cos(k * ksi[j + 1]) - Math.Cos(k * ksi[j])) + Math.Sin(k * x) * (Math.Sin(k * ksi[j + 1]) - Math.Sin(k * ksi[j])));
                    }
                    sum_j *= 1.0 / (k * k);

                    sum_k += sum_j;
                }
                sum_k *= 1.0 / Math.PI;

                y += sum_k;

                y_fourier[i] = y;

            }
            plot_ft.DiscreteFunction = new DiscreteFunction2D(x_fourier, y_fourier);
            plot_ft.Refresh();

            //Вычисляем остаток для суммы Фурье
            double[] y_fourier_diff = new double[Dens + 1];
            for (int i = 0; i < x_fourier.Length; i++)
            {
                y_fourier_diff[i] = y_fourier[i] - polylineValue(x_fourier[i]);
            }
            plotEst.DiscreteFunction = new DiscreteFunction2D(x_fourier, y_fourier_diff);
            plotEst.Refresh();


            //Рисуем пользовательскую оценку
            if (cbDrawEstimate.IsChecked == true)
            {
                string est_string = comboEstFunction.Text;
                var ex = new Expression(est_string);
                ex.Parameters["m"] = m;
                ex.Parameters["n_f"] = n_fourier;
                ex.Parameters["PI"] = PI;

                var value = (double) ex.Evaluate();
                var xs = new double[] {0, 2*PI};
                var ys = new double[] {value, value};
                var df = new DiscreteFunction2D(xs, ys);
                plotEst.DiscreteFunction = df;
                plotEst.Refresh();
            }
        }
    }
}
