using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallSpace {
    public int x, y, w, h;
    public List<Vector2> extraTiles;
    public MallSpace(int x, int y, int w, int h) {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        extraTiles = new List<Vector2>();
    }

    public Vector2 GetMiddleOfRoom
    {
        get { return new Vector2(x + w / 2, y + h / 2); }
    }

    public Vector2 GetStartPositionOfRoom
    {
        get { return new Vector2(x, y); }
    }

    public Vector2 GetHeightWidthofRoom
    {
        get { return new Vector2(w, h); }
    }

    public void AddTile(int x, int y) {
        extraTiles.Add(new Vector2(x, y));
    }
}
