using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Deck
{
    public List<Card> cards;

    private List<GameObject> cardPrefabs;

    public Deck(List<GameObject> prefabs)
    {
        cardPrefabs = prefabs;
        cards = new List<Card>();
        InitializeDeck();
        Shuffle();
    }

    private void InitializeDeck()
    {
        int prefabIndex = 0;

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                GameObject prefab = cardPrefabs[prefabIndex % 26];
                cards.Add(new Card(suit, rank, prefab, false));
                prefabIndex++;
            }
        }

        cards.Add(new Card(Suit.Hearts, Rank.Ace, cardPrefabs[26], false));
        cards.Add(new Card(Suit.Spades, Rank.Ace, cardPrefabs[27], false));

        prefabIndex += 2;
    }

    public void Shuffle()
    {
        // Shuffle the flat list of cards
        System.Random rng = new System.Random();
        int n = cards.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (cards[n], cards[k]) = (cards[k], cards[n]);
        }
    }

    public Card Draw()
    {
        if (cards.Count == 0)
        {
            throw new InvalidOperationException("No cards left in the deck.");
        }

        Card topCard = cards[0];
        cards.RemoveAt(0);
        return topCard;
    }

    public int CardsRemaining => cards.Count;
}
