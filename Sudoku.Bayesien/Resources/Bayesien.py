import numpy as np
import pymc as pm
import math
from math import *


# Create a 9x9 matrix representing the Sudoku puzzle
# Use 0 to represent empty cells
#sudoku = np.array([[5, 3, 0, 0, 7, 0, 0, 0, 0],
#                   [6, 0, 0, 1, 9, 5, 0, 0, 0],
#                   [0, 9, 8, 0, 0, 0, 0, 6, 0],
#                   [8, 0, 0, 0, 6, 0, 0, 0, 3],
#                   [4, 0, 0, 8, 0, 3, 0, 0, 1],
#                   [7, 0, 0, 0, 2, 0, 0, 0, 6],
#                   [0, 6, 0, 0, 0, 0, 2, 8, 0],
#                   [0, 0, 0, 4, 1, 9, 0, 0, 5],
#                   [0, 0, 0, 0, 8, 0, 0, 7, 9]])


sudoku = np.array(instance)

# Create a PyMC3 model
with pm.Model() as sudoku_model:
    # Create variables for each empty cell in the Sudoku puzzle
    variables = {}
    for i in range(9):
        for j in range(9):
            if sudoku[i, j] == 0:
                variables[f'{i},{j}'] = pm.DiscreteUniform(f'var_{i}_{j}', lower=1, upper=9)
                # Add constraints to ensure that each row contains unique numbers
    for i in range(9):
        row_vars = [variables[f'{i},{j}'] for j in range(9) if sudoku[i, j] == 0]
        pm.math.math.AllDifferent(row_vars)

    # Add constraints to ensure that each column contains unique numbers
    for j in range(9):
        col_vars = [variables[f'{i},{j}'] for i in range(9) if sudoku[i, j] == 0]
        pm.math.math.AllDifferent(col_vars)

    # Add constraints to ensure that each 3x3 sub-grid contains unique numbers
    for i in range(0, 9, 3):
        for j in range(0, 9, 3):
            subgrid_vars = [variables[f'{x},{y}'] for x in range(i, i+3) for y in range(j, j+3) if sudoku[x, y] == 0]
        pm.math.math.AllDifferent(subgrid_vars)
    
    # Use Metropolis-Hastings sampling method to solve the puzzle
    trace = pm.sample(draws=1000, tune=500, step=pm.Metropolis())

    
    # Extract the solution from the trace
    solution = np.zeros((9,9))
    for i in range(9):
        for j in range(9):
            if sudoku[i, j] == 0:
                solution[i, j] = trace['var_{i}_{j}'].mean()

# Print the solution
solution = np.zeros((9, 9))
for i in range(9):
    for j in range(9):
        if sudoku[i, j] == 0:
            solution[i, j] = trace[f'var_{i}_{j}'].mean()
#print(solution)
r=asNetArray(solution)

