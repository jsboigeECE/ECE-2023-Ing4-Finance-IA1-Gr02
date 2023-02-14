using Sudoku.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


namespace Sudoku.Norvig
{
	public class NorvigSolver : ISudokuSolver
	{
		public SudokuGrid Solve(SudokuGrid s)
		{
            // Convertir la grille de sudoku en une chaîne de caractères
            var sudokuString = s.Cells.Select(row => row.Select(cell => cell == 0 ? "." : cell.ToString(CultureInfo.InvariantCulture)).Aggregate((s1, s2) => s1 + s2)).Aggregate((s1, s2) => s1 + s2);

            // Utiliser la chaîne de caractères pour générer un dictionnaire en utilisant la méthode parse_grid
            var linqGrid = LinqSudokuSolver.parse_grid(sudokuString);

            // Résoudre la grille en utilisant la méthode search et obtenir un dictionnaire résolu
            var solved = LinqSudokuSolver.search(linqGrid);

            // Boucle à travers les lignes et les colonnes de la grille de sudoku
            for (int i = 0; i < 9; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    // Générer la clé pour le dictionnaire résolu en utilisant la ligne et la colonne
                    var key = ((char)('A' + i)) + j.ToString(CultureInfo.InvariantCulture);

                    // Récupérer la valeur pour la clé dans le dictionnaire résolu
                    var strCellValue = solved[key];

                    // Convertir la valeur en entier et l'ajouter à la grille de sudoku
                    s.Cells[i][j-1] = int.Parse(strCellValue);
                }
            }

            // Retourner la grille de sudoku résolue
            return s;

            
            /*int[,] grid = new int[9,9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    grid[i,j] = s.Cells[i][j];
                }
            }

            var solved = LinqSudokuSolver.search(grid);
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    s.Cells[i][j] = solved[i,j];
                }
            }
            return s;*/
            

		}
	}


}