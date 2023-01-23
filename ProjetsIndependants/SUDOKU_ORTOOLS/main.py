from ortools.sat.python import cp_model
import numpy
import random
import math


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
        '''
        for i in range(9):
            for j in range(9):
                print(solver.Value(sudoku[i][j]), end=" ")
            print()
        '''


# Example of solving :

sudoku_to_solve = numpy.array([[0, 0, 4, 1, 0, 0, 7, 9, 3],
                               [0, 9, 0, 0, 0, 0, 0, 0, 0],
                               [0, 0, 7, 2, 0, 0, 8, 0, 0],
                               [6, 0, 9, 0, 4, 0, 0, 0, 0],
                               [0, 3, 0, 5, 0, 9, 0, 6, 0],
                               [0, 0, 0, 0, 3, 0, 2, 0, 9],
                               [0, 0, 6, 0, 0, 8, 1, 0, 0],
                               [0, 0, 0, 0, 0, 0, 0, 3, 0],
                               [5, 8, 2, 0, 0, 6, 9, 0, 0]])

def facile(numeroSudoku):
    with open('Sudoku_Easy51.txt', 'r') as fichier:
        lignes = fichier.readlines()
        #print(lignes[numeroSudoku-1])
    return lignes[numeroSudoku-1]

def moyen(numeroSudoku):
    with open('Sudoku_hardest.txt', 'r') as fichier:
        lignes = fichier.readlines()
        #print(lignes[numeroSudoku - 1])
        lignes = [elem.replace('.', '0') for elem in lignes]
        #print(lignes[numeroSudoku - 1])
        #print(len(lignes[numeroSudoku - 1]))
    return lignes[numeroSudoku-1]

def difficile(numeroSudoku):
    with open('Sudoku_top95.txt', 'r') as fichier:
        lignes = fichier.readlines()
        #print(lignes[numeroSudoku - 1])
        lignes = [elem.replace('.', '0') for elem in lignes]
        #print(lignes[numeroSudoku - 1])
        #print(len(lignes[numeroSudoku - 1]))
    return lignes[numeroSudoku - 1]

def tradToGrilleAvecZeros(liste):
    grille = []
    grilleStr = []
    for i in range(0, 81):
        grilleStr.append(liste[i])

    '''
    for i in range(0, 9):
        liste = []
        for k in range(0, 9):
            liste = liste.append(lignes[])
    '''
    #print("grilleStr:", grilleStr, "\nlen =", len(grilleStr))

    for i in range(0, 9):
        grilleTemp = []
        for j in range(1, 10):
            grilleTemp.append(int(grilleStr[j+9*i-1]))
        grille.append(grilleTemp)

    #print("grille:", grille)

    return grille

def tradToGrilleAvecPoints(liste):
    grille = []
    grilleStr = []
    for i in range(0, 81):
        grilleStr.append(liste[i])

    '''
    for i in range(0, 9):
        liste = []
        for k in range(0, 9):
            liste = liste.append(lignes[])
    '''
    #print("grilleStr:", grilleStr, "\nlen =", len(grilleStr))

    for i in range(0, 9):
        grilleTemp = []
        for j in range(1, 10):
            grilleTemp.append(int(grilleStr[j + 9 * i - 1]))
        grille.append(grilleTemp)

    # print("grille:", grille)

    return grille

def resolutionSudoku(grille):
    print("\n\nChosen Puzzle:")
    display_sudoku(grille)

    print("Valid solution:")
    solveSudoku(grille)


def main():
    while True:
        difficulte = int(input("Select difficulty: 1-Easy, 2-Medium, 3-Hard\n"))
        if difficulte == 1:
            numeroSudoku = int(input("Choose a puzzle index between 1 and 51\n"))
            liste = facile(numeroSudoku)
            grille = tradToGrilleAvecZeros(liste)
        elif difficulte == 2:
            numeroSudoku = int(input("Choose a puzzle index between 1 and 11\n"))
            liste = moyen(numeroSudoku)
            grille = tradToGrilleAvecPoints(liste)
        elif difficulte == 3:
            numeroSudoku = int(input("Choose a puzzle index between 1 and 95\n"))
            liste = difficile(numeroSudoku)
            grille = tradToGrilleAvecPoints(liste)
        else:
            print("ERREUR")

        resolutionSudoku(numpy.array(grille))

main()