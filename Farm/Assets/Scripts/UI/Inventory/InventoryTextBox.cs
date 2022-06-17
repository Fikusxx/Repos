using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryTextBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTop1;
    [SerializeField] private TextMeshProUGUI textTop2;
    [SerializeField] private TextMeshProUGUI textTop3;
    [SerializeField] private TextMeshProUGUI textBottom1;
    [SerializeField] private TextMeshProUGUI textBottom2;
    [SerializeField] private TextMeshProUGUI textBottom3;


    // Set text values
    public void SetTextboxText(string top1, string top2, string top3, string bottom1, string bottom2, string bottom3)
    {
        textTop1.text = top1;
        textTop2.text = top2;
        textTop3.text = top3;

        textBottom1.text = bottom1;
        textBottom2.text = bottom2;
        textBottom3.text = bottom3;
    }
}
