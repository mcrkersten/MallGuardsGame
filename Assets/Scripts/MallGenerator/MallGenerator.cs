using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Tile {
    None = 0,
    Floor = 1,
    Door = 2,
}

public enum PathfindNode {
    None = 0,
    Walkable = 1,
    Nonwalkable = 2,

    //Only for initialization
    Door = 3,
}

public class MallGenerator : MonoBehaviour
{
    public bool spawnCubes;
    private static MallGenerator instance = null;
    public static MallGenerator Instance
    {
        get {
            if (instance == null) {
                // This is where the magic happens.
                instance = FindObjectOfType(typeof(MallGenerator)) as MallGenerator;
            }

            // If it is still null, create a new instance
            if (instance == null) {
                throw new System.ArgumentException("Parameter cannot be null", "MallGenerator");
            }
            return instance;
        }
    }

    public int mallWidth, mallHeight, hallWidht;
    public float scaleFactor;
    private Vector4 hallwaysize;
    public Vector2 plazaSize, halwayLenght;
    public Vector2 storeWidth, storeHeight;
    public GameObject debugBox;
    PathfindingNodeManager pathfindingNodeManager;
    public StoreFurnitureSpawner StoreFurnitureSpawner;
    public Pathfinding pathfinder;

    public List<Tile[,]> stores = new List<Tile[,]>();
    private List<MallSpace> storeSpaces = new List<MallSpace>();


    private GameObject wall, door, hallwayFloor, storeFloor;

    private Tile[,] hallways;
    private List<MallSpace> hallwaySpaces = new List<MallSpace>();

    private Tile[,] doors;

    public float gridSize;
    
    [HideInInspector]
    public List<GameObject> allGameObjectsMall = new List<GameObject>();
    private int plazaSizeWidth, plazaSizeHeight;
    public int storesOnHallway;

    public List<MallSpace> GetStoreSpaces
    {
        get { return storeSpaces; }
    }

    private void Start() {
        pathfindingNodeManager = PathfindingNodeManager.Instance;
        if (allGameObjectsMall == null || allGameObjectsMall.Count == 0) {
            GenerateMall();
        }
    }

    public void GenerateMall() {
        ClearMall();
        InitMall();

        //Set Scene
        CreateMainPlaza();
        GenerateStores();
        CreateMainHallways();
        CreateDoors();
        SpawnMall();

        //Pathfinding
        InitPathfindGrid();
        SpawnFurniture();
        SetPathfinderGridValues();
        pathfindingNodeManager.SetAllPathPointNeighbours();

        if (spawnCubes) {
            DebugCubes();
        }
        
        //PlacePlayerInMall(); //TO-DO
    }

    public void FindPathOnButtonPress() {
        List<PathPoint> listje = pathfindingNodeManager.ReturnNavPointList();
        PathPoint toFind = listje[Random.Range(0, listje.Count)];
        PathPoint start = listje[Random.Range(0, listje.Count)];
        if(toFind.GetNode != PathfindNode.Nonwalkable || start.GetNode != PathfindNode.Walkable) {
            pathfinder.FindPath(start.GetPosition, toFind.GetPosition);
        }
        else {
            FindPathOnButtonPress();
        }
    }

    private void InitMall() {
        wall = Resources.Load<GameObject>("Wall");
        door = Resources.Load<GameObject>("Door");
        hallwayFloor = Resources.Load<GameObject>("HallwayFloor");
        storeFloor = Resources.Load<GameObject>("StoreFloor");
        hallways = new Tile[mallWidth, mallHeight];
        doors = new Tile[mallWidth, mallHeight];
    }

