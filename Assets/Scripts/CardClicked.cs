using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;



public class CardClicked : MonoBehaviour, IPointerDownHandler
{

    public HandManager handManager;

    private void Start()
    {
        handManager = FindObjectOfType<HandManager>();
        AddPhysics2DRaycaster();
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform); // Kill tweens associated with this transform
    }

    private void AddPhysics2DRaycaster()
    {
        Physics2DRaycaster physicsRaycaster = FindObjectOfType<Physics2DRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject clickedGameObject = eventData.pointerCurrentRaycast.gameObject;
        Card clickedCard = handManager.CardsInHand
            .FirstOrDefault(card => card.AssociatedPrefab == clickedGameObject);

        if (clickedCard == null) return;

        Rank selectedRank = clickedCard.Rank;

        // Deselect other cards of a different rank
        foreach (Card card in handManager.CardsInHand)
        {
            if (card != clickedCard && card.Rank != selectedRank && card.Select)
            {
                card.Select = false;
                handManager.Deselected(card);
            }
        }

        // Toggle the selection of the clicked card
        clickedCard.Select = !clickedCard.Select;

        if (clickedCard.Select)
        {
            handManager.Selected(clickedCard);
        }
        else
        {
            handManager.Deselected(clickedCard);
        }
    }
}
