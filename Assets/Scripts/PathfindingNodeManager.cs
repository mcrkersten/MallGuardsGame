using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingNodeManager {
    private static PathfindingNodeManager instance = null;
    private static readonly object padlock = new object();
    public static PathfindingNodeManager Instance
    {
        get {
            lock (padlock) {
                if (instance == null) {
                    instance = new PathfindingNodeManager();
                }
                return instance;
            }
        }
    }
    private MallGenerator mallGenerator;

    PathfindingNodeManager() {
        mallGenerator = MallGenerator.Instance;
    }

    private List<PathPoint> allPathPoints = new List<PathPoint>();
    public List<PathPoint> FinalPath;

    //Add a PathPoint to the list of total PathPoints
    public void AddNavPoint(PathPoint point) {
        if (!allPathPoints.Contains(point)) {
            allPathPoints.Add(point);
        }
    }

    //Return PathPoint based on position
    public PathPoint GetPathPoint(Vector2 position) {
        foreach (PathPoint p in allPathPoints) {
            if (p.GetPosition == position) {
                return p;
            }
        }
        //return imposible Node
        Debug.Log("woops");
        return new PathPoint(1000, new Vector2(0,0), 1000, 0);
    }

    //Return a list of all neighbours of the given PathPoint
    public List<PathPoint> GetNeighbours(PathPoint parentPoint) {
        List<PathPoint> neighbourList = new List<PathPoint>();                               //List of neighbours we will return
        float gridStepSize = 1/mallGenerator.gridSize;                                         //Distance between gridpoints
        Vector2 parentPosition = parentPoint.GetPosition;                                    //Position of the Pathpoint that we are getting the neighbours from
        float xTestPosition;                                                                 //Variable to check if the xPosition is inside the mallgrid
        float yTestPosition;                                                                 //Variable to check if the yPosition is inside the mallgrid


        //Get rightside neighbour
        xTestPosition = parentPosition.x + gridStepSize;
        yTestPosition = parentPosition.y;
        //if (parentPosition.x > 0 && parentPosition.x < xTestPosition){                     //Check if the node Xposition does not fall outside the size of the grid        
            //if (parentPosition.y > 0 && parentPosition.y < yTestPosition){                 //Check if the node Yposition we are checking does not fall outside the size of the grid
                neighbourList.Add(GetPathPoint(new Vector2(xTestPosition, yTestPosition)));    //Add neigbour to the neighbourList
            //}
        //}
        //repeat for each side

        //Get leftSide neighbour
        xTestPosition = parentPosition.x - gridStepSize;
        yTestPosition = parentPosition.y;
        //if (parentPosition.x >= 0 && parentPosition.x < xTestPosition) {
            //if (parentPosition.y >= 0 && parentPosition.y < yTestPosition) {
                neighbourList.Add(GetPathPoint(new Vector2(xTestPosition, yTestPosition)));
            //}
        //}

        //Get topSide neighbour
        xTestPosition = parentPosition.x;
        yTestPosition = parentPosition.y + gridStepSize;
        //if (parentPosition.x >= 0 && parentPosition.x < xTestPosition) {
            //if (parentPosition.y >= 0 && parentPosition.y < yTestPosition) {
                neighbourList.Add(GetPathPoint(new Vector2(xTestPosition, yTestPosition)));
            //}
        //}

        //Get downSide neighbour
        xTestPosition = parentPosition.x;
        yTestPosition = parentPosition.y - gridStepSize;
        //if (parentPosition.x >= 0 && parentPosition.x < xTestPosition) {
            //if (parentPosition.y >= 0 && parentPosition.y < yTestPosition) {
                neighbourList.Add(GetPathPoint(new Vector2(xTestPosition, yTestPosition)));
            //}
        //}
        return neighbourList;
    }

    public List<PathPoint> ReturnNavPointList() {
        return allPathPoints;
    }

    public void ClearManager() {
        allPathPoints.Clear();
    }
}
