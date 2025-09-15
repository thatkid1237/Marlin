using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSS
{
    public sealed class RecursiveBinarySearchStrategy : IBinarySearchStrategy
    {
        public string Name => "BinarySearchRecursive";


        public int Search(LinkedList<double> list, double value, int minimum, int maximum)
        {
            if (minimum <= maximum - 1)
            {
                int middle = (minimum + maximum) / 2;
                double midVal = list.ElementAt(middle);
                if (value == midVal) return middle;
                else if (value < midVal) return Search(list, value, minimum, middle - 1);
                else return Search(list, value, middle + 1, maximum);
            }
            return minimum < list.Count ? minimum : list.Count - 1;
        }
    }
}
