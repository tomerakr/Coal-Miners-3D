using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonActions : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
        Cursor.visible = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowScoreboard()
    {
        var scoreboard = StaticData.m_scoreboard;
        var scoreLines = new List<GameObject>();

        for (int i = 0; i < Utility.TOP_SCORES; ++i)
        {
            if (PlayerPrefs.HasKey("score" + i))
            {
                var scoreLine = scoreboard.transform.Find("score line " + (i + 1)).gameObject;

                //save in player prefs: "score1": "<icon>,<name>,<score>"
                var scoreData = PlayerPrefs.GetString("score" + i).Split(",");
                scoreLine.transform.Find("Rank").gameObject.GetComponent<TMP_Text>().text = "# " + (i + 1);
                scoreLine.transform.Find("Name").gameObject.GetComponent<TMP_Text>().text = scoreData[1];
                scoreLine.transform.Find("Score").gameObject.GetComponent<TMP_Text>().text = scoreData[2];

                scoreLine.SetActive(true);
                scoreLines.Add(scoreLine);
            }
            else
            {
                break;
            }
        }

        StaticData.m_scoreLines = scoreLines;
        scoreboard.SetActive(true);
    }
}
