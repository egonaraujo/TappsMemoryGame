using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private CardsManager CardsManagerObject;

    [SerializeField]
    private GameObject EndPanel;
    
    private enum CardSelectionState
    {
        NONE,
        ONE,
        CORRECT_PAIR,
        INCORRECT_PAIR,
    }

    private CardSelectionState cardSelectionState = CardSelectionState.NONE;

    [SerializeField]
    private TextMeshProUGUI PairsText;

    [SerializeField]
    private TextMeshProUGUI TimerText;

    private float TimeElapsed;

    private int PairsFound;

    private bool isGameFinished;

    private void Start()
    {
        TimeElapsed = 0f;
        PairsFound = 0;
        isGameFinished = false;
        CardsManagerObject.Cards.ForEach(c => 
            c.GetComponent<CardController>().OnCardClicked += OnCardClicked);
        CardsManagerObject.OnEndGame += OnEndGame;
    }

    private void Update()
    {
        if(!isGameFinished)
        {
            TimeElapsed += Time.deltaTime;
            TimerText.text = FloatToTimer(TimeElapsed);
        }
    }

    public void OnEndGame()
    {
        EndPanel.SetActive(true);
        isGameFinished = true;
        Invoke("ChangeToMenuScene", 1.5f);
    }

    public void OnCardClicked(GameObject cardGameObject, int pairId)
    {
        CardsManagerObject.KeepCard(cardGameObject);
        UpdateGameState();

        if (cardSelectionState == CardSelectionState.NONE)
        {
            CardsManagerObject.SelectCard(cardGameObject, pairId);
            cardSelectionState = CardSelectionState.ONE;
        }
        else if (cardSelectionState == CardSelectionState.ONE)
        {
            if(cardGameObject == CardsManagerObject.firstSelectedCard.GameObject)
            {
                CardsManagerObject.firstSelectedCard.Keep = false;
                return;
            }

            CardsManagerObject.SelectCard(cardGameObject, pairId);
            var goodPair = CardsManagerObject.CheckCardsPair();

            if(goodPair)
            {
                PairsFound++;
                PairsText.text = "Pairs Found: " + PairsFound.ToString();
                cardSelectionState = CardSelectionState.CORRECT_PAIR;
            }
            else
            {
                cardSelectionState = CardSelectionState.INCORRECT_PAIR;
            }
        }
    }

    public void OnBackButtonClicked()
    {
        ChangeToMenuScene();
    }

    public void ChangeToMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }

    private void UpdateGameState()
    {
        switch(cardSelectionState)
        {
            case CardSelectionState.NONE:
            case CardSelectionState.ONE:
                break;
            case CardSelectionState.INCORRECT_PAIR:
                CardsManagerObject.UnselectCards();
                cardSelectionState = CardSelectionState.NONE;
                break;
            case CardSelectionState.CORRECT_PAIR:
                cardSelectionState = CardSelectionState.NONE;
                break;
        }
    }

    private string FloatToTimer(float timeElapsed)
    {
        var minutes = Mathf.FloorToInt(timeElapsed / 60f);
        var seconds = Mathf.FloorToInt(timeElapsed % 60f);
        var miliseconds = (TimeElapsed % 1f) * 1000;
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, miliseconds);
    }
}
