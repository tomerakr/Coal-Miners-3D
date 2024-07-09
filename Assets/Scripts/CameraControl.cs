using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private float m_sensitivity = 0.1f;
    private float m_maxAngleChange = 5f;

    private float m_startingX;
    private float m_startingY;

    private float m_rotationX = 0;
    private float m_rotationY = 0;

    void Start()
    {
        m_startingX = transform.rotation.eulerAngles.x;
        m_startingY = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        var mouseX = Input.GetAxis("Mouse X") * m_sensitivity;
        var mouseY = Input.GetAxis("Mouse Y") * m_sensitivity;

        m_rotationX -= mouseY;
        m_rotationY += mouseX;

        // Clamp the rotations to the specified limits
        m_rotationX = Mathf.Clamp(m_rotationX, m_startingX - m_maxAngleChange, m_startingX + m_maxAngleChange);
        m_rotationY = Mathf.Clamp(m_rotationY, m_startingY - m_maxAngleChange, m_startingY + m_maxAngleChange);

        transform.localEulerAngles = new Vector3(m_rotationX, m_rotationY, 0f);
    }
}
