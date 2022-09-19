using System.Collections.Generic;
using UnityEngine;

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
    private Unity.Mathematics.Random rnd = new Unity.Mathematics.Random();
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

    public void solution()
    {
        while (Moves < maxMoves)
        {
            bool isDirectionValid = false;
            this.tempRowInner = -1;
            this.tempColInner = -1;

            if (robotSensors()) //sensor checks obstacles 
            {
                if (this.board[this.Row, this.Col] == -10)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }

            /* Here the regular rules */
            //north
            if (direction == Direction.NORTH) //forward
            {
                checkNorth(); // perform more if statements with similarity being north
                isDirectionValid = true;
            }

            if (board[Row, Col] == -10)
            {
                break;
            }

            if ((this.tempRowInner == -1 || this.tempColInner == -1) && isDirectionValid)
            {
                direction = Direction.WEST; //-2 to do a turn
                Moves++; 
                continue;
            }

            if (isDirectionValid)
            {
                continue;
            }

            //northeast
            if (direction == Direction.NORTHEAST) //topright
            {
                checkNorthEast();
                isDirectionValid = true;
            }

            if (board[Row, Col] == -10)
            {
                break;
            }

            if ((this.tempRowInner == -1 || this.tempColInner == -1) && isDirectionValid)
            {
                direction = Direction.NORTHWEST; //-2 to do a turn
                Moves++; //Moves + 1
                continue;
            }

            if (isDirectionValid)
            {
                continue;
            }

            //east
            if (direction == Direction.EAST) //right
            {
                checkEast();
                isDirectionValid = true;
            }

            if (board[Row, Col] == -10)
            {
                break;
            }

            if ((this.tempRowInner == -1 || this.tempColInner == -1) && isDirectionValid)
            {
                direction = Direction.NORTH; //-2 to do a turn
                Moves++; //Moves + 1
                continue;
            }

            if (isDirectionValid)
            {
                continue;
            }

            //southeast
            if (direction == Direction.SOUTHEAST) //bottomright
            {
                checkSouthEast();
                isDirectionValid = true;
            }

            if (board[Row, Col] == -10)
            {
                break;
            }

            if ((this.tempRowInner == -1 || this.tempColInner == -1) && isDirectionValid)
            {
                direction = Direction.NORTHEAST; //-2 to do a turn
                Moves++; //Moves + 1
                continue;
            }

            if (isDirectionValid)
            {
                continue;
            }

            //south
            if (direction == Direction.SOUTH) //bottomright
            {
                checkSouth();
                isDirectionValid = true;
            }

            if (board[Row, Col] == -10)
            {
                break;
            }

            if ((this.tempRowInner == -1 || this.tempColInner == -1) && isDirectionValid)
            {
                direction = Direction.EAST; //-2 to do a turn
                Moves++; //Moves + 1
                continue;
            }

            if (isDirectionValid)
            {
                continue;
            }

            //southwest
            if (direction == Direction.SOUTHWEST) //bottomright
            {
                checkSouthWest();
                isDirectionValid = true;
            }

            if (board[Row, Col] == -10)
            {
                break;
            }

            if ((this.tempRowInner == -1 || this.tempColInner == -1) && isDirectionValid)
            {
                direction = Direction.SOUTHEAST; //-2 to do a turn
                Moves++; //Moves + 1
                continue;
            }

            if (isDirectionValid)
            {
                continue;
            }

            //west
            if (direction == Direction.WEST) //bottomright
            {
                checkWest();
                isDirectionValid = true;
            }

            if (board[Row, Col] == -10)
            {
                break;
            }

            if ((this.tempRowInner == -1 || this.tempColInner == -1) && isDirectionValid)
            {
                direction = Direction.SOUTH; //-2 to do a turn
                Moves++; //Moves + 1
                continue;
            }

            if (isDirectionValid)
            {
                continue;
            }

            //northwest
            if (direction == Direction.NORTHWEST) //bottomright
            {
                checkNorthWest();
                isDirectionValid = true;
            }

            if (board[Row, Col] == -10)
            {
                break;
            }

            if ((this.tempRowInner == -1 || this.tempColInner == -1) && isDirectionValid)
            {
                direction = Direction.SOUTHWEST; //-2 to do a turn
                Moves++; //Moves + 1
                continue;
            }

            if (isDirectionValid)
            {
                continue;
            }


            Debug.Log("here to prevent infinite loop");
            Moves++;
        }
    }


    /* RULES FOR REGULAR IF THAN  */
    private void checkNorth()
    {
        int cordsx = -1;
        int cordsy = -1;
        int straightCell = -1;
        bool straightCellSmaller = false;
        int leftCell = -1;
        bool leftCellSmaller = false;
        int rightCell = -1;
        bool rightCellSmaller = false;

        if (Row - 1 >= 0)
        {
            straightCell = board_robot_moves[Row - 1, Col];
            straightCellSmaller = false;
        }

        if (Row - 1 >= 0 && Col - 1 >= 0)
        {
            leftCell = board_robot_moves[Row - 1, Col - 1];
            leftCellSmaller = false;
        }

        if (Row - 1 >= 0 && Col + 1 < board.GetLength(1))
        {
            rightCell = board_robot_moves[Row - 1, Col + 1];
            rightCellSmaller = false;
        }


        if (straightCell == -1)
        {
            straightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (leftCell == -1)
        {
            leftCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (rightCell == -1)
        {
            rightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
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
            direction = Direction.NORTH;
            cordsx = (Row - 1);
            cordsy = (Col); //(deepcopy(Row) - 1) , (deepcopy(Col))
        }

        if (leftCellSmaller)
        {
            direction = Direction.NORTHWEST;
            cordsx = Row - 1;
            cordsy = Col - 1; //(deepcopy(Row) - 1) , (deepcopy(Col) - 1)
        }

        if (rightCellSmaller)
        {
            direction = Direction.NORTHEAST;
            cordsx = Row - 1;
            cordsy = Col + 1; //(deepcopy(Row) - 1) , (deepcopy(Col) + 1)
        }

        if (cordsx != -1 || cordsy != -1)
        {
            boardMoves.Add(new Vector2(cordsx, cordsy));
            Row = cordsx; //deepcopy(cordsx)
            Col = cordsy; //deepcopy(cordsy)
            Moves++; //#Moves + 1
            board_robot_moves[Row, Col] += 1;
        }

        tempRowInner = cordsx;
        tempColInner = cordsy;
    }


    private void checkNorthEast()
    {
        int cordsx = -1;
        int cordsy = -1;
        int straightCell = -1;
        bool straightCellSmaller = false;
        int leftCell = -1;
        bool leftCellSmaller = false;
        int rightCell = -1;
        bool rightCellSmaller = false;
        if (Row - 1 >= 0 && Col + 1 < board.GetLength(1))
        {
            straightCell = board_robot_moves[Row - 1, Col + 1];
            straightCellSmaller = false;
        }

        if (Row - 1 >= 0)
        {
            leftCell = board_robot_moves[Row - 1, Col];
            leftCellSmaller = false;
        }

        if (Col + 1 < board.GetLength(1))
        {
            rightCell = board_robot_moves[Row, Col + 1];
            rightCellSmaller = false;
        }

        if (straightCell == -1)
        {
            straightCell = maxMoves + 1;
        }

        if (leftCell == -1)
        {
            leftCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (rightCell == -1)
        {
            rightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
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
            direction = Direction.NORTHEAST;
            cordsx = Row - 1;
            cordsy = Col + 1; //(deepcopy(Row) - 1) , (deepcopy(Col) + 1)
        }

        if (leftCellSmaller)
        {
            direction = Direction.NORTH;
            cordsx = Row - 1;
            cordsy = Col; //(deepcopy(Row) - 1) , (deepcopy(Col))
        }

        if (rightCellSmaller)
        {
            direction = Direction.EAST;
            cordsx = Row;
            cordsy = Col + 1; //(deepcopy(Row)) , (deepcopy(Col) + 1)
        }

        if (cordsx != -1 || cordsy != -1)
        {
            boardMoves.Add(new Vector2(cordsx, cordsy));
            Row = cordsx; //deepcopy(cordsx)
            Col = cordsy; //deepcopy(cordsy)
            Moves++; //Moves + 1
            board_robot_moves[Row, Col] += 1;
        }

        tempRowInner = cordsx;
        tempColInner = cordsy;
    }

    private void checkEast()
    {
        int cordsx = -1;
        int cordsy = -1;
        int straightCell = -1;
        bool straightCellSmaller = false;
        int leftCell = -1;
        bool leftCellSmaller = false;
        int rightCell = -1;
        bool rightCellSmaller = false;

        if (Col + 1 < board.GetLength(1))
        {
            straightCell = board_robot_moves[Row, Col + 1];
            straightCellSmaller = false;
        }

        if (Row - 1 >= 0 && Col + 1 < board.GetLength(1))
        {
            leftCell = board_robot_moves[Row - 1, Col + 1];
            leftCellSmaller = false;
        }

        if (Row + 1 < board.GetLength(0) && Col + 1 < board.GetLength(1))
        {
            rightCell = board_robot_moves[Row + 1, Col + 1];
            rightCellSmaller = false;
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
            direction = Direction.EAST;
            cordsx = Row;
            cordsy = Col + 1; //(deepcopy(Row)) , (deepcopy(Col) + 1)
        }

        if (leftCellSmaller)
        {
            direction = Direction.NORTHEAST;
            cordsx = Row - 1;
            cordsy = Col + 1; //(deepcopy(Row) - 1) , (deepcopy(Col) + 1)
        }

        if (rightCellSmaller)
        {
            direction = Direction.SOUTHEAST;
            cordsx = Row + 1;
            cordsy = Col + 1; //(deepcopy(Row) + 1) , (deepcopy(Col) + 1)
        }

        if (cordsx != -1 || cordsy != -1)
        {
            boardMoves.Add(new Vector2(cordsx, cordsy));
            Row = cordsx; //deepcopy(cordsx)
            Col = cordsy; //deepcopy(cordsy)
            Moves++; //Moves + 1
            board_robot_moves[Row, Col] += 1;
        }

        tempRowInner = cordsx;
        tempColInner = cordsy;
    }

    private void checkSouthEast()
    {
        int cordsx = -1;
        int cordsy = -1;
        int straightCell = -1;
        bool straightCellSmaller = false;
        int leftCell = -1;
        bool leftCellSmaller = false;
        int rightCell = -1;
        bool rightCellSmaller = false;

        if (Row + 1 < board.GetLength(0) && Col + 1 < board.GetLength(1))
        {
            straightCell = board_robot_moves[Row + 1, Col + 1];
            straightCellSmaller = false;
        }

        if (Col + 1 < board.GetLength(1))
        {
            leftCell = board_robot_moves[Row, Col + 1];
            leftCellSmaller = false;
        }

        if (Row + 1 < board.GetLength(0))
        {
            rightCell = board_robot_moves[Row + 1, Col];
            rightCellSmaller = false;
        }

        if (straightCell == -1)
        {
            straightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (leftCell == -1)
        {
            leftCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (rightCell == -1)
        {
            rightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
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
            direction = Direction.SOUTHEAST;
            cordsx = Row + 1;
            cordsy = Col + 1; //(deepcopy(Row) + 1) , (deepcopy(Col) + 1)
        }

        if (leftCellSmaller)
        {
            direction = Direction.EAST;
            cordsx = Row;
            cordsy = Col + 1; //(deepcopy(Row)) , (deepcopy(Col) + 1)
        }

        if (rightCellSmaller)
        {
            direction = Direction.SOUTH;
            cordsx = Row + 1;
            cordsy = Col; //(deepcopy(Row) + 1) , (deepcopy(Col))
        }

        if (cordsx != -1 || cordsy != -1)
        {
            boardMoves.Add(new Vector2(cordsx, cordsy));
            Row = cordsx;
            Col = cordsy; //deepcopy(cordsy)
            Moves++; //Moves + 1
            board_robot_moves[Row, Col] += 1;
        }

        tempRowInner = cordsx;
        tempColInner = cordsy;
    }


    private void checkSouth()
    {
        int cordsx = -1;
        int cordsy = -1;
        int straightCell = -1;
        bool straightCellSmaller = false;
        int leftCell = -1;
        bool leftCellSmaller = false;
        int rightCell = -1;
        bool rightCellSmaller = false;

        if (Row + 1 < board.GetLength(0))
        {
            straightCell = board_robot_moves[Row + 1, Col];
            straightCellSmaller = false;
        }

        if (Row + 1 < board.GetLength(0) && Col + 1 < board.GetLength(1))
        {
            leftCell = board_robot_moves[Row + 1, Col + 1];
            leftCellSmaller = false;
        }

        if (Row + 1 < board.GetLength(0) && Col - 1 >= 0)
        {
            rightCell = board_robot_moves[Row + 1, Col - 1];
            rightCellSmaller = false;
        }

        if (straightCell == -1)
        {
            straightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (leftCell == -1)
        {
            leftCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (rightCell == -1)
        {
            rightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
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
            direction = Direction.SOUTH;
            cordsx = Row + 1;
            cordsy = Col; // (deepcopy(Row) + 1) , (deepcopy(Col))
        }

        if (leftCellSmaller)
        {
            direction = Direction.SOUTHEAST;
            cordsx = Row + 1;
            cordsy = Col + 1; //(deepcopy(Row) + 1) , (deepcopy(Col) + 1)
        }

        if (rightCellSmaller)
        {
            direction = Direction.SOUTHWEST;
            cordsx = Row + 1;
            cordsy = Col - 1; //(deepcopy(Row) + 1) , (deepcopy(Col) - 1)
        }

        if (cordsx != -1 || cordsy != -1)
        {
            boardMoves.Add(new Vector2(cordsx, cordsy));
            Row = cordsx; //deepcopy(cordsx)
            Col = cordsy;
            Moves++; //Moves + 1
            board_robot_moves[Row, Col] += 1;
        }

        tempRowInner = cordsx;
        tempColInner = cordsy;
    }


    private void checkSouthWest()
    {
        int cordsx = -1;
        int cordsy = -1;
        int straightCell = -1;
        bool straightCellSmaller = false;
        int leftCell = -1;
        bool leftCellSmaller = false;
        int rightCell = -1;
        bool rightCellSmaller = false;

        if (Row + 1 < board.GetLength(0) && Col - 1 >= 0)
        {
            straightCell = board_robot_moves[Row + 1, Col - 1];
            straightCellSmaller = false;
        }

        if (Row + 1 < board.GetLength(0))
        {
            leftCell = board_robot_moves[Row + 1, Col];
            leftCellSmaller = false;
        }

        if (Col - 1 >= 0)
        {
            rightCell = board_robot_moves[Row, Col - 1];
            rightCellSmaller = false;
        }

        if (straightCell == -1)
        {
            straightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (leftCell == -1)
        {
            leftCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (rightCell == -1)
        {
            rightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
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
            direction = Direction.SOUTHWEST;
            cordsx = Row + 1;
            cordsy = Col - 1; //(deepcopy(Row) + 1) , (deepcopy(Col) - 1)
        }

        if (leftCellSmaller)
        {
            direction = Direction.SOUTH;
            cordsx = Row + 1;
            cordsy = Col; //(deepcopy(Row) + 1) , (deepcopy(Col))
        }

        if (rightCellSmaller)
        {
            direction = Direction.WEST;
            cordsx = Row;
            cordsy = Col - 1; //(deepcopy(Row)) , (deepcopy(Col) - 1)
        }

        if (cordsx != -1 || cordsy != -1)
        {
            boardMoves.Add(new Vector2(cordsx, cordsy));
            Row = cordsx; //deepcopy(cordsx)
            Col = cordsy; //deepcopy(cordsy)
            Moves++; //Moves + 1
            board_robot_moves[Row, Col] += 1;
        }

        tempRowInner = cordsx;
        tempColInner = cordsy;
    }


    private void checkWest()
    {
        int cordsx = -1;
        int cordsy = -1;
        int straightCell = -1;
        bool straightCellSmaller = false;
        int leftCell = -1;
        bool leftCellSmaller = false;
        int rightCell = -1;
        bool rightCellSmaller = false;

        if (Col - 1 >= 0)
        {
            straightCell = board_robot_moves[Row, Col - 1];
            straightCellSmaller = false;
        }

        if (Row + 1 < board.GetLength(0) && Col - 1 >= 0)
        {
            leftCell = board_robot_moves[Row + 1, Col - 1];
            leftCellSmaller = false;
        }

        if (Row - 1 >= 0 && Col - 1 >= 0)
        {
            rightCell = board_robot_moves[Row - 1, Col - 1];
            rightCellSmaller = false;
        }

        if (straightCell == -1)
        {
            straightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (leftCell == -1)
        {
            leftCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (rightCell == -1)
        {
            rightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
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
            direction = Direction.WEST;
            cordsx = Row;
            cordsy = Col - 1; //(deepcopy(Row)) , (deepcopy(Col) - 1)
        }

        if (leftCellSmaller)
        {
            direction = Direction.SOUTHWEST;
            cordsx = Row + 1;
            cordsy = Col - 1; // (deepcopy(Row) + 1) , (deepcopy(Col) - 1)
        }

        if (rightCellSmaller)
        {
            direction = Direction.NORTHWEST;
            cordsx = Row - 1;
            cordsy = Col - 1; //(deepcopy(Row) - 1) , (deepcopy(Col) - 1)
        }

        if (cordsx != -1 || cordsy != -1)
        {
            boardMoves.Add(new Vector2(cordsx, cordsy));
            Row = cordsx; //(cordsx)
            Col = cordsy; //deepcopy(cordsy)
            Moves++; //Moves + 1
            board_robot_moves[Row, Col] += 1;
        }

        tempRowInner = cordsx;
        tempColInner = cordsy;
    }

    private void checkNorthWest()
    {
        int cordsx = -1;
        int cordsy = -1;
        int straightCell = -1;
        bool straightCellSmaller = false;
        int leftCell = -1;
        bool leftCellSmaller = false;
        int rightCell = -1;
        bool rightCellSmaller = false;

        if (Row - 1 >= 0 && Col - 1 >= 0)
        {
            straightCell = board_robot_moves[Row - 1, Col - 1];
            straightCellSmaller = false;
        }

        if (Col - 1 >= 0)
        {
            leftCell = board_robot_moves[Row, Col - 1];
            leftCellSmaller = false;
        }

        if (Row - 1 >= 0)
        {
            rightCell = board_robot_moves[Row - 1, Col];
            rightCellSmaller = false;
        }

        if (straightCell == -1)
        {
            straightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (leftCell == -1)
        {
            leftCell = maxMoves + 1; //deepcopy(maxMoves) + 1
        }

        if (rightCell == -1)
        {
            rightCell = maxMoves + 1; //deepcopy(maxMoves) + 1
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
            direction = Direction.NORTHWEST;
            cordsx = Row - 1;
            cordsy = Col - 1; //(deepcopy(Row) - 1) , (deepcopy(Col) - 1)
        }

        if (leftCellSmaller)
        {
            direction = Direction.WEST;
            cordsx = Row;
            cordsy = Col - 1; //(deepcopy(Row)) , (deepcopy(Col) - 1)
        }

        if (rightCellSmaller)
        {
            direction = Direction.NORTH;
            cordsx = Row - 1;
            cordsy = Col; //(deepcopy(Row) - 1) , (deepcopy(Col))
        }

        if (cordsx != -1 || cordsy != -1)
        {
            boardMoves.Add(new Vector2(cordsx, cordsy));
            Row = cordsx; //deepcopy(cordsx)
            Col = cordsy; //deepcopy(cordsy)
            Moves++; //Moves + 1
            board_robot_moves[Row, Col] += 1;
        }

        tempRowInner = cordsx;
        tempColInner = cordsy;
    }


    /* SENSOR FUNCTION CALLS THE OTHER NEEDED SENSORS */

    private bool robotSensors()
    {
        if (direction == Direction.NORTH)
        {
            directionNorth();
        }
        else if (direction == Direction.NORTHEAST)
        {
            directionNorthEast();
        }
        else if (direction == Direction.EAST)
        {
            directionEast();
        }
        else if (direction == Direction.SOUTHEAST)
        {
            directionSouthEast();
        }
        else if (direction == Direction.SOUTH)
        {
            directionSouth();
        }
        else if (direction == Direction.SOUTHWEST)
        {
            directionSouthWest();
        }
        else if (direction == Direction.WEST)
        {
            directionWest();
        }
        else if (direction == Direction.NORTHWEST)
        {
            directionNorthWest();
        }

        if (tempRowInner != -1 || tempColInner != -1)
        {
            boardMoves.Add(new Vector2(tempRowInner, tempColInner));
            Row = tempRowInner; //deepcopy(tempRow)
            Col = tempColInner; //deepcopy(tempCol)
            Moves++;
            board_robot_moves[Row, Col] += 1;
            return true;
        }

        return false;
    }


    /* FUNCTIONS BELOW ARE USED FOR SENSORS */

    private void directionNorth()
    {
        int cordsx = -1;
        int cordsy = -1;
        if (North())
        {
            direction = Direction.NORTH;
            cordsx = (Row - 1);
            cordsy = Col; // (deepcopy(Row) - 1) , (deepcopy(Col))
        }

        if (NorthWest())
        {
            direction = Direction.NORTHWEST;
            cordsx = (Row - 1);
            cordsy = (Col - 1); // (deepcopy(Row) - 1) , (deepcopy(Col) - 1)
        }

        if (NorthEast())
        {
            direction = Direction.NORTHEAST;
            cordsx = (Row - 1);
            cordsy = (Col + 1); // (deepcopy(Row) - 1) , (deepcopy(Col) +1)
        }

        this.tempRowInner = cordsx;
        this.tempColInner = cordsy;
    }

    private void directionNorthEast()
    {
        int cordsx = -1;
        int cordsy = -1;
        if (NorthEast())
        {
            direction = Direction.NORTHEAST;
            cordsx = (Row - 1);
            cordsy = (Col + 1); //(deepcopy(Row) - 1) , (deepcopy(Col) +1)
        }

        if (North())
        {
            direction = Direction.NORTH;
            cordsx = (Row - 1);
            cordsy = Col; //(deepcopy(Row) - 1) , (deepcopy(Col))
        }

        if (East())
        {
            direction = Direction.EAST;
            cordsx = (Row);
            cordsy = (Col + 1); //(deepcopy(Row) ) , (deepcopy(Col) +1)
        }

        this.tempRowInner = cordsx;
        this.tempColInner = cordsy;
    }

    private void directionEast()
    {
        int cordsx = -1;
        int cordsy = -1;
        if (East())
        {
            direction = Direction.EAST;
            cordsx = (Row);
            cordsy = (Col + 1); // (deepcopy(Row) ) , (deepcopy(Col) +1) 
        }

        if (NorthEast())
        {
            direction = Direction.NORTHEAST;
            cordsx = (Row - 1);
            cordsy = (Col + 1); //(deepcopy(Row) - 1) , (deepcopy(Col) +1)
        }

        if (SouthEast())
        {
            direction = Direction.SOUTHEAST;
            cordsx = (Row + 1);
            cordsy = (Col + 1); //(deepcopy(Row) +1) , (deepcopy(Col) +1) 
        }

        this.tempRowInner = cordsx;
        this.tempColInner = cordsy;
    }

    private void directionSouthEast()
    {
        int cordsx = -1;
        int cordsy = -1;
        if (SouthEast())
        {
            direction = Direction.SOUTHEAST;
            cordsx = (Row + 1);
            cordsy = (Col + 1); //(deepcopy(Row) +1) , (deepcopy(Col) +1)  
        }

        if (East())
        {
            direction = Direction.EAST;
            cordsx = (Row);
            cordsy = (Col + 1); //(deepcopy(Row) ) , (deepcopy(Col) +1)
        }

        if (South())
        {
            direction = Direction.SOUTH;
            cordsx = (Row + 1);
            cordsy = (Col); //(deepcopy(Row) +1), (deepcopy(Col))
        }

        this.tempRowInner = cordsx;
        this.tempColInner = cordsy;
    }

    private void directionSouth()
    {
        int cordsx = -1;
        int cordsy = -1;
        if (South())
        {
            direction = Direction.SOUTH;
            cordsx = (Row + 1);
            cordsy = Col; //(deepcopy(Row) +1), (deepcopy(Col))
        }

        if (SouthEast())
        {
            direction = Direction.SOUTHEAST;
            cordsx = (Row + 1);
            cordsy = (Col + 1); //(deepcopy(Row) +1) , (deepcopy(Col) +1)
        }

        if (SouthWest())
        {
            direction = Direction.SOUTHWEST;
            cordsx = (Row + 1);
            cordsy = (Col - 1); //(deepcopy(Row) +1) , (deepcopy(Col) -1)
        }

        this.tempRowInner = cordsx;
        this.tempColInner = cordsy;
    }

    private void directionSouthWest()
    {
        int cordsx = -1;
        int cordsy = -1;
        if (SouthWest())
        {
            direction = Direction.SOUTHWEST;
            cordsx = (Row + 1);
            cordsy = (Col - 1); //(deepcopy(Row) +1) , (deepcopy(Col) -1)
        }

        if (South())
        {
            direction = Direction.SOUTH;
            cordsx = (Row + 1);
            cordsy = (Col); //(deepcopy(Row) +1), (deepcopy(Col))
        }

        if (West())
        {
            direction = Direction.WEST;
            cordsx = Row;
            cordsy = (Col - 1); // (deepcopy(Row) ) , (deepcopy(Col) -1) 
        }

        this.tempRowInner = cordsx;
        this.tempColInner = cordsy;
    }

    private void directionWest()
    {
        int cordsx = -1;
        int cordsy = -1;
        if (West())
        {
            direction = Direction.WEST;
            cordsx = Row;
            cordsy = (Col - 1); //(deepcopy(Row) ) , (deepcopy(Col) -1)
        }

        if (SouthWest())
        {
            direction = Direction.SOUTHWEST;
            cordsx = (Row + 1);
            cordsy = (Col - 1); //deepcopy(Row) +1) , (deepcopy(Col) -1)
        }

        if (NorthWest())
        {
            direction = Direction.NORTHWEST;
            cordsx = (Row - 1);
            cordsy = (Col - 1); //(deepcopy(Row) -1), (deepcopy(Col) -1)
        }

        this.tempRowInner = cordsx;
        this.tempColInner = cordsy;
    }

    private void directionNorthWest()
    {
        int cordsx = -1;
        int cordsy = -1;
        if (NorthWest())
        {
            direction = Direction.NORTHWEST;
            cordsx = (Row - 1); //deepcopy
            cordsy = (Col - 1); //deepcopy
        }

        if (West())
        {
            direction = Direction.WEST;
            cordsx = Row; //(deepcopy(Row))
            cordsy = (Col - 1); //deepcopy
        }

        if (North())
        {
            direction = Direction.NORTH;
            cordsx = (Row - 1);
            cordsy = (Col); //(deepcopy(Row) - 1), (deepcopy(Col)))
        }

        this.tempRowInner = cordsx;
        this.tempColInner = cordsy;
    }


    private bool North()
    {
        int tempRow = Row; //deepcopy(Row)
        int tempCol = Col; //deepcopy(Col)
        bool isFoundGoal = false;
        while (tempRow - 1 >= 0)
        {
            tempRow = tempRow - 1;
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

    private bool NorthEast()
    {
        int tempRow = Row; //deepcopy(Row)
        int tempCol = Col; //deepcopy(Col)
        bool isFoundGoal = false;
        while (tempRow - 1 >= 0 && tempCol + 1 < board.GetLength(1))
        {
            tempRow = tempRow - 1;
            tempCol = tempCol + 1;
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

    private bool East()
    {
        int tempRow = Row; //deepcopy(Row)
        int tempCol = Col; //deepcopy(Col)
        bool isFoundGoal = false;
        while (tempCol + 1 < board.GetLength(1))
        {
            tempCol = tempCol + 1;
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

    private bool SouthEast()
    {
        int tempRow = Row; //deepcopy(Row)
        int tempCol = Col; //deepcopy(Col)
        bool isFoundGoal = false;
        while (tempRow + 1 < board.GetLength(0) && tempCol + 1 < board.GetLength(1))
        {
            tempRow = tempRow + 1;
            tempCol = tempCol + 1;
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

    private bool South()
    {
        int tempRow = Row; //deepcopy(Row)
        int tempCol = Col; //deepcopy(Col)
        bool isFoundGoal = false;
        while (tempRow + 1 < board.GetLength(0))
        {
            tempRow = tempRow + 1;
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


    private bool SouthWest()
    {
        int tempRow = Row; //deepcopy(Row)
        int tempCol = Col; //deepcopy(Col)
        bool isFoundGoal = false;
        while (tempRow + 1 < board.GetLength(0) && tempCol - 1 >= 0)
        {
            tempRow = tempRow + 1;
            tempCol = tempCol - 1;
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

    private bool West()
    {
        int tempRow = Row; //deepcopy(Row)
        int tempCol = Col; //deepcopy(Col)
        bool isFoundGoal = false;
        while (tempCol - 1 >= 0)
        {
            tempCol = tempCol - 1;
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

    private bool NorthWest()
    {
        int tempRow = Row; //deepcopy(Row)
        int tempCol = Col; //deepcopy(Col)
        bool isFoundGoal = false;
        while (tempRow - 1 >= 0 && tempCol - 1 >= 0)
        {
            tempRow = tempRow - 1;
            tempCol = tempCol - 1;
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