using UnityEngine;

public class Player
{
    private readonly float m_movementSpeed = 0.8f;
    private readonly float m_rotationSpeed = 150;
    private readonly float m_gravity = 9.81f;
    private readonly float m_jumpSpeed = 5;
    private readonly float m_meleeRange = 0.5f;
    private Vector3 m_moveVelocity;
    private Vector3 m_turnVelocity;
    private CharacterController m_characterController;
    private float m_dynamiteRadius;
    private bool m_alive = true;
    private bool m_canJump;
    private bool m_hasPickaxe;
    private int m_dynamiteCount;
    private int m_availableDynamites;
    private int m_life = 100;
    private int m_score = 0;
    private readonly string m_name;
    private readonly GameObject m_playerObject;
    private readonly Camera m_camera;
    private readonly float m_maxTilt = 20f;
    private float m_currentPitch;
    private readonly float m_dmgCooldown = 0.5f;
    private float m_timeSinceTakenDmg = 0;
    private Transform m_flashLightTransform;
    private Animator m_animator;

    public Player(GameObject playerObject/*, string name = "player 1"*/)
    {
        m_characterController = playerObject.GetComponent<CharacterController>();
        m_dynamiteRadius = Utility.DYNAMITE_RADIUS;
        m_alive = true;
        m_name = PlayerPrefs.GetString("Name");
        m_playerObject = playerObject;
        m_dynamiteCount = 1;
        m_availableDynamites = m_dynamiteCount;
        m_flashLightTransform = playerObject.transform.Find("Miner3D").Find("Light.001").Find("Flash Light");
        m_camera = playerObject.GetComponentInChildren<Camera>();
        m_currentPitch = 0;
        m_canJump = false;
        m_hasPickaxe = false;
        m_animator = playerObject.GetComponentInChildren<Animator>();
    }

    public void DoMovement()
    {
        if (!m_alive)
        {
            return;
        }

        var verticalInput = Input.GetAxis("Vertical");
        var horizontalInput = Input.GetAxis("Horizontal");
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");

        if (m_characterController.isGrounded)
        {
            m_moveVelocity = m_movementSpeed * verticalInput * m_playerObject.transform.forward;
            m_moveVelocity += m_movementSpeed * horizontalInput * m_playerObject.transform.right;

            if (m_canJump && Input.GetButtonDown("Jump"))
            {
                m_moveVelocity.y = m_jumpSpeed;
                m_canJump = false;
            }
        }
        m_turnVelocity = mouseX * m_rotationSpeed * m_playerObject.transform.up;

        m_moveVelocity.y -= m_gravity * Time.deltaTime;
        m_animator.SetBool("walking", m_moveVelocity.x != 0 || m_moveVelocity.z != 0);
        m_characterController.Move(m_moveVelocity * Time.deltaTime);
        m_playerObject.transform.Rotate(m_turnVelocity * Time.deltaTime);

        if (m_camera)
        {
            var pitch = -mouseY * m_rotationSpeed * Time.deltaTime;

            if (m_currentPitch + pitch <= m_maxTilt && m_currentPitch + pitch >= -m_maxTilt)
            {
                m_currentPitch += pitch;
            }

            m_currentPitch = Mathf.Clamp(m_currentPitch, -90f, 90f);

            var currentCameraRotation = m_camera.transform.localEulerAngles;
            currentCameraRotation.x = m_currentPitch;
            m_camera.transform.localEulerAngles = currentCameraRotation;

            var currentFlashlightRotation = m_flashLightTransform.localEulerAngles;
            currentFlashlightRotation.x = -m_currentPitch - Utility.FLASHLIGHT_PITCH;
            m_flashLightTransform.localEulerAngles = currentFlashlightRotation;
        }
    }

    public void DealDamage(int dmg)
    {
        if (Time.time - m_timeSinceTakenDmg <= m_dmgCooldown)
        {
            return;
        }
        m_timeSinceTakenDmg = Time.time;
        m_life -= dmg;
        if (m_life <= 0)
        {
            m_alive = false;
        }
    }

    public bool IsAlive() { return m_alive; }

    public void AddScore(int score) { m_score += score; }

    public void IncreaseHp(int hp)
    {
        m_life += hp;
        if (m_life > Utility.MAX_HP)
        {
            m_life = Utility.MAX_HP;
        }
    }

    public void AddJump() { m_canJump = true; }

    public int GetScore() { return m_score; }

    public int GetHp() { return m_life; }

    public bool CanJump() { return m_canJump; }

    public string GetName() {  return m_name; }

    // == dynamite ==
    public bool DroppedDynamite()
    {
        if (!m_alive || 0 == m_availableDynamites)
        {
            return false;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            --m_availableDynamites;
            return true;
        }

        return false;
    }

    public float GetDynamiteRadius()
    {
        return m_dynamiteRadius;
    }

    public void SetBombRaius(float radius)
    {
        m_dynamiteRadius = radius;
    }

    public void IncreaseDynamiteAmount(int amount)
    {
        m_dynamiteCount += amount;
        m_availableDynamites += amount;
        if (m_dynamiteCount > Utility.MAX_DYNAMITES)
        {
            m_dynamiteCount = m_availableDynamites = Utility.MAX_DYNAMITES;
        }
    }

    public void RestoreDynamite()
    {
        if (m_availableDynamites == m_dynamiteCount)
        {
            return;
        }
        ++m_availableDynamites;
    }

    public int GetDynamiteCount()
    {
        return m_availableDynamites;
    }

    // == pickaxe ==
    public bool ActivatedPickaxe()
    {
        if (!m_alive || !m_hasPickaxe)
        {
            return false;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            return true;
        }

        return false;
    }

    public bool CanUsePickaxe() { return m_hasPickaxe; }

    public void AddPickaxe() { m_hasPickaxe = true; }

    public void RemovePickaxe() { m_hasPickaxe = false; }

    public float GetMeleeRange() { return m_meleeRange; }
}
