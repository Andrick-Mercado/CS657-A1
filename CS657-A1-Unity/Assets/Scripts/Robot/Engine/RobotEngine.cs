using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RobotEngine
{
    private int[,] board;
    private int[,] Actual_Map;
    private int[,] board_robot_moves;
    private int startLocRow;
    private int startLocCol;
    private int endLocRow;
    private int endLocCol;
    private int direction;
    private float envirement;
    private int maxMoves;
    private string fileName;
    private int[] startingCords;
    private int Row;
    private int Col;
    private int Moves;
    private int tempRowInner;
    private int tempColInner;
    private List<Vector2> boardMoves;
    private bool solutionFound;

    /*
    * @param board (type) 2D list - holds the robot map
    * @param direction (type) Int - holds an integer between 0 and 7
    * indicating all possible robot rotations
    * @param envirement (type) float - holds a float with the odds
    * of an obstacle being generated in the map 
    * .9 is 10% obstalces, .8 is 20% obstacles, .6 is 40% obstacles
    * @param maxMoves (type) int - holds max number of moves robot can make
    * before stopping
    */
    public RobotEngine(int numberRows, int numberCols, int startLocRow, int startLocCol, int endLocRow, int endLocCol,
        int direction, float envirement, int maxMoves)
    {
        this.board = MakeValues(numberRows, numberCols, startLocRow, startLocCol, endLocRow, endLocCol);
        this.direction = direction;
        this.envirement = envirement;
        this.maxMoves = maxMoves;
        this.Actual_Map = Make_Actual_Map();
        this.board_robot_moves = Make_Robot_Moves_Map();
        this.Row = startLocRow;
        this.Col = startLocCol;
        this.startingCords = new int[2] {startLocRow, startLocCol};
        this.Moves = 0;
        boardMoves = new List<Vector2>();
        solutionFound = false;
    }

    private int[,] MakeValues(int r, int c, int sr, int sc, int er, int ec)
    {
        int[,] nums = new int[r, c];

        for (int i = 0; i < nums.GetLength(0); i++)
        {
            for (int j = 0; j < nums.GetLength(1); j++)
            {
                nums[i, j] = 2;
            }
        }

        nums[sr, sc] = 1;
        nums[er, ec] = -10;
        return nums;
    }


    /* This class generates the obstacles(0) and free spaces(1) on the board */

    private int[,] Make_Actual_Map()
    {
        int[,] array_map_copy = board.Clone() as int[,]; //.DeepCopy();
        for (int i = 0; i < array_map_copy.GetLength(0); i++)
        {
            for (int j = 0; j < array_map_copy.GetLength(1); j++)
            {
                if (array_map_copy[i, j] == 2)
                {
                    if (Random.Range(0f, 1f) >= envirement)
                    {
                        array_map_copy[i, j] = 0;
                    }

                    else
                    {
                        array_map_copy[i, j] = 1;
                    }
                }
            }
        }

        return array_map_copy;
    }

    private int[,] Make_Robot_Moves_Map()
    {
        int[,] array_map_moves_copy = board.Clone() as int[,]; //deepcopy(board)
        for (int i = 0; i < array_map_moves_copy.GetLength(0); i++)
        {
            for (int j = 0; j < array_map_moves_copy.GetLength(1); j++)
            {
                if (Actual_Map[i, j] == 1 || Actual_Map[i, j] == -10)
                {
                    array_map_moves_copy[i, j] = 0;
                }
                else
                {
                    array_map_moves_copy[i, j] = -1;
                }
            }
        }

        return array_map_moves_copy;
    }


    //class used to test inside of unity
    public void print_robot_moves_map()
    {
        if (Moves == maxMoves)
        {
            Debug.Log(" - Robot did not find a solution \n");
        }
        else
        {
            Debug.Log($" - Robot number of moves = {Moves}");
            solutionFound = true;
        }
    }

    public List<Vector2> GetBoardMoves()
    {
        return boardMoves;
    }

    public int[,] GetBoard()
    {
        return Actual_Map;
    }

    public bool GetSolutionFound()
    {
        return solutionFound;
    }

    public int GetMoves()
    {
        return Moves;
    }

    public void Solution()
    {
        while (Moves < maxMoves)
        {
            this.tempRowInner = -1;
            this.tempColInner = -1;

            if (RobotSensors()) //sensor checks obstacles 
            {
                if (this.board[this.Row, this.Col] == -10)
                {
                    break;
                }
                continue;
            }
            
            /* Here the regular rules */
            //north
            if (CheckDirection(direction))
            {
                if (board[Row, Col] == -10)
                {
                    break;
                }
                continue;
            }
            // perform more if statements with similarity being north

            if (board[Row, Col] == -10)
            {
                break;
            }

            if ((this.tempRowInner == -1 || this.tempColInner == -1) )
            {
                direction = RobotTurn(direction);; //-2 to do a turn
                boardMoves.Add(new Vector2(-5, direction));
                Moves++; 
                continue;
            }

            //here to prevent infinite loop
            Moves++;
        }
    }

    private int RobotTurn(int rawDirection)
    {
        switch (rawDirection)
        {
            case 1:
                return 7;
            case 0:
                return 6;
            default:
                return rawDirection - 2;
        }
    }


    /* RULES FOR REGULAR IF THAN  */
    private bool CheckDirection(int rawDirection)
    {
        int cordsX = -1;
        int cordsY = -1;
        int straightCell = -1;
        int leftCell = -1;
        int rightCell = -1;
        bool straightCellSmaller = false;
        bool leftCellSmaller = false;
        bool rightCellSmaller = false;
        int normalizedLeftDirection = rawDirection - 1 < 0 ? 7 : rawDirection - 1;
        int normalizedRightDirection = (rawDirection + 1) % 8;
        bool resultMoved = false;

        try
        {
            straightCell = board_robot_moves[Row + RowChanger(rawDirection), Col + ColChanger(rawDirection)];
        }
        catch (Exception error)
        {
            Debug.Log("Wall Hit"); 
        }
        
        try
        {
            leftCell = board_robot_moves[Row + RowChanger(normalizedLeftDirection), Col + ColChanger(normalizedLeftDirection)];
        }
        catch (Exception error)
        {
            Debug.Log("Wall Hit"); 
        }

        try
        {
            rightCell = board_robot_moves[Row + RowChanger(normalizedRightDirection), Col + ColChanger(normalizedRightDirection)];
        }
        catch (Exception error)
        {
            Debug.Log("Wall Hit"); 
        }

        if (straightCell == -1)
        {
            straightCell = maxMoves + 1; 
        }

        if (leftCell == -1)
        {
            leftCell = maxMoves + 1;
        }

        if (rightCell == -1)
        {
            rightCell = maxMoves + 1; 
        }

        if (straightCell <= leftCell && straightCell <= rightCell && straightCell != maxMoves + 1)
        {
            straightCellSmaller = true;
        }

        if (leftCell <= rightCell && leftCell != maxMoves + 1 && !straightCellSmaller)
        {
            leftCellSmaller = true;
        }

        if (rightCell != maxMoves + 1 && !straightCellSmaller && !leftCellSmaller)
        {
            rightCellSmaller = true;
        }

        if (straightCellSmaller)
        {
            direction = rawDirection;
            cordsX = Row + RowChanger(rawDirection);
            cordsY = Col + ColChanger(rawDirection);
        }

        if (leftCellSmaller)
        {
            direction = normalizedLeftDirection;
            boardMoves.Add(new Vector2(-5, normalizedLeftDirection));
            cordsX = Row + RowChanger(normalizedLeftDirection);
            cordsY = Col + ColChanger(normalizedLeftDirection);
        }

        if (rightCellSmaller)
        {
            direction = normalizedRightDirection;
            boardMoves.Add(new Vector2(-5, normalizedRightDirection));
            cordsX = Row + RowChanger(normalizedRightDirection);
            cordsY = Col + ColChanger(normalizedRightDirection);
        }

        if (cordsX != -1 || cordsY != -1)
        {
            boardMoves.Add(new Vector2(cordsX, cordsY));
            Row = cordsX; 
            Col = cordsY; 
            Moves++; 
            board_robot_moves[Row, Col] += 1;
            resultMoved = true;
        }

        tempRowInner = cordsX;
        tempColInner = cordsY;
        return resultMoved;
    }


    /* SENSOR FUNCTION CALLS THE OTHER NEEDED SENSORS */

    private bool RobotSensors()
    {
        SensorAtDirection(this.direction);

        if (tempRowInner != -1 || tempColInner != -1)
        {
            boardMoves.Add(new Vector2(tempRowInner, tempColInner));
            Row = tempRowInner; 
            Col = tempColInner; 
            Moves++;
            board_robot_moves[Row, Col] += 1;
            return true;
        }

        return false;
    }


    /* FUNCTIONS BELOW ARE USED FOR SENSORS */

    private void SensorAtDirection(int rawDirection)
    {
        int cordsX = -1;
        int cordsY = -1;
        int normalizedLeftDirection = rawDirection - 1 < 0 ? 7 : rawDirection - 1;
        int normalizedRightDirection = (rawDirection + 1) % 8;
        
        if (SensorDetectsGoal(rawDirection))
        {
            direction = rawDirection;
            cordsX = Row + RowChanger(rawDirection); 
            cordsY = Col + ColChanger(rawDirection); 
        }

        if (SensorDetectsGoal(normalizedLeftDirection))
        {
            direction = normalizedLeftDirection;
            boardMoves.Add(new Vector2(-5, normalizedLeftDirection));
            cordsX = Row + RowChanger(normalizedLeftDirection);
            cordsY = Col + ColChanger(normalizedLeftDirection);
        }

        
        if (SensorDetectsGoal(normalizedRightDirection))
        {
            direction = normalizedRightDirection;
            boardMoves.Add(new Vector2(-5, normalizedRightDirection));
            cordsX = Row + RowChanger(normalizedRightDirection);
            cordsY = Col + ColChanger(normalizedRightDirection); 
        }

        this.tempRowInner = cordsX;
        this.tempColInner = cordsY;
    }

    private bool SensorDetectsGoal(int rawDirection)
    {
        int tempRow = Row; 
        int tempCol = Col; 
        bool isFoundGoal = false;
        int rowChanger = RowChanger(rawDirection);
        int colChanger = ColChanger(rawDirection);
        
        while (CheckOutOfBounds(tempRow, tempCol))
        {
            tempRow += rowChanger;
            tempCol += colChanger;
            if (Actual_Map[tempRow, tempCol] == 1)
            {
                board[tempRow, tempCol] = 1;
            }
            else if (Actual_Map[tempRow, tempCol] == 0)
            {
                board[tempRow, tempCol] = 0;
                break;
            }
            else if (Actual_Map[tempRow, tempCol] == -10)
            {
                isFoundGoal = true;
            }
        }

        return isFoundGoal;
    }
    

    private static int RowChanger(int directionSensor)
    {
        return directionSensor switch
        {
            Direction.NORTH => -1,
            Direction.NORTHEAST => -1,
            Direction.SOUTHEAST => 1,
            Direction.SOUTH => 1,
            Direction.SOUTHWEST => 1,
            Direction.NORTHWEST => -1,
            _ => 0
        };
    }
    
    private static int ColChanger(int directionSensor)
    {
        return directionSensor switch
        {
            Direction.NORTHEAST => 1,
            Direction.EAST => 1,
            Direction.SOUTHEAST => 1,
            Direction.SOUTHWEST => -1,
            Direction.WEST => -1,
            Direction.NORTHWEST => -1,
            _ => 0
        };
    }

    private bool CheckOutOfBounds(int tempRow, int tempCol)
    {
        return tempRow - 1 >= 0 && tempCol - 1 >= 0 && tempRow + 1 < board.GetLength(0) &&
               tempCol + 1 < board.GetLength(1);
    }
}

public static class Direction
{
    public const int NORTH = 0;
    public const int NORTHEAST = 1;
    public const int EAST = 2;
    public const int SOUTHEAST = 3;
    public const int SOUTH = 4;
    public const int SOUTHWEST = 5;
    public const int WEST = 6;
    public const int NORTHWEST = 7;
}