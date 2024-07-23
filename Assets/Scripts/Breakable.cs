using System.Collections;
using UnityEngine;
using Alteruna;

public class Breakable : AttributesSync
{
    private Vector2 m_positionInMatrix;
    public GameObject[] m_giftsPrefab;
    public GameObject m_fracturedWall;

    public void SetPosition(Vector2 position) { m_positionInMatrix = position; }

    public void DestroyWall(Player destroyer)
    {
        if (destroyer == null) { return; }
        
        destroyer.AddScore(Utility.BREALABLE_SCORE);
        var giftIndex = Random.Range(0, m_giftsPrefab.Length);
        BroadcastRemoteMethod(nameof(DestroyWall2), giftIndex);
    }

    [SynchronizableMethod]
    private void DestroyWall2(int giftIndex)
    {
        StartCoroutine(FructuredWallAnimation());
        DropGift(giftIndex);
    }

    private IEnumerator FructuredWallAnimation()
    {
        var fructuredWall = Instantiate(m_fracturedWall, gameObject.transform.position, Quaternion.identity);
        fructuredWall.GetComponent<AudioSource>().Play();
        
        gameObject.transform.localScale = new Vector3(0, 0, 0);

        yield return new WaitForSeconds(2.5f);

        Destroy(fructuredWall);
        Destroy(gameObject);
    }

    private void DropGift(int giftIndex)
    {
        if (Random.Range(0, 100) > Utility.GIFT_CHANCE)
        {
            return;
        }

        var giftPos = new Vector3(transform.position.x, m_giftsPrefab[giftIndex].transform.position.y, transform.position.z);
        Instantiate(m_giftsPrefab[giftIndex], giftPos, Quaternion.identity);
    }
}
