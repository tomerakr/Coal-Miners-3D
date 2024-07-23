using Alteruna;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Dynamite : AttributesSync
{
    private readonly float m_explosionDelay = 2.5f;
    private readonly int m_explosionAnimationTime = 2;
    private Player m_owner;
    [SynchronizableField] private float m_radius;
    public int m_maxHits = 25;
    public LayerMask m_hitLayer;
    public LayerMask m_blockExplosionLayer;
    public GameObject m_explosionPrefab;
    public GameObject m_bigExplosionPrefab;
    private Collider[] m_hits;

    void Start()
    {
        
    }

    public void SetOwner(Player owner, float radius)
    {
        m_owner = owner;
        m_radius = radius;
        BroadcastRemoteMethod("Explode");
    }

    [SynchronizableMethod]
    public void Explode()
    {
        m_hits = new Collider[m_maxHits];
        StartCoroutine(Explode2());
    }

    private IEnumerator Explode2()
    {
        yield return new WaitForSeconds(m_explosionDelay);

        var isBigDynamite = m_radius > Utility.DYNAMITE_RADIUS;

        var minDmg = isBigDynamite ? Utility.BIG_DYNAMITE_DMG_MIN : Utility.SMALL_DYNAMITE_DMG_MIN;
        var maxDmg = isBigDynamite ? Utility.BIG_DYNAMITE_DMG_MAX : Utility.SMALL_DYNAMITE_DMG_MAX;

        var hitsCount = Physics.OverlapSphereNonAlloc(transform.position, m_radius, m_hits, m_hitLayer);

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
                    var dmg = Mathf.FloorToInt(Mathf.Lerp(maxDmg, minDmg, distance / m_radius));
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

        m_owner?.RestoreDynamite();
        var explosionPrefab = isBigDynamite ? m_bigExplosionPrefab : m_explosionPrefab;
        var explosionAnimation = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        transform.localScale = new Vector3(0, 0, 0);
        
        yield return new WaitForSeconds(m_explosionAnimationTime);

        Destroy(explosionAnimation);
        Destroy(gameObject);
    }
}