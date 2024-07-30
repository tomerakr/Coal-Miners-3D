using System.Collections;
using UnityEngine;

public class Fogcollision : MonoBehaviour
{
    public int m_fogDamge { get; set; }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DamagePlayer(other.gameObject));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines(); // Stop damaging the player when they exit the zone
        }
    }

    private IEnumerator DamagePlayer(GameObject playerObj)
    {
        var player = playerObj.GetComponent<PlayerObject>();
        if (player)
        {
            while (true)
            {
                player.DealDamage(m_fogDamge);
                yield return null; // Wait for the next frame
            }
        }
    }
}