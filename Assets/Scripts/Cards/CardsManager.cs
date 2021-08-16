using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CardPrefab;

    [HideInInspector]
    public List<GameObject> Cards;
    
    public System.Action OnEndGame;

    public class SelectedCard
    {
        public GameObject GameObject;
        public int PairId;
        public CardController CardController;
        public bool Keep = false;

        public SelectedCard(
            GameObject gameObject,
            int pairId,
            CardController cardController,
            bool keep)
        {
            GameObject = gameObject;
            PairId = pairId;
            CardController = cardController;
            Keep = keep;
        }
    }

    public SelectedCard firstSelectedCard;

    public SelectedCard secondSelectedCard;

    private void Awake()
    {
        Cards = new List<GameObject>();
        CreateCards();
        Random.InitState((int)System.DateTime.Now.Ticks);
        ShuffleDeck();
        PositionCards();
    }
    
    public void SelectCard(GameObject cardGameObject, int pairId)
    {
        if( firstSelectedCard!=null 
            && firstSelectedCard.Keep
            && firstSelectedCard.GameObject == cardGameObject)
        {
            firstSelectedCard.Keep = false;
            return;
        }

        if( secondSelectedCard != null 
            && secondSelectedCard.Keep
            && secondSelectedCard.GameObject == cardGameObject)
        {
            secondSelectedCard.Keep = false;
            firstSelectedCard = secondSelectedCard;
            secondSelectedCard = null;
            return;
        }
        
        SaveAndTurnCardUp(cardGameObject, pairId);
    }

    public void SaveAndTurnCardUp(GameObject cardGameObject, int pairId)
    {
        var newSelectedCard = new SelectedCard(
            cardGameObject,
            pairId,
            cardGameObject.GetComponent<CardController>(),
            false);
        newSelectedCard.CardController.TurnCardUp();

        if(firstSelectedCard == null)
        {
            firstSelectedCard = newSelectedCard;
        }
        else
        {
            secondSelectedCard = newSelectedCard;
        }
    }

    public void KeepCard(GameObject cardGameObject)
    {
        if(firstSelectedCard != null && cardGameObject == firstSelectedCard.GameObject)
        {
            firstSelectedCard.Keep = true;
        }

        if(secondSelectedCard != null && cardGameObject == secondSelectedCard.GameObject)
        {
            secondSelectedCard.Keep = true;
        }
    }

    public void UnselectCards()
    {
        if(!firstSelectedCard.Keep)
        {
            firstSelectedCard.CardController.TurnCardDown();
            firstSelectedCard = null;
        }

        if(!secondSelectedCard.Keep)
        {
            secondSelectedCard.CardController.TurnCardDown();
            secondSelectedCard = null;
        }
    }

    public bool CheckCardsPair()
    {
        if(firstSelectedCard.PairId == secondSelectedCard.PairId)
        {
            StartCoroutine(DestroyCards());
            return true;
        }
        else
        {
            return false;
        }
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
        newCard.transform.SetParent(gameObject.transform);
        Cards.Add(newCard);
    }

    private void PositionCards()
    {
        var bordersPadding = 20f;
        var topPadding = 60f;
        var drawingWidth = Camera.main.pixelWidth - 2 * bordersPadding;
        var drawingHeight = Camera.main.pixelHeight - 2 * bordersPadding - topPadding;
        Vector2 cardGrid = CalculateCardGrid();
        var widthSpaceEachCard = (float)drawingWidth / (float)cardGrid.x;
        var heightSpaceEachCard = (float)drawingHeight / (float)cardGrid.y;
        var paddingBetweenCards = 8f;
        ResizeCards(widthSpaceEachCard, heightSpaceEachCard, drawingWidth, drawingHeight, paddingBetweenCards);
        var canvasBottomLeftOrigin = new Vector3(-drawingWidth/2, -(drawingHeight + topPadding)/2);

        var cardsIndex = 0;
        for (int y = 0; y < cardGrid.y; y++)
        {
            for (int x = 0; x < cardGrid.x; x++)
            {
                Cards[cardsIndex].transform.localPosition = canvasBottomLeftOrigin
                                                            + new Vector3(x * widthSpaceEachCard + paddingBetweenCards,
                                                                          y * heightSpaceEachCard + paddingBetweenCards);
                cardsIndex++;
            }
        }
    }

    private Vector2 CalculateCardGrid()
    {
        var nbrOfPairs = GameManager.Instance.NumberOfPairs;
        if (nbrOfPairs == 1)
        {
            return new Vector2(2,1);
        }

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

    private void ResizeCards(float widthSpaceEachCard, float heightSpaceEachCard, float drawingWidth, float drawingHeight, float paddingBetweenCards)
    {
        // 95 pixels = each unit of scale
        var newWidth = (widthSpaceEachCard - paddingBetweenCards*2) / 95f;
        var newHeight = (heightSpaceEachCard - paddingBetweenCards*2) / 95f;
        var newScale = (newWidth < newHeight)? newWidth : newHeight;
        foreach (var card in Cards)
        {
            card.transform.localScale = new Vector3(newScale, newScale, newScale);
        }
        return;
    }

    private IEnumerator DestroyCards()
    {
        secondSelectedCard.CardController.TurnCardUp();
        firstSelectedCard.CardController.StartFade();
        secondSelectedCard.CardController.StartFade();
        var firstCardToDestroy = firstSelectedCard.GameObject;
        var secondCardToDestroy = secondSelectedCard.GameObject;
        firstSelectedCard = null;
        secondSelectedCard = null;
        yield return new WaitForSeconds(1.5f);
        Cards.Remove(firstCardToDestroy);
        Cards.Remove(secondCardToDestroy);
        Destroy(firstCardToDestroy);
        Destroy(secondCardToDestroy);

        if(Cards.Count == 0)
        {
            OnEndGame?.Invoke();
        }
    }
}
