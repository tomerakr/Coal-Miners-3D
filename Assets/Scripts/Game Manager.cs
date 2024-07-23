using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject m_playerPrefab;
    public GameObject m_coalShaftObj;
    public GameObject m_gameData;
    public List<Transform> m_spawns;
    private GameObject m_menu;
    private float m_timer = 0;
    private TMP_Text m_timerText;
    private GameObject[] m_players;
    private bool m_endedGame = false;
    private CoalShaft m_coalShaft;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        var multiplayer = Alteruna.Multiplayer.Instance;
        multiplayer.AvatarSpawnLocations = m_spawns;
        multiplayer.SpawnAvatar();

        m_coalShaft = m_coalShaftObj.GetComponent<CoalShaft>();
        m_menu = m_gameData.transform.Find("Menu").gameObject;
        m_timerText = m_gameData.transform.Find("Timer").GetComponent<TMP_Text>();
    }
        
    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(m_timer);
        m_timerText.text = timeSpan.ToString(@"mm\:ss");
        //opens menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();            
        }
        m_coalShaft.CloseMap(m_timer);
        //if (AllPlayersAreDead())
        //{
        //    EndOfGame();
        //}
    }

    private void ToggleMenu()
    {
        if (m_menu.activeInHierarchy)
        {
            Cursor.visible = false;
            Time.timeScale = 1;
            m_menu.SetActive(false);
        }
        else
        {
            Cursor.visible = true;
            Time.timeScale = 0;
            m_menu.SetActive(true);
        }
    }

    private bool AllPlayersAreDead()
    {
        foreach (var player in m_players)
        {
            var playerObj = player.GetComponent<PlayerObject>(); //TODO: check why sometimes its null
            if (playerObj.IsAlive())
            {
                return false;
            }
        }

        return true;
    }

    private void EndOfGame()
    {
        if (m_endedGame) { return; }

        UpdateScoreboard();
        m_endedGame = true;
    }

    private void UpdateScoreboard()
    {
        var playersData = GetPlayersData();

        for (int i = 0; i < Utility.TOP_SCORES; ++i)
        {
            if (playersData.Count == 0) { break; }

            var key = "score" + i;

            if (PlayerPrefs.HasKey(key))
            {
                //save in player prefs: "score<index>": "<icon>,<name>,<score>"
                var scoreData = PlayerPrefs.GetString(key).Split(",");
                var topPlayerScore = playersData[0].Split(",");

                int.TryParse(scoreData[2], out var currScore);
                int.TryParse(topPlayerScore[2], out var playerScore);

                if (playerScore > currScore)
                {
                    playersData.Add(PlayerPrefs.GetString(key)); //TODO: apply sort from function here too
                    PlayerPrefs.DeleteKey(key);
                    PlayerPrefs.SetString(key, playersData[0]);
                    playersData.RemoveAt(0);
                }
            }
            else
            {
                PlayerPrefs.SetString(key, playersData[0]);
                playersData.RemoveAt(0);
            }
        }
    }

    private List<string> GetPlayersData()
    {
        var playerData = new List<string>(); //TODO: sort this list by score (in function)
        foreach (var playerObj in m_players)
        {
            var player = playerObj.GetComponent<PlayerObject>();
            playerData.Add($"icon,{player.GetName()},{player.GetScore()}");
        }

        return playerData;
    }
}