    private void InitPathfindGrid() {

        float gridPercentage = (1 / gridSize);

        //Stores
        int roomNumber = 0;
        foreach(MallSpace i in storeSpaces) {
            List<Vector2> grid = GetPlazaGrid(i, true, roomNumber);
            foreach(Vector2 x in grid) {
                
                Vector2 position = new Vector2(x.x - gridPercentage, x.y - gridPercentage);
                for (int z = 0; z < gridSize; z++) {
                    for (int q = 0; q < gridSize; q++) {
                        if (doors[(int)x.x, (int)x.y] == Tile.Door) {
                            pathfindingNodeManager.AddNavPoint(new PathPoint(roomNumber, new Vector2(position[0], position[1]), 1, PathfindNode.Door));
                        }
                        else 
                        {
                            pathfindingNodeManager.AddNavPoint(new PathPoint(roomNumber, new Vector2(position[0], position[1]), 1, PathfindNode.Walkable));
                        }
                        position[0] += gridPercentage;
                    }
                    position[0] = (x.x - gridPercentage);
                    position[1] += gridPercentage;
                }              
            }
            roomNumber++;
        }

        //Hallways and plaza
        foreach (MallSpace i in hallwaySpaces) {
            PathPoint existTester;
            List<Vector2> hallGrid = GetPlazaGrid(i, false, 0);
            foreach (Vector2 x in hallGrid) {
                Vector2 position = new Vector2(x.x - gridPercentage, x.y - gridPercentage);
                for (int z = 0; z < gridSize; z++) {
                    for (int q = 0; q < gridSize; q++) {
                        existTester = pathfindingNodeManager.GetPathPoint(new Vector2(position[0], position[1]));
                        if(existTester == null) { //if the point does not exist.
                            if (doors[(int)x.x, (int)x.y] == Tile.Door) {
                                pathfindingNodeManager.AddNavPoint(new PathPoint(roomNumber, new Vector2(position[0], position[1]), 1, PathfindNode.Door));
                            }
                            else {
                                pathfindingNodeManager.AddNavPoint(new PathPoint(roomNumber, new Vector2(position[0], position[1]), 1, PathfindNode.Walkable));
                            }
                        }
                        position[0] += gridPercentage;
                    }
                    position[0] = (x.x - gridPercentage);
                    position[1] += gridPercentage;
                }
            }
        }
    }

    private void DebugCubes() {
        //Create cubes for debugging
        List<PathPoint> pl = pathfindingNodeManager.ReturnNavPointList();
        foreach (PathPoint p in pl) {
            if (p.GetNode == PathfindNode.Walkable) {
                GameObject s = SpawnObjectForMall(debugBox,
                    new Vector3(p.GetPosition.x, 0, p.GetPosition.y),
                    Quaternion.LookRotation(new Vector3(0, 0, 1)),
                    TexturePicker.Instance.GetHallwayWallTexture());
                s.GetComponent<Renderer>().sharedMaterial.color = new Color(0, 255, 0);
            }
            else if(p.GetNode == PathfindNode.Nonwalkable) {
                GameObject s = SpawnObjectForMall(debugBox,
                    new Vector3(p.GetPosition.x, 0, p.GetPosition.y),
                    Quaternion.LookRotation(new Vector3(0, 0, 1)),
                    TexturePicker.Instance.GetHallwayWallTexture());
                s.GetComponent<Renderer>().sharedMaterial.color = new Color(255, 0, 0);
            }
        }
    }

