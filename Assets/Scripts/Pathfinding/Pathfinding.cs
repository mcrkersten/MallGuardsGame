using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private PathfindingNodeManager pathfindingNodeManager;
    private Vector2 startPosition;
    private Vector2 TargetPosition;

    public void FindPath(Vector2 start, Vector2 target) {
        pathfindingNodeManager = PathfindingNodeManager.Instance;
        PathPoint startPoint = pathfindingNodeManager.GetPathPoint(start);
        PathPoint targetPoint = pathfindingNodeManager.GetPathPoint(target);

        List<PathPoint> openList = new List<PathPoint>();
        HashSet<PathPoint> closedList = new HashSet<PathPoint>();

        openList.Add(startPoint);
        while(openList.Count > 0) {
            PathPoint currentPoint = openList[0];                //Create a point and set it to the first item in the open list
            for (int i = 1; i < openList.Count; i++)             //Loop through the open list starting from the second object
            {
                //If the f cost of that object is less than or equal to the f cost of the current node
                if (openList[i].FCost < currentPoint.FCost || openList[i].FCost == currentPoint.FCost && openList[i].ihCost < currentPoint.ihCost)
                {
                    currentPoint = openList[i];                  //Set the current point to that object
                }
            }
            openList.Remove(currentPoint);                       //Remove that from the open list
            closedList.Add(currentPoint);                        //And add it to the closed list

            if (currentPoint == targetPoint)                     //If the current point is the same as the target node
            {
                GetFinalPath(startPoint, targetPoint);           //Calculate the final path
            }

            //Loop through each neighbor of the current point
            foreach (PathPoint NeighborNode in pathfindingNodeManager.GetNeighbours(currentPoint))
            {
                //If the neighbor is nonWalkable or has already been checked
                if (NeighborNode.GetNode == PathfindNode.Nonwalkable || closedList.Contains(NeighborNode))
                {
                    continue;                                    //Skip it
                }
                float MoveCost = currentPoint.igCost + GetManhattenDistance(currentPoint, NeighborNode);//Get the F cost of that neighbor

                //If the f cost is greater than the g cost or it is not in the open list
                if (MoveCost < NeighborNode.igCost || !openList.Contains(NeighborNode))
                {
                    NeighborNode.igCost = MoveCost;             //Set the g cost to the f cost
                    NeighborNode.ihCost = GetManhattenDistance(NeighborNode, targetPoint);//Set the h cost
                    NeighborNode.parent = currentPoint;         //Set the parent of the point for retracing steps

                    //If the neighbor is not in the openlist
                    if (!openList.Contains(NeighborNode))
                    {
                        openList.Add(NeighborNode);             //Add it to the list
                    }
                }
            }
        }
    }


    void GetFinalPath(PathPoint startPoint, PathPoint endPoint) {
        List<PathPoint> FinalPath = new List<PathPoint>();        //List to hold the path sequentially 
        PathPoint currentPoint = endPoint;                        //PathPoint to store the current node being checked

        while (currentPoint != startPoint)                        //While loop to work through each node going through the parents to the beginning of the path
        {
            FinalPath.Add(currentPoint);                          //Add that node to the final path
            currentPoint = currentPoint.parent;                   //Move onto its parent node
        }

        FinalPath.Reverse();                                     //Reverse the path to get the correct order
        pathfindingNodeManager.FinalPath = FinalPath;            //Set the final path
    }

    float GetManhattenDistance(PathPoint pointA, PathPoint pointB) {
        float ix = Mathf.Abs(pointA.GetPosition.x - pointB.GetPosition.x); //x1-x2
        float iy = Mathf.Abs(pointA.GetPosition.y - pointB.GetPosition.y);//y1-y2
        return ix + iy;//Return the sum
    }
}
