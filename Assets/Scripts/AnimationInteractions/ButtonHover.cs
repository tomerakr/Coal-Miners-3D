using TMPro;
using UnityEngine;

public class ButtonHover : MonoBehaviour
{
    public TMP_Text m_buttonText;

    public void HoverEnter()
    {
        m_buttonText.fontSize += 20;
    }

    public void HoverLeave()
    {
        m_buttonText.fontSize -= 20;
    }
}
