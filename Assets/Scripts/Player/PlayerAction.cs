using Alteruna;
using UnityEngine;

public class PlayerAction : AttributesSync
{
    public Alteruna.Avatar m_avatar;
    private Player m_player;
    private float m_bombRadius;
    private Spawner m_spawner;
    public GameObject m_lastDynamite;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_avatar.IsMe) { return; }

        var playerObj = GetComponentInParent<PlayerObject>();
        m_spawner = Multiplayer.Instance.gameObject.GetComponent<Spawner>();
        m_player = playerObj.GetPlayer();
        m_bombRadius = m_player.GetDynamiteRadius();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_avatar.IsMe) { return; }

        if (m_player.DroppedDynamite())
        {
            var spawnPosition = new Vector3(transform.position.x, 0.1f, transform.position.z);
            var dynamitePrefab = m_player.GetDynamiteRadius() > Utility.DYNAMITE_RADIUS ? "BigDynamite" : "Dynamite";

            m_lastDynamite = m_spawner.Spawn(dynamitePrefab, spawnPosition, Quaternion.identity);
            m_lastDynamite.GetComponent<Dynamite>().SetOwner(m_player, m_bombRadius);
        }
    }
}
