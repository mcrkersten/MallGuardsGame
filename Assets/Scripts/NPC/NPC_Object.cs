using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Object : MonoBehaviour
{
    public delegate void NavigationCall(QueueObject queueObject);
    public static event NavigationCall OnNavigaitonCall;

    public delegate void ClosestdNavPointCall(GameObject thisNPC);
    public static event ClosestdNavPointCall WhatIsMyClosestNavPoint;

    public List<PathPoint> navigation;
    public PathPoint closestNavigationPoint;

    public void Navigation()
    {

    }
}
