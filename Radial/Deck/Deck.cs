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

        /// <summary>
        /// Creates a Deck with no elements set in any of the Sets.
        /// </summary>
        public Deck() : this(Array.Empty<T>(), new Random()) { }
        
        /// <summary>
        /// Creates a Deck with no elements set in any of the Sets, and with a shared Random provider
        /// </summary>
        /// <param name="random">A random provider to be used for shuffling</param>
        public Deck(Random random) : this(Array.Empty<T>(), random) { }

        /// <summary>
        /// Creates a Deck with the provided elements being populated to the Library.
        /// </summary>
        /// <param name="library">The set to be populated to the Library</param>
        public Deck(IEnumerable<T> library) : this(library, new Random()) { }

        /// <summary>
        /// Creates a Deck with the provided elements being populated to the Library.
        /// </summary>
        /// <param name="library">The set to be populated to the Library</param>
        /// <param name="random">A random provider to be used for shuffling</param>
        public Deck(IEnumerable<T> library, Random random)
        {
            Library = new Set<T>(library, random);
            Discarded = new Set<T>(random);
            Exiled = new Set<T>(random);
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

        /// <summary>
        /// Replaces the existing Random provider with a new one, for use in shuffling.
        /// </summary>
        public void ReplaceRandomProvider(Random newProvider)
        {
            Library.ReplaceRandomProvider(newProvider);
            Discarded.ReplaceRandomProvider(newProvider);
            Exiled.ReplaceRandomProvider(newProvider);
        }
    }
}