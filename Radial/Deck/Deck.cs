using System;
using System.Collections.Generic;

namespace Radial.Deck
{
    public class Deck<T>
    {
        public Set<T> Library;
        public Set<T> Discarded;
        public Set<T> Exiled;

        public int LibraryAndDiscardSize => Library.Size + Discarded.Size;

        public Deck(Set<T> library)
        {
            Library = library;
            Discarded = new Set<T>();
            Exiled = new Set<T>();
        }

        /// <summary>
        /// Moves all items in the Discarded set to the bottom of the Library set.
        /// </summary>
        public Deck<T> ReturnDiscardsToBottomOfLibrary()
        {
            Library.AddToBottom(new Stack<T>(Discarded.Items));
            Discarded.Clear();
            return this;
        }

        /// <summary>
        /// Moves all items in the Exiled set to the bottom of the Library set.
        /// </summary>
        public Deck<T> ReturnExiledToBottomOfLibrary()
        {
            Library.AddToBottom(new Stack<T>(Exiled.Items));
            Exiled.Clear();
            return this;
        }

        /// <summary>
        /// Draws an item, shuffling the Discards back into the Library if necessary.
        /// You can Draw from the Library directly if you do not want this behaviour.
        /// </summary>
        public T Draw()
        {
            if (Library.Size == 0)
            {
                ReturnDiscardsToBottomOfLibrary();
                if (Library.Size == 0)
                {
                    throw new ArgumentException("Requested an item when there are no items in the Library or Discard pile.");
                }
                Library.Shuffle();
            }

            return Library.Draw();
        }

        /// <summary>
        /// Draws a number of items, shuffling the Discards back into the Library if necessary.
        /// You can Draw from the Library directly if you do not want this behaviour.
        /// </summary>
        public List<T> Draw(int numToDraw)
        {
            if (numToDraw > LibraryAndDiscardSize)
            {
                throw new ArgumentException($"Cannot draw more items than are in the Library & Discard pile. Requested: {numToDraw}, Available: {LibraryAndDiscardSize}");
            }
            var drawn = new List<T>();
            for (var i = 0; i < numToDraw; i++)
            {
                drawn.Add(Draw());
            }

            return drawn;
        }
    }
}