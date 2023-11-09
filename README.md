Deck
===========

## Concept

This is the `UPM` repository for Deck, the generic C# deck/card/shuffling abstraction library.

When designing games, I often think about random events and items in terms of "cards in a deck." This makes it easier to
conceptualize odds, weights, repetition, and probabilities.

This library is a generic implementation of that concept -- it doesn't presuppose cards, and instead uses generic types
of your own choosing.

Feel free to send feedback, submit pull requests, or open issues!

## Requirements

- This packages requires access to the `System` and `System.Collections.Generic` namespaces, which should be trivial for
any standard project package.
- Tested in Unity 2019.4.0f1, should work in anything newer.
- This package utilizes C# language features introduced in C# 8.0, and thus requires a compiler that supports that, eg:
.NET Core 3.x or .NET 5.0+

## Installation

Install it via the Unity Package Manager by:
- Opening your project in Unity
- Open the Package Manager window (`Window > Package Manager`)
- Click the `+` button in the top left corner of the window
- Select `Add package from git URL...`
- Enter the following url, and you'll be up to date: `https://github.com/RadialGames/Deck.git`

Or if you aren't using Unity, just grab the C# files and start using them as-is. Nothing in this package requires Unity;
and there are no Unity-specific dependencies. You can use these files directly in any C# project.

## Usage

All files in this package are in the `Radial.Deck` namespace. Access them by adding the following to the top of your
files:

```c#
using Radial.Deck;
```

### QuickStart

You can quickly create a new `Deck` and start using it with the following code:

```c#
var deck = new Deck<int>(); // Create the initial deck, and populate it
deck.Library.AddToTop(1);
deck.Library.AddToTop(2);
deck.Library.AddToTop(3);

int[] myHand = deck.Draw(2); // draws [3,2]

deck.Discard(myHand); // Discard your hand

myHand = deck.Draw(2); // Auto shuffles the discarded items back into the library, and draws [1,3] (non-deterministic)
```

Note that the type used when declaring the `Deck` is not locked to `int`, it can be of any basic type or your own
constructs.

### Set

The foundation of this package is a `Set<T>` of items. The items can be of any type. A `Set` could be thought of as
any collection of cards -- a draw pile, a discard pile, a graveyard, whatever your project needs.

Sets can be initialized with any `IEnumerable` such as a `Stack<T>` or a `Queue<T>` of items; the draw-order of items is
important and preserved, with the first index (0) of an array or List being drawn first.

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
request a certain number of cards to be Drawn, which will return an array of items:

```c#
var set = new Set<int>();
set.AddToTop(1);
set.AddToTop(2);
set.AddToTop(3);
var itemsInHand = set.Draw(2); // [3, 2]
```

It's important to note that a `Set` on its own does not track items that have been drawn - once you draw all the items
of a set, the Set will be empty, and you will have to re-populate it (and perhaps shuffle it).

```c#
var set = new Set<int>();
set.AddToTop(1);
var itemInHand = set.Draw(); // 1

set.Size(); // 0; calling Draw() again at this point will cause an Exception.
set.AddToBottom(itemInHand); // add the item back to the bottom of the Set
itemInHand = set.Draw(); // Draw a new item (still 1!)
```

You can `AddToBottom()`, `AddToTop()`, or `AddAtIndex()`, so you have flexibility on where items can be inserted.
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

(TODO: an InHand Set, that helps manage that common state further).

The primary role of the `Deck` class is to help automate the common process of drawing from a `Library`, and shuffling
`Discarded` items back into your Library when the Library is empty.

`Deck`s are initialized with a collection of items as the initial state of the system.

```c#
var initialLibrary = new int[] { 1, 2, 3 };

var deck = new Deck<int>(initialLibrary);
var itemInHand = deck.Draw(); // 1
deck.Discard(item); // place this item in the discard pile for future use

var itemsInHand = deck.Draw(3); // [2, 3, 1] - this is deterministic as there is only one item in the discard pile.
deck.Discard(itemsInHand); // place all the items back into discard.
deck.Library.Size(); // 0
deck.Discarded.Size(); // 3

deck.Draw(); // random result (shuffled)
```

A `Deck` also contains an `Exiled` set, which is not shuffled back into the `Library` unless explicitly requested.


```c#
deck.ReturnExiledToBottomOfLibrary();
deck.Library.Shuffle();
```

## Randomization

The `Set` and `Deck` classes both accept a `Random` object in their constructors, which can be used to control the
random seed used when `Set.Shuffle()` is called (thus making the shuffle, optionally, deterministic).

If no `Random` object is provided, a new one is generated with non-deterministic results.

You can call `Deck.ReplaceRandomProvider(new Random(mySeed))` to replace the random provider for all `Set`s in your
`Deck` in a single operation.
