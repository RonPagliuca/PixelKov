using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class ScreenShakeController : MonoBehaviour
{
    public Image screenFlashImage; // Reference to the UI Image for screen flash

    void Start()
    {

        // Set the RectTransform properties to stretch the image to the canvas
        RectTransform rt = screenFlashImage.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(0, 0);
        rt.offsetMax = new Vector2(0, 0);

        // Set the initial color to fully transparent
        screenFlashImage.color = new Color(1, 1, 1, 0);
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = originalPosition.x + Random.Range(-1f, 1f) * magnitude;
            float y = originalPosition.y + Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
    public IEnumerator ShakeScreen(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    public IEnumerator FlashScreen(float flashDuration)
    {
        screenFlashImage.color = new Color(1, 1, 1, 1); // Set the flash color to white with full opacity
        yield return new WaitForSeconds(flashDuration); // Duration of the flash
        screenFlashImage.color = new Color(1, 1, 1, 0); // Reset to fully transparent
    }
}