    private void CreateDoors() {
        int i = 0;
        foreach (MallSpace space in storeSpaces) {
            if(hallways[(int)space.GetMiddleOfRoom.x, ((int)space.GetMiddleOfRoom.y - (int)space.GetHeightWidthofRoom.y / 2) - 1] == Tile.Floor) {
                stores[i][(int)space.GetMiddleOfRoom.x, ((int)space.GetMiddleOfRoom.y - (int)space.GetHeightWidthofRoom.y / 2)] = Tile.Door;
                hallways[(int)space.GetMiddleOfRoom.x, ((int)space.GetMiddleOfRoom.y - (int)space.GetHeightWidthofRoom.y / 2) - 1] = Tile.Door;

                doors[(int)space.GetMiddleOfRoom.x, ((int)space.GetMiddleOfRoom.y - (int)space.GetHeightWidthofRoom.y / 2)] = Tile.Door;
                doors[(int)space.GetMiddleOfRoom.x, ((int)space.GetMiddleOfRoom.y - (int)space.GetHeightWidthofRoom.y / 2) - 1] = Tile.Door;
            }
            else {
                stores[i][(int)space.GetMiddleOfRoom.x, ((int)space.GetMiddleOfRoom.y + (int)space.GetHeightWidthofRoom.y / 2)] = Tile.Door;
                hallways[(int)space.GetMiddleOfRoom.x, ((int)space.GetMiddleOfRoom.y + (int)space.GetHeightWidthofRoom.y / 2) + 1] = Tile.Door;

                doors[(int)space.GetMiddleOfRoom.x, ((int)space.GetMiddleOfRoom.y + (int)space.GetHeightWidthofRoom.y / 2)] = Tile.Door;
                doors[(int)space.GetMiddleOfRoom.x, ((int)space.GetMiddleOfRoom.y + (int)space.GetHeightWidthofRoom.y / 2) + 1] = Tile.Door;
            }     
            i++;         
        }
    }

    private void SetPathfinderGridValues() {
        foreach (PathPoint p in pathfindingNodeManager.ReturnNavPointList()) {
            Vector2 pos = p.GetPosition;
            if(p.GetNode == PathfindNode.Door) {
                p.SetNode = PathfindNode.Walkable;
                continue;
            }

            PathPoint theNeighbour;
            float gridStepSize = 1 / gridSize;                                       //Distance between gridpoints
            float xTestPosition = pos.x - gridStepSize;
            float yTestPosition = pos.y - gridStepSize;

            for (float x = xTestPosition; x <= pos.x + gridStepSize; x += gridStepSize) {
                for (float y = yTestPosition; y <= pos.y + gridStepSize; y += gridStepSize) {
                    //if we are on the node tha was passed in, skip this iteration.
                    if (x == 0 && y == 0) {
                        continue;
                    }
                    theNeighbour = PathfindingNodeManager.Instance.GetPathPoint(new Vector2(x, y));
                    if(theNeighbour == null) {
                        p.SetNode = PathfindNode.Nonwalkable;
                    }
                    else if (theNeighbour.GetNode == PathfindNode.None || theNeighbour != null && theNeighbour.GetStoreNumber != p.GetStoreNumber) {
                        p.SetNode = PathfindNode.Nonwalkable;
                    }
                }
            }
        }
    }

    private void SpawnMall() {
        //Spawn Hallway Floor
        DoActionForPartOfGrid(0, 0, mallWidth, mallHeight,
            (x, y) => {
                if (hallways[x, y] == Tile.Floor || hallways[x, y] == Tile.Door) {
                    SpawnObjectForMall(hallwayFloor, new Vector3(x, 0, y), Quaternion.identity, TexturePicker.Instance.GetHallwayFloorTexture());
                }
            }
        );

        //Spawn Store Door
        DoActionForPartOfGrid(0, 0, mallWidth, mallHeight,
        (x, y) => {
            if (hallways[x, y] == Tile.None) {
                DoActionForPartOfGrid(x - 1, y - 1, 3, 3,
                    (i, j) => {
                        if ((x == i || y == j) && hallways[i, j] == Tile.Door) {
                            SpawnObjectForMall(door,
                                new Vector3(x, 0, y),
                                Quaternion.LookRotation(new Vector3(i, 0, j) - new Vector3(x, 0, y)),
                                TexturePicker.Instance.GetHallwayWallTexture());
                        }else if((x == i || y == j) && hallways[i, j] == Tile.Floor) {
                            SpawnObjectForMall(wall,
                                new Vector3(x, 0, y),
                                Quaternion.LookRotation(new Vector3(i, 0, j) - new Vector3(x, 0, y)),
                                TexturePicker.Instance.GetHallwayWallTexture());
                        }
                    }
                    );
            }
        }
        );

        //Spawn Store Floor
        foreach (Tile[,] str in stores) {
            //Get floorTexture for room
            Texture2D floorTexture = TexturePicker.Instance.GetFloorTexture();
            DoActionForPartOfGrid(0, 0, mallWidth, mallHeight,
            (x, y) => {
                if (str[x, y] == Tile.Floor && hallways[x, y] != Tile.Floor || str[x, y] == Tile.Door) {
                    SpawnObjectForMall(storeFloor, new Vector3(x, 0, y), Quaternion.identity, floorTexture);
                }
            }
        );
        }

        //Spawn store Wall
        foreach (Tile[,] str in stores) {
            //Get WallTexture for room
            Texture2D wallTexture = TexturePicker.Instance.GetWallTexture();
            DoActionForPartOfGrid(0, 0, mallWidth, mallHeight,
        (x, y) => {
            //Get wallTexture for room
            if (str[x, y] == Tile.None) {
                DoActionForPartOfGrid(x - 1, y - 1, 3, 3,
                    (i, j) => {
                        if ((x == i || y == j) && str[i, j] == Tile.Floor && hallways[x, y] != Tile.Floor) {
                            SpawnObjectForMall(wall,
                                new Vector3(x, 0, y),
                                Quaternion.LookRotation(new Vector3(i, 0, j) - new Vector3(x, 0, y)),
                                wallTexture);
                        }
                    }
                    );
            }

        }
        );
        }
    }

