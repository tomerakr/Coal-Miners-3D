using UnityEngine;
using Alteruna;

public class MenuController : AttributesSync
{
    public GameObject[] m_buttons;
    public GameObject m_scoreboard;
    public GameObject m_howToPlay;
    public GameObject m_nameInput;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        StaticData.m_buttons = m_buttons;
        StaticData.m_scoreboard = m_scoreboard;
        if (!PlayerPrefs.HasKey("Name"))
        {
            m_nameInput.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenues();
        }
    }

    private void CloseMenues()
    {
        if (m_howToPlay != null)
        {
            m_howToPlay.SetActive(false);
        }

        //close scoreboardx
        var scoreboard = StaticData.m_scoreboard;
        var scoreLines = StaticData.m_scoreLines;

        if (scoreLines == null || !scoreboard) { return; }

        foreach (var line in scoreLines)
        {
            line.SetActive(false);
        }

        scoreboard.SetActive(false);
    }
}
