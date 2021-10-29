using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitFade : MonoBehaviour
{
    public float fadeInSpeed = 1;
    public float fadeOutSpeed = 1;
    public float maxOpaque = 1;
    public RawImage image;

    private bool isActive;
    private float currentI;

    public void StartFade()
    {
        if(isActive)
        {
            StopAllCoroutines();
            StartCoroutine(PerformHitFade(currentI));
        }

        else
        {
            StartCoroutine(PerformHitFade(0));
        }
    }

    private IEnumerator PerformHitFade(float startingIndex)
    {
        isActive = true;

        for(float i = startingIndex; i <= maxOpaque; i += Time.deltaTime * fadeInSpeed)
        {
            currentI = i;
            image.color = new Color(image.color.r, image.color.g, image.color.b, i);
            yield return null;
        }

        for (float i = maxOpaque; i > 0; i -= Time.deltaTime * fadeOutSpeed)
        {
            currentI = i;
            image.color = new Color(image.color.r, image.color.g, image.color.b, i);
            yield return null;
        }

        isActive = false;
        gameObject.SetActive(false);
    }
}
