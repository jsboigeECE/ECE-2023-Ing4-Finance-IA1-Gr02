using System;
using System.Collections.Generic;

namespace SudokuTools
{
    public class SudokuTool
    {
        public static int[][] prepMatrix()
        {
            int[][] toReturn = new int[Constants.BoardSize][];
            for (int i = 0; i < Constants.BlockSize; i++)
            {
                toReturn[i] = new int[Constants.BlockSize];
            }

            return toReturn;
        }

        /// <summary>
        /// Creates a copy of the provided matrix
        /// </summary>
        /// <param name="matrix">int[][]</param>
        /// <returns>another int[][] with the same values as passed in</returns>
        public static int[][] DuplicateMatrix(int[][] matrix)
        {
            var result = CreateMatrix(Constants.BoardSize, Constants.BoardSize);

            for (var i = 0; i < Constants.BoardSize; ++i)
                for (var j = 0; j < Constants.BoardSize; ++j)
                    result[i][j] = matrix[i][j];
            return result;
        }

        /// <summary>
        /// Generates a matrix (grid, or array of arrays) representing the sudoku puzzle
        /// </summary>
        /// <param name="rows">number of rows</param>
        /// <param name="columns">number of columns per row</param>
        /// <returns>an array of int[][]</returns>
        public static int[][] CreateMatrix(int rows, int columns)
        {
            var result = new int[rows][];
            for (var row = 0; row < rows; ++row)
                result[row] = new int[columns];
            return (result);
        }


        /// <summary>
        /// returns the row/column coordinate of the top-left cell in the block identified
        /// </summary>
        /// <param name="block">index of a 3x3 block</param>
        /// <returns>returns the (row,column) coordinate of the top-left cell</returns>
        public static int[] Corner(int block)
        {
            int r = -1, c = -1;

            if (block == 0 || block == 1 || block == 2) r = 0; // first row
            else if (block == 3 || block == 4 || block == 5)
                r = 3;
            else if (block == 6 || block == 7 || block == 8)
                r = 6;

            if (block == 0 || block == 3 || block == 6) c = 0;
            else if (block == 1 || block == 4 || block == 7)
                c = 3;
            else if (block == 2 || block == 5 || block == 8)
                c = 6;

            return new[] { r, c };
        }

        /// <summary>
        /// Returns the index of one of the nine blocks, 
        /// the one that contains the cell at the provided row/cell coordinates
        /// </summary>
        /// <param name="r">row index</param>
        /// <param name="c">column index</param>
        /// <returns>Returns the index of a block, as number left to right, top to bottom</returns>
        public static int Block(int r, int c)
        {
            if (r >= 0 && r <= 2 && c >= 0 && c <= 2)
                return 0;
            if (r >= 0 && r <= 2 && c >= 3 && c <= 5)
                return 1;
            if (r >= 0 && r <= 2 && c >= 6 && c <= 8)
                return 2;
            if (r >= 3 && r <= 5 && c >= 0 && c <= 2)
                return 3;
            if (r >= 3 && r <= 5 && c >= 3 && c <= 5)
                return 4;
            if (r >= 3 && r <= 5 && c >= 6 && c <= 8)
                return 5;
            if (r >= 6 && r <= 8 && c >= 0 && c <= 2)
                return 6;
            if (r >= 6 && r <= 8 && c >= 3 && c <= 5)
                return 7;
            if (r >= 6 && r <= 8 && c >= 6 && c <= 8)
                return 8;
            throw new Exception("Unable to find Block()");
        }

        internal static int GetBestWorkerIndex(List<Organism> hive, int workerCount)
        {
            var previousError = hive[0].Error;
            var bestWkrIndex = 0;
            for (var i = 0; i < workerCount; ++i)
            {
                if (hive[i].Error < previousError)
                {
                    previousError = hive[i].Error;
                    bestWkrIndex = i;
                }
            }

            return bestWkrIndex;
        }

        internal static int GetBestExplorerIndex(List<Organism> hive, int numWorker)
        {
            var previousError = hive[numWorker].Error;
            var bestExpIndex = numWorker;
            for (var i = numWorker; i < hive.Count; ++i)
            {
                if (hive[i].Error < previousError)
                {
                    previousError = hive[i].Error;
                    bestExpIndex = i;
                }
            }

            return bestExpIndex;
        }

