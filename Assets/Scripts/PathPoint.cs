using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint {
    private Vector2 position;   //The position of the node in realworld
    private int storeNumber;    //To see if point is in same room.
    private float weight;       //The weight this node carries
    private PathfindNode node;  //The type of node

    public float igCost;
    public float ihCost;

    public float FCost { get { return igCost + ihCost; } }
    public PathPoint parent;    //Parent node for A*
    public List<PathPoint> neighbours = new List<PathPoint>();

    public PathPoint(int storeNumber, Vector2 position, float weight, PathfindNode node) {
        this.storeNumber = storeNumber;
        this.position = position;
        this.weight = weight;
        this.node = node;
    }

    public void GetNeighbours() {
        PathPoint theNeighbour;

        float gridSize = MallGenerator.Instance.gridSize;
        float xx = position.x;
        float yy = position.y;
        float gridStepSize = 1 / MallGenerator.Instance.gridSize;

        xx -= gridStepSize;
        yy -= gridStepSize;

        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                theNeighbour = PathfindingNodeManager.Instance.GetPathPoint(new Vector2(xx, yy));
                if (theNeighbour != null) {
                    neighbours.Add(theNeighbour);
                }
                yy += gridStepSize;
            }
            xx += gridStepSize;

            yy = position.y;
            yy -= gridStepSize;
        }
    }

    public Vector2 GetPosition
    {
        get { return position; }
    }

    public int GetStoreNumber
    {
        get { return storeNumber; }
    }

    public PathfindNode GetNode
    {
        get { return node; }
    }

    public PathfindNode SetNode
    {
        set { this.node = value; }
    }
}
