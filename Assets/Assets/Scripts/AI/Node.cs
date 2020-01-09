using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public List<Node> neighbours;
    public Vector3Int cell = new Vector3Int();

    public Node()
    {
        neighbours = new List<Node>();
    }

}
