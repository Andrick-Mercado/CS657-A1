#!/usr/bin/env python
# coding: utf-8

# ## CS 657 Intelligent Systems - Assignment 1 rule-base expert system setting
# By Andrick Mercado
# 
# You must develop your program in the rule-base expert system setting consisting of a rule base,and a data-base (map of the environment discovered so far), After each move you must go through the if-then rules sequentially and use the data-base to determine which rule is fired (see forward and backwardchaining)

# In[1]:


from copy import deepcopy
import random
from enum import IntEnum

class Direction(IntEnum):
    NORTH = 0
    NORTHEAST = 1
    EAST = 2
    SOUTHEAST = 3
    SOUTH = 4
    SOUTHWEST = 5
    WEST = 6
    NORTHWEST = 7
    
class StudentSolution:
    """
     * @param board (type) 2D list - holds the robot map
     * @param direction (type) Int - holds an integer between 0 and 7
     * indicating all possible robot rotations
     * @param envirement (type) float - holds a float with the odds
     * of an obstacle being generated in the map 
     * .9 is 10% obstalces, .8 is 20% obstacles, .6 is 40% obstacles
     * @param maxMoves (type) int - holds max number of moves robot can make
     * before stopping
    """
    def __init__(self, board=False ,startLocRow = 0, startLocCol = 0, direction = 0, envirement = .9, maxMoves = 200, fileName = None):
        self.board = board
        self.direction = direction
        self.envirement = envirement
        self.maxMoves = maxMoves
        self.Actual_Map, self.board_robot_moves = self.Make_Actual_Map() 
        self.Row = startLocRow
        self.Col = startLocCol
        self.startingCords = startLocRow, startLocCol
        self.Moves = 0
        self.fileName = fileName + '\\output.txt'
    
    ''' This class generates the obstacles(0) and free spaces(1) on the board'''
    def Make_Actual_Map(self):
        array_map_copy = deepcopy(self.board)
        array_map_moves_copy = deepcopy(self.board)
        for i in range(len(array_map_copy)):
            for j in range(len(array_map_copy[i])):
                if array_map_copy[i][j] == 2:
                    if random.uniform(0, 1)>= self.envirement:
                        array_map_moves_copy[i][j] = -1
                        array_map_copy[i][j] = 0
                    else:
                        array_map_moves_copy[i][j] = 0
                        array_map_copy[i][j] = 1
                else:
                    array_map_moves_copy[i][j] = 0
                
        return array_map_copy, array_map_moves_copy

    def print_robot_moves_map(self):
        self.board_robot_moves[ self.startingCords[0]][ self.startingCords[1]] = 'S'
        self.board[ self.startingCords[0]][ self.startingCords[1]] = 'S'
        self.Actual_Map[ self.startingCords[0]][ self.startingCords[1]] = 'S'
        tmpDashes = '\n'+ str("-----") * len(self.board[0] )+ "\n"
        tmpDashes2 =  str("-----") * len(self.board[0]) + "\n"
        
        if  self.Moves == self.maxMoves:
            tmp_str = " - Robot did not find a solution \n"
        else:
            tmp_str = " - Robot number of moves = "+ str( self.Moves ) +"\n"
            
        tmp_envirement =  "Robot Map Moves View - Obstable %"+ str(round((1 - self.envirement)*100) )
        tmp_board =  ( [list( map(str,i) ) for i in self.board_robot_moves] )
        report_storage_lines = [' | '.join(line) for line in tmp_board]
        report_storage_text = tmpDashes.join(report_storage_lines)
        
        tmp_envirement2 =  "Robot Map View - Obstable %"+ str(round((1 - self.envirement)*100) )
        tmp_board2 =  ( [list( map(str,i) ) for i in self.board] )
        report_storage_lines2 = [' | '.join(line) for line in tmp_board2]
        report_storage_text2 = tmpDashes.join(report_storage_lines2)
        
        tmp_envirement3 =  "Actual Map View - Obstable %"+ str(round((1 - self.envirement)*100) )
        tmp_board3 =  ( [list( map(str,i) ) for i in self.Actual_Map] )
        report_storage_lines3 = [' | '.join(line) for line in tmp_board3]
        report_storage_text3 = tmpDashes.join(report_storage_lines3)
        
        with open(self.fileName,"w") as f:
            f.write(tmpDashes2)
            f.write(tmp_envirement)
            f.write(tmp_str)
            f.write(tmpDashes2)
            f.write(report_storage_text)
            f.write(tmpDashes)
            
            f.write(tmpDashes2)
            f.write(tmp_envirement2)
            f.write(tmp_str)
            f.write(tmpDashes2)
            f.write(report_storage_text2)
            f.write(tmpDashes2)
            
            f.write(tmpDashes2)
            f.write(tmp_envirement3)
            f.write(tmpDashes)
            f.write(report_storage_text3)
            f.write(tmpDashes)
            f.close()

        
        
    def solution(self):
        
        while self.Moves < self.maxMoves :
            isDirectionValid = False
            tempRow = None
            tempCol = None
            
            if( self.robotSensors() ):#sensor checks obstacles 
                if self.board[self.Row][self.Col] == 'D':
                    break
                else: 
                    continue
            
            ''' Here the regular rules '''
            
            #north
            if self.direction == Direction.NORTH :#forward
                tempRow, tempCol = self.checkNorth() # perform more if statements with similarity being north
                isDirectionValid = True
            if  self.board[self.Row][self.Col] == 'D': 
                break
            if (tempRow == None or tempCol == None) and isDirectionValid:
                self.direction = Direction.WEST #-2 to do a turn
                self.Moves += 1#self.Moves + 1
                continue
            if isDirectionValid:
                continue
             
            #northeast
            if self.direction == Direction.NORTHEAST : #topright
                tempRow, tempCol = self.checkNorthEast()
                isDirectionValid = True
            if  self.board[self.Row][self.Col] == 'D':
                break
            if (tempRow == None or tempCol == None) and isDirectionValid :
                self.direction = Direction.NORTHWEST #-2 to do a turn
                self.Moves += 1#self.Moves + 1
                continue
            if isDirectionValid:
                continue
            
            #east
            if self.direction == Direction.EAST : #right
                tempRow, tempCol = self.checkEast()
                isDirectionValid = True
            if  self.board[self.Row][self.Col] == 'D':
                break
            if (tempRow == None or tempCol == None) and isDirectionValid :
                self.direction = Direction.NORTH #-2 to do a turn
                self.Moves += 1#self.Moves + 1
                continue
            if isDirectionValid:
                continue
            
            #southeast
            if self.direction == Direction.SOUTHEAST : #bottomright
                tempRow, tempCol = self.checkSouthEast()
                isDirectionValid = True
            if self.board[self.Row][self.Col] == 'D':
                break
            if (tempRow == None or tempCol == None) and isDirectionValid :
                self.direction = Direction.NORTHEAST #-2 to do a turn
                self.Moves += 1#self.Moves + 1
                continue
            if isDirectionValid:
                continue
            
            #south
            if self.direction == Direction.SOUTH : #bottomright
                tempRow, tempCol = self.checkSouth()
                isDirectionValid = True
            if  self.board[self.Row][self.Col] == 'D':
                break
            if (tempRow == None or tempCol == None) and isDirectionValid :
                self.direction = Direction.EAST #-2 to do a turn
                self.Moves += 1#self.Moves + 1
                continue
            if isDirectionValid:
                continue
            
            #southwest
            if self.direction == Direction.SOUTHWEST : #bottomright
                tempRow, tempCol = self.checkSouthWest()
                isDirectionValid = True
            if self.board[self.Row][self.Col] == 'D':
                break
            if (tempRow == None or tempCol == None) and isDirectionValid :
                self.direction = Direction.SOUTHEAST #-2 to do a turn
                self.Moves += 1#self.Moves + 1
                continue
            if isDirectionValid:
                continue
            
            #west
            if self.direction == Direction.WEST : #bottomright
                tempRow, tempCol = self.checkWest()
                isDirectionValid = True
            if self.board[self.Row][self.Col] == 'D':
                break
            if (tempRow == None or tempCol == None) and isDirectionValid :
                self.direction = Direction.SOUTH #-2 to do a turn
                self.Moves += 1#self.Moves + 1
                continue
            if isDirectionValid:
                continue
            
            #northwest
            if self.direction == Direction.NORTHWEST : #bottomright
                tempRow, tempCol = self.checkNorthWest()
                isDirectionValid = True
            if self.board[self.Row][self.Col] == 'D':
                break
            if (tempRow == None or tempCol == None) and isDirectionValid :
                self.direction = Direction.SOUTHWEST #-2 to do a turn
                self.Moves += 1#self.Moves + 1
                continue
            if isDirectionValid:
                continue
                
            #print("here to prevent infinite loop")   
            self.Moves += 1#self.Moves + 1
        return self.board
    
    ''' RULES FOR REGULAR IF THAN  '''
    def checkNorth(self):
        cordsx, cordsy = None, None
        straightCell, straightCellSmaller = -1, False
        leftCell, leftCellSmaller = -1, False
        rightCell, rightCellSmaller = -1, False

        if self.Row -1 >= 0 :
            straightCell, straightCellSmaller = self.board_robot_moves[self.Row -1][self.Col], False
            
        if self.Row -1 >= 0 and self.Col -1 >= 0 :
            leftCell, leftCellSmaller = self.board_robot_moves[self.Row -1][self.Col-1], False
            
        if self.Row -1 >= 0 and self.Col +1 < len( self.board[0] ) :
            rightCell, rightCellSmaller = self.board_robot_moves[self.Row -1][self.Col+1], False
            

        if straightCell == -1 :
            straightCell = deepcopy(self.maxMoves) +1
        if leftCell == -1 :
            leftCell = deepcopy(self.maxMoves) +1
        if rightCell == -1 :
            rightCell = deepcopy(self.maxMoves) +1

        if straightCell <= leftCell and straightCell <= rightCell and straightCell != self.maxMoves +1:
            straightCellSmaller = True
        if leftCell <= rightCell and leftCell != self.maxMoves +1 and not straightCellSmaller:
            leftCellSmaller = True
        if rightCell != self.maxMoves +1 and not straightCellSmaller and not leftCellSmaller:
            rightCellSmaller = True

        if straightCellSmaller:
            self.direction = Direction.NORTH
            cordsx, cordsy = (deepcopy(self.Row) - 1) , (deepcopy(self.Col))
        if leftCellSmaller: 
            self.direction = Direction.NORTHWEST
            cordsx, cordsy = (deepcopy(self.Row) - 1) , (deepcopy(self.Col)-1)
        if rightCellSmaller:
            self.direction = Direction.NORTHEAST
            cordsx, cordsy = (deepcopy(self.Row) - 1) , (deepcopy(self.Col)+1)
        
        if cordsx != None or cordsy != None:
            self.Row = deepcopy(cordsx)
            self.Col = deepcopy(cordsy)
            self.Moves += 1#self.Moves + 1
            self.board_robot_moves[self.Row][self.Col] += 1
        
        return cordsx, cordsy
    
    def checkNorthEast(self):
        cordsx, cordsy = None, None
        straightCell, straightCellSmaller = -1, False
        leftCell, leftCellSmaller = -1, False
        rightCell, rightCellSmaller = -1, False
        if self.Row -1 >= 0 and self.Col +1 < len( self.board[0] ) :
            straightCell, straightCellSmaller = self.board_robot_moves[self.Row -1][self.Col+1], False
        if self.Row -1 >= 0 :
            leftCell, leftCellSmaller = self.board_robot_moves[self.Row -1][self.Col], False
        if self.Col +1 < len( self.board[0] ) :
            rightCell, rightCellSmaller = self.board_robot_moves[self.Row ][self.Col+1], False
        
        if straightCell == -1 :
            straightCell = deepcopy(self.maxMoves) +1
        if leftCell == -1 :
            leftCell = deepcopy(self.maxMoves) +1
        if rightCell == -1 :
            rightCell = deepcopy(self.maxMoves) +1

        if straightCell <= leftCell and straightCell <= rightCell and straightCell != self.maxMoves +1:
            straightCellSmaller = True
        if leftCell <= rightCell and leftCell != self.maxMoves +1 and not straightCellSmaller:
            leftCellSmaller = True
        if rightCell != self.maxMoves +1 and not straightCellSmaller and not leftCellSmaller:
            rightCellSmaller = True
        
        if straightCellSmaller:
            self.direction = Direction.NORTHEAST
            cordsx, cordsy = (deepcopy(self.Row) - 1) , (deepcopy(self.Col)+1)
        if leftCellSmaller: 
            self.direction = Direction.NORTH
            cordsx, cordsy =(deepcopy(self.Row) - 1) , (deepcopy(self.Col))
        if rightCellSmaller:
            self.direction = Direction.EAST
            cordsx, cordsy =  (deepcopy(self.Row) ) , (deepcopy(self.Col)+1)
        
        if cordsx != None or cordsy != None:
            self.Row = deepcopy(cordsx)
            self.Col = deepcopy(cordsy)
            self.Moves += 1#self.Moves + 1
            self.board_robot_moves[self.Row][self.Col] += 1
        
        return cordsx, cordsy
    
    
    def checkEast(self):
        cordsx, cordsy = None, None
        straightCell, straightCellSmaller = -1, False
        leftCell, leftCellSmaller = -1, False
        rightCell, rightCellSmaller = -1, False
        if self.Col +1 < len( self.board[0] ) :
            straightCell, straightCellSmaller = self.board_robot_moves[self.Row ][self.Col+1], False
        if self.Row -1 >= 0 and self.Col +1 < len( self.board[0] ) :
            leftCell, leftCellSmaller = self.board_robot_moves[self.Row -1][self.Col +1], False
        if self.Row +1 < len(self.board) and self.Col +1 < len( self.board[0] ):
            rightCell, rightCellSmaller = self.board_robot_moves[self.Row +1][self.Col+1], False
        
        if straightCell == -1 :
            straightCell = deepcopy(self.maxMoves) +1
        if leftCell == -1 :
            leftCell = deepcopy(self.maxMoves) +1
        if rightCell == -1 :
            rightCell = deepcopy(self.maxMoves) +1

        if straightCell <= leftCell and straightCell <= rightCell and straightCell != self.maxMoves +1:
            straightCellSmaller = True
        if leftCell <= rightCell and leftCell != self.maxMoves +1 and not straightCellSmaller:
            leftCellSmaller = True
        if rightCell != self.maxMoves +1 and not straightCellSmaller and not leftCellSmaller:
            rightCellSmaller = True
        
        if straightCellSmaller:
            self.direction = Direction.EAST
            cordsx, cordsy = (deepcopy(self.Row)  ) , (deepcopy(self.Col)+1)
        if leftCellSmaller: 
            self.direction = Direction.NORTHEAST
            cordsx, cordsy = (deepcopy(self.Row) - 1) , (deepcopy(self.Col)+1)
        if rightCellSmaller:
            self.direction = Direction.SOUTHEAST
            cordsx, cordsy =  (deepcopy(self.Row) +1) , (deepcopy(self.Col)+1)
        
        if cordsx != None or cordsy != None:
            self.Row = deepcopy(cordsx)
            self.Col = deepcopy(cordsy)
            self.Moves += 1#self.Moves + 1
            self.board_robot_moves[self.Row][self.Col] += 1
        
        return cordsx, cordsy
    
    def checkSouthEast(self):
        cordsx, cordsy = None, None
        straightCell, straightCellSmaller = -1, False
        leftCell, leftCellSmaller = -1, False
        rightCell, rightCellSmaller = -1, False
        if self.Row +1 < len(self.board) and self.Col +1 < len( self.board[0] ):
            straightCell, straightCellSmaller = self.board_robot_moves[self.Row +1][self.Col+1], False
        if self.Col +1 < len( self.board[0] ) :
            leftCell, leftCellSmaller = self.board_robot_moves[self.Row ][self.Col +1], False
        if self.Row +1 < len(self.board) :
            rightCell, rightCellSmaller = self.board_robot_moves[self.Row +1][self.Col], False
        
        if straightCell == -1 :
            straightCell = deepcopy(self.maxMoves) +1
        if leftCell == -1 :
            leftCell = deepcopy(self.maxMoves) +1
        if rightCell == -1 :
            rightCell = deepcopy(self.maxMoves) +1

        if straightCell <= leftCell and straightCell <= rightCell and straightCell != self.maxMoves +1:
            straightCellSmaller = True
        if leftCell <= rightCell and leftCell != self.maxMoves +1 and not straightCellSmaller:
            leftCellSmaller = True
        if rightCell != self.maxMoves +1 and not straightCellSmaller and not leftCellSmaller:
            rightCellSmaller = True
        
        if straightCellSmaller:
            self.direction = Direction.SOUTHEAST
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col)+1)
        if leftCellSmaller: 
            self.direction = Direction.EAST
            cordsx, cordsy = (deepcopy(self.Row) ) , (deepcopy(self.Col)+1)
        if rightCellSmaller:
            self.direction = Direction.SOUTH
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col))
            
        if cordsx != None or cordsy != None:
            self.Row = deepcopy(cordsx)
            self.Col = deepcopy(cordsy)
            self.Moves += 1#self.Moves + 1
            self.board_robot_moves[self.Row][self.Col] += 1
            
        return cordsx, cordsy
    
    def checkSouth(self):
        cordsx, cordsy = None, None
        straightCell, straightCellSmaller = -1, False
        leftCell, leftCellSmaller = -1, False
        rightCell, rightCellSmaller = -1, False
        if self.Row +1 < len(self.board) :
            straightCell, straightCellSmaller = self.board_robot_moves[self.Row +1][self.Col], False
        if self.Row +1 < len(self.board) and self.Col +1 < len( self.board[0] ) :
            leftCell, leftCellSmaller = self.board_robot_moves[self.Row +1][self.Col +1], False
        if self.Row +1 < len(self.board) and self.Col -1 >= 0 :
            rightCell, rightCellSmaller = self.board_robot_moves[self.Row +1][self.Col -1], False
        
        if straightCell == -1 :
            straightCell = deepcopy(self.maxMoves) +1
        if leftCell == -1 :
            leftCell = deepcopy(self.maxMoves) +1
        if rightCell == -1 :
            rightCell = deepcopy(self.maxMoves) +1

        if straightCell <= leftCell and straightCell <= rightCell and straightCell != self.maxMoves +1:
            straightCellSmaller = True
        if leftCell <= rightCell and leftCell != self.maxMoves +1 and not straightCellSmaller:
            leftCellSmaller = True
        if rightCell != self.maxMoves +1 and not straightCellSmaller and not leftCellSmaller:
            rightCellSmaller = True
        
        if straightCellSmaller:
            self.direction = Direction.SOUTH
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col) )
        if leftCellSmaller: 
            self.direction = Direction.SOUTHEAST
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col) +1)
        if rightCellSmaller:
            self.direction = Direction.SOUTHWEST
            cordsx, cordsy =  (deepcopy(self.Row) +1) , (deepcopy(self.Col) -1)
            
        if cordsx != None or cordsy != None:
            self.Row = deepcopy(cordsx)
            self.Col = deepcopy(cordsy)
            self.Moves += 1#self.Moves + 1
            self.board_robot_moves[self.Row][self.Col] += 1
            
        return cordsx, cordsy
    
    def checkSouthWest(self):
        cordsx, cordsy = None, None
        straightCell, straightCellSmaller = -1, False
        leftCell, leftCellSmaller = -1, False
        rightCell, rightCellSmaller = -1, False
        if self.Row +1 < len(self.board) and self.Col -1 >= 0 :
            straightCell, straightCellSmaller = self.board_robot_moves[self.Row +1][self.Col -1], False
        if self.Row +1 < len(self.board) :
            leftCell, leftCellSmaller = self.board_robot_moves[self.Row +1][self.Col ], False
        if self.Col -1 >= 0:
            rightCell, rightCellSmaller = self.board_robot_moves[self.Row ][self.Col -1], False
        
        if straightCell == -1 :
            straightCell = deepcopy(self.maxMoves) +1
        if leftCell == -1 :
            leftCell = deepcopy(self.maxMoves) +1
        if rightCell == -1 :
            rightCell = deepcopy(self.maxMoves) +1

        if straightCell <= leftCell and straightCell <= rightCell and straightCell != self.maxMoves +1:
            straightCellSmaller = True
        if leftCell <= rightCell and leftCell != self.maxMoves +1 and not straightCellSmaller:
            leftCellSmaller = True
        if rightCell != self.maxMoves +1 and not straightCellSmaller and not leftCellSmaller:
            rightCellSmaller = True
        
        if straightCellSmaller:
            self.direction = Direction.SOUTHWEST
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col) -1)
        if leftCellSmaller: 
            self.direction = Direction.SOUTH
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col) )
        if rightCellSmaller:
            self.direction = Direction.WEST
            cordsx, cordsy =  (deepcopy(self.Row) ) , (deepcopy(self.Col) -1)
            
        if cordsx != None or cordsy != None:
            self.Row = deepcopy(cordsx)
            self.Col = deepcopy(cordsy)
            self.Moves += 1#self.Moves + 1
            self.board_robot_moves[self.Row][self.Col] += 1
            
        return cordsx, cordsy
    
    def checkWest(self):
        cordsx, cordsy = None, None
        straightCell, straightCellSmaller = -1, False
        leftCell, leftCellSmaller = -1, False
        rightCell, rightCellSmaller = -1, False
        if self.Col -1 >= 0 :
            straightCell, straightCellSmaller = self.board_robot_moves[self.Row ][self.Col -1], False
        if self.Row +1 < len(self.board) and self.Col -1 >= 0 :
            leftCell, leftCellSmaller = self.board_robot_moves[self.Row +1][self.Col -1], False
        if self.Row -1 >= 0 and self.Col -1 >= 0:
            rightCell, rightCellSmaller = self.board_robot_moves[self.Row -1][self.Col -1], False
        
        if straightCell == -1 :
            straightCell = deepcopy(self.maxMoves) +1
        if leftCell == -1 :
            leftCell = deepcopy(self.maxMoves) +1
        if rightCell == -1 :
            rightCell = deepcopy(self.maxMoves) +1

        if straightCell <= leftCell and straightCell <= rightCell and straightCell != self.maxMoves +1:
            straightCellSmaller = True
        if leftCell <= rightCell and leftCell != self.maxMoves +1 and not straightCellSmaller:
            leftCellSmaller = True
        if rightCell != self.maxMoves +1 and not straightCellSmaller and not leftCellSmaller:
            rightCellSmaller = True
        
        if straightCellSmaller:
            self.direction = Direction.WEST
            cordsx, cordsy = (deepcopy(self.Row) ) , (deepcopy(self.Col) -1)
        if leftCellSmaller: 
            self.direction = Direction.SOUTHWEST
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col) -1)
        if rightCellSmaller:
            self.direction = Direction.NORTHWEST
            cordsx, cordsy =  (deepcopy(self.Row) -1) , (deepcopy(self.Col) -1)
            
        if cordsx != None or cordsy != None:
            self.Row = deepcopy(cordsx)
            self.Col = deepcopy(cordsy)
            self.Moves += 1#self.Moves + 1
            self.board_robot_moves[self.Row][self.Col] += 1
            
        return cordsx, cordsy
    
    def checkNorthWest(self):
        cordsx, cordsy = None, None
        straightCell, straightCellSmaller = -1, False
        leftCell, leftCellSmaller = -1, False
        rightCell, rightCellSmaller = -1, False
        if self.Row -1 >= 0 and self.Col -1 >= 0 :
            straightCell, straightCellSmaller = self.board_robot_moves[self.Row -1][self.Col -1], False
        if self.Col -1 >= 0 :
            leftCell, leftCellSmaller = self.board_robot_moves[self.Row ][self.Col -1], False
        if self.Row -1 >= 0 :
            rightCell, rightCellSmaller = self.board_robot_moves[self.Row -1][self.Col], False
        
        if straightCell == -1 :
            straightCell = deepcopy(self.maxMoves) +1
        if leftCell == -1 :
            leftCell = deepcopy(self.maxMoves) +1
        if rightCell == -1 :
            rightCell = deepcopy(self.maxMoves) +1

        if straightCell <= leftCell and straightCell <= rightCell and straightCell != self.maxMoves +1:
            straightCellSmaller = True
        if leftCell <= rightCell and leftCell != self.maxMoves +1 and not straightCellSmaller:
            leftCellSmaller = True
        if rightCell != self.maxMoves +1 and not straightCellSmaller and not leftCellSmaller:
            rightCellSmaller = True
        
        if straightCellSmaller:
            self.direction = Direction.NORTHWEST
            cordsx, cordsy = (deepcopy(self.Row) -1) , (deepcopy(self.Col) -1)
        if leftCellSmaller: 
            self.direction = Direction.WEST
            cordsx, cordsy = (deepcopy(self.Row) ) , (deepcopy(self.Col) -1)
        if rightCellSmaller:
            self.direction = Direction.NORTH
            cordsx, cordsy =  (deepcopy(self.Row) -1) , (deepcopy(self.Col) )
            
        if cordsx != None or cordsy != None:
            self.Row = deepcopy(cordsx)
            self.Col = deepcopy(cordsy)
            self.Moves += 1#self.Moves + 1
            self.board_robot_moves[self.Row][self.Col] += 1
            
        return cordsx, cordsy
    ''' SENSOR FUNCTION CALLS THE OTHER NEEDED SENSORS '''
    def robotSensors(self):
        tempRow, tempCol = None, None
        if self.direction == Direction.NORTH :
            tempRow, tempCol = self.directionNorth()
        elif self.direction == Direction.NORTHEAST :
            tempRow, tempCol = self.directionNorthEast()
        elif self.direction == Direction.EAST :
            tempRow, tempCol = self.directionEast()
        elif self.direction == Direction.SOUTHEAST :
            tempRow, tempCol = self.directionSouthEast()
        elif self.direction == Direction.SOUTH :
            tempRow, tempCol = self.directionSouth()
        elif self.direction == Direction.SOUTHWEST :
            tempRow, tempCol = self.directionSouthWest()
        elif self.direction == Direction.WEST :
            tempRow, tempCol = self.directionWest()
        elif self.direction == Direction.NORTHWEST :
            tempRow, tempCol = self.directionNorthWest()                
                  
        if tempRow != None or tempCol != None:
            self.Row = deepcopy(tempRow)
            self.Col = deepcopy(tempCol)
            self.Moves += 1
            self.board_robot_moves[self.Row][self.Col] += 1
            return True
        
        return False 
                
        ''' FUNCTIONS BELOW ARE USED FOR SENSORS '''        
    def directionNorth(self):
        cordsx, cordsy = None, None 
        if self.North():
            self.direction = Direction.NORTH 
            cordsx, cordsy =  (deepcopy(self.Row) - 1) , (deepcopy(self.Col))
        if self.NorthWest():
            self.direction = Direction.NORTHWEST
            cordsx, cordsy = (deepcopy(self.Row) - 1) , (deepcopy(self.Col) - 1)
        if self.NorthEast():
            self.direction = Direction.NORTHEAST
            cordsx, cordsy = (deepcopy(self.Row) - 1) , (deepcopy(self.Col) +1)
        
        return cordsx, cordsy
        
        
    def directionNorthEast(self):
        cordsx, cordsy = None, None 
        if self.NorthEast():
            self.direction = Direction.NORTHEAST
            cordsx, cordsy = (deepcopy(self.Row) - 1) , (deepcopy(self.Col) +1)
        if self.North():
            self.direction = Direction.NORTH
            cordsx, cordsy = (deepcopy(self.Row) - 1) , (deepcopy(self.Col))
        if self.East():
            self.direction = Direction.EAST
            cordsx, cordsy = (deepcopy(self.Row) ) , (deepcopy(self.Col) +1)
        
        return cordsx, cordsy
    
    
    def directionEast(self):
        cordsx, cordsy = None, None 
        if self.East():
            self.direction = Direction.EAST
            cordsx, cordsy = (deepcopy(self.Row) ) , (deepcopy(self.Col) +1)
        if self.NorthEast():
            self.direction = Direction.NORTHEAST
            cordsx, cordsy = (deepcopy(self.Row) - 1) , (deepcopy(self.Col) +1)
        if self.SouthEast():
            self.direction = Direction.SOUTHEAST
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col) +1)

        return cordsx, cordsy
        
        
    def directionSouthEast(self):
        cordsx, cordsy = None, None
        if self.SouthEast():
            self.direction = Direction.SOUTHEAST
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col) +1)
        if self.East():
            self.direction = Direction.EAST
            cordsx, cordsy = (deepcopy(self.Row) ) , (deepcopy(self.Col) +1)
        if self.South():
            self.direction = Direction.SOUTH
            cordsx, cordsy = (deepcopy(self.Row) +1), (deepcopy(self.Col))

        return cordsx, cordsy 
        
        
    def directionSouth(self):
        cordsx, cordsy = None, None 
        if self.South():
            self.direction = Direction.SOUTH
            cordsx, cordsy = (deepcopy(self.Row) +1), (deepcopy(self.Col))
        if self.SouthEast():
            self.direction = Direction.SOUTHEAST
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col) +1)
        if self.SouthWest():
            self.direction = Direction.SOUTHWEST
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col) -1)
        
        return cordsx, cordsy 
        
    
    def directionSouthWest(self):
        cordsx, cordsy = None, None
        if self.SouthWest():
            self.direction = Direction.SOUTHWEST
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col) -1)
        if self.South():
            self.direction = Direction.SOUTH
            cordsx, cordsy = (deepcopy(self.Row) +1), (deepcopy(self.Col))
        if self.West():
            self.direction = Direction.WEST
            cordsx, cordsy = (deepcopy(self.Row) ) , (deepcopy(self.Col) -1)
        
        return cordsx, cordsy
        
        
    def directionWest(self):
        cordsx, cordsy = None, None
        if self.West():
            self.direction = Direction.WEST
            cordsx, cordsy = (deepcopy(self.Row) ) , (deepcopy(self.Col) -1)
        if self.SouthWest():
            self.direction = Direction.SOUTHWEST
            cordsx, cordsy = (deepcopy(self.Row) +1) , (deepcopy(self.Col) -1)
        if self.NorthWest():
            self.direction = Direction.NORTHWEST
            cordsx, cordsy = (deepcopy(self.Row) -1), (deepcopy(self.Col) -1)
        
        return cordsx, cordsy
        
        
    def directionNorthWest(self):
        cordsx, cordsy = None, None
        if self.NorthWest():
            self.direction = Direction.NORTHWEST
            cordsx, cordsy = (deepcopy(self.Row) -1), (deepcopy(self.Col) -1)
        if self.West():
            self.direction = Direction.WEST
            cordsx, cordsy = (deepcopy(self.Row) ) , (deepcopy(self.Col) -1)
        if self.North():
            self.direction = Direction.NORTH
            cordsx, cordsy = (deepcopy(self.Row) -1) , (deepcopy(self.Col) )
        
        return cordsx, cordsy 

        
    def North(self):
        tempRow = deepcopy(self.Row)
        tempCol = deepcopy(self.Col)
        isFoundGoal = False
        while tempRow -1 >= 0:
            tempRow = tempRow - 1
            if self.Actual_Map[tempRow][tempCol] == 1:
                self.board[tempRow][tempCol] = 1
            elif self.Actual_Map[tempRow][tempCol] == 0:
                self.board[tempRow][tempCol] = 0
                break
            elif self.Actual_Map[tempRow][tempCol] == 'D':
                isFoundGoal = True
        return isFoundGoal
    
    def NorthEast(self):
        tempRow = deepcopy(self.Row)
        tempCol = deepcopy(self.Col)
        isFoundGoal = False
        while tempRow -1 >= 0 and tempCol +1 < len( self.board[0] ):
            tempRow = tempRow - 1
            tempCol = tempCol +1
            if self.Actual_Map[tempRow][tempCol] == 1:
                self.board[tempRow][tempCol] = 1
            elif self.Actual_Map[tempRow][tempCol] == 0:
                self.board[tempRow][tempCol] = 0
                break
            elif self.Actual_Map[tempRow][tempCol] == 'D':
                isFoundGoal = True
        return isFoundGoal
    
    def East(self):
        tempRow = deepcopy(self.Row)
        tempCol = deepcopy(self.Col)
        isFoundGoal = False
        while tempCol +1 < len( self.board[0] ):
            tempCol = tempCol +1
            if self.Actual_Map[tempRow][tempCol] == 1:
                self.board[tempRow][tempCol] = 1
            elif self.Actual_Map[tempRow][tempCol] == 0:
                self.board[tempRow][tempCol] = 0
                break
            elif self.Actual_Map[tempRow][tempCol] == 'D':
                isFoundGoal = True
        return isFoundGoal
    
    def SouthEast(self):
        tempRow = deepcopy(self.Row)
        tempCol = deepcopy(self.Col)
        isFoundGoal = False
        while tempRow + 1< len(self.board ) and tempCol + 1< len(self.board[0]):
            tempRow = tempRow + 1
            tempCol = tempCol + 1
            if self.Actual_Map[tempRow][tempCol] == 1:
                self.board[tempRow][tempCol] = 1
            elif self.Actual_Map[tempRow][tempCol] == 0:
                self.board[tempRow][tempCol] = 0
                break
            elif self.Actual_Map[tempRow][tempCol] == 'D':
                isFoundGoal = True
        return isFoundGoal
    
    def South(self):
        tempRow = deepcopy(self.Row)
        tempCol = deepcopy(self.Col)
        isFoundGoal = False
        while tempRow + 1< len(self.board ):
            tempRow = tempRow + 1
            if self.Actual_Map[tempRow][tempCol] == 1:
                self.board[tempRow][tempCol] = 1
            elif self.Actual_Map[tempRow][tempCol] == 0:
                self.board[tempRow][tempCol] = 0
                break
            elif self.Actual_Map[tempRow][tempCol] == 'D':
                isFoundGoal = True
        return isFoundGoal
    
    def SouthWest(self):
        tempRow = deepcopy(self.Row)
        tempCol = deepcopy(self.Col)
        isFoundGoal = False
        while tempRow + 1< len(self.board ) and tempCol -1 >= 0:
            tempRow = tempRow + 1
            tempCol = tempCol -1
            if self.Actual_Map[tempRow][tempCol] == 1:
                self.board[tempRow][tempCol] = 1
            elif self.Actual_Map[tempRow][tempCol] == 0:
                self.board[tempRow][tempCol] = 0
                break
            elif self.Actual_Map[tempRow][tempCol] == 'D':
                isFoundGoal = True
        return isFoundGoal
    
    def West(self):
        tempRow = deepcopy(self.Row)
        tempCol = deepcopy(self.Col)
        isFoundGoal = False
        while tempCol -1 >= 0:
            tempCol = tempCol -1
            if self.Actual_Map[tempRow][tempCol] == 1:
                self.board[tempRow][tempCol] = 1
            elif self.Actual_Map[tempRow][tempCol] == 0:
                self.board[tempRow][tempCol] = 0
                break
            elif self.Actual_Map[tempRow][tempCol] == 'D':
                isFoundGoal = True
        return isFoundGoal
    
    def NorthWest(self):
        tempRow = deepcopy(self.Row)
        tempCol = deepcopy(self.Col)
        isFoundGoal = False
        while tempRow -1 >= 0 and tempCol -1 >= 0:
            tempRow = tempRow -1
            tempCol = tempCol -1
            if self.Actual_Map[tempRow][tempCol] == 1:
                self.board[tempRow][tempCol] = 1
            elif self.Actual_Map[tempRow][tempCol] == 0:
                self.board[tempRow][tempCol] = 0
                break
            elif self.Actual_Map[tempRow][tempCol] == 'D':
                isFoundGoal = True
        return isFoundGoal