        internal static int GetWorstWorkerIndex(List<Organism> hive, int numWorker)
        {
            var worstWkrIndex = 0; // index of worst worker
            var previousError = hive[0].Error;
            for (var i = 0; i < numWorker; ++i)
            {
                if (hive[i].Error > previousError)
                {
                    previousError = hive[i].Error;
                    worstWkrIndex = i;
                }
            }

            return worstWkrIndex;
        }

        public class TestResult
        {
            public bool isOptimal = false;
            public SolutionStatus resultStatus = SolutionStatus.Invalid;
            public int errorCount = int.MaxValue;
        }

        public static TestResult Test(int[][] matrix)
        {
            // wrapper for the Errors method because?
            var result = new TestResult { errorCount = Errors(matrix) };
            result.isOptimal = result.errorCount == 0;
            result.resultStatus = result.isOptimal ? SolutionStatus.Optimal :
                result.errorCount < 5 ? SolutionStatus.Close : SolutionStatus.Invalid;
            return result;
        }

        /// <summary>
        /// inspects each row and each column, taking count each time a number is missing
        /// skips inspection of blocks; to speed up the testing.
        /// </summary>
        /// <param name="matrix">int[][]</param>
        /// <returns>a number of times a 1-9 is missing in each row and each column</returns>
        public static int Errors(int[][] matrix)
        {
            const int bs = Constants.BoardSize;

            // the score, or number times a row and column entry is missing a 1-9 number. 
            // when none are missing, it's possible to have an optimal solution
            var score = 0;

            // row and column errors store the count found for each number. 

            // test rows errors (loops i then j)
            for (var i = 0; i < bs; ++i) // each row
            {
                // e.g. [0] = count of 1's, [1] = count of 2's ... [9] = count of 9's
                int[] rowDupValueCount = new int[bs];
                // count the number of times each number (1-9) is used in this row
                for (var j = 0; j < bs; ++j) // walk down column of curr row
                {
                    var v = matrix[i][j]; // will get a number between 1 to 9             
                    ++rowDupValueCount[v - 1]; // counts each time a number is found in the row [0-8]
                }

                for (var k = 0; k < bs; ++k) // add up the number of zeros found in each row
                {
                    if (rowDupValueCount[k] == 0)
                        ++score;
                }
            } // each row


            // test columns errors (loops j,i)
            for (var j = 0; j < bs; ++j) // each column
            {
                // e.g. [0] = count of 1's, [1] = count of 2's ... [9] = count of 9's
                int[] colDupValueCount = new int[bs];
                // count the number of times each number (1-9) is used in this column
                for (var i = 0; i < bs; ++i) // walk down 
                {
                    var v = matrix[i][j]; // will get a number between 1 to 9 
                    ++colDupValueCount[v - 1]; // counts each time a number is found in the column [0-8]
                }

                for (var k = 0; k < bs; ++k) // add up the number of zeros found in each column
                {
                    if (colDupValueCount[k] == 0)
                        ++score;
                }
            } // each column

            return score;
        } // Error

        /// <summary>
        /// This creates a new random solution and maps that solution onto provided problem
        /// </summary>
        /// <param name="problem">int[][]</param>
        /// <param name="rand">pass in your random object if you got one</param>
        /// <returns></returns>
        public static int[][] GetRandomMatrix(int[][] problem, Random rand = null)
        {
            Random rnd = rand ?? new Random();
            // make a copy of the current puzzle
            var newRandomMatrix = SudokuTool.DuplicateMatrix(problem);


            for (var block = 0; block < Constants.BoardSize; ++block)
            {
                // fill them with 1 through 9
                var corners = SudokuTool.Corner(block);
                var vals = new List<int>(Constants.BoardSize);
                for (var i = 1; i <= Constants.BoardSize; ++i)
                    vals.Add(i);

                // shuffle each cell with a value from another cell
                for (var k = 0; k < vals.Count; ++k)
                {
                    var ri = rnd.Next(k, vals.Count);
                    var tmp = vals[k];
                    vals[k] = vals[ri];
                    vals[ri] = tmp;
                }

                // remove the random cell values from the known cell values
                var r = corners[0];
                var c = corners[1];
                for (var i = r; i < r + 3; ++i)
                {
                    for (var j = c; j < c + 3; ++j)
                    {
                        var v = problem[i][j];
                        if (v != 0) // a fixed starting number is assumed correct
                            vals.Remove(v);
                    }
                }

                // walk thru block and add the new values to the random solution output
                var ptr = 0; // pointer into List
                for (var i = r; i < r + 3; ++i)
                {
                    for (var j = c; j < c + 3; ++j)
                    {
                        if (newRandomMatrix[i][j] == 0) // not occupied
                        {
                            var v = vals[ptr]; // get value from List
                            newRandomMatrix[i][j] = v;
                            ++ptr; // move to next value in List
                        }
                    }
                }
            } // each block, k

            return newRandomMatrix;
        } // RandomMatrix

