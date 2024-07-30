using System.Collections.Generic;
using TMPro;
using Alteruna;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonActions : AttributesSync
{
    public GameObject m_textField;
    public GameObject m_roomMenu;
    public GameObject m_howToPlay;

    [SerializeField] private int m_mapSeed;

    public void LoadGameScene()
    {
        StaticData.m_isOwner = true;
        var rand = new System.Random();
        m_mapSeed = rand.Next();
        StaticData.m_mapSeed = m_mapSeed;
        BroadcastRemoteMethod("LoadOtherUsersToScene");
    }

    [SynchronizableMethod]
    public void LoadOtherUsersToScene()
    {
        Multiplayer.Instance.LoadScene("Game");
    }

    public void ToggleRoomMenu()
    {
        if (m_roomMenu != null)
        {
            m_roomMenu.SetActive(!m_roomMenu.activeInHierarchy);
        }
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

    public void SaveName()
    {
        if (m_textField != null)
        {
            PlayerPrefs.SetString("Name", m_textField.GetComponent<TMP_Text>().text);
            m_textField.transform.parent.parent.parent.gameObject.SetActive(false);
        }
    }

    public void ShowHowToPlay()
    {
        if (m_howToPlay != null)
        {
            m_howToPlay.SetActive(true);
        }
    }
}
