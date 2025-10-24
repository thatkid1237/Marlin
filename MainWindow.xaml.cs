using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MSSS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SensorDataService _dataService = new();
        private SensorDataModel _dataModel = new();
        public MainWindow()
        {

            InitializeComponent();
        }

        private void LoadData()
        {
            // Clamp UI numeric inputs to required ranges
            double mu = Clamp(ParseDouble(tbMu.Text, 50), 35, 75);
            double sigma = Clamp(ParseDouble(tbSigma.Text, 10), 10, 20);
            tbMu.Text = mu.ToString();
            tbSigma.Text = sigma.ToString();

            // Generate 400 samples per sensor from the DLL (rounded to 4 decimals by the DLL)
            const int size = 400;
            _dataModel = _dataService.Load(mu, sigma, size);
        }
        private void ShowAllSensorData()
        {
            // For display only – allowed to construct a temporary list of anonymous rows
            int max = Math.Max(_dataModel.SensorA.Count, _dataModel.SensorB.Count);
            var rows = new List<object>(max);
            for (int i = 0; i < max; i++)
            {
                double? a = i < _dataModel.SensorA.Count ? _dataModel.SensorA.ElementAt(i) : (double?)null;
                double? b = i < _dataModel.SensorB.Count ? _dataModel.SensorB.ElementAt(i) : (double?)null;
                rows.Add(new { A = a, B = b });
            }
            lvBothSensors.ItemsSource = rows;
        }
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            ShowAllSensorData();
            DisplayListboxData(_dataModel.SensorA, lbSensorA);
            DisplayListboxData(_dataModel.SensorB, lbSensorB);
        }
        /// <summary>
        /// DisplayListboxData(list, listBox): binds the list’s values to the target ListBox.
        /// </summary>
        private void DisplayListboxData(LinkedList<double> list, ListBox listBox)
        {
            listBox.ItemsSource = list.ToList(); // display-only conversion (algorithms do NOT use arrays/lists)
        }
        private void btnSortASelection_Click(object sender, RoutedEventArgs e)
        {
            var sw = Stopwatch.StartNew();
            _dataService.Sort(_dataModel.SensorA, SortAlgorithm.Selection);
            sw.Stop();
            tbSortASelectionMs.Text = sw.ElapsedMilliseconds.ToString();
            ShowAllSensorData();
            DisplayListboxData(_dataModel.SensorA, lbSensorA);
        }
        private void btnSortAInsertion_Click(object sender, RoutedEventArgs e)
        {
            var sw = Stopwatch.StartNew();
            _dataService.Sort(_dataModel.SensorA, SortAlgorithm.Insertion);
            sw.Stop();
            tbSortAInsertionMs.Text = sw.ElapsedMilliseconds.ToString();
            ShowAllSensorData();
            DisplayListboxData(_dataModel.SensorA, lbSensorA);
        }
        private void btnSortBSelection_Click(object sender, RoutedEventArgs e)
        {
            var sw = Stopwatch.StartNew();
            _dataService.Sort(_dataModel.SensorB, SortAlgorithm.Selection);
            sw.Stop();
            tbSortBSelectionMs.Text = sw.ElapsedMilliseconds.ToString();
            ShowAllSensorData();
            DisplayListboxData(_dataModel.SensorB, lbSensorB);
        }
        private void btnSortBInsertion_Click(object sender, RoutedEventArgs e)
        {
            var sw = Stopwatch.StartNew();
            _dataService.Sort(_dataModel.SensorB, SortAlgorithm.Insertion);
            sw.Stop();
            tbSortBInsertionMs.Text = sw.ElapsedMilliseconds.ToString();
            ShowAllSensorData();
            DisplayListboxData(_dataModel.SensorB, lbSensorB);
        }
        private void btnRefreshBoth_Click(object sender, RoutedEventArgs e)
        {
            ShowAllSensorData();
        }
        private void btnSearchAIter_Click(object sender, RoutedEventArgs e)
        {
            if (!SensorDataService.IsSortedAscending(_dataModel.SensorA)) { MessageBox.Show("Please sort Sensor A first."); return; }
            if (!int.TryParse(tbSearchA.Text, out int searchInt)) { MessageBox.Show("Enter an integer for Search A."); return; }


            var sw = Stopwatch.StartNew();
            int idx = _dataService.Search(_dataModel.SensorA, searchInt, SearchMode.Iterative);
            sw.Stop();
            tbSearchAIterTicks.Text = sw.ElapsedTicks.ToString();


            DisplayListboxData(_dataModel.SensorA, lbSensorA);
            HighlightNeighbours(lbSensorA, idx);
        }
        private void btnSearchARec_Click(object sender, RoutedEventArgs e)
        {
            if (!SensorDataService.IsSortedAscending(_dataModel.SensorA)) { MessageBox.Show("Please sort Sensor A first."); return; }
            if (!int.TryParse(tbSearchA.Text, out int searchInt)) { MessageBox.Show("Enter an integer for Search A."); return; }


            var sw = Stopwatch.StartNew();
            int idx = _dataService.Search(_dataModel.SensorA, searchInt, SearchMode.Recursive);
            sw.Stop();
            tbSearchARecTicks.Text = sw.ElapsedTicks.ToString();


            DisplayListboxData(_dataModel.SensorA, lbSensorA);
            HighlightNeighbours(lbSensorA, idx);
        }
        private void btnSearchBIter_Click(object sender, RoutedEventArgs e)
        {
            if (!SensorDataService.IsSortedAscending(_dataModel.SensorB)) { MessageBox.Show("Please sort Sensor B first."); return; }
            if (!int.TryParse(tbSearchB.Text, out int searchInt)) { MessageBox.Show("Enter an integer for Search B."); return; }


            var sw = Stopwatch.StartNew();
            int idx = _dataService.Search(_dataModel.SensorB, searchInt, SearchMode.Iterative);
            sw.Stop();
            tbSearchBIterTicks.Text = sw.ElapsedTicks.ToString();


            DisplayListboxData(_dataModel.SensorB, lbSensorB);
            HighlightNeighbours(lbSensorB, idx);
        }
        private void btnSearchBRec_Click(object sender, RoutedEventArgs e)
        {
            if (!SensorDataService.IsSortedAscending(_dataModel.SensorB)) { MessageBox.Show("Please sort Sensor B first."); return; }
            if (!int.TryParse(tbSearchB.Text, out int searchInt)) { MessageBox.Show("Enter an integer for Search B."); return; }


            var sw = Stopwatch.StartNew();
            int idx = _dataService.Search(_dataModel.SensorB, searchInt, SearchMode.Recursive);
            sw.Stop();
            tbSearchBRecTicks.Text = sw.ElapsedTicks.ToString();


            DisplayListboxData(_dataModel.SensorB, lbSensorB);
            HighlightNeighbours(lbSensorB, idx);
        }
        // =======================
        // UI helpers
        // =======================


        private static void HighlightNeighbours(ListBox lb, int centerIndex)
        {
            lb.SelectedItems.Clear();
            for (int k = centerIndex - 2; k <= centerIndex + 2; k++)
            {
                if (k >= 0 && k < lb.Items.Count)
                    lb.SelectedItems.Add(lb.Items[k]);
            }
            if (centerIndex >= 0 && centerIndex < lb.Items.Count)
                lb.ScrollIntoView(lb.Items[centerIndex]);
        }
        private static double ParseDouble(string text, double fallback)
=> double.TryParse(text, out var v) ? v : fallback;


        private static double Clamp(double v, double min, double max)
        => v < min ? min : (v > max ? max : v);


        // Restrict search TextBoxes to integer input only
        private void IntegerOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(((TextBox)sender).Text + e.Text, out _);
        }
        private void IntegerOnly_Paste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!int.TryParse(text, out _)) e.CancelCommand();
            }
            else e.CancelCommand();
        }

    }
}