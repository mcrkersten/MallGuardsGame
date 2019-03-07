using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StoreFurnitureSpawner : MonoBehaviour
{
    private PathfindingNodeManager pathfindingNodeManager;
    public GameObject[] storeFurniture;
    public GameObject[] registers;
    public MallGenerator mallGenerator;

    public GameObject[] storeDecoration;


    public void SpawnBookStore(int storeNumber) {
        //GetPrefabs
        registers = Resources.LoadAll<GameObject>("Register");
        storeFurniture = Resources.LoadAll<GameObject>("BookStore");

        storeDecoration = Resources.LoadAll<GameObject>("StoreDecoration");
        pathfindingNodeManager = PathfindingNodeManager.Instance;
        List<PathPoint> pointsInRoom = new List<PathPoint>();

        float stepsSize = (1 / mallGenerator.gridSize);
        float gridSize = mallGenerator.gridSize;
        float gridPercentage = (1 / gridSize);

        //Get all points back from store
        //List for all points in room.
        foreach (PathPoint point in pathfindingNodeManager.ReturnNavPointList()) {
            if (point.GetStoreNumber == storeNumber) {
                pointsInRoom.Add(point);
            }
        }

        List<MallSpace> stores = mallGenerator.GetStoreSpaces;
        MallSpace currentStore = stores[storeNumber];
        Vector2 storeSize = currentStore.GetHeightWidthofRoom;

        if(storeSize == new Vector2(3, 5)) {
            SpawnObjectOnPlace(pointsInRoom, storeDecoration[0].GetComponent<FurnitureSpace>().size, new Vector2(1 + gridPercentage, 1 + gridPercentage), 90f, storeDecoration[0], storeNumber);
            SpawnObjectOnPlace(pointsInRoom, storeDecoration[0].GetComponent<FurnitureSpace>().size, new Vector2(1 + gridPercentage, 2 + gridPercentage), 90f, storeDecoration[0], storeNumber);
        }

        if (storeSize == new Vector2(5, 3)) {
            SpawnObjectOnPlace(pointsInRoom, storeDecoration[0].GetComponent<FurnitureSpace>().size, new Vector2(3 + gridPercentage *2, 1), 90f, storeDecoration[0], storeNumber);
        }

        if (storeSize == new Vector2(5, 5)) {
            SpawnObjectOnPlace(pointsInRoom, storeDecoration[1].GetComponent<FurnitureSpace>().size, new Vector2(1, 2), -90f, storeDecoration[1], storeNumber);
            SpawnObjectOnPlace(pointsInRoom, storeDecoration[1].GetComponent<FurnitureSpace>().size, new Vector2(3, 2), 0, storeDecoration[1], storeNumber);
            SpawnObjectOnPlace(pointsInRoom, storeDecoration[0].GetComponent<FurnitureSpace>().size, new Vector2(2 + gridPercentage, 2 + gridPercentage), 90f, storeDecoration[0], storeNumber);
            SpawnObjectOnPlace(pointsInRoom, storeDecoration[0].GetComponent<FurnitureSpace>().size, new Vector2(2 + gridPercentage, 2 - gridPercentage), 90f, storeDecoration[0], storeNumber);
        }

        List<PathPoint> bottomWall = GetBottomtWall(pointsInRoom);
        List<PathPoint> topWall = GetToptWall(pointsInRoom);
        List<PathPoint> leftWall = GetLeftWall(pointsInRoom);
        List<PathPoint> rightWall = GetRightWall(pointsInRoom);

        //Set all furniture with funtions
        SetRegister(pointsInRoom, storeNumber);
        SetFurnitureHorizontalWall(bottomWall, storeFurniture, storeNumber);
        SetFurnitureHorizontalWall(topWall, storeFurniture, storeNumber);
        SetFurnitureVerticalWall(leftWall, storeFurniture, storeNumber);
        SetFurnitureVerticalWall(rightWall, storeFurniture, storeNumber);
    }

    public void SpawnDrycleaning(int storeNumber)
    {
        //GetPrefabs
        registers = Resources.LoadAll<GameObject>("Register");
        storeFurniture = Resources.LoadAll<GameObject>("DryCleaningStore");

        storeDecoration = Resources.LoadAll<GameObject>("StoreDecoration");
        pathfindingNodeManager = PathfindingNodeManager.Instance;
        List<PathPoint> pointsInRoom = new List<PathPoint>();

        float stepsSize = (1 / mallGenerator.gridSize);
        float gridSize = mallGenerator.gridSize;
        float gridPercentage = (1 / gridSize);

        //Get all points back from store
        //List for all points in room.
        foreach (PathPoint point in pathfindingNodeManager.ReturnNavPointList())
        {
            if (point.GetStoreNumber == storeNumber)
            {
                pointsInRoom.Add(point);
            }
        }

        List<MallSpace> stores = mallGenerator.GetStoreSpaces;
        MallSpace currentStore = stores[storeNumber];
        Vector2 storeSize = currentStore.GetHeightWidthofRoom;

        if (storeSize == new Vector2(5, 5))
        {
            SpawnObjectOnPlace(pointsInRoom, storeDecoration[1].GetComponent<FurnitureSpace>().size, new Vector2(1, 3), -90f, storeDecoration[1], storeNumber);
            SpawnObjectOnPlace(pointsInRoom, storeDecoration[1].GetComponent<FurnitureSpace>().size, new Vector2(3, 3), 0, storeDecoration[1], storeNumber);
        }

        List<PathPoint> leftWall = GetLeftWall(pointsInRoom);
        List<PathPoint> rightWall = GetRightWall(pointsInRoom);

        SetFurnitureVerticalWall(leftWall, storeFurniture, storeNumber);
        SetFurnitureVerticalWall(rightWall, storeFurniture, storeNumber);

    }

    private void SetFurnitureHorizontalWall(List<PathPoint> wall, GameObject[] furniture, int storeNumber) {

        float stepsSize = (1 / mallGenerator.gridSize);
        for (int xx = 0; xx < wall.Count; xx++) {

            //If own position iteration is taken go to next iteration. 
            if (!TestPosition(wall[xx].GetPosition, storeNumber)) {
                continue;
            }

            //try for lenght furniture;
            for (int fur = 0; fur < storeFurniture.Length; fur++) {
                //Get Furniture and the x size.
                int g = Random.Range(0, furniture.Length);
                GameObject furniturePiece = furniture[g];
                int blocksX = (int)furniturePiece.GetComponent<FurnitureSpace>().GetSize.x;
                blocksX -= 1;
                Vector2 testPosition = new Vector2(wall[xx].GetPosition.x + (stepsSize * blocksX), wall[xx].GetPosition.y);
                //If furniture fits, place it.
                if (TestPosition(testPosition, storeNumber) && TestPosition(wall[xx].GetPosition, storeNumber)) {
                    //If point behind furniture = wall
                    if (!TestPosition(new Vector2(wall[xx].GetPosition.x + (stepsSize), wall[xx].GetPosition.y - (stepsSize)), storeNumber)) {
                        mallGenerator.allGameObjectsMall.Add(Instantiate(furniture[g],
                            new Vector3(wall[xx].GetPosition.x + (stepsSize * blocksX), 0, wall[xx].GetPosition.y), Quaternion.Euler(0, 180, 0),
                            null));
                    }
                    else {
                        mallGenerator.allGameObjectsMall.Add(Instantiate(furniture[g],
                            new Vector3(wall[xx].GetPosition.x, 0, wall[xx].GetPosition.y),
                            Quaternion.Euler(0, 0, 0),
                            null));
                    }

                    for (int yy = 0; yy <= blocksX; yy++) {
                        PathPoint temp = pathfindingNodeManager.GetPathPoint(new Vector2(wall[xx].GetPosition.x + (stepsSize * yy), wall[xx].GetPosition.y));
                        temp.SetNode = PathfindNode.Nonwalkable;
                    }
                    //Furniture fits and we placed it. No need for further checking.
                    break;
                }
            }
        }
    }

    private void SetFurnitureVerticalWall(List<PathPoint> wall, GameObject[] furniture, int storeNumber) {
        float stepsSize = (1 / mallGenerator.gridSize);
        for (int xx = 0; xx < wall.Count; xx++) {

            //If own position iteration is taken go to next iteration. 
            if (!TestPosition(wall[xx].GetPosition, storeNumber)) {
                continue;
            }

            //try for lenght furniture;
            for (int fur = 0; fur < storeFurniture.Length; fur++) {
                //Get Furniture and the x size.
                int g = Random.Range(0, furniture.Length);
                GameObject furniturePiece = furniture[g];
                int blocksX = (int)furniturePiece.GetComponent<FurnitureSpace>().GetSize.x;
                blocksX -= 1;
                Vector2 testPosition = new Vector2(wall[xx].GetPosition.x, wall[xx].GetPosition.y + (stepsSize * blocksX));
                //If furniture fits, place it.
                if (TestPosition(testPosition, storeNumber) && TestPosition(wall[xx].GetPosition, storeNumber)) {
                    //If point behind furniture = wall
                    if (!TestPosition(new Vector2(wall[xx].GetPosition.x - (stepsSize), wall[xx].GetPosition.y + (stepsSize * blocksX)), storeNumber)) {
                        mallGenerator.allGameObjectsMall.Add(Instantiate(furniture[g],
                            new Vector3(wall[xx].GetPosition.x, 0, wall[xx].GetPosition.y), Quaternion.Euler(0, -90, 0),
                            null));
                    }
                    else {
                        mallGenerator.allGameObjectsMall.Add(Instantiate(furniture[g],
                            new Vector3(wall[xx].GetPosition.x, 0, wall[xx].GetPosition.y + (stepsSize * blocksX)),
                            Quaternion.Euler(0, 90, 0),
                            null));
                    }

                    for (int yy = 0; yy <= blocksX; yy++) {
                        PathPoint temp = pathfindingNodeManager.GetPathPoint(new Vector2(wall[xx].GetPosition.x, wall[xx].GetPosition.y + (stepsSize * yy)));
                        temp.SetNode = PathfindNode.Nonwalkable;
                    }
                    //Furniture fits and we placed it. No need for further checking.
                    break;
                }
            }
        }
    }

    private void SetRegister(List<PathPoint> points, int storeNumber) {
        float gridSize = mallGenerator.gridSize;
        float gridPercentage = (1 / gridSize);
        List<MallSpace> stores = mallGenerator.GetStoreSpaces;
        MallSpace currentStore = stores[storeNumber];
        Vector2 storeCenter = currentStore.GetMiddleOfRoom;
        Vector2 storePosition = currentStore.GetStartPositionOfRoom;
        Vector2 storeSize = currentStore.GetHeightWidthofRoom;

        mallGenerator.allGameObjectsMall.Add(Instantiate(registers[0],
                            new Vector3(storeCenter.x, 0, storePosition.y + storeSize.y - 1), Quaternion.Euler(0, 180, 0),
                            null));

        Vector2 position = new Vector2(storeCenter.x - gridPercentage, (storePosition.y + storeSize.y - 1) - gridPercentage);
        for (int z = 0; z < gridSize; z++) {
            for (int q = 0; q < gridSize; q++) {
                PathPoint temp = pathfindingNodeManager.GetPathPoint(position);
                temp.SetNode = PathfindNode.Nonwalkable;
                position[0] += gridPercentage;
            }
            position[0] = (storeCenter.x - gridPercentage);
            position[1] += gridPercentage;
        }


    }

    private void SpawnObjectOnPlace(List<PathPoint> points, Vector2 dimensions ,Vector2 position, float rotation, GameObject furniture, int storeNumber) {
        float gridSize = mallGenerator.gridSize;
        float gridPercentage = (1 / gridSize);
        List<MallSpace> stores = mallGenerator.GetStoreSpaces;
        MallSpace currentStore = stores[storeNumber];
        Vector2 storeCenter = currentStore.GetMiddleOfRoom;
        Vector2 storePosition = currentStore.GetStartPositionOfRoom;
        Vector2 storeSize = currentStore.GetHeightWidthofRoom;

        Vector2 actuaPosiotion = new Vector2(position.x + storePosition.x, position.y + storePosition.y);

        mallGenerator.allGameObjectsMall.Add(Instantiate(furniture,
                            new Vector3(actuaPosiotion.x, 0, actuaPosiotion.y), Quaternion.Euler(0, rotation, 0),
                            null));

        Vector2 positionCube = new Vector2(actuaPosiotion.x - gridPercentage, actuaPosiotion.y - gridPercentage);
        for (int z = 0; z < dimensions.x; z++) {
            for (int q = 0; q < dimensions.y; q++) {
                PathPoint temp = pathfindingNodeManager.GetPathPoint(positionCube);
                temp.SetNode = PathfindNode.Nonwalkable;
                positionCube[0] += gridPercentage;
            }
            positionCube[0] = (actuaPosiotion.x - gridPercentage);
            positionCube[1] += gridPercentage;
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


    //GET WALLS
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

    public List<PathPoint> GetRightWall(List<PathPoint> points) {
        float xMax = points.Max(v => v.GetPosition.x);
        List<PathPoint> rightWall = new List<PathPoint>();
        for (int xx = 0; xx < points.Count; xx++) {
            if (points[xx].GetPosition.x == xMax) {
                rightWall.Add(points[xx]);
            }
        }
        return rightWall;
    }

    public List<PathPoint> GetCenterWallVertical(List<PathPoint> points)
    {
        float xCenter = points.Average(v => v.GetPosition.x);
        List<PathPoint> centerWall = new List<PathPoint>();
        for (int xx = 0; xx < points.Count; xx++)
        {
            if (points[xx].GetPosition.x == xCenter)
            {
                centerWall.Add(points[xx]);
            }
        }
        return centerWall;
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
            if (points[xx].GetPosition.y == yMax) {
                topWall.Add(points[xx]);
            }
        }
        return topWall;
    }
}
