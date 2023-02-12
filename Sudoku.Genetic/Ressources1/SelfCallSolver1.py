import clr
clr.AddReference("Sudoku.Shared")
clr.AddReference("Sudoku.Genetic")
from Sudoku.Genetic import GeneticSolver
netSolver = GeneticSolver()
solvedSudoku = netSolver.Solve(sudoku)