using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObscuringItemFader : MonoBehaviour
{

    // References
    private SpriteRenderer spriteRenderer;
    private float fadingMultiplier = 1.5f;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }


    private IEnumerator FadeOutRoutine()
    {
        float currentAlpha = spriteRenderer.color.a; // gets current Alpha color, which is 1

        while (currentAlpha > Settings.targetAlpha)
        {
            currentAlpha = currentAlpha - Time.deltaTime  * fadingMultiplier; // basically we want subtract a really small number each frame
            spriteRenderer.color = new Color(1, 1, 1, currentAlpha);
            yield return null;
        }

        spriteRenderer.color = new Color(1, 1, 1, Settings.targetAlpha); // sets Alpha that we aim for
    }

    private IEnumerator FadeInRoutine()
    {
        float currentAlpha = spriteRenderer.color.a; // gets current Alpha color, which is Settings.targetAlpha ~ 0.45f;

        while (spriteRenderer.color.a < 1)
        {
            currentAlpha = currentAlpha + Time.deltaTime * fadingMultiplier;
            spriteRenderer.color = new Color(1, 1, 1, currentAlpha);
            yield return null;
        }

        spriteRenderer.color = new Color(1, 1, 1, 1); // sets the default Alpha, which is 1
    }
}
