using System.Collections;
using UnityEngine;

public class TrainClicked : MonoBehaviour
{
    public Animator m_animator;
    private readonly int m_animationTime = 2;
    private bool m_clicked = false;

    private void OnMouseDown()
    {
        if (!m_clicked)
        {
            m_animator.SetTrigger("trainClicked");
            m_clicked = true;
            StartCoroutine(SetTrainGo());
        }
    }

    private IEnumerator SetTrainGo()
    {
        yield return new WaitForSeconds(m_animationTime);
        StaticData.trainGO = true;
        m_clicked = false;
    }
}
