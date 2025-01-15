using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum Suit
{
    Hearts = 1,
    Diamonds,
    Clubs,
    Spades
}

public enum Rank
{
    Two = 2,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}

public class Card
{
    public Suit Suit { get; }
    public Rank Rank { get; }
    public string Name { get; }
    public GameObject AssociatedPrefab { get; set; }
    public bool Select { get; set; }

    public Card(Suit suit, Rank rank, GameObject prefab, bool selected)
    {
        Suit = suit;
        Rank = rank;
        Name = $"{Rank} of {Suit}";
        AssociatedPrefab = prefab;
        Select = selected;
    }

    public int CalculateIndex()
    {
        return ((int)Suit - 1) * 13 + (int)Rank;
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}

