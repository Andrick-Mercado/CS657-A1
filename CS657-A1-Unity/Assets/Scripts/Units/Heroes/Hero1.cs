using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero1 : BaseHero
{
    public static Hero1 Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public IEnumerator StartMoving()
    {
        Debug.Log("Cor is run?");
        var tempMovesGraph = StudentSolution.Instance.movesGraph;
        for (var i = 0; i < tempMovesGraph.Count; i++)
        {
            transform.position = new Vector3 (tempMovesGraph[i].x, tempMovesGraph[i].y, 0);
            yield return new WaitForSeconds(1f); 
        }

        yield return null;
    }
}
