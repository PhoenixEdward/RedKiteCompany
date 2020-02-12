using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public List<Node> neighbours;
    public Vector3Int cell = new Vector3Int();
    static int lastID = 0;
    public int ID;
    public int MovementCost;

    public Node()
    {
        neighbours = new List<Node>();
        ID = lastID + 1;
        lastID = ID;
    }

}
