using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSS
{
    public sealed class IterativeBinarySearchStrategy : IBinarySearchStrategy
    {
        public string Name => "BinarySearchIterative";


        public int Search(LinkedList<double> list, double value, int minimum, int maximum)
        {
            while (minimum <= maximum - 1)
            {
                int middle = (minimum + maximum) / 2; 
                double midVal = list.ElementAt(middle);
                if (value == midVal) return middle;
                else if (value < midVal) maximum = middle - 1;
                else minimum = middle + 1;
            }
            // nearest neighbour index fallback
            return minimum < list.Count ? minimum : list.Count - 1;
        }
    }
}
