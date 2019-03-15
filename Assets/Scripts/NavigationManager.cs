using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    private List<QueueObject> queue = new List<QueueObject>();
    private Pathfinding pathfinding;

    private void InitListners()
    {
        NPC_Object.OnNavigaitonCall += AddToQueue;
    }

    private void Awake() {
        pathfinding = Pathfinding.Instance;
    }

    //TO-DO MAKE LISTNER
    public void AddToQueue(QueueObject q) {
        queue.Add(q);
    }

    public void Update() {

        if(queue.Count > 0) {
            StartCoroutine(CalculateRoute(queue[0]));
        }
    }

    IEnumerator CalculateRoute(QueueObject turn) {
        List<PathPoint> nav = pathfinding.FindPath(turn.start, turn.target);
        turn.comisionair.navigation = nav;
        queue.Remove(queue[0]);
        yield return new WaitForEndOfFrame();
    }
}
