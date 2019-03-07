using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode()]
public class TexturePicker : MonoBehaviour {
    private static TexturePicker instance = null;
    public static TexturePicker Instance
    {
        get {
            if (instance == null) {
                // This is where the magic happens.
                instance = FindObjectOfType(typeof(TexturePicker)) as TexturePicker;
            }

            // If it is still null, create a new instance
            if (instance == null) {
                GameObject i = new GameObject("TexturePicker");
                i.AddComponent(typeof(TexturePicker));
                instance = i.GetComponent<TexturePicker>();
            }
            return instance;
        }
    }

    // Use this for initialization
    public Texture2D GetWallTexture() {
        Texture2D[] wallTextures = Resources.LoadAll<Texture2D>("WallTextures");
        return wallTextures[Random.Range(0, wallTextures.Length)];
    }

    public Texture2D GetFloorTexture() {
        Texture2D[] floorTextures = Resources.LoadAll<Texture2D>("FloorTextures");
        return floorTextures[Random.Range(0, floorTextures.Length)];
    }

    public Texture2D GetHallwayFloorTexture() {
        Texture2D[] floorTextures = Resources.LoadAll<Texture2D>("HallwayFloorTextures");
        return floorTextures[Random.Range(0, floorTextures.Length)];
    }

    public Texture2D GetHallwayWallTexture() {
        Texture2D[] wallTextures = Resources.LoadAll<Texture2D>("HallwayWallTextures");
        return wallTextures[Random.Range(0, wallTextures.Length)];
    }
}