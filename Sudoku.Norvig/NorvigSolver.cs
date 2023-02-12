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
			var sudokuString = s.Cells.Select(row => row.Select(cell => cell == 0 ? "." : cell.ToString(CultureInfo.InvariantCulture)).Aggregate((s1, s2) => s1 + s2)).Aggregate((s1, s2) => s1 + s2);
			var linqGrid = LinqSudokuSolver.parse_grid(sudokuString);
			var solved = LinqSudokuSolver.search(linqGrid);
			for (int i = 0; i < 9; i++)
			{
				for (int j = 1; j < 10; j++)
				{
					var key = ((char)('A' + i)) + j.ToString(CultureInfo.InvariantCulture);
					var strCellValue = solved[key];
					s.Cells[i][j-1] = int.Parse(strCellValue);
				}
			}
			return s;

		}
	}


}