''' TESTING ZONE'''
isRandom = input("Should this test be made with pre designated values? (0 is false, 1 is true) : ")
if int(isRandom) == 0:
    fileName = str(input("where is the output directory? ex C:\\\\Users\\\\name\\\\Desktop : "))
    sizeRows = int(input("how many rows should this test run have? : "))
    sizeCols = int(input("how many columns should this test run have? : "))
    startLocRow = int(input("What row should the robot start in? : "))
    startLocCol = int(input("What column should the robot start in? : "))
    endLocRow = int(input("What row should the robot end in? : "))
    endLocCol = int(input("What column should the robot end in? : "))
    direction = int(input("What direction should it be facing? ex 0 is north, 4 is south, 6 is West..."))
    difficulty = float(input("Obstacle percentage ? ex .55 is 45% obstacles :"))
    maxMoves = int(input("Max number of moves the robot has before stopping? :"))

else:
    fileName = str(input("where is the output directory? ex C:\\\\Users\\\\name\\\\Desktop : "))
    sizeRows = 35
    sizeCols = 45
    startLocRow = 5
    startLocCol = 7
    endLocRow = 30
    endLocCol = 40
    direction = 4
    difficulty = 0.7
    maxMoves = 5000

array_map_01 = [ [2]*sizeCols for i in range(sizeRows)] 
array_map_01[startLocRow][startLocCol] = 1 
array_map_01[endLocRow][endLocCol] = 'D' 

test_01 = StudentSolution(array_map_01,startLocRow, startLocCol, direction, difficulty, maxMoves, fileName)
test_01.solution()
test_01.print_robot_moves_map()
print("Solution should be ready in the file specified" )
print("if no solution found most likely unsolvable board")

