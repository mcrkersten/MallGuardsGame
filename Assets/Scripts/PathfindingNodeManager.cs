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
        //return imposible Node | Needs to be caught in script by != null
        return null;
    }

    public void SetAllPathPointNeighbours() {
        foreach(PathPoint point in allPathPoints) {
            point.GetNeighbours();
        }
    }

    public List<PathPoint> ReturnNavPointList() {
        return allPathPoints;
    }

    public void Clear() {
        allPathPoints.Clear();
    }
}
