using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Tile {
    None = 0,
    Floor = 1,
    Door = 2,
}

public enum PathfindNode {
    None = 0,
    Walkable = 1,
    Nonwalkable = 2,
    Outside = 4,

    //Only for initialization
    Door = 3,
}

