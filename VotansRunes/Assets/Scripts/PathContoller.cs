using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathContoller : MonoBehaviour
{
    public List<Transform> Elements;

    public void OnDrawGizmos()
    {
        if (Elements == null || Elements.Count < 2) return;

        for (int i = 0; i < Elements.Count - 1; i++)
            Gizmos.DrawLine(Elements[i].position, Elements[i + 1].position);
    }
}
