using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public List<Node> neighbours;
    public Vector3Int cell = new Vector3Int();

    public Node()
    {
        neighbours = new List<Node>();
    }

}
