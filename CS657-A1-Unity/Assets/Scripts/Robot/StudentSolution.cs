using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudentSolution : MonoBehaviour
{
    public static StudentSolution Instance;
    
    public List<Vector2> movesGraph = new List<Vector2>();
    public int[,] ObstacleMap;
    public Dropdown dropdown;
    public Dropdown dropdowndirection;
    public Dropdown dropdownmoves;
    public int direction;
    
    [HideInInspector]
    public bool SolutionFound;
    [HideInInspector]
    public float envirement;

    public int maxMoves;

    [HideInInspector] public int moves;
    
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        envirement = 0.9f;
        direction = 0;
        maxMoves = 5000;
    }

    public void GenerateValuesArray()
    {
        RobotEngine test = new RobotEngine(35, 45, 5, 7, 30, 40, direction, envirement, maxMoves);
        test.Solution();
        test.print_robot_moves_map();

        movesGraph = test.GetBoardMoves();
        ObstacleMap = test.GetBoard();
        SolutionFound = test.GetSolutionFound();
        moves = test.GetMoves();
        
        GameManager.Instance.ChangeState(GameState.GenerateGrid);
    }

    public void SetEnvirement()
    {
        switch (dropdown.value)
        {
            case 0 :
                envirement = 0.90f;
                break;
            case 1 :
                envirement = 0.8f;
                break;
            case 2 :
                envirement = 0.55f;
                break;
            default:
                envirement = 0.9f;
                break;
        }
    }
    
    public void SetDirection()
    {
        direction = dropdowndirection.value;
    }
    
    public void SetMaxMoves()
    {
        switch (dropdownmoves.value)
        {
            case 0 :
                maxMoves = 5000;
                break;
            case 1 :
                maxMoves = 4000;
                break;
            case 2 :
                maxMoves = 3000;
                break;
            case 3 :
                maxMoves = 2000;
                break;
            case 4 :
                maxMoves = 1000;
                break;
            case 5 :
                maxMoves = 100;
                break;
            default:
                maxMoves = 5000;
                break;
        }
    }
    
    
    
}
