namespace SudokuTools
{
    public static class SampleProblems
    {
        /// ProblemLevels.Easy - from google sheets add-in @ https://chrome.google.com/webstore/detail/sudoku-sheets/eagolleeideiojopioiiaadjkneafmen?hl=en
        /// Solve with 100 organisms, 5000 maxEpochs after 00:25
        public static int[][] easyProblem()
        {

            int[][] m = SudokuTool.prepMatrix();
            m[0] = new int[] { 0, 0, 6, 7, 0, 3, 0, 2, 4 };
            m[1] = new int[] { 1, 0, 0, 2, 0, 0, 7, 0, 0 };
            m[2] = new int[] { 5, 0, 0, 0, 0, 1, 0, 0, 0 };

            m[3] = new int[] { 0, 0, 8, 0, 2, 0, 3, 4, 6 };
            m[4] = new int[] { 0, 0, 0, 0, 0, 8, 0, 0, 0 };
            m[5] = new int[] { 0, 0, 3, 0, 0, 0, 0, 0, 8 };

            m[6] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            m[7] = new int[] { 3, 0, 0, 0, 0, 4, 0, 0, 9 };
            m[8] = new int[] { 0, 9, 0, 6, 0, 0, 0, 8, 2 };
            return m;
        }



        /// <summary>
        ///  ProblemLevels.Medium -  
        /// from google sheets add-in @ https://chrome.google.com/webstore/detail/sudoku-sheets/eagolleeideiojopioiiaadjkneafmen?hl=en
        /// Solve with 50 organisms, 5000 maxEpochs after 00:41
        /// Solve with 100 organisms, 5000 maxEpochs after 00:25
        /// </summary>
        /// <returns></returns>
        public static int[][] mediumProblem()
        {
            int[][] m = SudokuTool.prepMatrix();
            m[0] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1 };
            m[1] = new int[] { 0, 8, 6, 3, 0, 0, 0, 0, 2 };
            m[2] = new int[] { 0, 0, 0, 2, 1, 0, 0, 4, 0 };

            m[3] = new int[] { 0, 0, 9, 0, 0, 6, 7, 5, 0 };
            m[4] = new int[] { 4, 0, 0, 0, 5, 0, 1, 0, 0 };
            m[5] = new int[] { 0, 0, 0, 0, 0, 9, 0, 0, 3 };

            m[6] = new int[] { 0, 0, 0, 0, 9, 0, 0, 0, 0 };
            m[7] = new int[] { 8, 0, 5, 0, 0, 2, 0, 0, 0 };
            m[8] = new int[] { 0, 0, 0, 0, 0, 0, 0, 6, 9 };
            return m;
        }
        /// <summary>
        ///  ProblemLevels.Hard -  
        /// from google sheets add-in @ https://chrome.google.com/webstore/detail/sudoku-sheets/eagolleeideiojopioiiaadjkneafmen?hl=en
        /// Solved with 150 organisms, 5000 maxEpochs after 00:12
        /// </summary>
        /// <returns></returns>
        public static int[][] hardProblem()
        {
            int[][] m = SudokuTool.prepMatrix();
            m[0] = new int[] { 5, 0, 0, 0, 0, 0, 0, 0, 3 };
            m[1] = new int[] { 0, 0, 0, 0, 4, 0, 0, 2, 1 };
            m[2] = new int[] { 0, 0, 1, 0, 7, 9, 0, 0, 0 };

            m[3] = new int[] { 6, 0, 5, 4, 0, 0, 0, 0, 0 };
            m[4] = new int[] { 0, 2, 0, 0, 6, 0, 0, 0, 0 };
            m[5] = new int[] { 0, 0, 3, 0, 0, 8, 6, 0, 0 };

            m[6] = new int[] { 0, 0, 0, 3, 0, 0, 0, 0, 8 };
            m[7] = new int[] { 0, 4, 0, 0, 9, 0, 0, 0, 0 };
            m[8] = new int[] { 1, 0, 0, 0, 0, 6, 7, 0, 0 };
            return m;
        }

        /// <summary>  
        /// /// ProblemLevels.Insane -
        /// from google sheets add-in @ https://chrome.google.com/webstore/detail/sudoku-sheets/eagolleeideiojopioiiaadjkneafmen?hl=en
        /// solved quickly using numberorganims = 100, maxepochs = 19,000
        /// </summary>
        /// <returns></returns>
        public static int[][] insaneProblem()
        {
            int[][] m = SudokuTool.prepMatrix();
            m[0] = new int[] { 0, 0, 0, 5, 0, 0, 0, 0, 0 };
            m[1] = new int[] { 5, 7, 0, 0, 4, 0, 9, 0, 8 };
            m[2] = new int[] { 0, 0, 8, 6, 0, 0, 0, 0, 0 };

            m[3] = new int[] { 0, 5, 7, 0, 0, 0, 0, 3, 0 };
            m[4] = new int[] { 3, 9, 0, 0, 0, 0, 0, 4, 0 };
            m[5] = new int[] { 0, 0, 1, 0, 7, 0, 5, 0, 0 };

            m[6] = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 6 };
            m[7] = new int[] { 0, 1, 4, 0, 0, 0, 3, 0, 0 };
            m[8] = new int[] { 0, 0, 0, 8, 0, 0, 0, 1, 5 };
            return m;
        }

        /// <summary>
        /// from http://elmo.sbs.arizona.edu/sandiway/sudoku/examples.html
        /// most difficult problem found by Internet search.
        /// solved eventually using numberorganims = 25, maxepochs = 5,000.
        /// solution when seed = 577 (i.e., 577 attempts ~ 20 min.)
        /// </summary>
        /// <returns></returns>
        public static int[][] impossibleProblem()
        {
            int[][] m = SudokuTool.prepMatrix();
            m[0] = new int[] { 0, 2, 0, 0, 0, 0, 0, 0, 0 };
            m[1] = new int[] { 0, 0, 0, 6, 0, 0, 0, 0, 3 };
            m[2] = new int[] { 0, 7, 4, 0, 8, 0, 0, 0, 0 };

            m[3] = new int[] { 0, 0, 0, 0, 0, 3, 0, 0, 2 };
            m[4] = new int[] { 0, 8, 0, 0, 4, 0, 0, 1, 0 };
            m[5] = new int[] { 6, 0, 0, 5, 0, 0, 0, 0, 0 };

            m[6] = new int[] { 0, 0, 0, 0, 1, 0, 7, 8, 0 };
            m[7] = new int[] { 5, 0, 0, 0, 0, 9, 0, 0, 0 };
            m[8] = new int[] { 0, 0, 0, 0, 0, 0, 0, 4, 0 };
            return m;
        }
    }
}
