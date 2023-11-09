Deck
===========

This is the Unity Package Manager repository for Deck, the generic C# deck/card/shuffling library. 

Install it via `UPM`:
```
https://github.com/RadialGames/Deck.git
```

Or if you aren't using Unity, just grab the C# files and start using them as-is.

## Concept

When designing games, I often think about random events and items in terms of "cards in a deck." This makes it easier to
conceptualize odds, weights, repetition, and probabilities.

This library is a generic implementation of that concept -- it doesn't 
presuppose cards, and instead uses generic types of your own choosing.

Nothing in this package requires Unity; you can use these files directly in any C# project.

## Usage

### Set

The foundation of this package is a `Set<T>` of items. The items can be of any type. A `Set` could be thought of as
a hand of cards, a draw pile, or a discard pile.

Sets can be initialized with a `Stack<T>` or a `Queue<T>` of items; the draw-order of items is important and preserved.

```c#
var items = new Stack<int>(); // LIFO
items.Add(2);
items.Add(1); // top card on the deck, will be drawn first

var set = new Set<int>(items);
set.Draw(); // 1
set.Draw(); // 2
```

You can also initialize a `Set` with no items, and add them later.

```c#
var set = new Set<string>();
set.Size(); // 0
set.AddToTop("foo");
set.AddToTop("bar");

set.Draw(); // "bar"
```

As demonstrated above, the `Draw()` function will remove an item from the top of the Set, and return it. You can also
request a certain number of cards to be Drawn, which will return a List of items:

```c#
var set = new Set<int>();
set.AddToTop(1);
set.AddToTop(2);
set.AddToTop(3);
var itemsInHand = Set.Draw(2); // [3, 2]
```

It's important to note that a `Set` on its own does not track items that have been drawn - once you draw all the items
of a set, the Set will be empty, and you will have to re-populate it (and perhaps shuffle it).

```c#
var set = new Set<int>();
set.AddToTop(1);
var itemInHand = Set.Draw(); // 1

set.Size(); // 0; calling Draw() again at this point will cause an Exception.
set.AddToBottom(itemInHand); // add the item back to the bottom of the Set
itemInHand = Set.Draw(); // Draw a new item (still 1!)
```

You can `AddToBottom()` or `AddToTop()` of a Set, which will add the item to the bottom or top of the Set, respectively.
This will alter the order in which items will be drawn; items are always drawn from the top first.

```c#
var set = new Set<int>();
set.AddToTop(1);
set.AddToTop(2);
set.AddToBottom(3);

set.Draw(); // 2
```

You can also `Clear()` a Set to wipe it completely, and `Shuffle()` a set to randomize the order of the items inside.

### Deck

A Deck is a helper collection of Sets that can be shuffled and drawn from; it tracks an individual Library, Discarded,
and Exiled Set. It can be thought of as all the cards in the box, minus anything that is "in play" (and managed by your
own game logic).

(coming soon: an InHand Set, that helps manage the state further).

The primary role of the `Deck` class is to help automate the common process of drawing from a `Library`, and shuffling
`Discarded` items back into your Library when the Library is empty.

`Deck`s are initialized with a `Set` of items as the initial state of the system.

```c#
var initialLibrary = new Set<int>();
initialLibrary.AddToBottom(1);
initialLibrary.AddToBottom(2);
initialLibrary.AddToBottom(3);

var deck = new Deck<int>(initialLibrary);
var item = deck.Draw(); // 1
deck.Discarded.AddToTop(item); // place this item in the discard pile for future use

var items = deck.Draw(3); // [2, 3, 1] - this is deterministic as there is only one item in the discard pile.
deck.Discarded.AddToTop(items); // place all the items back into discard.
deck.Library.Size(); // 0
deck.Discarded.Size(); // 3

deck.Draw(); // random result (shuffled)
```

A `Deck` also contains an `Exiled` set, which is not shuffled back into the `Library` unless explicitly requested.


```c#
deck.ReturnExiledToBottomOfLibrary();
deck.Library.Shuffle();
```

### Shuffler

This is a static class that implements a simple Fischer-Yates randomization algorithm. It is used
to hold a `System.Random` instance for (optional) deterministic results.

By default, if you ignore this class, it will self-initialize and produce random results each time. Alternatively;

```C#
var seed = 12345; 
Shuffler.SetRandomSeed(seed);

deck.Library.Shuffle();
deck.Draw(); // deterministic result
```