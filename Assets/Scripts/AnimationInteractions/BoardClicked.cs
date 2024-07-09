using System.Collections;
using UnityEngine;

public class BoradClicked : MonoBehaviour
{
    public Animator m_animator;
    private readonly int m_animationTime = 2;
    private bool m_clicked = false;

    private void OnMouseDown()
    {
        if (StaticData.trainGO && !m_clicked)
        {
            m_animator.SetTrigger("boardClicked");
            m_clicked = true;
            StartCoroutine(SetTrainBack());
        }
    }

    private IEnumerator SetTrainBack()
    {
        yield return new WaitForSeconds(m_animationTime);
        StaticData.trainGO = false;
        m_clicked = false;
    }
}
