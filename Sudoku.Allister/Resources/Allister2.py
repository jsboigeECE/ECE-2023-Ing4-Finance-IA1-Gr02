from ortools.sat.python import cp_model
import numpy
import random
import math

'''
instance = ((0, 0, 0, 0, 9, 4, 0, 3, 0),
            (0, 0, 0, 5, 1, 0, 0, 0, 7),
            (0, 8, 9, 0, 0, 0, 0, 4, 0),
            (0, 0, 0, 0, 0, 0, 2, 0, 8),
            (0, 6, 0, 2, 0, 1, 0, 5, 0),
            (1, 0, 2, 0, 0, 0, 0, 0, 0),
            (0, 7, 0, 0, 0, 0, 5, 2, 0),
            (9, 0, 0, 0, 6, 5, 0, 0, 0),
            (0, 4, 0, 9, 7, 0, 0, 0, 0))
'''



# r = [ [ m.evaluate(X[i][j]).as_long() for j in range(9) ]
#      for i in range(9) ]

def display_sudoku(sudoku):
    print(" -------------------------\n")
    (i, j) = (0, 0)
    for i in range(9):
        print(" | ", end="")
        for j in range(9):
            print("{} ".format(sudoku[i, j]) if (math.fmod((j + 1), 3)) else "{} | ".format(sudoku[i, j]), end="")
        print("\n")
        # print(sudoku[i, j], end=" ")
        if not (math.fmod((i + 1), 3)):
            print(" -------------------------\n")
    print("\n\n")


def print_grid(tab):
	for i in range(9):
		for j in range(9):
			print (tab[i][j], end = " "),
		print ()

# Allow to put initial constraint of the Sudoku
def initialize_sudoku(model_or_tool, sudoku_init, sudoku_to_do):
    for i in range(9):
        for j in range(9):
            if sudoku_to_do[i][j] != 0:  # If case of sudoku is filled
                sudoku_init[i][j] = model_or_tool.NewIntVar(int(sudoku_to_do[i][j]), int(sudoku_to_do[i][j]),
                                                            'column: %i' % i)
    return sudoku_init


def solveSudoku(sudoku):
    model = cp_model.CpModel()
    sudoku2 = [[model.NewIntVar(1, 9, 'column: %i' % i) for i in range(9)] for j in range(9)]
    sudoku = initialize_sudoku(model, sudoku2, sudoku)

    # Constraint in line
    for i in range(9):
        line = []
        for j in range(9):
            line.append(sudoku[i][j])
        model.AddAllDifferent(line)

    # Constraint in column
    for i in range(9):
        column = []
        for j in range(9):
            column.append(sudoku[j][i])
        model.AddAllDifferent(column)

    # Constraint in sector
    for index in range(9):
        sector = []
        for i in [(index // 3) * 3, (index // 3) * 3 + 1, (index // 3) * 3 + 2]:
            for j in [(index % 3) * 3, (index % 3) * 3 + 1, (index % 3) * 3 + 2]:
                sector.append(sudoku[i][j])
                model.AddAllDifferent(sector)

    # Initialize the solver
    solver = cp_model.CpSolver()

    # Solving
    status = solver.Solve(model)

    if status == cp_model.FEASIBLE or status == cp_model.OPTIMAL:
        print(" -------------------------\n")
        (i, j) = (0, 0)
        for i in range(9):
            print(" | ", end="")
            for j in range(9):
                print("{} ".format(solver.Value(sudoku[i][j])) if (math.fmod((j + 1), 3)) else "{} | ".format(
                    solver.Value(sudoku[i][j])), end="")
            print("\n")
            # print(sudoku[i, j], end=" ")
            if not (math.fmod((i + 1), 3)):
                print(" -------------------------\n")
        print("\n\n")

        sudokuResolu = []
        for i in range(9):
            for j in range(9):
                #print(solver.Value(sudoku[i][j]), end="")
                sudokuResolu.append(solver.Value(sudoku[i][j]))

        #print(sudokuResolu)

        #print(solver.Value(sudoku))

        '''
        print("SUDOKU")
        for i in range(9):
            for j in range(9):
                sudokuResolu.append(solver.Value(sudoku[i][j]))
        # print(sudokuResolu)

        print("SUDOKU RESOLU")
        for i in range(81):
            print(sudokuResolu[i])
        '''
        
        return sudokuResolu


# Example of solving :




ri = [[0,0,0,0,9,4,0,3,0],
[0,0,0,5,1,0,0,0,7],
[0,8,9,0,0,0,0,4,0],
[0,0,0,0,0,0,2,0,8],
[0,6,0,2,0,1,0,5,0],
[1,0,2,0,0,0,0,0,0],
[0,7,0,0,0,0,5,2,0],
[9,0,0,0,6,5,0,0,0],
[0,4,0,9,7,0,0,0,0]]
rr = solveSudoku(instance)
print(rr)
for i in range (9):
    for j in range(9):
        ri[i][j]=rr[i*9+j]
#print_grid(r)
#r = instance
r=ri
