using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInChildren<ObscuringItemFader>())
        {
            var obscuringItemFader = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

            foreach (var item in obscuringItemFader)
            {
                item.FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInChildren<ObscuringItemFader>())
        {
            var obscuringItemFader = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

            foreach (var item in obscuringItemFader)
            {
                item.FadeIn();
            }
        }
    }
}