    private GameObject SpawnObjectForMall(GameObject obj, Vector3 position, Quaternion rotation, Texture2D texture) {
        //Use this to scale spawned objects
        GameObject g = Instantiate(obj, position * scaleFactor, rotation);
        g.transform.localScale *= scaleFactor;
        g.transform.SetParent(transform, true);

        Renderer[] childerenRenderer = g.GetComponentsInChildren<Renderer>();
        foreach(Renderer r in childerenRenderer) {
            Material mat = new Material(Resources.Load<Material>("BaseMaterial"));
            mat.mainTexture = texture;
            r.material = mat;
        }

        allGameObjectsMall.Add(g);
        return g;
    }

    //Generate store tiles [NOT GAME-OBJECTS]
    private void GenerateStores() {
        //On Right top hall
        int xPos;
        int yPos;

        xPos = (int)hallwaySpaces[0].GetMiddleOfRoom.x + (plazaSizeWidth / 2);
        xPos = xPos % 2 == 0 ? xPos - 1 : xPos;
        hallwaysize[0] = xPos;
        yPos = (int)hallwaySpaces[0].GetMiddleOfRoom.y + (plazaSizeHeight / 2) + (hallWidht/2);
        yPos = yPos % 2 == 0 ? yPos - 1 : yPos;
  
        for (int i = 0; i < storesOnHallway; i++) {
            int randX = Random.Range((int)storeWidth.x, (int)storeWidth.y);
            randX = randX % 2 == 0 ? randX - 1 : randX;
            int randY = Random.Range((int)storeHeight.x, (int)storeHeight.y);
            randY = randY % 2 == 0 ? randY - 1 : randY;

            Tile[,] tempStore = new Tile[mallWidth, mallHeight];
            DoActionForPartOfGrid(xPos, yPos, randX, randY, (k, j) => { tempStore[k, j] = Tile.Floor; });
            stores.Add(tempStore);          
            
            MallSpace plaza = new MallSpace(xPos, yPos, randX, randY);
            storeSpaces.Add(plaza);

            //Set size forH Hallway
            xPos += randX;
            if (hallwaysize.y < xPos) {
                hallwaysize[1] = xPos;
            }
        }


        //On Right bottom;
        xPos = (int)hallwaySpaces[0].GetMiddleOfRoom.x + (plazaSizeWidth / 2);
        xPos = xPos % 2 == 0 ? xPos - 1 : xPos;
        yPos = (int)hallwaySpaces[0].GetMiddleOfRoom.y - (hallWidht / 2);

        for (int i = 0; i < storesOnHallway; i++) {
            int randX = Random.Range((int)storeWidth.x, (int)storeWidth.y);
            randX = randX % 2 == 0 ? randX - 1 : randX;
            int randY = Random.Range((int)storeHeight.x, (int)storeHeight.y);
            randY = randY % 2 == 0 ? randY - 1 : randY;

            Tile[,] tempStore = new Tile[mallWidth, mallHeight];
            DoActionForPartOfGrid(xPos, yPos - randY, randX, randY, (k, j) => { tempStore[k, j] = Tile.Floor; });
            stores.Add(tempStore);
            
            MallSpace plaza = new MallSpace(xPos, yPos - randY, randX, randY);
            storeSpaces.Add(plaza);

            //Set size forH Hallway
            xPos += randX;
            if (hallwaysize.y < xPos) {
                hallwaysize[1] = xPos;
            }         
        }


        //On Right top;
        xPos = (int)hallwaySpaces[0].GetMiddleOfRoom.x - (plazaSizeWidth / 2);
        xPos = xPos % 2 == 0 ? xPos - 1 : xPos;
        xPos += 1;
        hallwaysize[3] = xPos;
        yPos = yPos = (int)hallwaySpaces[0].GetMiddleOfRoom.y + (plazaSizeHeight / 2) + (hallWidht / 2);
        yPos = yPos % 2 == 0 ? yPos - 1 : yPos;

        for (int i = 0; i < storesOnHallway; i++) {
            int randX = Random.Range((int)storeWidth.x, (int)storeWidth.y);
            randX = randX % 2 == 0 ? randX - 1 : randX;
            int randY = Random.Range((int)storeHeight.x, (int)storeHeight.y);
            randY = randY % 2 == 0 ? randY - 1 : randY;

            xPos -= randX;
            Tile[,] tempStore = new Tile[mallWidth, mallHeight];
            DoActionForPartOfGrid(xPos, yPos, randX, randY, (k, j) => { tempStore[k, j] = Tile.Floor; });
            stores.Add(tempStore);

            MallSpace plaza = new MallSpace(xPos, yPos, randX, randY);
            storeSpaces.Add(plaza);

            if (hallwaysize.w > xPos) {
                hallwaysize[3] = xPos;
            }
        }

        //On Right bottom
        xPos = (int)hallwaySpaces[0].GetMiddleOfRoom.x - (plazaSizeWidth / 2);
        xPos = xPos % 2 == 0 ? xPos - 1 : xPos;
        xPos += 1;
        yPos = yPos = (int)hallwaySpaces[0].GetMiddleOfRoom.y - (hallWidht / 2);

        for (int i = 0; i < storesOnHallway; i++) {
            int randX = Random.Range((int)storeWidth.x, (int)storeWidth.y);
            randX = randX % 2 == 0 ? randX - 1 : randX;
            int randY = Random.Range((int)storeHeight.x, (int)storeHeight.y);
            randY = randY % 2 == 0 ? randY - 1 : randY;

            xPos -= randX;
            Tile[,] tempStore = new Tile[mallWidth, mallHeight];
            DoActionForPartOfGrid(xPos, yPos - randY, randX, randY, (k, j) => { tempStore[k, j] = Tile.Floor; });
            stores.Add(tempStore);

            MallSpace plaza = new MallSpace(xPos, yPos - randY, randX, randY);
            storeSpaces.Add(plaza);

            if (hallwaysize.w > xPos) {
                hallwaysize[3] = xPos;
            }
        }
    }

