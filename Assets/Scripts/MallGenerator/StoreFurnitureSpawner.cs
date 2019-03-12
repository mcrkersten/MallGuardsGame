using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StoreFurnitureSpawner : MonoBehaviour {
    private static StoreFurnitureSpawner instance = null;
    public static StoreFurnitureSpawner Instance
    {
        get {
            if (instance == null) {
                // This is where the magic happens.
                instance = FindObjectOfType(typeof(StoreFurnitureSpawner)) as StoreFurnitureSpawner;
            }

            // If it is still null, create a new instance
            if (instance == null) {
                throw new System.ArgumentException("Parameter cannot be null", "StoreFurnitureSpawner");
            }
            return instance;
        }
    }

    private Tiles tiles;
    private PathfindingNodeManager pathfindingNodeManager;
    private ObjectSpawner objectSpawner;
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
        tiles = Tiles.Instance;
        objectSpawner = ObjectSpawner.Instance;
        pathfindingNodeManager = PathfindingNodeManager.Instance;
        stepsSize = (1 / tiles.gridSize);
        gridSize = tiles.gridSize;
        gridPercentage = (1 / gridSize);
        registers = Resources.LoadAll<GameObject>("Register");
        pointsInRoom = new List<PathPoint>();
        foreach (PathPoint point in pathfindingNodeManager.ReturnNavPointList()) {
            if (point.GetStoreNumber == storeNumber) {
                pointsInRoom.Add(point);
            }
        }

        stores = tiles.GetStoreSpaces;
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
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 1), new Vector2(1, 1), 0, storeDecoration[5], storeNumber);
        }

        if(storeSize == new Vector2(3, 5)) {
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, storeDecoration[0].GetComponent<FurnitureSpace>().size, new Vector2(1 + gridPercentage, 1 + gridPercentage), 90f, storeDecoration[0], storeNumber);
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, storeDecoration[0].GetComponent<FurnitureSpace>().size, new Vector2(1 + gridPercentage, 2 + gridPercentage), 90f, storeDecoration[0], storeNumber);
        }

        if (storeSize == new Vector2(5, 3)) {
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, storeDecoration[0].GetComponent<FurnitureSpace>().size, new Vector2(3 + gridPercentage *2, 1), 90f, storeDecoration[0], storeNumber);
        }

        if (storeSize == new Vector2(5, 5)) {
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, storeDecoration[1].GetComponent<FurnitureSpace>().size, new Vector2(1, 2), -90f, storeDecoration[1], storeNumber);
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, storeDecoration[1].GetComponent<FurnitureSpace>().size, new Vector2(3, 2), 0, storeDecoration[1], storeNumber);
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, storeDecoration[0].GetComponent<FurnitureSpace>().size, new Vector2(2 + gridPercentage, 2 + gridPercentage), 90f, storeDecoration[0], storeNumber);
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, storeDecoration[0].GetComponent<FurnitureSpace>().size, new Vector2(2 + gridPercentage, 2 - gridPercentage), 90f, storeDecoration[0], storeNumber);
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
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 1), new Vector2(2, 3), 0, storeDecoration[5], storeNumber);
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 1), new Vector2(2, 1), 0, storeDecoration[5], storeNumber);
            for (int i = 1; i < 4; i++) {
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1 + gridPercentage, i + gridPercentage), 0, storeDecoration[2], storeNumber);
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(3 - gridPercentage, i + gridPercentage), 0, storeDecoration[2], storeNumber);
            }
        }

        if(storeSize == new Vector2(3, 3)) {
            if (TestIfDoor(pointsInRoom, storeNumber)) {
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 3), new Vector2(1, 2), 0, storeDecoration[4], storeNumber);
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1, 2 - gridPercentage), 0, storeDecoration[2], storeNumber);
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1, 1 -gridPercentage), 0, storeDecoration[2], storeNumber);
            }
            else {
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 3), new Vector2(1, 0), 180, storeDecoration[4], storeNumber);
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1, 2), 0, storeDecoration[2], storeNumber);
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1, 1), 0, storeDecoration[2], storeNumber);
            }
        }

        if(storeSize == new Vector2(3, 5)) {
            for (int i = 1; i < 4; i++) {
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1, i + gridPercentage), 0, storeDecoration[2], storeNumber);
            }
            if (TestIfDoor(pointsInRoom, storeNumber)) {
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 3), new Vector2(1 , 4), 0, storeDecoration[4], storeNumber);
            }
            else {
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 3), new Vector2(1, 0), 180, storeDecoration[4], storeNumber);
            }
        }

        if(storeSize == new Vector2(5, 3)) {
            for (int i = 1; i < 3; i++) {
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(1, i), 0, storeDecoration[2], storeNumber);
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 3), new Vector2(3, i), 0, storeDecoration[2], storeNumber);
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
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(1 + gridPercentage, 0 + gridPercentage*2), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(3, 0 + gridPercentage * 2), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
            }
            else {
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(3, 2 - gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
                objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(1 + gridPercentage, 2 - gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
            }
        }

        if(storeSize == new Vector2(3, 5)) {
            for (int i = 1; i < 3; i++) {
                if (TestIfDoor(pointsInRoom, storeNumber)) {
                    objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(1 + gridPercentage * 2, i + 1), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
                    objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(1, i + 1), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
                }
                else {
                    objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(1 + gridPercentage * 2, i + gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
                    objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(1, i + gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
                }
            }
        }

        if(storeSize == new Vector2(5, 5)) {

            objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(1, 1), new Vector2(2, 2), 0, storeDecoration[5], storeNumber);
            List<PathPoint> bottomWall = GetBottomtWall(pointsInRoom);
            List<PathPoint> topWall = GetToptWall(pointsInRoom);
            SetFurnitureHorizontalWall(bottomWall, storeFurniture, storeNumber);
            SetFurnitureHorizontalWall(topWall, storeFurniture, storeNumber);

            objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(2 - gridPercentage,2 - gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(3 - gridPercentage, 2 - gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(2 - gridPercentage, 3 - gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
            objectSpawner.SpawnObjectOnPlace(pointsInRoom, new Vector2(2, 2), new Vector2(3 - gridPercentage, 3 - gridPercentage), 0, fruits[Random.Range(0, fruits.Length)], storeNumber);
        }

        List<PathPoint> leftWall = GetLeftWall(pointsInRoom);
        List<PathPoint> rightWall = GetRightWall(pointsInRoom);

        
        SetFurnitureVerticalWall(leftWall, storeFurniture, storeNumber);
        SetFurnitureVerticalWall(rightWall, storeFurniture, storeNumber);
    }

    private void SetFurnitureHorizontalWall(List<PathPoint> wall, GameObject[] furniture, int storeNumber) {

        float stepsSize = (1 / tiles.gridSize);
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
                        objectSpawner.SpawnIndividualObjects(furniture[g],
                            new Vector3(wall[xx].GetPosition.x + (stepsSize * blocksX), 0, wall[xx].GetPosition.y), Quaternion.Euler(0, 180, 0),
                            mallGenerator.gameObject.transform);
                    }
                    else {
                        objectSpawner.SpawnIndividualObjects(furniture[g],
                            new Vector3(wall[xx].GetPosition.x, 0, wall[xx].GetPosition.y),
                            Quaternion.Euler(0, 0, 0),
                            mallGenerator.gameObject.transform);
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
        float stepsSize = (1 / tiles.gridSize);
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
                        objectSpawner.SpawnIndividualObjects(furniture[g],
                            new Vector3(wall[xx].GetPosition.x, 0, wall[xx].GetPosition.y), Quaternion.Euler(0, -90, 0),
                            mallGenerator.gameObject.transform);
                    }
                    else {
                        objectSpawner.SpawnIndividualObjects(furniture[g],
                            new Vector3(wall[xx].GetPosition.x, 0, wall[xx].GetPosition.y + (stepsSize * blocksX)),
                            Quaternion.Euler(0, 90, 0),
                            mallGenerator.gameObject.transform);
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
        float gridSize = tiles.gridSize;
        float gridPercentage = (1 / gridSize);
        List<MallSpace> stores = tiles.GetStoreSpaces;
        MallSpace currentStore = stores[storeNumber];
        Vector2 storeCenter = currentStore.GetMiddleOfRoom;
        Vector2 storePosition = currentStore.GetStartPositionOfRoom;
        Vector2 storeSize = currentStore.GetHeightWidthofRoom;
        Tile[,] tilesT;
        tilesT = tiles.stores[storeNumber];

        float x = storeSize.x - 1;
        float y = storeSize.y - 1;

        if (TestIfDoor(points, storeNumber)) {
            objectSpawner.SpawnObjectOnPlace(points, new Vector2(3, 3), new Vector2(x/2,y), 180, registers[0], storeNumber);
        }
        else {
            objectSpawner.SpawnObjectOnPlace(points, new Vector2(3, 3), new Vector2(x / 2, 0), 0, registers[0], storeNumber);
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
        float gridSize = tiles.gridSize;
        float gridPercentage = (1 / gridSize);
        List<MallSpace> stores = tiles.GetStoreSpaces;
        MallSpace currentStore = stores[storeNumber];
        Vector2 storeCenter = currentStore.GetMiddleOfRoom;
        Vector2 storePosition = currentStore.GetStartPositionOfRoom;
        Vector2 storeSize = currentStore.GetHeightWidthofRoom;
        Tile[,] tilesT;
        tilesT = tiles.stores[storeNumber];

        float x = storeSize.x - 1;
        float y = storeSize.y - 1;

        if (tilesT[Mathf.RoundToInt(storePosition.x + (gridPercentage * gridSize) * (storeSize.x / gridSize)), (int)storePosition.y] == Tile.Door) {
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
