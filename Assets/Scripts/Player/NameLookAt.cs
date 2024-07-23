using UnityEngine;

public class NameLookAt : MonoBehaviour
{
    private Camera m_Camera;
    private Vector3 m_offset = new (0f, 180f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.allCameras[0];
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(m_Camera.transform);
        transform.Rotate(m_offset);
    }
}
