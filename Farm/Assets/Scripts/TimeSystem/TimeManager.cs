using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>
{
    // Main Data
    private int gameSecond = 0;
    private int gameMinute = 55;
    private int gameHour = 11;
    private int gameDay = 1;
    private int gameYear = 1;
    private string gameDayOfWeek = "Mon";

    // Extra Data
    private Season gameSeason = Season.Spring;
    private float gameTick = 0f;
    private bool gameClockPaused = false;


    private void Start()
    {
        EventHandler.CallAdvancedGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void Update()
    {
        if (gameClockPaused == false)
        {
            GameTick();
        }
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime; // adds really small amount every frame, like 0.001/2~

        if (gameTick >= Settings.secondsPerGameSecond) // when gameTick gets to that number
        {
            gameTick -= Settings.secondsPerGameSecond; // kinda reset it to 0

            UpdateGameSecond(); // this means 1 GAME SECOND passed (0.012f real seconds passed) and we update game timer
        }
    }

    // You can reduce number of events to lets say Hour (to change lighting) || Season (to change overall Sprites) || and maybe Year to trigger some ingame events
    private void UpdateGameSecond() // Why the fk DateTime doesnt work.............
    {
        gameSecond++;

        if (gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++;

            if (gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++;

                if (gameHour > 23)
                {
                    gameHour = 0;
                    gameDay++;

                    if (gameDay > 29)
                    {
                        gameDay = 1;
                        int gs = (int)gameSeason;
                        gs++;
                        gameSeason = (Season)gs;

                        if (gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs;

                            gameYear++;

                            if (gameYear > 9999)
                            {
                                gameYear = 1;
                            }

                            EventHandler.CallAdvancedGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                        }

                        EventHandler.CallAdvancedGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }
                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvancedGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }

                EventHandler.CallAdvancedGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }

            EventHandler.CallAdvancedGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            Debug.Log($"Year: {gameYear} || Season: {gameSeason} || Day: {gameDay} || Hour: {gameHour} || Minute: {gameMinute}");
        }
    }

    private string GetDayOfWeek()
    {
        return gameDayOfWeek switch
        {
            "Mon" => "Tues",
            "Tues" => "Wed",
            "Wed" => "Thurs",
            "Thurs" => "Fri",
            "Fri" => "Sat",
            "Sat" => "Sun",
            _ => "Mon",
        };
    }
}