        /// <summary>
        /// Randomly swap the values in two cells of a random block of the supplied matrix
        /// </summary>
        /// <param name="problem">the original puzzle </param>
        /// <param name="matrix">the current working solution</param>
        /// <param name="rand">pass in your random object if you got one</param>
        /// <returns></returns>
        public static int[][] EvolveMatrixAsync(int[][] problem, int[][] matrix, Random rand = null)
        {
            Random rnd = rand ?? new Random();
            // pick a random 3x3 block,
            // pick two random cells in block
            // swap values

            var result = SudokuTool.DuplicateMatrix(matrix);

            var block = rnd.Next(0, 9); // [0,8]
            //Console.WriteLine("block = " + block);
            var corners = SudokuTool.Corner(block);
            //Console.WriteLine("corners = " + corners[0] + " " + corners[1]);


            // look for at minimum 2 cells in the neighbor matrix that might be useful in the problem
            var cells = new List<int[]>();
            for (var i = corners[0]; i < corners[0] + 3; ++i)
            {
                for (var j = corners[1]; j < corners[1] + 3; ++j)
                {
                    //Console.WriteLine("problem " + i + " " + j + " = " + problem[i][j]);
                    if (problem[i][j] == 0) // a non-fixed value
                    {
                        cells.Add(new[] { i, j });
                    }
                }
            }

            if (cells.Count < 2)
            {
               // _sb.AppendLine($"Found only {cells.Count} useful cell in the neighbor.");
                throw new Exception("block " + block + " doesn't have two values to swap!");
            }

            // pick two. suppose there are 4 possible cells 0,1,2,3
            var k1 = rnd.Next(0, cells.Count); // 0,1,2,3
            var inc = rnd.Next(1, cells.Count); // 1,2,3
            var k2 = (k1 + inc) % cells.Count;
            //Console.WriteLine("k1 k2 = " + k1 + " " + k2);

            var r1 = cells[k1][0];
            var c1 = cells[k1][1];
            var r2 = cells[k2][0];
            var c2 = cells[k2][1];

            //Console.WriteLine("r1 c1 = " + r1 + " " + c1);
            //Console.WriteLine("r2 c2 = " + r2 + " " + c2);

            var tmp = result[r1][c1];
            result[r1][c1] = result[r2][c2];
            result[r2][c2] = tmp;

            return result;
        } // NeighborMatrix


        /// <summary>
        /// for each block (3x3) in matrix2, there's a 50% chance to be added into matrix1
        /// </summary>
        /// <param name="matrix1">mating partner 1</param>
        /// <param name="matrix2">mating partner 2</param>
        /// <param name="chance">0.50 or some percential double value</param>
        /// <param name="rand">pass in your random object if you got one</param>
        /// <returns>a single matrix with some relationships between both provided</returns>
        public static int[][] MatingResult(int[][] matrix1, int[][] matrix2, double chance = 0.50, Random rand = null)
        {
            Random rnd = rand ?? new Random();
            var result = SudokuTool.DuplicateMatrix(matrix1);
            for (var block = 0; block < 9; ++block)
            {
                if (rnd.NextDouble() < chance)
                {
                    var corners = SudokuTool.Corner(block);
                    for (var i = corners[0]; i < corners[0] + 3; ++i)
                    {
                        for (var j = corners[1]; j < corners[1] + 3; ++j)
                        {
                            result[i][j] = matrix2[i][j];
                        }
                    }
                }
            }
            return result;// Task.FromResult(result);
        }
    }
}