    private void CreateMainPlaza() {
        //Shape of Plaza is always an odd number (3x3,5x5, ect).
        plazaSizeWidth = Random.Range((int)plazaSize.x, (int)plazaSize.y + 1);
        plazaSizeWidth = plazaSizeWidth % 2 == 0 ? plazaSizeWidth - 1 : plazaSizeWidth;
        plazaSizeHeight = Random.Range((int)plazaSize.x, (int)plazaSize.y + 1);
        plazaSizeHeight = plazaSizeHeight % 2 == 0 ? plazaSizeHeight - 1 : plazaSizeHeight;

        //position of Plaza is always in center of the "map" and an odd number (3x3,5x5, ect).
        int xPos = (mallWidth / 2) - (plazaSizeWidth / 2);
        xPos = xPos % 2 == 0 ? xPos - 1 : xPos;
        int yPos = (mallHeight / 2) - (plazaSizeHeight / 2);
        yPos = yPos % 2 == 0 ? yPos - 1 : yPos;

        DoActionForPartOfGrid(xPos, yPos, plazaSizeWidth, plazaSizeHeight, (i, j) => { hallways[i, j] = Tile.Floor; });
        MallSpace plaza = new MallSpace(xPos, yPos, plazaSizeHeight, plazaSizeWidth);
        hallwaySpaces.Add(plaza);
    }

