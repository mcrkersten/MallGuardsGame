using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    private static ObjectSpawner instance = null;
    public static ObjectSpawner Instance
    {
        get {
            if (instance == null) {
                // This is where the magic happens.
                instance = FindObjectOfType(typeof(ObjectSpawner)) as ObjectSpawner;
            }

            // If it is still null, create a new instance
            if (instance == null) {
                throw new System.ArgumentException("Parameter cannot be null", "ObjectSpawner");
            }
            return instance;
        }
    }


    public GameObject debugBox;

    [HideInInspector]
    public List<GameObject> allGameObjectsMall = new List<GameObject>();
    private Tiles tiles;
    private MallGenerator mallGenerator;
    private int mallWidth, mallHeight;

    private GameObject wall, door, hallwayFloor, storeFloor;

    public void GenerateMall() {
        SpawnMall();
    }

    public void Init() {
        tiles = Tiles.Instance;
        mallGenerator = MallGenerator.Instance;

        mallWidth = tiles.mallWidth;
        mallHeight = tiles.mallHeight;

        wall = Resources.Load<GameObject>("Wall");
        door = Resources.Load<GameObject>("Door");
        hallwayFloor = Resources.Load<GameObject>("HallwayFloor");
        storeFloor = Resources.Load<GameObject>("StoreFloor");
    }

    private void SpawnMall() {
        //Spawn Hallway Floor
        DoActionForPartOfGrid(0, 0, mallWidth, mallHeight,
            (x, y) => {
                if (tiles.hallways[x, y] == Tile.Floor || tiles.hallways[x, y] == Tile.Door) {
                    SpawnObjectForMall(hallwayFloor, new Vector3(x, 0, y), Quaternion.identity, TexturePicker.Instance.GetHallwayFloorTexture());
                }
            }
        );

        //Spawn Store Door
        DoActionForPartOfGrid(0, 0, mallWidth, mallHeight,
        (x, y) => {
            if (tiles.hallways[x, y] == Tile.None) {
                DoActionForPartOfGrid(x - 1, y - 1, 3, 3,
                    (i, j) => {
                        if ((x == i || y == j) && tiles.hallways[i, j] == Tile.Door) {
                            SpawnObjectForMall(door,
                                new Vector3(x, 0, y),
                                Quaternion.LookRotation(new Vector3(i, 0, j) - new Vector3(x, 0, y)),
                                TexturePicker.Instance.GetHallwayWallTexture());
                        }
                        else if ((x == i || y == j) && tiles.hallways[i, j] == Tile.Floor) {
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
        foreach (Tile[,] str in tiles.stores) {
            //Get floorTexture for room
            Texture2D floorTexture = TexturePicker.Instance.GetFloorTexture();
            DoActionForPartOfGrid(0, 0, mallWidth, mallHeight,
            (x, y) => {
                if (str[x, y] == Tile.Floor && tiles.hallways[x, y] != Tile.Floor || str[x, y] == Tile.Door) {
                    SpawnObjectForMall(storeFloor, new Vector3(x, 0, y), Quaternion.identity, floorTexture);
                }
            }
        );
        }

        //Spawn store Wall
        foreach (Tile[,] str in tiles.stores) {
            //Get WallTexture for room
            Texture2D wallTexture = TexturePicker.Instance.GetWallTexture();
            DoActionForPartOfGrid(0, 0, mallWidth, mallHeight,
        (x, y) => {
            //Get wallTexture for room
            if (str[x, y] == Tile.None) {
                DoActionForPartOfGrid(x - 1, y - 1, 3, 3,
                    (i, j) => {
                        if ((x == i || y == j) && str[i, j] == Tile.Floor && tiles.hallways[x, y] != Tile.Floor) {
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

    public void SpawnOutside() {
        DoActionForPartOfGrid(0, 0, mallWidth, mallHeight,
            (x, y) => {
                if (tiles.outside[x, y] != Tile.Floor) {
                    if(PathfindingNodeManager.Instance.GetPathPoint(new Vector2(x,y)) == null) {
                        SpawnObjectForMall(hallwayFloor, new Vector3(x, 0, y), Quaternion.identity, TexturePicker.Instance.OutsideFloorTextures());
                        PathfindingNodeManager.Instance.AddNavPoint(new PathPoint(mallGenerator.hallNumber + 1, new Vector2(x, y), 2, PathfindNode.Outside));
                        Debug.Log("k");
                    }
                }
            }
        );
    }

    public void DebugCubes() {
        //Create cubes for debugging
        List<PathPoint> pl = PathfindingNodeManager.Instance.ReturnNavPointList();
        foreach (PathPoint p in pl) {
            if (p.GetNode == PathfindNode.Walkable || p.GetNode == PathfindNode.Outside) {
                GameObject s = SpawnObjectForMall(debugBox,
                    new Vector3(p.GetPosition.x, 0, p.GetPosition.y),
                    Quaternion.LookRotation(new Vector3(0, 0, 1)),
                    TexturePicker.Instance.GetHallwayWallTexture());
                s.GetComponent<Renderer>().sharedMaterial.color = new Color(0, 255, 0);
                continue;
            }

            else if (p.GetNode == PathfindNode.Nonwalkable) {
                GameObject s = SpawnObjectForMall(debugBox,
                    new Vector3(p.GetPosition.x, 0, p.GetPosition.y),
                    Quaternion.LookRotation(new Vector3(0, 0, 1)),
                    TexturePicker.Instance.GetHallwayWallTexture());
                s.GetComponent<Renderer>().sharedMaterial.color = new Color(255, 0, 0);
                continue;
            }
            else if (p.GetNode == PathfindNode.Door) {
                GameObject s = SpawnObjectForMall(debugBox,
                    new Vector3(p.GetPosition.x, 0, p.GetPosition.y),
                    Quaternion.LookRotation(new Vector3(0, 0, 1)),
                    TexturePicker.Instance.GetHallwayWallTexture());
                s.GetComponent<Renderer>().sharedMaterial.color = new Color(0, 0, 255);
                continue;

            }
        }
    }

    private GameObject SpawnObjectForMall(GameObject obj, Vector3 position, Quaternion rotation, Texture2D texture) {
        //Use this to scale spawned objects
        GameObject g = Instantiate(obj, position, rotation);
        g.transform.localScale *= 1;
        g.transform.SetParent(transform, true);

        Renderer[] childerenRenderer = g.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in childerenRenderer) {
            Material mat = new Material(Resources.Load<Material>("BaseMaterial"));
            mat.mainTexture = texture;
            r.material = mat;
        }

        allGameObjectsMall.Add(g);
        return g;
    }

    public void SpawnIndividualObjects(GameObject gObject, Vector3 position, Quaternion rotation, Transform parent) {
        allGameObjectsMall.Add(Instantiate(gObject, position, rotation, parent));
    }

    public void SpawnObjectOnPlace(List<PathPoint> points, Vector2 dimensions, Vector2 position, float rotation, GameObject furniture, int storeNumber) {
        float gridSize = tiles.gridSize;
        float gridPercentage = (1 / gridSize);
        List<MallSpace> stores = tiles.GetStoreSpaces;
        MallSpace currentStore = stores[storeNumber];
        Vector2 storeCenter = currentStore.GetMiddleOfRoom;
        Vector2 storePosition = currentStore.GetStartPositionOfRoom;
        Vector2 storeSize = currentStore.GetHeightWidthofRoom;

        Vector2 actuaPosiotion = new Vector2(position.x + storePosition.x, position.y + storePosition.y);

        allGameObjectsMall.Add(Instantiate(furniture,
                            new Vector3(actuaPosiotion.x, 0, actuaPosiotion.y), Quaternion.Euler(0, rotation, 0),
                            mallGenerator.gameObject.transform));

        Vector2 positionCube = new Vector2(actuaPosiotion.x - gridPercentage, actuaPosiotion.y - gridPercentage);
        for (int z = 0; z < dimensions.x; z++) {
            for (int q = 0; q < dimensions.y; q++) {
                PathPoint temp = PathfindingNodeManager.Instance.GetPathPoint(positionCube);
                temp.SetNode = PathfindNode.Nonwalkable;
                positionCube[0] += gridPercentage;
            }
            positionCube[0] = (actuaPosiotion.x - gridPercentage);
            positionCube[1] += gridPercentage;
        }
    }

    public void DoActionForPartOfGrid(int x, int y, int w, int h, System.Action<int, int> action) {
        for (int xx = x; xx < x + w; xx++) {
            for (int yy = y; yy < y + h; yy++) {
                if (xx < 0 || yy < 0 || xx >= mallWidth || yy >= mallHeight) {
                    continue;
                }
                action.Invoke(xx, yy);
            }
        }
    }

    public void Clear() {
        for (int i = 0; i < allGameObjectsMall.Count; i++) {
            DestroyImmediate(allGameObjectsMall[i]);
        }
        allGameObjectsMall.Clear();
    }
}
