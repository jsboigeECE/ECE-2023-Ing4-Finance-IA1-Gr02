using Sudoku.Shared;


namespace Sudoku.Backtracking
{


    public class BackTrackingSolverCS2 : ISudokuSolver
    {

        public static bool isSafe(SudokuGrid s,int row, int col,int num)
        {

            // Row has the unique (row-clash)
            for (int d = 0; d < s.Cells.GetLength(0); d++)
            {

                // Check if the number
                // we are trying to
                // place is already present in
                // that row, return false;
                if (s.Cells[row][d] == num)
                {
                    return false;
                }
            }

            // Column has the unique numbers (column-clash)
            for (int r = 0; r < s.Cells.GetLength(0); r++)
            {

                // Check if the number
                // we are trying to
                // place is already present in
                // that column, return false;
                if (s.Cells[r][col] == num)
                {
                    return false;
                }
            }

            // corresponding square has
            // unique number (box-clash)
            int sqrt = (int)Math.Sqrt(s.Cells.GetLength(0));
            int boxRowStart = row - row % sqrt;
            int boxColStart = col - col % sqrt;

            for (int r = boxRowStart;
            r < boxRowStart + sqrt; r++)
            {
                for (int d = boxColStart;
                d < boxColStart + sqrt; d++)
                {
                    if (s.Cells[r][d] == num)
                    {
                        return false;
                    }
                }
            }

            // if there is no clash, it's safe
            return true;
        }

        public bool Solve(SudokuGrid s)
        {
            int row = -1;
            int col = -1;
            bool isEmpty = true;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (s.Cells[i][j] == 0)
                    {
                        row = i;
                        col = j;

                        // We still have some remaining
                        // missing values in Sudoku
                        isEmpty = false;
                        break;
                    }
                }
                if (!isEmpty)
                {
                    break;
                }
            }

            // no empty space left
            if (isEmpty)
            {
                return true;
            }

            // else for each-row backtrack
            for (int num = 1; num <= 9; num++)
            {
                if (isSafe(s, row, col, num))
                {
                    s.Cells[row][col] = num;
                    if (Solve(s))
                    {

                        // Print(board, n);
                        return true;

                    }
                    else
                    {

                        // Replace it
                        s.Cells[row][col] = 0;
                    }
                }
            }
            return false;

        }

        SudokuGrid ISudokuSolver.Solve(SudokuGrid s)
        {

            if (Solve(s))
            {

                return s;
            }
            else
            {
                Console.Write("No solution");
                return s;

            }
        }
    }
}