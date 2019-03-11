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

    float stepsSize;
    float gridSize;
    float gridPercentage;
    List<PathPoint> pointsInRoom = new List<PathPoint>();
    public GameObject[] storeDecoration;
    List<MallSpace> stores = new List<MallSpace>();
    MallSpace currentStore;
    Vector2 storeSize;

    private void InitSettings(int storeNumber) {
        pathfindingNodeManager = PathfindingNodeManager.Instance;
        stepsSize = (1 / mallGenerator.gridSize);
        gridSize = mallGenerator.gridSize;
        gridPercentage = (1 / gridSize);
        registers = Resources.LoadAll<GameObject>("Register");
        pointsInRoom = new List<PathPoint>();
        foreach (PathPoint point in pathfindingNodeManager.ReturnNavPointList()) {
            if (point.GetStoreNumber == storeNumber) {
                pointsInRoom.Add(point);
            }
        }

        stores = mallGenerator.GetStoreSpaces;
        currentStore = stores[storeNumber];
        storeSize = currentStore.GetHeightWidthofRoom;
        storeDecoration = Resources.LoadAll<GameObject>("StoreDecoration");
    }

    public void SpawnBookStore(int storeNumber) {
        InitSettings(storeNumber);
        storeFurniture = Resources.LoadAll<GameObject>("BookStore");
      

        //Get all points back from store
        //List for all points in room.

        if(storeSize == new Vector2(3, 3)) {
            SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 1), new Vector2(1, 1), 0, storeDecoration[5], storeNumber);
        }

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
        InitSettings(storeNumber);
        storeFurniture = Resources.LoadAll<GameObject>("DryCleaningStore");

        if (storeSize == new Vector2(5, 5))
        {
            SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 1), new Vector2(2, 3), 0, storeDecoration[5], storeNumber);
            SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 1), new Vector2(2, 1), 0, storeDecoration[5], storeNumber);
            for (int i = 1; i < 4; i++) {
                SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1 + gridPercentage, i + gridPercentage), 0, storeDecoration[2], storeNumber);
                SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(3 - gridPercentage, i + gridPercentage), 0, storeDecoration[2], storeNumber);
            }
        }

        if(storeSize == new Vector2(3, 3)) {
            if (TestIfDoor(pointsInRoom, storeNumber)) {
                SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 3), new Vector2(1, 2), 0, storeDecoration[4], storeNumber);
                SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1, 2 - gridPercentage), 0, storeDecoration[2], storeNumber);
                SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1, 1 -gridPercentage), 0, storeDecoration[2], storeNumber);
            }
            else {
                SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 3), new Vector2(1, 0), 180, storeDecoration[4], storeNumber);
                SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1, 2), 0, storeDecoration[2], storeNumber);
                SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1, 1), 0, storeDecoration[2], storeNumber);
            }
        }

        if(storeSize == new Vector2(3, 5)) {
            for (int i = 1; i < 4; i++) {
                SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1, i + gridPercentage), 0, storeDecoration[2], storeNumber);
            }
            if (TestIfDoor(pointsInRoom, storeNumber)) {
                SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 3), new Vector2(1 , 4), 0, storeDecoration[4], storeNumber);
            }
            else {
                SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 3), new Vector2(1, 0), 180, storeDecoration[4], storeNumber);
            }
        }

        if(storeSize == new Vector2(5, 3)) {
            for (int i = 1; i < 3; i++) {
                SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1, i), 0, storeDecoration[2], storeNumber);
                SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(3, i), 0, storeDecoration[2], storeNumber);
            }
        }

        List<PathPoint> leftWall = GetLeftWall(pointsInRoom);
        List<PathPoint> rightWall = GetRightWall(pointsInRoom);

        SetFurnitureVerticalWall(leftWall, storeFurniture, storeNumber);
        SetFurnitureVerticalWall(rightWall, storeFurniture, storeNumber);

    }

    public void SpawnGroceryStore(int storeNumber) {
        InitSettings(storeNumber);
        storeFurniture = Resources.LoadAll<GameObject>("GroceryStore/Fridges");
        GameObject[] fruits = Resources.LoadAll<GameObject>("GroceryStore/GroceryStoreDecoration");

        SetRegister(pointsInRoom, storeNumber);

        if (storeSize == new Vector2(5, 3)) {
            if (TestIfDoor(pointsInRoom, storeNumber)) {
                SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(1 + gridPercentage, 0 + gridPercentage*2), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
                SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(3, 0 + gridPercentage * 2), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
            }
            else {
                SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(3, 2 - gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
                SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(1 + gridPercentage, 2 - gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
            }
        }

        if(storeSize == new Vector2(3, 5)) {
            for (int i = 1; i < 3; i++) {
                if (TestIfDoor(pointsInRoom, storeNumber)) {
                    SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(1 + gridPercentage * 2, i + 1), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
                    SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(1, i + 1), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
                }
                else {
                    SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(1 + gridPercentage * 2, i + gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
                    SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(1, i + gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
                }
            }
        }

        if(storeSize == new Vector2(5, 5)) {

            SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 1), new Vector2(2, 2), 0, storeDecoration[5], storeNumber);
            List<PathPoint> bottomWall = GetBottomtWall(pointsInRoom);
            List<PathPoint> topWall = GetToptWall(pointsInRoom);
            SetFurnitureHorizontalWall(bottomWall, storeFurniture, storeNumber);
            SetFurnitureHorizontalWall(topWall, storeFurniture, storeNumber);

            SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(2 - gridPercentage,2 - gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
            SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(3 - gridPercentage, 2 - gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
            SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(2 - gridPercentage, 3 - gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
            SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(3 - gridPercentage, 3 - gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
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
                            mallGenerator.gameObject.transform));
                    }
                    else {
                        mallGenerator.allGameObjectsMall.Add(Instantiate(furniture[g],
                            new Vector3(wall[xx].GetPosition.x, 0, wall[xx].GetPosition.y),
                            Quaternion.Euler(0, 0, 0),
                            mallGenerator.gameObject.transform));
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
                            mallGenerator.gameObject.transform));
                    }
                    else {
                        mallGenerator.allGameObjectsMall.Add(Instantiate(furniture[g],
                            new Vector3(wall[xx].GetPosition.x, 0, wall[xx].GetPosition.y + (stepsSize * blocksX)),
                            Quaternion.Euler(0, 90, 0),
                            mallGenerator.gameObject.transform));
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

    private void SpawnObjectOnPlace(List<PathPoint> points, Vector2 dimensions, Vector2 position, float rotation, GameObject furniture, int storeNumber) {
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
                            mallGenerator.gameObject.transform));

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

    private void SetRegister(List<PathPoint> points, int storeNumber) {
        float gridSize = mallGenerator.gridSize;
        float gridPercentage = (1 / gridSize);
        List<MallSpace> stores = mallGenerator.GetStoreSpaces;
        MallSpace currentStore = stores[storeNumber];
        Vector2 storeCenter = currentStore.GetMiddleOfRoom;
        Vector2 storePosition = currentStore.GetStartPositionOfRoom;
        Vector2 storeSize = currentStore.GetHeightWidthofRoom;
        Tile[,] tiles;
        tiles = mallGenerator.stores[storeNumber];

        float x = storeSize.x - 1;
        float y = storeSize.y - 1;

        if (TestIfDoor(points, storeNumber)) {
            SpawnObjectOnPlace(points, new Vector2(3, 3), new Vector2(x/2,y), 180, registers[0], storeNumber);
        }
        else {
            SpawnObjectOnPlace(points, new Vector2(3, 3), new Vector2(x / 2, 0), 0, registers[0], storeNumber);
        }
    }

    public bool TestPosition(Vector2 toTest, int storeNumber) {
        PathPoint tested = pathfindingNodeManager.GetPathPoint(toTest);
        if (tested != null) {
            if (tested.GetStoreNumber == storeNumber) {
                if (tested.GetNode == PathfindNode.Walkable && tested.GetNode != PathfindNode.Door) {
                    return true;
                }
            }
            return false;
        }
        return false;
    }

    private bool TestIfDoor(List<PathPoint> points, int storeNumber) {
        float gridSize = mallGenerator.gridSize;
        float gridPercentage = (1 / gridSize);
        List<MallSpace> stores = mallGenerator.GetStoreSpaces;
        MallSpace currentStore = stores[storeNumber];
        Vector2 storeCenter = currentStore.GetMiddleOfRoom;
        Vector2 storePosition = currentStore.GetStartPositionOfRoom;
        Vector2 storeSize = currentStore.GetHeightWidthofRoom;
        Tile[,] tiles;
        tiles = mallGenerator.stores[storeNumber];

        float x = storeSize.x - 1;
        float y = storeSize.y - 1;

        if (tiles[Mathf.RoundToInt(storePosition.x + (gridPercentage * gridSize) * (storeSize.x / gridSize)), (int)storePosition.y] == Tile.Door) {
            return true;
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
