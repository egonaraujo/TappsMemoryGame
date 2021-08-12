using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject CardPrefab;

    private List<GameObject> Cards;

    private void Start() 
    {
        Cards = new List<GameObject>();
        CreateCards();
        Random.InitState((int)System.DateTime.Now.Ticks);
        ShuffleDeck();
        PositionCards();
    }

    private void PositionCards()
    {
        //int 
    }

    private void ShuffleDeck()
    {
        for (int cardIndex = 0; cardIndex < Cards.Count; cardIndex++)
        {
            var swappedCard = Cards[cardIndex];
            var randomIndex = Random.Range(0,Cards.Count);
            Cards[cardIndex] = Cards[randomIndex];
            Cards[randomIndex] = swappedCard;
        }
    }

    public void OnCardClicked(int pairId)
    {
        Debug.Log("Clicked on me " + pairId.ToString());
    }

    private void CreateCards()
    {
        for (int pairId = 0; pairId < GameManager.Instance.NumberOfPairs; pairId++)
        {
            CreateSingleCard(pairId);
            CreateSingleCard(pairId);
        }
    }

    private void CreateSingleCard(int pairId)
    {
        var newCard = Instantiate(CardPrefab);
        var newCardController = newCard.GetComponent<CardController>();
        newCardController.Init(pairId);
        newCardController.OnCardClicked += OnCardClicked;
        newCard.transform.SetParent(gameObject.transform);
        Cards.Add(newCard);
    }
}
