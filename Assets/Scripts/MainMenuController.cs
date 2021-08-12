using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField numberOfPairsInputField;

    public void OnPlayButtonClick()
    {
        string numberOfPairsStr = numberOfPairsInputField.text;
        if (numberOfPairsStr == String.Empty)
        {
            numberOfPairsStr = numberOfPairsInputField.placeholder.GetComponent<TextMeshProUGUI>().text;
        }
        GameManager.Instance.NumberOfPairs = Convert.ToInt32(numberOfPairsStr);
        SceneManager.LoadScene("GameScene");
    }
}
