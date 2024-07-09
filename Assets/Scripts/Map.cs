using System.Collections.Generic;
using UnityEngine;
using static CoalShaft;
using Random = System.Random;

public class Map
{
    private MapSign[,] m_matrix;
    private List<Vector2> m_outline;
    private readonly Random m_rand;

    public Map()
    {
        m_rand = new Random();
        CreateMap();
    }

    private void CreateMap()
    {
        m_matrix = new MapSign[Utility.MATRIX_SIZE, Utility.MATRIX_SIZE];
        m_outline = new List<Vector2>();

        for (int row = 0; row < m_matrix.GetLength(1); ++row)
        {
            for (int col = 0; col < m_matrix.GetLength(0); ++col)
            {
                var pos = new Vector2(col, row);
                if (row  == 0 || row == m_matrix.GetLength(1) - 1 || col == 0 || col == m_matrix.GetLength(0) - 1)
                {
                    m_matrix[col, row] = MapSign.WALL;
                    m_outline.Add(pos);
                }
                else
                {
                    m_matrix[col, row] = ChooseMapSign(pos);
                }
            }
        }
    }

    private MapSign ChooseMapSign(Vector2 pos)
    {
        var x = m_rand.Next(100);
        MapSign mapSign;

        if (pos.x % 2 == 1 && pos.y % 2 == 1)
        {
            mapSign = MapSign.WALL;
        }
        else if (x < Utility.BREAKABLE_PERCENTAGE)
        {
            mapSign = MapSign.BREAKABLE;
        }
        else
        {
            mapSign = MapSign.AIR;
        }

        return mapSign;
    }

    public int GetNumOfRows()
    {
        return m_matrix.GetLength(1);
    }

    public int GetNumOfCols()
    {
        return m_matrix.GetLength(0);
    }

    public MapSign GetMapSign(Vector2 pos)
    {
        return m_matrix[(int)pos.x, (int)pos.y];
    }
    
    public List<Vector2> GetOutline() { return m_outline; }
}