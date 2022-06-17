using System.Text;
using UnityEngine;
using TMPro;

public class GameClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI seasonText;
    [SerializeField] private TextMeshProUGUI yearText;

    private StringBuilder ampm = new StringBuilder(); // we are updating time more often than once a sec. String is bad here.

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateGametime;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateGametime;
    }

    private void UpdateGametime(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // Update time

        ampm.Clear();
        ampm.Append(gameHour >= 12 ? "pm" : "am");

        if (gameHour >= 13) // cause we do ampm, which is from 0-12
        {
            gameHour -= 12;
        }


        timeText.text = $"{gameHour} : {gameMinute} {ampm}";
        dateText.text = $"{gameDayOfWeek}. {gameDay}";
        seasonText.text = gameSeason.ToString();
        yearText.text = $"Year {gameYear}";
    }
}
