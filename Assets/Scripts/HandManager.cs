using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using System.Linq;
using TMPro;


public class HandManager : MonoBehaviour
{
    public int startHandSize = 3;
    public int curHandSize = 0;
    public TextMeshProUGUI cardsRemainingText;
    //private int maxHandSize = 20;

    [SerializeField] private List<GameObject> cardPrefabs;

    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;

    public float cardSpacing;

    public List<Card> CardsInHand = new();

    public Deck deck;

    public float firstCardPosition;
    public Spline spline;
    public GameObject deckBack; // Reference to the "deck_back" GameObject



    private void Start()
    {
        deck = new Deck(cardPrefabs);
        spline = splineContainer.Spline;

        UpdateCardsRemainingText();  // Initialize text on start

        StartCoroutine(DrawEnough(startHandSize));

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) DrawCard();
        if (Input.GetMouseButtonDown(1))
        {
            foreach (Card card in CardsInHand)
            {
                Deselected(card);
            }
        };
    }

    public System.Collections.IEnumerator DrawEnough(int amount)
    {
        //yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < amount; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(0.03f);
        }
    }

    private void UpdateCardsRemainingText()
    {
        if (cardsRemainingText != null)
        {
            cardsRemainingText.text = "[" + deck.CardsRemaining.ToString() + "]";
        }
        // If there are no cards left, destroy the deck (draw pile)
        if (deck.CardsRemaining == 0 && deckBack != null)
        {
            Destroy(deckBack);  // Destroy the deck_back GameObject
        }
    }

    private void DrawCard()
    {
        if (deck.cards.Count <= 0) return;

        Card curCard = deck.Draw();
        UpdateCardsRemainingText();  // Initialize text on start

        GameObject instantiatedCard = Instantiate(
            curCard.AssociatedPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );
        curCard.AssociatedPrefab = instantiatedCard;

        CardsInHand.Add(curCard);
        SortRank();
        curHandSize++;
        UpdateCardPositions();
    }

    public void UpdateCardPositions()
    {
        if (CardsInHand.Count == 0) return;

        cardSpacing = 0.125f;

        if (curHandSize >= 8) cardSpacing = 1f / curHandSize;
        firstCardPosition = 0.5f - (CardsInHand.Count - 1) * cardSpacing / 2;


        for (int i = 0; i < CardsInHand.Count; i++)
        {
            Card card = CardsInHand[i];
            CalculateCardPosition(i, firstCardPosition, spline);
        }
    }
    public void Selected(Card card)
    {
        // Calculate the vector between the card's "up" vector and the global Y-axis
        Vector3 localUp = card.AssociatedPrefab.transform.up;
        Vector3 globalY = Vector3.up;
        Vector3 blendedUp = (localUp + globalY).normalized;

        // Move the card along this blended vector
        card.AssociatedPrefab.transform.DOBlendableMoveBy(blendedUp * 0.5f, 0.25f);
    }


    public void Deselected(Card card)
    {
        int index = CardsInHand.IndexOf(card);
        CalculateCardPosition(index, firstCardPosition, spline);
    }

    public void CalculateCardPosition(int i, float firstCardPosition, Spline spline)
    {
        float p = firstCardPosition + i * cardSpacing;
        Vector3 splinePosition = spline.EvaluatePosition(p);
        Vector3 forward = spline.EvaluateTangent(p);
        Vector3 up = spline.EvaluateUpVector(p);
        Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

        float zOffset = i * 0.01f;
        float yOffset = -Math.Min(CardsInHand.Count * 0.0075f, 0.135f);
        Vector3 adjustedPosition = splinePosition - new Vector3(0, yOffset, zOffset);

        GameObject cardGameObject = CardsInHand[i].AssociatedPrefab;
        cardGameObject.transform.DOMove(adjustedPosition, 0.25f);
        cardGameObject.transform.DOLocalRotateQuaternion(rotation, 0.25f);
    }

    public void SortRank()
    {
        // Sort the cards in hand by rank
        CardsInHand = CardsInHand.OrderBy(card => (int)card.Rank).ToList();
    }
}