    private void CreateMainHallways() {
        int hallwayHorizontalLenght = Random.Range((int)halwayLenght.x, (int)halwayLenght.y + 1);
        //Generate Horizontal PathWay to right
        DoActionForPartOfGrid(
            (int)hallwaySpaces[0].GetMiddleOfRoom.x,
            (int)hallwaySpaces[0].GetMiddleOfRoom.y - 1,
            (int)hallwaysize[1] - (int)hallwaySpaces[0].GetMiddleOfRoom.x,
            hallWidht,
            (i, j) => { hallways[i, j] = Tile.Floor; });
        hallwaySpaces.Add(new MallSpace((int)hallwaySpaces[0].GetMiddleOfRoom.x,
            (int)hallwaySpaces[0].GetMiddleOfRoom.y - 1,
            (int)hallwaysize[1] - (int)hallwaySpaces[0].GetMiddleOfRoom.x,
            hallWidht));

        //Generate Horizontal PathWay to left
        DoActionForPartOfGrid(
            (int)hallwaysize[3],
            (int)hallwaySpaces[0].GetMiddleOfRoom.y - 1,
            (int)hallwaySpaces[0].GetMiddleOfRoom.x - (int)hallwaysize[3],
            hallWidht,
            (i, j) => { hallways[i, j] = Tile.Floor; });
        hallwaySpaces.Add(new MallSpace((int)hallwaysize[3],
            (int)hallwaySpaces[0].GetMiddleOfRoom.y - 1,
            (int)hallwaySpaces[0].GetMiddleOfRoom.x - (int)hallwaysize[3],
            hallWidht));

        int hallwayVerticalLenght = Random.Range((int)halwayLenght.x, (int)halwayLenght.y + 1);
        //Generate Vertical PathWay to top;
        DoActionForPartOfGrid(
            (int)hallwaySpaces[0].GetMiddleOfRoom.x -1,
            (int)hallwaySpaces[0].GetMiddleOfRoom.y,
            hallWidht,
            (int)(hallwaySpaces[0].GetMiddleOfRoom.y + hallwayVerticalLenght) - (int)hallwaySpaces[0].GetMiddleOfRoom.x + 1,
            (i, j) => { hallways[i, j] = Tile.Floor; });
        hallwaySpaces.Add(new MallSpace((int)hallwaySpaces[0].GetMiddleOfRoom.x - 1,
            (int)hallwaySpaces[0].GetMiddleOfRoom.y,
            hallWidht,
            (int)(hallwaySpaces[0].GetMiddleOfRoom.y + hallwayVerticalLenght) - (int)hallwaySpaces[0].GetMiddleOfRoom.x + 1));

        //Generate Vertical PathWay to bottom;
        DoActionForPartOfGrid(
            (int)hallwaySpaces[0].GetMiddleOfRoom.x -1,
            (int)hallwaySpaces[0].GetMiddleOfRoom.y - hallwayVerticalLenght,
            hallWidht,
            hallwayHorizontalLenght,
            (i, j) => { hallways[i, j] = Tile.Floor; });
        hallwaySpaces.Add(new MallSpace((int)hallwaySpaces[0].GetMiddleOfRoom.x - 1,
            (int)hallwaySpaces[0].GetMiddleOfRoom.y - hallwayVerticalLenght,
            hallWidht,
            hallwayHorizontalLenght));
    }

