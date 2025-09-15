using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSS
{
    public interface ISortStrategy
    {
        string Name { get; }


        /// <summary>
        /// Sort the linked list in-place.
        /// </summary>
        /// <param name="list">Target linked list of doubles.</param>
        /// <returns>true if sorted; false otherwise.</returns>
        bool Sort(LinkedList<double> list);
    }


    /// <summary>
    /// Binary search strategy abstraction – enables polymorphic iterative/recursive implementations.
    /// </summary>
    public interface IBinarySearchStrategy
    {
        string Name { get; }


        /// <summary>
        /// Search for a value in the linked list using provided min/max bounds (index space).
        /// Returns the element index if found; otherwise returns the nearest neighbour index (per spec).
        /// </summary>
        int Search(LinkedList<double> list, double value, int minimum, int maximum);
    }
}
