using Alteruna;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalShaft : AttributesSync
{
    public GameObject       m_wallPrefab;
    public GameObject       m_breakablePrefab;
    public GameObject       m_fogPrefab;
    private GameObject[,]   m_objectMatrix;
    private Map             m_map;
    private CellData [][]   m_mapMatrix;
    private Spawner         m_spawner;
    private int             m_timeClosed = 0;
    private List<CellData>  m_fogPositions;
    private Vector3         m_mapCenter;
    private bool            m_startedClosing = false;
    private List<GameObject> m_fogObjects;

    public enum MapSign
    {
        AIR,
        BREAKABLE,
        WALL
    }

    public struct CellData
    {
        public MapSign _sign;
        public Vector2 _pos;
        public bool _outline;
        public bool _rotate;
        public Vector3 _worldPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_fogObjects    = new List<GameObject>();
        m_fogPositions  = new List<CellData>();

        CreateFogPositions();

        if (!StaticData.m_isOwner) { return; }

        m_spawner       = Multiplayer.Instance.gameObject.GetComponent<Spawner>();
        m_map           = new Map(StaticData.m_mapSeed);
        m_mapMatrix     = m_map.GetMatrix();
        CreateMap();

        //only the owner will create the map
        //    foreach (var avatar in avatars)
        //    {
        //        if (avatar.IsOwner && StaticData.m_isOwner)
        //        {
        //            m_map = new Map(StaticData.m_mapSeed);
        //            m_mapMatrix = m_map.GetMatrix();
        //            m_fogPositions = new List<CellData>();
        //            CreateMap();
        //            break;
        //        }
        //    }
    }

    private void CreateFogPositions()
    {
        var fogPos = new CellData
        {
            _worldPos = new Vector3(0, 0, (int)(Utility.MATRIX_SIZE / 2)),
            _rotate = false
        };
        m_fogPositions.Add(fogPos);

        fogPos = new CellData
        {
            _worldPos = new Vector3((int)(Utility.MATRIX_SIZE / 2), 0, 0),
            _rotate = true
        };
        m_fogPositions.Add(fogPos);

        fogPos = new CellData
        {
            _worldPos = new Vector3((int)(Utility.MATRIX_SIZE / 2), 0, Utility.MATRIX_SIZE),
            _rotate = true
        };
        m_fogPositions.Add(fogPos);

        fogPos = new CellData
        {
            _worldPos = new Vector3(Utility.MATRIX_SIZE - 2, 0, (int)(Utility.MATRIX_SIZE / 2)),
            _rotate = false
        };
        m_fogPositions.Add(fogPos);
    }

    private void CreateMap()
    {
        m_objectMatrix  = new GameObject[Utility.MATRIX_SIZE, Utility.MATRIX_SIZE];

        for (int row = 0; row < m_mapMatrix.Length; ++row)
        {
            for (int col = 0; col < m_mapMatrix[row].Length; ++col)
            {
                m_objectMatrix[col, row] = CreateObject(m_mapMatrix[row][col]);
                if (m_mapMatrix[row][col]._outline)
                {
                    if (row == (int)(m_mapMatrix.Length / 2) || col == (int)(m_mapMatrix[row].Length / 2))
                    {
                        m_mapMatrix[row][col]._worldPos = m_objectMatrix[col, row].transform.position;
                    }
                    var collider = m_objectMatrix[col, row].GetComponent<BoxCollider>();
                    var newSize = new Vector3(collider.size.x, collider.size.y * 3f, collider.size.z);
                    var newCenter = new Vector3(collider.center.x, collider.center.y * 3f, collider.center.z);
                    collider.size = newSize;
                    collider.center = newCenter;
                }
                else if (row == (int)(m_mapMatrix.Length / 2) && col == (int)(m_mapMatrix[row].Length / 2)) //midle spot
                {
                    m_mapCenter = m_objectMatrix[col, row].transform.position;
                }
            }
        }
    }

    private GameObject CreateObject(CellData cellData)
    {
        var rotation = cellData._rotate? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 90, 0);
        GameObject gameObject;
        GameObject prefab;
        string yagel;
        switch (cellData._sign)
        {
            case MapSign.BREAKABLE:
                prefab = m_breakablePrefab;
                yagel = "breakable";
                break;
            case MapSign.WALL:
                prefab = m_wallPrefab;
                yagel = "wall";
                break;
            default:
                return null;
        }

        var diff = cellData._rotate ? Utility.OBJECT_MARGIN : 0;
        var pos = cellData._pos;
        Vector3 worldPos;
        if (pos.x == m_mapMatrix[0].Length - 1)
        {
            worldPos = new Vector3(pos.x - 2 * Utility.OBJECT_MARGIN, prefab.transform.position.y, pos.y + diff);
        }
        else
        {
            worldPos = new Vector3(pos.x - diff, prefab.transform.position.y, pos.y + diff);
        }

        gameObject = m_spawner.Spawn(yagel, worldPos, rotation, new Vector3(0.349f, 0.349f, 0.349f));
        gameObject.transform.SetParent(transform);
        return gameObject;
    }

    public void CloseMap(float timer)
    {
        //if (!StaticData.m_isOwner) { return; }

        if (timer > Utility.CLOSE_MAP_TIME_INTERVAL * (m_timeClosed + 1) && m_timeClosed < Utility.MAX_CLOSE_TIMES)
        {
            if (0 == m_timeClosed) // first time of closing map
            {
                var boxCollider = m_fogPrefab.GetComponent<BoxCollider>();
                var boxCenter = boxCollider.center;
                var boxSize = boxCollider.size;
                
                var fogRotation = Quaternion.identity;
                var fogCenter = new Vector3(boxCenter.x, boxCenter.y, 0);

                foreach (var fog in m_fogPositions)
                {
                    CreateFogField(fog, boxSize, fogRotation, fogCenter, boxCenter);
                }
            }
            else
            {
                //increase damage done
            }
            m_timeClosed += 1;
        }
        if (1 == m_timeClosed && !m_startedClosing)
        {
            m_startedClosing = true;
            StartCoroutine(MoveToPosition());
        }
    }

    private void CreateFogField(CellData fog, Vector3 boxSize, Quaternion fogRotation, Vector3 fogCenter, Vector3 boxCenter)
    {
        Vector3 pos;
        var addSub = 0;

        if (m_fogPositions.IndexOf(fog) < m_fogPositions.Count - 1)
        {
            addSub = m_fogPositions.IndexOf(fog) < 2 ? 1 : -1;
        }

        if (!fog._rotate)
        {
            fogRotation = Quaternion.Euler(0, 90, 0);
            fogCenter = boxCenter;
            pos = new Vector3(fog._worldPos.x - boxSize.z * addSub, fog._worldPos.y, fog._worldPos.z);
        }
        else
        {
            pos = new Vector3(fog._worldPos.x, fog._worldPos.y, fog._worldPos.z - (boxSize.z / 2) * addSub);
        }

        var fogScale = m_fogPrefab.transform.localScale;
        var fogObj = Instantiate(m_fogPrefab, pos, fogRotation);
        fogObj.transform.localScale = new Vector3(fogScale.x * (Utility.MATRIX_SIZE - 2), fogScale.y, fogScale.z);
        fogObj.GetComponent<BoxCollider>().center = fogCenter;

        m_fogObjects.Add(fogObj);
    }

    IEnumerator MoveToPosition(float duration = 20)
    {
        float time = 0;

        Vector3[] targetPositions = new Vector3[m_fogObjects.Count];
        Vector3[] startPositions = new Vector3[m_fogObjects.Count];

        foreach (var fog in m_fogObjects)
        {
            var index = m_fogObjects.IndexOf(fog);
            var direction = (m_mapCenter - fog.transform.position).normalized;
            var target = fog.transform.position + direction * 3.4f;
            targetPositions[index] = target;
            startPositions[index] = fog.transform.position;
        }

        while (time < duration)
        {
            foreach (var fog in m_fogObjects)
            {
                var index = m_fogObjects.IndexOf(fog);
                m_fogObjects[index].transform.position = Vector3.Lerp(startPositions[index], targetPositions[index], time / duration);
            }
            time += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        foreach (var fog in m_fogObjects) // Ensure the object reaches the target position
        {
            var index = m_fogObjects.IndexOf(fog);
            m_fogObjects[index].transform.position = targetPositions[index];
        }
    }
}
