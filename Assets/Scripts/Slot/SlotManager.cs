using System.Collections;
using UnityEngine;

public class SlotMachineManager : MonoBehaviour
{
    [Header("Reels")]
    [SerializeField] private ReelController[] reels;

    [Header("Game Settings")]
    [SerializeField] private int startingBalance = 1000;
    [SerializeField] private int betAmount = 100;

    [Header("Managers")]
    [SerializeField] private PayoutManager payoutManager;
    [SerializeField] private UIManager uiManager;

    [Header("Reel Timing")]
    [SerializeField] private float reelStartDelay = 0.2f;
    [SerializeField] private float extraWaitAfterSpin = 0.2f;

    private int balance;

    // Converted to a public property with a private setter so other scripts 
    // (like LeverController) can read it, but only this script can change it.
    public bool IsSpinning { get; private set; }

    private void Start()
    {
        balance = startingBalance;
        UpdateUI();
    }

    public void StartSpin()
    {
        if (IsSpinning) return;

        if (balance < betAmount)
        {
            if (uiManager != null)
            {
                uiManager.ShowResult("NOT ENOUGH BALANCE!");
            }
            return;
        }

        StartCoroutine(SpinGame());
    }

    private IEnumerator SpinGame()
    {
        IsSpinning = true;

        // 1. Deduct bet and clear UI
        balance -= betAmount;
        UpdateUI();

        if (uiManager != null)
        {
            uiManager.ShowResult("");
        }

        // 2. Generate random results
        int[] results = GenerateRandomResults();

        // 3. Start reels with a staggered delay
        for (int i = 0; i < reels.Length; i++)
        {
            if (reels[i] != null)
            {
                StartCoroutine(SpinReelWithDelay(reels[i], results[i], i * reelStartDelay));
            }
        }

        // 4. Wait for all reels to finish spinning
        yield return new WaitUntil(AreAllReelsStopped);
        yield return new WaitForSeconds(extraWaitAfterSpin);

        // 5. Check results and conclude spin
        CheckWin(results);

        IsSpinning = false;
        UpdateUI();
    }

    private IEnumerator SpinReelWithDelay(ReelController reel, int targetSymbol, float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        yield return reel.Spin(targetSymbol);
    }

    private bool AreAllReelsStopped()
    {
        foreach (ReelController reel in reels)
        {
            // If any reel exists and is still spinning, return false
            if (reel != null && reel.IsSpinning)
            {
                return false;
            }
        }
        return true;
    }

    private int[] GenerateRandomResults()
    {
        int[] results = new int[reels.Length];
        int symbolCount = System.Enum.GetValues(typeof(SlotSymbol)).Length;

        for (int i = 0; i < results.Length; i++)
        {
            results[i] = Random.Range(0, symbolCount);
        }

        return results;
    }

    private void CheckWin(int[] results)
    {
        if (results == null || results.Length < 3) return;

        // Check if all symbols match the first one
        bool isWin = true;
        int firstSymbol = results[0];

        for (int i = 1; i < results.Length; i++)
        {
            if (results[i] != firstSymbol)
            {
                isWin = false;
                break;
            }
        }

        if (!isWin)
        {
            if (uiManager != null) uiManager.ShowResult("TRY AGAIN!");
            return;
        }

        // Handle Win
        int payout = payoutManager.CalculatePayout(firstSymbol, betAmount);
        balance += payout;

        if (uiManager != null)
        {
            if ((SlotSymbol)firstSymbol == SlotSymbol.Seven)
            {
                uiManager.ShowResult($"JACKPOT! +{payout}");
            }
            else
            {
                uiManager.ShowResult($"YOU WIN! +{payout}");
            }
        }
    }

    private void UpdateUI()
    {
        if (uiManager == null) return;

        uiManager.UpdateBalance(balance);
        uiManager.UpdateBet(betAmount);
    }
}