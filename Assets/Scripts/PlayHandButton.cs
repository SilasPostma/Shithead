using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PlayHandButton : MonoBehaviour
{
    public HandManager handManager;
    public GameManager gameManager;
    private bool OnCooldown;

    private void Start()
    {
        handManager = FindObjectOfType<HandManager>();
        gameManager = FindObjectOfType<GameManager>();
        OnCooldown = false;
    }
    public void SortHand()
    {
        Debug.Log("Button clicked!");
        handManager.SortRank();
    }

    public void PlayHand()
    {
        if (!OnCooldown)
        {
            StartCoroutine(StartCooldown());
            List<Card> selectedCards = handManager.CardsInHand
                .Where(card => card.Select) // Properly reference Select as a property
                .ToList();

            if (selectedCards.Count == 0) return;

            gameManager.HandPlayed(selectedCards);
        }
    }

    public System.Collections.IEnumerator StartCooldown()
    {
        OnCooldown = true;
        yield return new WaitForSeconds(0.5f);
        OnCooldown = false;
    }
}
