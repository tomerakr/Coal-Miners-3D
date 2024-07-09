using System.Collections;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    private Vector2 m_positionInMatrix;
    public GameObject[] m_giftsPrefab;
    public GameObject m_fracturedWall;

    public void SetPosition(Vector2 position) { m_positionInMatrix = position; }

    public void DestroyWall(PlayerObject dynamiteOwner)
    {
        StartCoroutine(FructuredWallAnimation());
        DropGift(transform.position);
        dynamiteOwner.AddScore(Utility.BREALABLE_SCORE);
    }

    private IEnumerator FructuredWallAnimation()
    {
        var _ = Instantiate(m_fracturedWall, gameObject.transform.position, Quaternion.identity);
        var audio = _.GetComponent<AudioSource>();
        audio.Play();
        gameObject.transform.localScale = new Vector3(0, 0, 0);

        yield return new WaitForSeconds(2.5f);

        Destroy(_);
        Destroy(gameObject);
    }

    private void DropGift(Vector3 pos)
    {
        if (Random.Range(0, 100) > Utility.GIFT_CHANCE)
        {
            return;
        }

        var giftIndex = Random.Range(0, m_giftsPrefab.Length);
        var giftPos = new Vector3(pos.x, m_giftsPrefab[giftIndex].transform.position.y, pos.z);
        Instantiate(m_giftsPrefab[giftIndex], giftPos, Quaternion.identity);
    }
}
