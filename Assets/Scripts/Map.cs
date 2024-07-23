using System;
using System.Collections.Generic;
using UnityEngine;
using static CoalShaft;
using Random = System.Random;

public class Map
{
    private CellData[][] m_matrix;
    private Random m_rand;

    public Map(int seed = 17)
    {
        m_rand = new Random(seed);
        CreateMap();
    }

    private void CreateMap()
    {
        m_matrix = new CellData[Utility.MATRIX_SIZE][];

        for (int row = 0; row < m_matrix.Length; ++row)
        {
            m_matrix[row] = new CellData[Utility.MATRIX_SIZE];

            for (int col = 0; col < m_matrix[row].Length; ++col)
            {
                var cell = new CellData { _pos = new Vector2(row, col) };

                if (row  == 0 || row == m_matrix.Length - 1 || col == 0 || col == m_matrix[row].Length - 1)
                {
                    cell._outline = true;
                    cell._sign = MapSign.WALL;
                }
                else
                {
                    cell._sign = ChooseMapSign(cell._pos, m_rand.Next(100));
                }

                cell._rotate = m_rand.Next(100) <= Utility.FLIP_CHANCE || col == 0 || col == m_matrix.Length - 1;

                if (row == 0 || row == m_matrix[0].Length - 1)
                {
                    cell._rotate = false;
                }
                m_matrix[row][col] = cell;
            }
        }
    }

    private MapSign ChooseMapSign(Vector2 pos, int rand)
    {
        MapSign mapSign;

        if (pos.x % 2 == 1 && pos.y % 2 == 1)
        {
            mapSign = MapSign.WALL;
        }
        else if (rand < Utility.BREAKABLE_PERCENTAGE)
        {
            mapSign = MapSign.BREAKABLE;
        }
        else
        {
            mapSign = MapSign.AIR;
        }

        return mapSign;
    }

    public CellData[][] GetMatrix() { return m_matrix; }
}