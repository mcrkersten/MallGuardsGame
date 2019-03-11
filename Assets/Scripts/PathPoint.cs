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
        float gridStepSize = 1 / MallGenerator.Instance.gridSize;                                       //Distance between gridpoints

        float xTestPosition = position.x - gridStepSize;
        float yTestPosition = position.y - gridStepSize;
        for (float x = xTestPosition; x <= position.x + gridStepSize; x += gridStepSize) {
            for (float y = yTestPosition; y <= position.y + gridStepSize; y += gridStepSize) {
                //if we are on the node tha was passed in, skip this iteration.
                if (x == 0 && y == 0) {
                    continue;
                }
                theNeighbour = PathfindingNodeManager.Instance.GetPathPoint(new Vector2(x, y));
                if (theNeighbour != null) {
                    neighbours.Add(theNeighbour);                                          //Add neigbour to the neighbourList
                }
            }
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
