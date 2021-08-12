using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int NumberOfPairs;

    public int LowestTimeThisRun;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            NumberOfPairs = 5;
            LowestTimeThisRun = int.MaxValue;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
