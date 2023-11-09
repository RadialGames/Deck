using System;
using System.Collections.Generic;
using System.Linq;

namespace Radial.Deck
{
    // A set of items that can be used on its own, or managed as part of a Deck
    public class Set<T>
    {
        // Most external interactions with this class is through Stacks, to enforce LIFO ordering. However, operations
        // with this Set are easier to do with a straight List, so we store the data that way.
        // Index 0 == top of deck, first to be drawn
        public List<T> Items;

        public int Size => Items.Count;
        
        /// <summary>
        /// Create a new Set that contains no elements.
        /// </summary>
        public Set()
        {
            Items = new List<T>();
        }
        
        /// <summary>
        /// Create a new Set with elements initialized
        /// </summary>
        /// <param name="items">Draw order is preserved, with the first index (0) being first drawn.</param>
        public Set(IEnumerable<T> items)
        {
            AddToTop(items);
        }
        
        public void RemoveAllOfType(T item)
        {
            while (Items.Contains(item))
            {
                Items.Remove(item);
            }
        }

        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Adds an item to the bottom of the set (will be drawn last)
        /// </summary>
        public void AddToBottom(T item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Adds multiple items to the bottom of the set. The top item of the provided items will be drawn first, once
        /// the rest of the Items are exhausted.
        /// </summary>
        /// <param name="items">Items presented in the order they should be drawn (index 0 being drawn before the others).</param>
        public void AddToBottom(IEnumerable<T> items)
        {
            Items.AddRange(items);
        }

        /// <summary>
        /// Adds an item to the top of the set (to be drawn first)
        /// </summary>
        public void AddToTop(T item)
        {
            AddAtIndex(0, item);
        }

        /// <summary>
        /// Adds items to the top of the set
        /// </summary>
        /// <param name="items">index 0 will be drawn first</param>
        public void AddToTop(IEnumerable<T> items)
        {
            Items.InsertRange(0, items);
        }

        public void AddAtIndex(int index, T item)
        {
            Items.Insert(index, item);
        }

        /// <summary>
        /// Draw a single item from the top of the set (LIFO)
        /// </summary>
        /// <returns></returns>
        public T Draw()
        {
            if (Size <= 0)
            {
                throw new Exception("Tried to draw more items than exist in the set");
            }
            
            var item = Items[0];
            Items.RemoveAt(0);
            return item;
        }

        /// <summary>
        /// Draw a number of items from the top of the set (LIFO)
        /// </summary>
        /// <param name="numToDraw">The number of items to draw</param>
        /// <output>A list of the drawn items, where index 0 is the first item drawn</output>
        /// <exception cref="ArgumentException">Will throw error if you ask for more cards than exist in the set</exception>
        public T[] Draw(int numToDraw)
        {
            if (numToDraw > Size)
            {
                throw new ArgumentException($"Cannot draw more items than are in the set. Requested: {numToDraw}, Available: {Size}");
            }

            var output = new T[numToDraw];
            for (int i = 0; i < numToDraw; i++)
            {
                output[i] = Draw();
            }

            return output;
        }
        
        public void Shuffle()
        {
            Items = Shuffler.Shuffle(Items).ToList();
        }
    }
}