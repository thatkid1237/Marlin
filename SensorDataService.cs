using Galileo6;
using System;
using System.Collections.Generic;

namespace MSSS
{
    public enum SortAlgorithm
    {
        Selection,
        Insertion
    }

    public enum SearchMode
    {
        Iterative,
        Recursive
    }

    public sealed class SensorDataModel
    {
        public LinkedList<double> SensorA { get; } = new();
        public LinkedList<double> SensorB { get; } = new();
    }

    public sealed class SensorDataService
    {
        private readonly ReadData _reader;
        private readonly ISortStrategy _selectionSort;
        private readonly ISortStrategy _insertionSort;
        private readonly IBinarySearchStrategy _iterativeSearch;
        private readonly IBinarySearchStrategy _recursiveSearch;

        public SensorDataService()
            : this(new ReadData(),
                   new SelectionSortStrategy(),
                   new InsertionSortStrategy(),
                   new IterativeBinarySearchStrategy(),
                   new RecursiveBinarySearchStrategy())
        {
        }

        public SensorDataService(
            ReadData reader,
            ISortStrategy selectionSort,
            ISortStrategy insertionSort,
            IBinarySearchStrategy iterativeSearch,
            IBinarySearchStrategy recursiveSearch)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _selectionSort = selectionSort ?? throw new ArgumentNullException(nameof(selectionSort));
            _insertionSort = insertionSort ?? throw new ArgumentNullException(nameof(insertionSort));
            _iterativeSearch = iterativeSearch ?? throw new ArgumentNullException(nameof(iterativeSearch));
            _recursiveSearch = recursiveSearch ?? throw new ArgumentNullException(nameof(recursiveSearch));
        }

        public SensorDataModel Load(double mu, double sigma, int sampleCount)
        {
            if (sampleCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sampleCount), "Sample count must be greater than zero.");
            }

            var model = new SensorDataModel();
            Populate(model.SensorA, sampleCount, () => _reader.SensorA(mu, sigma));
            Populate(model.SensorB, sampleCount, () => _reader.SensorB(mu, sigma));
            return model;
        }

        public bool Sort(LinkedList<double> list, SortAlgorithm algorithm)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            var strategy = algorithm switch
            {
                SortAlgorithm.Selection => _selectionSort,
                SortAlgorithm.Insertion => _insertionSort,
                _ => throw new ArgumentOutOfRangeException(nameof(algorithm))
            };

            return strategy.Sort(list);
        }

        public int Search(LinkedList<double> list, double value, SearchMode mode)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            var strategy = mode switch
            {
                SearchMode.Iterative => _iterativeSearch,
                SearchMode.Recursive => _recursiveSearch,
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };

            return strategy.Search(list, value, 0, list.Count);
        }

        public static bool IsSortedAscending(LinkedList<double> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count < 2) return true;

            var node = list.First;
            while (node!.Next != null)
            {
                if (node.Value > node.Next!.Value)
                {
                    return false;
                }
                node = node.Next;
            }

            return true;
        }

        private static void Populate(LinkedList<double> target, int count, Func<double> sampleProvider)
        {
            target.Clear();
            for (int i = 0; i < count; i++)
            {
                target.AddLast(sampleProvider());
            }
        }
    }
}
