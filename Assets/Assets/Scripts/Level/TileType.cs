using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileType
{
    public enum Type
    {
        Empty,
        Floor,
        Wall
    }

    Type tileType;

    public bool isWalkable = true;

    public float movementCost;

    public TileType(string _Type)
    {
        if(_Type == "Floor")
        {
            tileType = Type.Floor;
            movementCost = 1;
        }
        if (_Type == "Wall")
        {
            tileType = Type.Wall;
            movementCost = 100;
            isWalkable = false;
        }
        if (_Type == "Empty")
        {
            tileType = Type.Empty;
            movementCost = 100;
            isWalkable = false;
        }
    }
}
