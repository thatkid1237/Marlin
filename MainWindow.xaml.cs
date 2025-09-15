using Galileo6;
using System.Diagnostics;
using System.Text;
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
        private LinkedList<double> _sensorA = new LinkedList<double>();
        private LinkedList<double> _sensorB = new LinkedList<double>();
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


            _sensorA = new LinkedList<double>();
            _sensorB = new LinkedList<double>();


            var reader = new Galileo6.ReadData();


            // Generate 400 samples per sensor from the DLL (rounded to 4 decimals by the DLL)
            int size = 400;
            for (int i = 0; i < size; i++)
            {
                double a = reader.SensorA(mu, sigma);
                double b = reader.SensorB(mu, sigma);
                _sensorA.AddLast(a);
                _sensorB.AddLast(b);
            }
        }
        private void ShowAllSensorData()
        {
            // For display only – allowed to construct a temporary list of anonymous rows
            int max = Math.Max(_sensorA.Count, _sensorB.Count);
            var rows = new List<object>(max);
            for (int i = 0; i < max; i++)
            {
                double? a = i < _sensorA.Count ? _sensorA.ElementAt(i) : (double?)null;
                double? b = i < _sensorB.Count ? _sensorB.ElementAt(i) : (double?)null;
                rows.Add(new { A = a, B = b });
            }
            lvBothSensors.ItemsSource = rows;
        }
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            ShowAllSensorData();
            DisplayListboxData(_sensorA, lbSensorA);
            DisplayListboxData(_sensorB, lbSensorB);
        }
        private int NumberOfNodes(LinkedList<double> list) => list.Count;


        /// <summary>
        /// DisplayListboxData(list, listBox): binds the list’s values to the target ListBox.
        /// </summary>
        private void DisplayListboxData(LinkedList<double> list, ListBox listBox)
        {
            listBox.ItemsSource = list.ToList(); // display-only conversion (algorithms do NOT use arrays/lists)
        }
        private static bool IsSortedAscending(LinkedList<double> list)
        {
            if (list.Count < 2) return true;
            var node = list.First;
            while (node!.Next != null)
            {
                if (node.Value > node.Next!.Value) return false;
                node = node.Next;
            }
            return true;
        }
        private bool SelectionSort(LinkedList<double> list)
        {
            ISortStrategy sorter = new SelectionSortStrategy();
            return sorter.Sort(list);
        }
        private bool InsertionSort(LinkedList<double> list)
        {
            ISortStrategy sorter = new InsertionSortStrategy();
            return sorter.Sort(list);
        }
        private int BinarySearchIterative(LinkedList<double> list, double value, int min, int max)
        {
            IBinarySearchStrategy searcher = new IterativeBinarySearchStrategy();
            return searcher.Search(list, value, min, max);
        }
        private int BinarySearchRecursive(LinkedList<double> list, double value, int min, int max)
        {
            IBinarySearchStrategy searcher = new RecursiveBinarySearchStrategy();
            return searcher.Search(list, value, min, max);
        }
        private void btnSortASelection_Click(object sender, RoutedEventArgs e)
        {
            var sw = Stopwatch.StartNew();
            SelectionSort(_sensorA);
            sw.Stop();
            tbSortASelectionMs.Text = sw.ElapsedMilliseconds.ToString();
            ShowAllSensorData();
            DisplayListboxData(_sensorA, lbSensorA);
        }
        private void btnSortAInsertion_Click(object sender, RoutedEventArgs e)
        {
            var sw = Stopwatch.StartNew();
            InsertionSort(_sensorA);
            sw.Stop();
            tbSortAInsertionMs.Text = sw.ElapsedMilliseconds.ToString();
            ShowAllSensorData();
            DisplayListboxData(_sensorA, lbSensorA);
        }
        private void btnSortBSelection_Click(object sender, RoutedEventArgs e)
        {
            var sw = Stopwatch.StartNew();
            SelectionSort(_sensorB);
            sw.Stop();
            tbSortBSelectionMs.Text = sw.ElapsedMilliseconds.ToString();
            ShowAllSensorData();
            DisplayListboxData(_sensorB, lbSensorB);
        }
        private void btnSortBInsertion_Click(object sender, RoutedEventArgs e)
        {
            var sw = Stopwatch.StartNew();
            InsertionSort(_sensorB);
            sw.Stop();
            tbSortBInsertionMs.Text = sw.ElapsedMilliseconds.ToString();
            ShowAllSensorData();
            DisplayListboxData(_sensorB, lbSensorB);
        }
        private void btnRefreshBoth_Click(object sender, RoutedEventArgs e)
        {
            ShowAllSensorData();
        }
        private void btnSearchAIter_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSortedAscending(_sensorA)) { MessageBox.Show("Please sort Sensor A first."); return; }
            if (!int.TryParse(tbSearchA.Text, out int searchInt)) { MessageBox.Show("Enter an integer for Search A."); return; }


            var sw = Stopwatch.StartNew();
            int idx = BinarySearchIterative(_sensorA, searchInt, 0, NumberOfNodes(_sensorA));
            sw.Stop();
            tbSearchAIterTicks.Text = sw.ElapsedTicks.ToString();


            DisplayListboxData(_sensorA, lbSensorA);
            HighlightNeighbours(lbSensorA, idx);
        }
        private void btnSearchARec_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSortedAscending(_sensorA)) { MessageBox.Show("Please sort Sensor A first."); return; }
            if (!int.TryParse(tbSearchA.Text, out int searchInt)) { MessageBox.Show("Enter an integer for Search A."); return; }


            var sw = Stopwatch.StartNew();
            int idx = BinarySearchRecursive(_sensorA, searchInt, 0, NumberOfNodes(_sensorA));
            sw.Stop();
            tbSearchARecTicks.Text = sw.ElapsedTicks.ToString();


            DisplayListboxData(_sensorA, lbSensorA);
            HighlightNeighbours(lbSensorA, idx);
        }
        private void btnSearchBIter_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSortedAscending(_sensorB)) { MessageBox.Show("Please sort Sensor B first."); return; }
            if (!int.TryParse(tbSearchB.Text, out int searchInt)) { MessageBox.Show("Enter an integer for Search B."); return; }


            var sw = Stopwatch.StartNew();
            int idx = BinarySearchIterative(_sensorB, searchInt, 0, NumberOfNodes(_sensorB));
            sw.Stop();
            tbSearchBIterTicks.Text = sw.ElapsedTicks.ToString();


            DisplayListboxData(_sensorB, lbSensorB);
            HighlightNeighbours(lbSensorB, idx);
        }
        private void btnSearchBRec_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSortedAscending(_sensorB)) { MessageBox.Show("Please sort Sensor B first."); return; }
            if (!int.TryParse(tbSearchB.Text, out int searchInt)) { MessageBox.Show("Enter an integer for Search B."); return; }


            var sw = Stopwatch.StartNew();
            int idx = BinarySearchRecursive(_sensorB, searchInt, 0, NumberOfNodes(_sensorB));
            sw.Stop();
            tbSearchBRecTicks.Text = sw.ElapsedTicks.ToString();


            DisplayListboxData(_sensorB, lbSensorB);
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