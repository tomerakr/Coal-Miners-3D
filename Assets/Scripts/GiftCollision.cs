using System.Collections;
using UnityEngine;

public class GiftCollision : MonoBehaviour
{
    private bool m_takenGift = false;

    private void OnTriggerEnter(Collider other)
    {
        var playerObj = other.gameObject.GetComponent<PlayerObject>();
        if (!playerObj || m_takenGift)
        {
            return;
        }

        m_takenGift = true;
        StartCoroutine(PlayAudio());
        playerObj.AddGift(gameObject.name);
    }

    private IEnumerator PlayAudio()
    {
        var audio = gameObject.GetComponent<AudioSource>();
        if (audio)
        {
            audio.Play();
        }
        var soundLength = audio.clip.length;
        gameObject.transform.localScale = new Vector3(0, 0, 0);

        yield return new WaitForSeconds(soundLength);

        Destroy(transform.parent.gameObject);

    }
}
