using UnityEngine;

public class PayoutManager : MonoBehaviour
{
    [Header("Payout Multipliers")]

    [SerializeField] private int cherryMultiplier = 2;
    [SerializeField] private int bellMultiplier = 3;
    [SerializeField] private int barMultiplier = 5;
    [SerializeField] private int sevenMultiplier = 10;

    public int CalculatePayout(
        int symbolIndex,
        int betAmount)
    {
        int multiplier = GetMultiplier(symbolIndex);

        return betAmount * multiplier;
    }

    private int GetMultiplier(int symbolIndex)
    {
        switch ((SlotSymbol)symbolIndex)
        {
            case SlotSymbol.Cherry:
                return cherryMultiplier;

            case SlotSymbol.Bell:
                return bellMultiplier;

            case SlotSymbol.Bar:
                return barMultiplier;

            case SlotSymbol.Seven:
                return sevenMultiplier;

            default:
                return 0;
        }
    }
}