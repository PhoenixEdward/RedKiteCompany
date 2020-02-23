using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{
    public class Cell
    {

        //corner only really needed for the generator algorithm
        public enum Type
        {
            Empty,
            Floor,
            Wall,
            Door,
            Corner,
            Spawn,
            OccupiedEnemy,
            OccupiedAlly,
            PassableProp
        }

        public Type TileType { get; private set; }

        public bool IsWalkable { get; private set; } = true;

        public float movementCost { get; private set; }

        public float rangeCost { get; private set; }

        public Cell(Cell.Type type)
        {
            TileType = type;

            if (type == Cell.Type.Floor)
            {
                movementCost = 1;
                rangeCost = 1;
            }
            if (type == Cell.Type.Wall)
            {
                movementCost = 100;
                rangeCost = 100;
                IsWalkable = false;
            }
            if (type == Cell.Type.Empty)
            {
                movementCost = 100;
                rangeCost = 1;
                IsWalkable = false;
            }
            if (type == Cell.Type.Door)
            {
                movementCost = 100;
                rangeCost = 100;
                IsWalkable = false;
            }
            if (type == Cell.Type.Corner)
            {
                movementCost = 100;
                rangeCost = 100;
                IsWalkable = false;
            }
            if (type == Cell.Type.Spawn)
            {
                movementCost = 1;
                rangeCost = 1;
            }
            if (type == Cell.Type.OccupiedEnemy)
            {
                movementCost = 1;
                rangeCost = 1;
            }
            if(type == Cell.Type.OccupiedAlly)
            {
                movementCost = 1;
                rangeCost = 1;
            }
            if (type == Cell.Type.PassableProp)
            {
                movementCost = 1;
                rangeCost = 1;
            }
        }
    }
}