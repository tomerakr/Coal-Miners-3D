using UnityEngine;
using Alteruna;

public class Loadnewscene : AttributesSync
{
    public void LoadGame()
    {
        BroadcastRemoteMethod();
    }

    [SynchronizableMethod]
    public void LoadForOthers()
    {
        Debug.Log("Before");
        var multiplayer = Multiplayer.Instance;
        multiplayer.LoadScene("Game");
        Debug.Log("After");

    }
}
