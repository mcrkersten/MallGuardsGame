using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpace : MonoBehaviour {
    public Vector2 size;
    public Vector2 position;

    public Vector2 GetSize
    {
        get { return size; }
    }

    public Vector2 GetPosition
    {
        get { return position; }
    }
}
