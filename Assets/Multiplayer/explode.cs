using Alteruna.Trinity;
using Alteruna;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explode : AttributesSync
{
    [SynchronizableField] private float radius = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(radius);
    }

    public void setRadius()
    {
        radius = 2.5f;
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        Debug.Log("Before explosion");
        yield return new WaitForSeconds(1);
        Debug.Log("Exploded");
    }
}
