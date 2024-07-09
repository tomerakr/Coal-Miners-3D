using UnityEngine;

public class CoalShaft : MonoBehaviour
{
    public GameObject       m_wallPrefab;
    public GameObject       m_breakablePrefab;
    private GameObject[,]   m_objectMatrix;
    private Map             m_map;

    public enum MapSign
    {
        AIR,
        BREAKABLE,
        WALL
    }

    // Start is called before the first frame update
    void Start()
    {
        m_map = new Map();
        CreateMap();
    }

    private void CreateMap()
    {
        m_objectMatrix  = new GameObject[Utility.MATRIX_SIZE, Utility.MATRIX_SIZE];
        var outline = m_map.GetOutline();

        for (int row = 0; row < m_map.GetNumOfRows(); ++row)
        {
            for (int col = 0; col < m_map.GetNumOfCols(); ++col)
            {
                m_objectMatrix[col, row] = CreateObject(new Vector2(col, row));
                if (outline.Contains(new Vector2(col, row)))
                {
                    var collider = m_objectMatrix[col, row].GetComponent<BoxCollider>();
                    var newSize = new Vector3(collider.size.x, collider.size.y * 3f, collider.size.z);
                    var newCenter = new Vector3(collider.center.x, collider.center.y * 3f, collider.center.z);
                    collider.size = newSize;
                    collider.center = newCenter;
                }
            }
        }
    }

    private GameObject CreateObject(Vector2 pos)
    {
        var rotate = (Random.Range(0, 100) <= Utility.FLIP_CHANCE || pos.y == 0 || pos.y == m_map.GetNumOfRows() - 1);
        if (pos.x == 0 || pos.x == m_map.GetNumOfCols() - 1)
        {
            rotate = false;
        }
        var rotation = rotate ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 90, 0);
        var mapSign = m_map.GetMapSign(pos);
        GameObject gameObject;
        GameObject prefab;
        switch (mapSign)
        {
            case MapSign.BREAKABLE:
                prefab = m_breakablePrefab;
                break;
            case MapSign.WALL:
                prefab = m_wallPrefab;
                break;
            default:
                return null;
        }

        var diff = rotate ? Utility.OBJECT_MARGIN : 0;
        Vector3 worldPos;
        if (pos.x == m_map.GetNumOfCols() - 1)
        {
            worldPos = new Vector3(pos.x - 2 * Utility.OBJECT_MARGIN, prefab.transform.position.y, pos.y + diff);
        }
        else
        {
            worldPos = new Vector3(pos.x - diff, prefab.transform.position.y, pos.y + diff);
        }
        gameObject = Instantiate(prefab, worldPos, rotation);
        gameObject.transform.SetParent(transform);

        return gameObject;
    }
}
