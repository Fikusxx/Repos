using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemNudge : MonoBehaviour // i'd rather prefer normal animation motion xd
{

    private WaitForSeconds pause;
    private bool isAnimating = false;

    private void Awake()
    {
        pause = new WaitForSeconds(0.04f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAnimating == false)
        {
            if (gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                StartCoroutine(WobbleLeft()); // if player is moving from right to left
            }
            else
            {
                StartCoroutine(WobbleRight()); // if otherwise
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isAnimating == false)
        {
            if (gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                StartCoroutine(WobbleLeft()); // if player is moving from right to left
            }
            else
            {
                StartCoroutine(WobbleRight()); // if otherwise
            }
        }
    }

    private IEnumerator WobbleLeft()
    {
        isAnimating = true; // so the animation wont trigger again

        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0, 0, 2); // rotates left
            yield return pause;
        }

        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0, 0, -2); // rotates right
            yield return pause;
        }

        gameObject.transform.GetChild(0).Rotate(0, 0, 2); // rotates "1 pixel" back to normal, like posteffect
        yield return pause;
        isAnimating = false; // animation can trigger again
    }

    private IEnumerator WobbleRight()
    {
        isAnimating = true; // so the animation wont trigger again

        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0, 0, -2); // rotates left
            yield return pause;
        }

        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0, 0, 2); // rotates right
            yield return pause;
        }

        gameObject.transform.GetChild(0).Rotate(0, 0, -2); // rotates "1 pixel" back to normal, like posteffect
        yield return pause;
        isAnimating = false; // animation can trigger again
    }
}
