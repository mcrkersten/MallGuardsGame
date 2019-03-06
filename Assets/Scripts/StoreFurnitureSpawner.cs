using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StoreFurnitureSpawner : MonoBehaviour
{
    private PathfindingNodeManager pathfindingNodeManager;
    public GameObject[] bookStoreFurniture;
    public MallGenerator mallGenerator;


    public void SpawnBookStore(int storeNumber) {
        //GetPrefabs
        bookStoreFurniture = Resources.LoadAll<GameObject>("BookStore");
        pathfindingNodeManager = PathfindingNodeManager.Instance;
        List<PathPoint> pointsInRoom = new List<PathPoint>();
        float stepsSize = (1 / mallGenerator.gridSize);

        //Get all points back from store
        //List for all points in room.
        foreach (PathPoint point in pathfindingNodeManager.ReturnNavPointList()) {
            if (point.GetStoreNumber == storeNumber) {
                pointsInRoom.Add(point);
            }
        }


        List<PathPoint> bottomWall = GetBottomtWall(pointsInRoom);
        for (int xx = 0; xx < bottomWall.Count; xx++) {
            Vector2 testPosition = new Vector2(bottomWall[xx].GetPosition.x + (stepsSize * 2), bottomWall[xx].GetPosition.y);
            if (TestPosition(testPosition, storeNumber) && TestPosition(bottomWall[xx].GetPosition, storeNumber)) {
                //If point behind furniture = wall
                if (!TestPosition(new Vector2(bottomWall[xx].GetPosition.x + (stepsSize), bottomWall[xx].GetPosition.y - (stepsSize)), storeNumber)) {
                    mallGenerator.allGameObjectsMall.Add(Instantiate(bookStoreFurniture[0], new Vector3(bottomWall[xx].GetPosition.x + (stepsSize * 2), 0, bottomWall[xx].GetPosition.y), Quaternion.Euler(0, 180, 0), null));
                }
                else {
                    mallGenerator.allGameObjectsMall.Add(Instantiate(bookStoreFurniture[0], new Vector3(bottomWall[xx].GetPosition.x, 0, bottomWall[xx].GetPosition.y), Quaternion.Euler(0, 0, 0), null));
                }
                
                for(int yy = 0; yy < 3; yy++) {
                    PathPoint temp = pathfindingNodeManager.GetPathPoint(new Vector2(bottomWall[xx].GetPosition.x + (stepsSize * yy), bottomWall[xx].GetPosition.y));
                    temp.SetNode = PathfindNode.Nonwalkable;
                }           
            }
        }
    }

    public bool TestPosition(Vector2 toTest, int storeNumber) {
        PathPoint tested = pathfindingNodeManager.GetPathPoint(toTest);
        if(tested.GetStoreNumber == storeNumber) {
            if(tested.GetNode == PathfindNode.Walkable && tested.GetNode != PathfindNode.Door) {
                return true;
            }
        }
        return false;
    }

    public List<PathPoint> GetLeftWall(List<PathPoint> points) {
        float xMin = points.Min(v => v.GetPosition.x);
        List<PathPoint> leftWall = new List<PathPoint>();
        for (int xx = 0; xx < points.Count; xx++) {
            if (points[xx].GetPosition.x == xMin) {
                leftWall.Add(points[xx]);
            }
        }
        return leftWall;
    }

    public List<PathPoint> GetRighttWall(List<PathPoint> points) {
        float xMax = points.Max(v => v.GetPosition.x);
        List<PathPoint> rightWall = new List<PathPoint>();
        for (int xx = 0; xx < points.Count; xx++) {
            if (points[xx].GetPosition.x == xMax) {
                rightWall.Add(points[xx]);
            }
        }
        return rightWall;
    }

    public List<PathPoint> GetBottomtWall(List<PathPoint> points) {
        float yMin = points.Min(v => v.GetPosition.y);
        List<PathPoint> bottomWall = new List<PathPoint>();
        for (int xx = 0; xx < points.Count; xx++) {
            if (points[xx].GetPosition.y == yMin) {
                bottomWall.Add(points[xx]);
            }
        }
        return bottomWall;
    }

    public List<PathPoint> GetToptWall(List<PathPoint> points) {
        float yMax = points.Max(v => v.GetPosition.y);
        List<PathPoint> topWall = new List<PathPoint>();
        for (int xx = 0; xx < points.Count; xx++) {
            if (points[xx].GetPosition.x == yMax) {
                topWall.Add(points[xx]);
            }
        }
        return topWall;
    }
}
