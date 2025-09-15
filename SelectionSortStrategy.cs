using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSS
{
    public sealed class SelectionSortStrategy : ISortStrategy
    {
        public string Name => "SelectionSort";


        public bool Sort(LinkedList<double> list)
        {
            int max = list.Count;
            for (int i = 0; i < max - 1; i++)
            {
                int min = i;
                for (int j = i + 1; j < max; j++)
                {
                    if (list.ElementAt(j) < list.ElementAt(min))
                        min = j;
                }


                // Supplied code – find nodes for swap
                LinkedListNode<double> currentMin = list.Find(list.ElementAt(min));
                LinkedListNode<double> currentI = list.Find(list.ElementAt(i));


                // Swap node values (not nodes) to keep structure simple
                double temp = currentMin!.Value;
                currentMin.Value = currentI!.Value;
                currentI.Value = temp;
            }
            return true;
        }
    }
}
