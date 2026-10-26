
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotGameManager : MonoBehaviour
{
    [Header("Game Components")]
    [SerializeField] private ReelController[] reels;
    [SerializeField] private RewardManager rewardManager;

    [Header("Player Settings")]
    [SerializeField] private int startingBalance = 1000;
    [SerializeField] private int startingBet = 10;
    [SerializeField] private int betStep = 10;

    [Header("Spin Settings")]
    [SerializeField] private float spinDuration = 2f;
    [SerializeField] private float reelStopDelay = 0.4f;

    [Header("UI")]
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button spinButton;
    [SerializeField] private Button increaseBetButton;
    [SerializeField] private Button decreaseBetButton;

    private int balance;
    private int betAmount;
    private bool isSpinning;

    private void Start()
    {
        balance = Mathf.Max(0, startingBalance);
        betAmount = Mathf.Max(1, startingBet);

        UpdateUI();
        SetMessage("Press SPIN to play!");
    }

    public void Spin()
    {
        if (isSpinning)
        {
            spinButton.interactable = false;
        }

        if (!ValidateReels())
        {
            SetMessage("Reel setup error. Check the Inspector.");
            return;
        }

        if (betAmount <= 0 || balance < betAmount)
        {
            SetMessage("Not enough coins!");
            return;
        }

        balance -= betAmount;
        isSpinning = true;

        if (spinButton != null)
        {
            spinButton.interactable = false;
        }


        UpdateUI();
        SetMessage("Spinning...");

        StartCoroutine(SpinRoutine());
    }

    private bool ValidateReels()
    {
        if (reels == null || reels.Length == 0)
        {
            return false;
        }
      

        foreach (ReelController reel in reels)
        {
            if (reel == null || reel.SymbolCount == 0)
            {
                return false;
            }
            
        }

        return true;
    }

    private IEnumerator SpinRoutine()
    {
        int[] results = new int[reels.Length];

        
        for (int i = 0; i < reels.Length; i++)  // Choose the final results before starting the animation.
        {
            results[i] = Random.Range(0, reels[i].SymbolCount);
        }

        
        for (int i = 0; i < reels.Length; i++)     // Stop each reel slightly later than the previous reel.
        {
            float duration = spinDuration + i * reelStopDelay;
            reels[i].SpinTo(results[i], duration);
        }

        float totalDuration = spinDuration + (reels.Length - 1) * reelStopDelay;

        yield return new WaitForSeconds(totalDuration);

        int payout = rewardManager != null? rewardManager.CalculatePayout(results, betAmount) : 0;

        if (payout > 0)
        {
            balance += payout;
            SetMessage($"YOU WIN! +{payout} coins");
        }
        else
        {
            SetMessage("No win. Try again!");
        }

        if (betAmount > balance)
        {
            
            betAmount = balance;        // Keep the bet within the available balance
        }

        isSpinning = false;

        if (spinButton != null)
        {
            spinButton.interactable = balance >= betAmount;
        }
 

        UpdateUI();
    }

    public void IncreaseBet()
    {
        if (isSpinning)
        {
            return;
        }

        if (betAmount + betStep <= balance)
        {
            betAmount += betStep;
            UpdateUI();
        }
    }

    public void DecreaseBet()
    {
        if (isSpinning)
        {
            return;
        }
  

        if (betAmount > betStep)
        {
            betAmount -= betStep;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (balanceText != null)
        {
            balanceText.text = $"COINS: {balance}";
        }
           

        if (betText != null)
        {
            betText.text = $"{betAmount}";
        }
            

        
        if (spinButton != null)              // Spin is enabled whenever the player can afford the bet
        {
           spinButton.interactable = !isSpinning && balance >= betAmount && betAmount > 0;
        }

        if (increaseBetButton != null)
        {
            increaseBetButton.interactable = !isSpinning && betAmount + betStep <= balance;
        }
       

        if (decreaseBetButton != null)
        {
            decreaseBetButton.interactable = !isSpinning && betAmount > betStep;
        }
      
    }

    private void SetMessage(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
        }
          
    }
}

