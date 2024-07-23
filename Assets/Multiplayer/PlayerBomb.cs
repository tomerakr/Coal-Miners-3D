using Alteruna;
using UnityEngine;


public class PlayerBomb : AttributesSync
{
    public Spawner _spawner;
    [SynchronizableField] public GameObject _bomb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _bomb = _spawner.Spawn(0, transform.position);
            BroadcastRemoteMethod("Explode");
        }
    }

    [SynchronizableMethod]
    public void Explode()
    {
        _bomb.GetComponent<explode>().setRadius();

        //yield return new WaitForSeconds(1);
        //float myValue = parameters.Get("value", 0);
        //string myString = parameters.Get("string_value", "default value");
        //Destroy(_bomb);
    }
}
