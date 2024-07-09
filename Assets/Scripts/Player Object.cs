using Alteruna;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObject : MonoBehaviour
{
    private Player m_player;
    public GameObject m_dynamitePrefab;
    public GameObject m_bigDynamitePrefab;
    public GameObject m_giftIcon;
    public AudioClip m_pickaxeAudio;
    public LayerMask m_breakableLayer;
    public LayerMask m_characterLayer;
    private List<Tuple<string, GameObject>> m_giftCollection;
    private bool m_dead = false;
    //private Spawner m_spawner;

    //private Alteruna.Avatar m_avatar;
    //private Multiplayer m_multiplayer;

    // Start is called before the first frame update
    void Start()
    {
        //m_avatar = GetComponent<Alteruna.Avatar>();

        //if (!m_avatar.IsMe) { return; }

        //if (m_avatar.IsOwner)
        //{
        //    var characterLayerIndex = Mathf.RoundToInt(Mathf.Log(m_characterLayer.value, 2));
        //    foreach (Transform child in transform.Find("Miner3D"))
        //    {
        //        child.gameObject.layer = characterLayerIndex;
        //    }
        //}

        //m_multiplayer = Multiplayer.Instance;
        //m_spawner = GetComponent<Spawner>();
        
        m_player = new Player(gameObject);
        m_giftCollection = new List<Tuple<string, GameObject>>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!m_avatar.IsMe) { return; }

        if (!m_player.IsAlive())
        {
            KillPlayer();
            //TODO: allow iteration over cameras: above board and as other players(can look not play)
            return;
        }

        m_player.DoMovement();
        CheckDropDynamite();
        CheckUsedPickaxe();
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        try
        {
            var canvas = transform.Find("Canvas");

            var scoreText = canvas.Find("Score");
            scoreText.gameObject.GetComponent<TMP_Text>().text = m_player.GetScore().ToString();

            var dynamite = canvas.Find("Dynamite");
            var dynamiteCountText = dynamite.Find("Count");
            dynamiteCountText.gameObject.GetComponent<TMP_Text>().text = m_player.GetDynamiteCount().ToString();

            if (m_player.GetDynamiteRadius() > Utility.DYNAMITE_RADIUS)
            {
                var dynamiteIcon = dynamite.transform.Find("Icon");

                var bigDynamiteSprite = Resources.Load<Sprite>("Textures/" + Utility.BIG_DYNAMITE_ICON);
                dynamiteIcon.GetComponent<Image>().sprite = bigDynamiteSprite;
            }
        }
        catch (Exception e) 
        {
            Debug.LogException(e);
        }

        var hpBar = gameObject.GetComponentInChildren<Slider>();
        if (hpBar != null)
        {
            hpBar.value = m_player.GetHp();
        }
        if (!m_player.CanJump())
        {
            RemoveGiftIcon(Utility.GIFT_SHOES_ICON);
        }
        if (!m_player.CanUsePickaxe())
        {
            RemoveGiftIcon(Utility.GIFT_PICKAXE_ICON);
        }
    }

    private void KillPlayer()
    {
        if (m_dead) return;

        //TODO: apply death animation
        //yield for animation length
        
        gameObject.SetActive(false);

        m_dead = true;
    }

    private void CheckDropDynamite()
    {
        if (m_player.DroppedDynamite())
        {
            var spawnPosition = new Vector3(transform.position.x, 0.1f, transform.position.z);
            var dynamitePrefab = m_player.GetDynamiteRadius() > Utility.DYNAMITE_RADIUS ? m_bigDynamitePrefab : m_dynamitePrefab;
            var dynamite = Instantiate(dynamitePrefab, spawnPosition, Quaternion.identity);
            dynamite.GetComponent<Dynamite>().SetOwner(this);

        }
        //var _ = m_spawner.Spawn(dynamitePrefab, spawnPosition, Quaternion.identity);

        //var param = new ProcedureParameters();
        //param.Set("ownerName", gameObject.name);
        //Debug.Log("about to set owner");
        //m_multiplayer.InvokeRemoteProcedure("SetOwner", UserId.All, param);
    }

    private void CheckUsedPickaxe()
    {
        var indicator = transform.Find("Canvas").Find("Indicator");

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, m_player.GetMeleeRange(), m_breakableLayer.value))
        {
            if (m_player.CanUsePickaxe())
            {
                indicator.gameObject.GetComponent<Image>().color = Color.red;
                indicator.localScale = new Vector3(0.2f, 0.2f, 1);
            }

            if (m_player.ActivatedPickaxe())
            {
                var audioSrc = GetComponent<AudioSource>();
                audioSrc.clip = m_pickaxeAudio;
                audioSrc.Play();
                hit.collider.gameObject.GetComponent<Breakable>().DestroyWall(this);
                m_player.RemovePickaxe();
            }
        }
        else
        {
            indicator.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0.2f);
            indicator.localScale = new Vector3(0.1f, 0.1f, 1);
        }
    }

    public void AddGift(string giftName)
    {
        switch (giftName)
        {
            case "GiftHeart":
                m_player.IncreaseHp(Utility.HP_GIFT_AMOUNT);
                break;
            case "BigDynamiteGift":
                m_player.SetBombRaius(Utility.BIG_DYNAMITE_RADIUS);
                break;
            case "PlusOneGift":
                m_player.IncreaseDynamiteAmount(Utility.DYNAMITE_INCREASE);
                break;
            case "GiftPickaxe":
                m_player.AddPickaxe();
                AddGiftIcon(Utility.GIFT_PICKAXE_ICON);
                break;
            case "GiftShoes":
                AddGiftIcon(Utility.GIFT_SHOES_ICON);
                m_player.AddJump();
                break;
            case "GiftPoints":
                break;
            case "GiftBuildWall":
                break;
            default:
                Debug.Log(giftName);
                break;
        }
    }

    private void AddGiftIcon(string giftName)
    {
        if (GiftInGiftCollection(giftName))
        {
            return;
        }

        try
        {
            var giftPrefab = Instantiate(m_giftIcon, new Vector3(0, 0, 0), Quaternion.identity);
            giftPrefab.transform.SetParent(gameObject.transform.Find("Canvas"));
            var gift = giftPrefab.transform;

            var giftPos = GetGiftIconPos(gift, m_giftCollection.Count);

            gift.SetLocalPositionAndRotation(giftPos, Quaternion.identity);

            var giftSprite = Resources.Load<Sprite>("Textures/" + giftName);
            gift.transform.Find("icon").GetComponent<Image>().sprite = giftSprite;

            m_giftCollection.Add(new Tuple<string, GameObject>(giftName, giftPrefab));
        }
        catch (Exception e)
        {
            Debug.Log($"Could not add gift icon, {e}");
        }
    }

    private void RemoveGiftIcon(string giftName)
    {
        if (!GiftInGiftCollection(giftName))
        {
            return;
        }

        foreach (var giftIcon in m_giftCollection)
        {
            if (giftIcon.Item1 == giftName)
            {
                Destroy(giftIcon.Item2);
                m_giftCollection.RemoveAt(m_giftCollection.IndexOf(giftIcon));
                break;
            }
        }

        foreach (var giftIcon in m_giftCollection)
        {
            var gift = giftIcon.Item2.transform;
            var giftPos = GetGiftIconPos(gift, m_giftCollection.Count - 1);
            gift.SetLocalPositionAndRotation(giftPos, Quaternion.identity);
        }
    }

    private Vector3 GetGiftIconPos(Transform gift, int giftsCount)
    {
        var giftBackground = gift.transform.Find("background");

        var giftIconWidth = giftBackground.GetComponent<Image>().rectTransform.rect.width;
        var xPos = Utility.START_X_GIFT_ICON + giftsCount * (giftIconWidth + Utility.GIFT_ICON_SPCAE) + Utility.GIFT_ICON_SPCAE;
        return new Vector3(xPos, Utility.START_Y_GIFT_ICON, 0);
    }

    private bool GiftInGiftCollection(string giftName)
    {
        foreach (var gift in m_giftCollection)
        {
            if (gift.Item1 == giftName)
            {
                return true;
            }
        }
        return false;
    }

    public void DealDamage(int dmg)
    {
        m_player.DealDamage(dmg);
    }

    public string GetName() { return m_player.GetName(); }

    public int GetScore() { return m_player.GetScore(); }

    public bool IsAlive() { return m_player.IsAlive(); }

    public float GetDynamiteRadius() { return  m_player.GetDynamiteRadius(); }

    public void RestoreDynamite() { m_player.RestoreAvailableDynamites(); }

    public void AddScore(int score) { m_player.AddScore(score); }
}
