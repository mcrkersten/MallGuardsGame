using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class MallGenerator : MonoBehaviour
{
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

    public bool spawnCubes;
    public Pathfinding pathfinder;

    private Tiles tiles;
    private StoreFurnitureSpawner storeFurnitureSpawner;
    private ObjectSpawner objectSpawner;
    private PathfindingNodeManager pathfindingNodeManager;

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

    private void Awake() {

        mallWidth = mallWidth % 2 == 0 ? mallWidth - 1 : mallWidth;
        mallHeight = mallHeight % 2 == 0 ? mallHeight - 1 : mallHeight;

        storeFurnitureSpawner = StoreFurnitureSpawner.Instance;
        pathfindingNodeManager = PathfindingNodeManager.Instance;
        pathfinder = new Pathfinding();

        tiles = Tiles.Instance;
        tiles.Init();

        objectSpawner = ObjectSpawner.Instance;
        objectSpawner.Init();
    }

    private void Start() {
        if (objectSpawner.allGameObjectsMall == null || objectSpawner.allGameObjectsMall.Count == 0) {
            tiles.Generate();
        }
    }

    public void GenerateMall() {
        Awake();
        ClearMall();
        tiles.Generate();
        objectSpawner.Generate();
        tiles.InitPathfindGrid();

        //SpawnFurniture();
        tiles.SetPathfinderGridValues();
        pathfindingNodeManager.SetAllPathPointNeighbours();

        if (spawnCubes) {
            objectSpawner.DebugCubes();
        }
    }



    public void ClearMall() {
        pathfindingNodeManager = PathfindingNodeManager.Instance;
        tiles = Tiles.Instance;

        objectSpawner.Clear();
        pathfindingNodeManager.Clear();
        tiles.Clear();
    }


    private void SpawnFurniture() {
        int x = 0;
        int last = 0;
        foreach (MallSpace store in tiles.storeSpaces) {
            int random = Random.Range(0, 3);
            if (random == last) {
                random = Random.Range(0, 3);
            }
            last = random;
            switch (random) {
                case 0:
                    storeFurnitureSpawner.SpawnBookStore(x);
                    break;
                case 1:
                    storeFurnitureSpawner.SpawnDrycleaning(x);
                    break;
                case 2:
                    storeFurnitureSpawner.SpawnGroceryStore(x);
                    break;
            }
            x++;
        }
    }


    public void FindPathOnButtonPress() {
        List<PathPoint> listje = pathfindingNodeManager.ReturnNavPointList();
        PathPoint toFind = listje[Random.Range(0, listje.Count)];
        PathPoint start = listje[Random.Range(0, listje.Count)];
        if(toFind.GetNode != PathfindNode.Nonwalkable && start.GetNode != PathfindNode.Nonwalkable) {
            pathfinder.FindPath(start.GetPosition, toFind.GetPosition);
        }
        else {
            FindPathOnButtonPress();
        }
    }

    private void OnDrawGizmos() {
        pathfindingNodeManager = PathfindingNodeManager.Instance;
        if (pathfindingNodeManager.ReturnNavPointList().Count > 0)//If the grid is not empty
        {
            foreach (PathPoint n in pathfindingNodeManager.ReturnNavPointList())//Loop through every node in the grid
            {
                if (n.GetNode == PathfindNode.Nonwalkable)//If the current node is a wall node
                {
                    Gizmos.color = Color.white;//Set the color of the node
                }
                else {
                    Gizmos.color = Color.yellow;//Set the color of the node
                }


                if (pathfindingNodeManager.FinalPath != null)//If the final path is not empty
                {
                    if (pathfindingNodeManager.FinalPath.Contains(n))//If the current node is in the final path
                    {
                        Gizmos.color = Color.red;//Set the color of that node
                    }

                }
                Gizmos.DrawCube(new Vector3(n.GetPosition.x, 0, n.GetPosition.y), new Vector3(.3f, .3f, .3f));//Draw the node at the position of the node.
            }
        }
    }
}



