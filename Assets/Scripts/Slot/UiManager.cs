using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text resultText;

    public void UpdateBalance(int balance)
    {
        if (balanceText != null)
        {
            // The :N0 adds commas to large numbers (e.g., 1,000)
            balanceText.text = $"BALANCE: {balance:N0}";
        }
    }

    public void UpdateBet(int bet)
    {
        if (betText != null)
        {
            betText.text = $"BET: {bet:N0}";
        }
    }

    public void ShowResult(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
        }
    }
}