    public void ClearMall() {
        pathfindingNodeManager = PathfindingNodeManager.Instance;
        for (int i = 0; i < allGameObjectsMall.Count; i++) {
            DestroyImmediate(allGameObjectsMall[i]);
        }
        allGameObjectsMall.Clear();
        hallwaySpaces.Clear();
        pathfindingNodeManager.ClearManager();
        storeSpaces.Clear();
        hallways = new Tile[mallWidth, mallHeight];
        stores = new List<Tile[,]>();
        hallwaysize = new Vector4();
    }

    private void DoActionForPartOfGrid(int x, int y, int w, int h, System.Action<int, int> action) {
        for (int xx = x; xx < x + w; xx++) {
            for (int yy = y; yy < y + h; yy++) {
                if (xx < 0 || yy < 0 || xx >= mallWidth || yy >= mallHeight) {
                    continue;
                }
                action.Invoke(xx, yy);
            }
        }
    }

    private List<Vector2> GetPlazaGrid(MallSpace plaza, bool isStore, int roomNumber) {
        List<Vector2> list = new List<Vector2>();
        for (int x = plaza.x; x < plaza.x + plaza.w; x++) {
            for (int y = plaza.y; y < plaza.y + plaza.h; y++) {
                if (x < 0 || y < 0 || x >= mallWidth || y >= mallHeight) { break; }
                if (isStore) {
                    if (stores[roomNumber][x, y] == Tile.Floor && hallways[x, y] != Tile.Floor || stores[roomNumber][x, y] == Tile.Door) {
                        list.Add(new Vector2(x, y));
                    }
                }
                else {
                    if (hallways[x, y] == Tile.Floor || hallways[x, y] == Tile.Door) {
                        list.Add(new Vector2(x, y));
                    }
                }              
            }
        }
        return list;
    }

    private void SpawnFurniture() {
        int x = 0;
        int last = 0;
        foreach (MallSpace store in storeSpaces) {
            int random = Random.Range(0, 3);
            if (random == last) {
                random = Random.Range(0, 3);
            }
            last = random;
            switch (random) {
                case 0:
                    StoreFurnitureSpawner.SpawnBookStore(x);
                    break;
                case 1:
                    StoreFurnitureSpawner.SpawnDrycleaning(x);
                    break;
                case 2:
                    StoreFurnitureSpawner.SpawnGroceryStore(x);
                    break;
            }
            x++;
        }
    }


    //private void OnDrawGizmos() {
    //    pathfindingNodeManager = PathfindingNodeManager.Instance;
    //    if (pathfindingNodeManager.ReturnNavPointList().Count > 0)//If the grid is not empty
    //    {
    //        foreach (PathPoint n in pathfindingNodeManager.ReturnNavPointList())//Loop through every node in the grid
    //        {
    //            if (n.GetNode == PathfindNode.Nonwalkable)//If the current node is a wall node
    //            {
    //                Gizmos.color = Color.white;//Set the color of the node
    //            }
    //            else {
    //                Gizmos.color = Color.yellow;//Set the color of the node
    //            }


    //            if (pathfindingNodeManager.FinalPath != null)//If the final path is not empty
    //            {
    //                if (pathfindingNodeManager.FinalPath.Contains(n))//If the current node is in the final path
    //                {
    //                    Gizmos.color = Color.red;//Set the color of that node
    //                }

    //            }
    //            Gizmos.DrawCube(new Vector3(n.GetPosition.x, 0, n.GetPosition.y), new Vector3(.3f, .3f, .3f));//Draw the node at the position of the node.
    //        }
    //    }
    //}
}



