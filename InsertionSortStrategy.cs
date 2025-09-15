using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSS
{
    public sealed class InsertionSortStrategy : ISortStrategy
    {
        public string Name => "InsertionSort";


        public bool Sort(LinkedList<double> list)
        {
            int max = list.Count;
            for (int i = 0; i < max - 1; i++)
            {
                for (int j = i + 1; j > 0; j--)
                {
                    if (list.ElementAt(j - 1) > list.ElementAt(j))
                    {
                        LinkedListNode<double> current = list.Find(list.ElementAt(j));
                        LinkedListNode<double> prev = current!.Previous!;
                        double temp = prev.Value;
                        prev.Value = current.Value;
                        current.Value = temp;
                    }
                    else break; 
                }
            }
            return true;
        }
    }
}
