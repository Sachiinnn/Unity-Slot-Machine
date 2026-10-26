using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [Header("Multiplier for each symbol index")]
    [SerializeField] private int[] multipliers = { 20, 5, 8, 10 };



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int CalculatePayout(int[] results, int betAmount)
    {
        if (results == null || results.Length == 0)
        {
            return 0;
        }


        int firstSymbol = results[0];

        
        for (int i = 1; i < results.Length; i++)    // Every reel must show the same symbol to win.
        {
            if (results[i] != firstSymbol)
                return 0;
        }

        if (firstSymbol < 0 || firstSymbol >= multipliers.Length)
        {
            return 0;
        }
      

        return betAmount * multipliers[firstSymbol];
    }
}

