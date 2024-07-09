using Alteruna;
using Alteruna.Trinity;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    private readonly float m_explosionDelay = 2.5f;
    private readonly int m_explosionAnimationTime = 2;
    private PlayerObject m_owner;
    //private Multiplayer     m_multiplayer;
    public int m_maxHits = 25;
    public LayerMask m_hitLayer;
    public LayerMask m_blockExplosionLayer;
    public GameObject m_explosionPrefab;
    public GameObject m_bigExplosionPrefab;
    private Collider[] m_hits;

    //private void Awake()
    //{
    //    m_multiplayer = Multiplayer.Instance;
    //    m_multiplayer.RegisterRemoteProcedure("SetOwner", SetOwner);
    //    Debug.Log("registered");
    //}

    void Start()
    {
        m_hits = new Collider[m_maxHits];
        StartCoroutine(Explode());
    }

    //public void SetOwner(ushort fromUser, ProcedureParameters parameters, uint callId, ITransportStreamReader processor)
    //{
    //    parameters.Get("ownerName", out string name);
    //    var player = m_multiplayer.GetAvatar(name).gameObject.GetComponentInParent<PlayerObject>();
    //    m_owner = player;
    //}

    public void SetOwner(PlayerObject owner)
    {
        m_owner = owner;
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(m_explosionDelay);

        var radius = m_owner.GetDynamiteRadius();
        var isBigDynamite = radius > Utility.DYNAMITE_RADIUS;

        var minDmg = isBigDynamite ? Utility.BIG_DYNAMITE_DMG_MIN : Utility.SMALL_DYNAMITE_DMG_MIN;
        var maxDmg = isBigDynamite ? Utility.BIG_DYNAMITE_DMG_MAX : Utility.SMALL_DYNAMITE_DMG_MAX;

        var hitsCount = Physics.OverlapSphereNonAlloc(transform.position, radius, m_hits, m_hitLayer);

        var hits = m_hits.Where(hit => hit != null).ToArray();
        Array.Sort(hits, (hit1, hit2) =>
        {
            var d1 = Vector3.Distance(transform.position, hit1.transform.position);
            var d2 = Vector3.Distance(transform.position, hit2.transform.position);
            return (d1.CompareTo(d2));
        });



        foreach (var hit in hits)
        {
            var distance = Vector3.Distance(transform.position, hit.transform.position);

            if (!Physics.Raycast(transform.position, (hit.transform.position - transform.position).normalized, distance, m_blockExplosionLayer.value))
            {
                if (hit.gameObject.name.Contains("Player"))
                {
                    var dmg = Mathf.FloorToInt(Mathf.Lerp(maxDmg, minDmg, distance / radius));
                    hit.gameObject.GetComponent<PlayerObject>().DealDamage(dmg);
                }
                else
                {
                    hit.gameObject.GetComponent<Breakable>().DestroyWall(m_owner);
                    int layerIndex = Mathf.RoundToInt(Mathf.Log(m_blockExplosionLayer.value, 2));
                    hit.gameObject.layer = layerIndex;
                }
            }
        }

        m_owner.RestoreDynamite();
        var explosionPrefab = isBigDynamite ? m_bigExplosionPrefab : m_explosionPrefab;
        var explosionAnimation = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        transform.localScale = new Vector3(0, 0, 0);
        
        yield return new WaitForSeconds(m_explosionAnimationTime);

        Destroy(explosionAnimation);
        Destroy(gameObject);
    }
}