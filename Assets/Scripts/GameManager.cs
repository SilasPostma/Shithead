using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    public HandManager handManager;
    public GameObject backDeck;
    private float zAxis = -1f;
    private float zAxis2 = -1f;
    public List<Card> PlayedCards = new();
    public List<Card> DiscardPile = new();

    public TextMeshProUGUI WinText;




    private void Start()
    {
        handManager = FindObjectOfType<HandManager>();
    }

    // Update the HandPlayed method
    public void HandPlayed(List<Card> selectedCardPrefabs)
    {
        if (CheckAllowed(selectedCardPrefabs[0], selectedCardPrefabs.Count))
        {
            foreach (Card cardPrefab in selectedCardPrefabs)
            {
                // Add to played pile
                PlayedCards.Add(cardPrefab);


                // Animate the card prefab to its new position and rotation
                GameObject go = cardPrefab.AssociatedPrefab;

                var random = new System.Random();
                int rotationOffset = random.Next(-10, 10);

                go.transform.DOMove(new Vector3(0, 1f, -zAxis), 0.25f);
                go.transform.DOLocalRotate(new Vector3(0, 0, rotationOffset), 0.25f);

                zAxis += 0.01f;

                handManager.CardsInHand.Remove(cardPrefab);
                handManager.curHandSize--;
            }
            if (handManager.CardsInHand.Count == 0 && handManager.deck.CardsRemaining == 0)
            {
                StartCoroutine(Won());
            }

            int cardDiff = -handManager.CardsInHand.Count + handManager.startHandSize;

            handManager.StartCoroutine(handManager.DrawEnough(cardDiff));

            handManager.UpdateCardPositions();


            StartCoroutine(WaitForDraw());
        }
        else
        {
            foreach (Card cardPrefab in selectedCardPrefabs)
            {
                StartCoroutine(NoShake(cardPrefab, 10));
            }

        }
    }
    private System.Collections.IEnumerator Won()
    {
        if (WinText != null)
        {
            WinText.text = "You Win!";
        }
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0;
    }


    private System.Collections.IEnumerator NoShake(Card cardToShake, int shakes)
    {
        float shakeAngle = 3f;
        for (int i = 0; i < shakes; i++)
        {
            cardToShake.AssociatedPrefab.transform.DOLocalRotate(new Vector3(0, 0, shakeAngle), 0.05f);
            shakeAngle = -shakeAngle;
            yield return new WaitForSeconds(0.05f);
        }

        int index = handManager.CardsInHand.IndexOf(cardToShake);
        handManager.CalculateCardPosition(index, handManager.firstCardPosition, handManager.spline);

    }

    private bool IsMoveAllowed(Card cardToCheck, int amountPlayed)
    {
        // Similar to CheckAllowed but without modifying state
        List<Card> relevantCards = PlayedCards.Where(card => (int)card.Rank != 3).ToList();

        for (int i = 0; i < amountPlayed; i++)
        {
            relevantCards.Add(cardToCheck);
        }

        if (relevantCards.Count >= 4)
        {
            var topFour = relevantCards.Skip(relevantCards.Count - 4).ToList();
            if (topFour.All(card => card.Rank == topFour.First().Rank))
            {
                return true;
            }
        }

        // if (PlayedCards.Count == 0)
        // {
        //     if ((int)cardToCheck.Rank == 10 || (int)cardToCheck.Rank != 10)
        //     {
        //         return true; // Any card can be played if no cards are on the board
        //     }
        // }

        if ((int)cardToCheck.Rank == 2 || (int)cardToCheck.Rank == 3)
        {
            return true;
        }

        Card lastRelevantCard = null;
        for (int i = PlayedCards.Count - 1; i >= 0; i--)
        {
            if ((int)PlayedCards[i].Rank != 3)
            {
                lastRelevantCard = PlayedCards[i];
                break;
            }
        }

        if (lastRelevantCard != null)
        {
            if ((int)lastRelevantCard.Rank == 7)
            {
                return cardToCheck.Rank <= lastRelevantCard.Rank;
            }

            return cardToCheck.Rank >= lastRelevantCard.Rank;
        }

        return true;
    }

    private bool CheckAllowed(Card cardToCheck, int amountPlayed)
    {
        // Use IsMoveAllowed for validation
        if (!IsMoveAllowed(cardToCheck, amountPlayed))
        {
            return false;
        }

        // Original logic that modifies state
        List<Card> relevantCards = PlayedCards.Where(card => (int)card.Rank != 3).ToList();

        for (int i = 0; i < amountPlayed; i++)
        {
            relevantCards.Add(cardToCheck);
        }

        if (relevantCards.Count >= 4)
        {
            var topFour = relevantCards.Skip(relevantCards.Count - 4).ToList();
            if (topFour.All(card => card.Rank == topFour.First().Rank))
            {
                DiscardPile = PlayedCards;
                PlayedCards.Add(cardToCheck);
                PlayedCards.Reverse();
                StartCoroutine(ToDiscardMove());
                return true;
            }
        }

        if (PlayedCards.Count == 0)
        {
            if ((int)cardToCheck.Rank == 10)
            {
                DiscardPile = PlayedCards;
                PlayedCards.Add(cardToCheck);
                PlayedCards.Reverse();
                StartCoroutine(ToDiscardMove());
                return true;
            }
            return true;
        }

        if ((int)cardToCheck.Rank == 2 || (int)cardToCheck.Rank == 3)
        {
            return true;
        }

        Card lastRelevantCard = null;
        for (int i = PlayedCards.Count - 1; i >= 0; i--)
        {
            if ((int)PlayedCards[i].Rank != 3)
            {
                lastRelevantCard = PlayedCards[i];
                break;
            }
        }

        if (lastRelevantCard != null)
        {
            if ((int)lastRelevantCard.Rank == 7)
            {
                return cardToCheck.Rank <= lastRelevantCard.Rank;
            }

            if ((int)cardToCheck.Rank == 10 && cardToCheck.Rank >= lastRelevantCard.Rank)
            {
                DiscardPile = PlayedCards;
                PlayedCards.Add(cardToCheck);
                PlayedCards.Reverse();
                StartCoroutine(ToDiscardMove());
                return true;
            }

            return cardToCheck.Rank >= lastRelevantCard.Rank;
        }
        else if (lastRelevantCard == null && (int)cardToCheck.Rank == 10)
        {
            StartCoroutine(ToDiscardMove());
            return true;

        }

        return true;
    }

    private bool HasMove(List<Card> cards)
    {
        var distinctCards = cards.Distinct().ToList();

        foreach (Card card in distinctCards)
        {
            if (IsMoveAllowed(card, 1))  // Use IsMoveAllowed instead of CheckAllowed
            {
                return true;
            }
        }

        return false;
    }



    private System.Collections.IEnumerator ToDiscardMove()
    {
        yield return new WaitForSeconds(0.25f);

        foreach (Card cardInPile in PlayedCards)
        {
            cardInPile.AssociatedPrefab.transform.DOMove(new Vector3(-7.25f, 0.5f, -zAxis2), 0.25f);
            zAxis2 += 0.01f;
            yield return new WaitForSeconds(0.03f);
        }
        DiscardDisplay();
        yield return new WaitForSeconds(0.05f);

        foreach (Card cardInPile in PlayedCards)
        {
            Destroy(cardInPile.AssociatedPrefab);
        }

        PlayedCards.Clear();
    }

    private void DiscardDisplay()
    {
        GameObject singleDiscard = Instantiate(
            backDeck,
            new Vector3(-7.25f, 0.6f, 1f),
            Quaternion.identity
        );
    }

    private System.Collections.IEnumerator WaitForDraw()
    {
        yield return new WaitForSeconds(0.25f);

        if (!HasMove(handManager.CardsInHand))
        {
            yield return new WaitForSeconds(1f);

            foreach (Card cardToAdd in PlayedCards)
            {
                handManager.CardsInHand.Add(cardToAdd);
                handManager.curHandSize++;
            }
            handManager.SortRank();
            handManager.UpdateCardPositions();
            PlayedCards.Clear();
        };
    }
}
