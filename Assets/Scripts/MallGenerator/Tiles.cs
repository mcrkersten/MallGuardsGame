using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles
{
    private static Tiles instance = null;
    private static readonly object padlock = new object();
    public static Tiles Instance
    {
        get {
            lock (padlock) {
                if (instance == null) {
                    instance = new Tiles();
                }
                return instance;
            }
        }
    }

    Tiles() {

    }

    public List<Tile[,]> stores = new List<Tile[,]>();
    public Tile[,] hallways;
    public Tile[,] doors;
    public Tile[,] outside;
    private MallGenerator mallGenerator;
    private PathfindingNodeManager pathfindingNodeManager;
    private List<MallSpace> hallwaySpaces = new List<MallSpace>();
    public  List<MallSpace> storeSpaces = new List<MallSpace>();
    private Vector4 hallwaysize;
    private int plazaSizeWidth, plazaSizeHeight;

    [HideInInspector]
    public float gridSize = 3;
    [HideInInspector]
    public int hallWidht = 3;
    [Header("Plaza dimensions")]
    public Vector2 plazaSize;

    [Header("Store Settings")]
    [Tooltip("Minimum and maximum width of store")]
    public Vector2 storeWidth;
    [Tooltip("Minimum and maximum height of store")]
    public Vector2 storeHeight;
    [Tooltip("Amount of stores on each side of a hall")]
    public int storesOnHallway;

    [Header("Max size of grid")]
    public int mallWidth, mallHeight;

    public void Init() {
        mallGenerator = MallGenerator.Instance;
        pathfindingNodeManager = PathfindingNodeManager.Instance;
        hallWidht = mallGenerator.hallWidht;
        gridSize = mallGenerator.gridSize;
        plazaSize = mallGenerator.plazaSize;
        storeWidth = mallGenerator.storeWidth;
        storeHeight = mallGenerator.storeHeight;
        storesOnHallway = mallGenerator.storesOnHallway;
        mallWidth = mallGenerator.mallWidth;
        mallHeight = mallGenerator.mallHeight;

        stores = new List<Tile[,]>();
        hallways = new Tile[mallWidth, mallHeight];
        doors = new Tile[mallWidth, mallHeight];
        outside = new Tile[mallWidth, mallHeight];

        storeSpaces = new List<MallSpace>();
        hallwaySpaces = new List<MallSpace>();
    }

    public void Generate() {
        CreateMainPlaza();
        GenerateStores();
        CreateMainHallways();
        CreateDoors();
    }
     
    //Generate store tiles [NOT GAME-OBJECTS]
    private void GenerateStores() {
        //On Right top hall
        int xPos;
        int yPos;

        xPos = (int)hallwaySpaces[0].GetMiddleOfRoom.x + (plazaSizeWidth / 2);
        xPos = xPos % 2 == 0 ? xPos - 1 : xPos;
        yPos = (int)hallwaySpaces[0].GetMiddleOfRoom.y + (plazaSizeHeight / 2) + (hallWidht / 2);
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

            //Set size for Hallway
            xPos += randX;
            //yPos += randY;
            if (hallwaysize[0] < xPos) {
                hallwaysize[0] = xPos;
            }
            if (hallwaysize[2] < randY) {
                hallwaysize[2] = randY;
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
            if (hallwaysize[0] < xPos) {
                hallwaysize[0] = xPos;
            }
            if (hallwaysize[1] < randY) {
                hallwaysize[1] = randY;
            }
        }


        //On Left top;
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

            if (hallwaysize[3] > xPos) {
                hallwaysize[3] = xPos;
            }
            if (hallwaysize[2] < randY) {
                hallwaysize[2] = randY;
            }
        }

        //On Left bottom
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

            if (hallwaysize[3] > xPos) {
                hallwaysize[3] = xPos;
            }
            if (hallwaysize[1] < randY) {
                hallwaysize[1] = randY;
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

        //Generate Horizontal PathWay to right
        DoActionForPartOfGrid(
            (int)hallwaySpaces[0].GetMiddleOfRoom.x,
            (int)hallwaySpaces[0].GetMiddleOfRoom.y - 1,
            (int)hallwaysize[0] - (int)hallwaySpaces[0].GetMiddleOfRoom.x,
            hallWidht,
            (i, j) => { hallways[i, j] = Tile.Floor; });
        hallwaySpaces.Add(new MallSpace((int)hallwaySpaces[0].GetMiddleOfRoom.x,
            (int)hallwaySpaces[0].GetMiddleOfRoom.y - 1,
            (int)hallwaysize[0] - (int)hallwaySpaces[0].GetMiddleOfRoom.x,
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


        //Generate Vertical PathWay to top;
        DoActionForPartOfGrid(
            (int)hallwaySpaces[0].GetMiddleOfRoom.x - 1,
            (int)hallwaySpaces[0].GetMiddleOfRoom.y,
            hallWidht,
            (int)hallwaysize[2] + (int)hallwaySpaces[0].h / 2,
            (i, j) => { hallways[i, j] = Tile.Floor; });
        hallwaySpaces.Add(new MallSpace((int)hallwaySpaces[0].GetMiddleOfRoom.x - 1,
            (int)hallwaySpaces[0].GetMiddleOfRoom.y,
            hallWidht,
            (int)hallwaysize[2] + (int)hallwaySpaces[0].h / 2));

        //Generate Vertical PathWay to bottom;
        DoActionForPartOfGrid(
            (int)hallwaySpaces[0].GetMiddleOfRoom.x - 1,
            (int)hallwaySpaces[0].GetMiddleOfRoom.y - (int)hallwaysize[1] - 1,
            hallWidht,
            (int)hallwaysize[1],
            (i, j) => { hallways[i, j] = Tile.Floor; });
        hallwaySpaces.Add(new MallSpace((int)hallwaySpaces[0].GetMiddleOfRoom.x - 1,
            (int)hallwaySpaces[0].GetMiddleOfRoom.y - (int)hallwaysize[1] - 1,
            hallWidht,
            (int)hallwaysize[1]));
    }

    private void CreateDoors() {
        int i = 0;
        foreach (MallSpace space in storeSpaces) {
            if (hallways[(int)space.GetMiddleOfRoom.x, ((int)space.GetMiddleOfRoom.y - (int)space.GetHeightWidthofRoom.y / 2) - 1] == Tile.Floor) {
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

        //X-horizotal right
        if (hallways[(int)hallwaysize[0], (int)hallwaySpaces[0].GetMiddleOfRoom.y] != Tile.Floor) {
            hallways[(int)hallwaysize[0] - 1, (int)hallwaySpaces[0].GetMiddleOfRoom.y] = Tile.Door;
            doors[(int)hallwaysize[0] - 1, (int)hallwaySpaces[0].GetMiddleOfRoom.y] = Tile.Door;
        }

        //X-horizotal left
        if (hallways[(int)hallwaysize[3] - 1, (int)hallwaySpaces[0].GetMiddleOfRoom.y] != Tile.Floor) {
            hallways[(int)hallwaysize[3], (int)hallwaySpaces[0].GetMiddleOfRoom.y] = Tile.Door;
            doors[(int)hallwaysize[3], (int)hallwaySpaces[0].GetMiddleOfRoom.y] = Tile.Door;
        }

        //X-vertical top
        if (hallways[(int)hallwaySpaces[0].GetMiddleOfRoom.x, (((int)hallwaysize[1] + 1) + ((int)hallwaySpaces[0].GetMiddleOfRoom.y + 1))] != Tile.Floor) {
            hallways[(int)hallwaySpaces[0].GetMiddleOfRoom.x, (((int)hallwaysize[1]) + ((int)hallwaySpaces[0].GetMiddleOfRoom.y + 1))] = Tile.Door;
            doors[(int)hallwaySpaces[0].GetMiddleOfRoom.x, (((int)hallwaysize[1]) + ((int)hallwaySpaces[0].GetMiddleOfRoom.y + 1))] = Tile.Door;
        }

        //X-vertical down
        if (hallways[(int)hallwaySpaces[0].GetMiddleOfRoom.x, ((int)hallwaySpaces[0].GetMiddleOfRoom.y - 1) - ((int)hallwaysize[2] + 1)] != Tile.Floor) {
            hallways[(int)hallwaySpaces[0].GetMiddleOfRoom.x, ((int)hallwaySpaces[0].GetMiddleOfRoom.y - 1) - (int)hallwaysize[2]] = Tile.Door;
            doors[(int)hallwaySpaces[0].GetMiddleOfRoom.x, ((int)hallwaySpaces[0].GetMiddleOfRoom.y - 1) - (int)hallwaysize[2]] = Tile.Door;
        }
    }

    public void InitPathfindGrid() {

        float gridPercentage = (1 / gridSize);

        //Stores
        int roomNumber = 0;
        foreach (MallSpace baseSpace in storeSpaces) {
            List<Vector2> grid = GetPlazaGrid(baseSpace, true, roomNumber);
            foreach (Vector2 x in grid) {

                Vector2 position = new Vector2(x.x - gridPercentage, x.y - gridPercentage);
                for (int z = 0; z < gridSize; z++) {
                    for (int q = 0; q < gridSize; q++) {
                        if (doors[(int)x.x, (int)x.y] == Tile.Door) {
                            pathfindingNodeManager.AddNavPoint(new PathPoint(roomNumber, new Vector2(position[0], position[1]), 1, PathfindNode.Door));
                        }
                        else if(stores[roomNumber][(int)x.x, (int)x.y] != Tile.None) {
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
        mallGenerator.hallNumber = roomNumber;
        //Hallways and plaza
        foreach (MallSpace i in hallwaySpaces) {
            PathPoint existTester;
            List<Vector2> hallGrid = GetPlazaGrid(i, false, 0);
            foreach (Vector2 x in hallGrid) {
                Vector2 position = new Vector2(x.x - gridPercentage, x.y - gridPercentage);
                for (int z = 0; z < gridSize; z++) {
                    for (int q = 0; q < gridSize; q++) {
                        existTester = pathfindingNodeManager.GetPathPoint(new Vector2(position[0], position[1]));
                        if (existTester == null) { //if the point does not exist.
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

    public void SetPathfinderGridValues() {
        List<PathPoint> points = pathfindingNodeManager.ReturnNavPointList();
        PathPoint neighbour;
        Vector2 pointPosition;

        foreach (PathPoint pnt in points) {
            if (pnt.GetNode != PathfindNode.Outside) {

                if (pnt.GetNode == PathfindNode.Door) {
                    //pnt.SetNode = PathfindNode.Walkable;
                    continue;
                }

                if (pnt == null) {
                    continue;
                }

                pointPosition = pnt.GetPosition;

                float xx = pointPosition.x;
                float yy = pointPosition.y;
                float gridStepSize = 1 / gridSize;

                xx -= gridStepSize;
                yy -= gridStepSize;

                for (int x = 0; x < gridSize; x++) {
                    for (int y = 0; y < gridSize; y++) {
                        neighbour = pathfindingNodeManager.GetPathPoint(new Vector2(xx, yy));
                        if (neighbour == null) {
                            pnt.SetNode = PathfindNode.Nonwalkable;
                        }
                        else if (neighbour.GetNode == PathfindNode.None || neighbour.GetStoreNumber != pnt.GetStoreNumber) {
                            pnt.SetNode = PathfindNode.Nonwalkable;
                        }
                        yy += gridStepSize;
                    }
                    xx += gridStepSize;

                    yy = pointPosition.y;
                    yy -= gridStepSize;
                }
            }

            else if(pnt.GetNode == PathfindNode.Outside) {
                if (pnt == null) {
                    continue;
                }

                pointPosition = pnt.GetPosition;
                float xx = pointPosition.x;
                float yy = pointPosition.y;

                xx -= 1;
                yy -= 1;

                for (int x = 0; x < 2; x++) {
                    for (int y = 0; y < 2; y++) {
                        neighbour = pathfindingNodeManager.GetPathPoint(new Vector2(xx, yy));
                        if (neighbour == null) {
                            pnt.SetNode = PathfindNode.Nonwalkable;
                        }
                        else if (neighbour.GetNode == PathfindNode.None || neighbour.GetStoreNumber != pnt.GetStoreNumber) {
                            pnt.SetNode = PathfindNode.Nonwalkable;
                        }
                        yy += 1;
                    }
                    xx += 1;

                    yy = pointPosition.y;
                    yy -= 1;
                }
            }
        }
    }
    

   public void DoActionForPartOfGrid(int x, int y, int w, int h, System.Action<int, int> action) {
        for (int xx = x; xx < x + w; xx++) {
            for (int yy = y; yy < y + h; yy++) {
                if (xx < 0 || yy < 0 || xx >= mallWidth || yy >= mallHeight) {
                    continue;
                }
                outside[xx, yy] = Tile.Floor;
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

    public List<MallSpace> GetStoreSpaces
    {
        get { return storeSpaces; }
    }

    public void Clear() {
        storeSpaces.Clear();
        hallways = new Tile[mallWidth, mallHeight];
        stores = new List<Tile[,]>();
        hallwaysize = new Vector4();
        hallwaySpaces.Clear();
    }
}
