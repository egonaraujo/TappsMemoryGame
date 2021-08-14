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
        var padding = 20f;
        var drawingWidth = Camera.main.pixelWidth - 2 * padding;
        var drawingHeight = Camera.main.pixelHeight - 2 * padding;
        Vector2 cardGrid = CalculateCardGrid();
        var widthSpaceEachCard = (float)drawingWidth / (float)cardGrid.x;
        var heightSpaceEachCard = (float)drawingHeight / (float)cardGrid.y;
        ResizeCards(widthSpaceEachCard, heightSpaceEachCard);
        var canvasBottomLeftOrigin = new Vector3(-drawingHeight/2, -drawingWidth/2);

        var cardsIndex = 0;
        for (int y = 0; y < cardGrid.y; y++)
        {
            for (int x = 0; x < cardGrid.x; x++)
            {
                Cards[cardsIndex].transform.localPosition = canvasBottomLeftOrigin 
                                                            + new Vector3(x*widthSpaceEachCard, y*heightSpaceEachCard);
                cardsIndex++;
            }
        }
    }

    private void ResizeCards(float widthSpaceEachCard, float heightSpaceEachCard)
    {
        //throw new System.NotImplementedException();
        return;
    }

    private Vector2 CalculateCardGrid()
    {
        var nbrOfPairs = GameManager.Instance.NumberOfPairs;

        // minimal grid is a line of "pairs" width, "2" height
        var minSize = 2;
        var maxSize = nbrOfPairs;
        var nbrOfCards = nbrOfPairs * 2;

        for (int i = 3; i < maxSize; i++)
        {
            if (nbrOfPairs % i == 0)
            {
                minSize = i;
                maxSize = nbrOfCards / i;
            }
        }

        return new Vector2(maxSize,minSize);
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
