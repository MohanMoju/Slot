using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class LeverController : MonoBehaviour, IPointerClickHandler
{
    [Header("Lever Animation")]
    [SerializeField] private RectTransform leverTransform;
    [SerializeField] private float pullDistance = 100f;
    [SerializeField] private float pullDuration = 0.2f;
    [SerializeField] private float returnDuration = 0.3f;

    [Header("Slot Machine")]
    [SerializeField] private SlotMachineManager slotMachineManager;

    private Vector2 originalPosition;
    private bool isPulling;

    private void Start()
    {
        if (leverTransform == null)
        {
            leverTransform = GetComponent<RectTransform>();
        }

        originalPosition = leverTransform.anchoredPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Prevent pulling if the lever is animating OR if the machine is actively spinning
        if (isPulling /* || slotMachineManager.IsSpinning */)
            return;

        StartCoroutine(PullLever());
    }

    private IEnumerator PullLever()
    {
        isPulling = true;

        // 1. Pull lever down
        yield return MoveLever(
            originalPosition,
            originalPosition + (Vector2.down * pullDistance),
            pullDuration
        );

        // 2. Start reels
        if (slotMachineManager != null)
        {
            slotMachineManager.StartSpin();
        }

        // 3. Return lever
        yield return MoveLever(
            originalPosition + (Vector2.down * pullDistance), // Start from the bottom
            originalPosition,
            returnDuration
        );

        isPulling = false;
    }

    private IEnumerator MoveLever(Vector2 startPosition, Vector2 targetPosition, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Calculate normalized time (0 to 1) and apply smoothing
            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            leverTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        // Ensure it snaps perfectly to the target at the end
        leverTransform.anchoredPosition = targetPosition;
    }
}