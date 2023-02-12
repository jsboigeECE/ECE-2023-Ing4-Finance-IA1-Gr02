namespace SudokuTools
{
    public class ProblemGenerator : Problem
    {
        private static Problem _problem;

        private static int[][] ProblemRowS
        {
            get { return _problem.Rows; }
            set { _problem.Rows = value; }
        }
        public static Problem GetProblem(ProblemLevels level)
        {
            // has a wrapper implimentation
            _problem = new Problem();

            switch (level)
            {
                case ProblemLevels.Easy:
                    ProblemRowS = SampleProblems.easyProblem();
                    break;
                    
                case ProblemLevels.Medium:
                    // August 2014 MSDN article problem -- difficult
                    // solved using no = 200, me = 5000
                    ProblemRowS = SampleProblems.mediumProblem();
                    break;
                case ProblemLevels.Hard:
                    // http://ieeexplore.ieee.org/stamp/stamp.jsp?tp=&arnumber=5412260
                    // very difficult.
                    // solved using no = 200, me = 9000
                    ProblemRowS = SampleProblems.hardProblem();
                    break;

                case ProblemLevels.Insane:
                    // http://elmo.sbs.arizona.edu/sandiway/sudoku/examples.html
                    // EXTREMELY difficult.
                    // solved quickly using no = 100, me = 19,000
                    ProblemRowS = SampleProblems.insaneProblem();
                    break;

                case ProblemLevels.Impossible:
                    // http://elmo.sbs.arizona.edu/sandiway/sudoku/examples.html
                    // most difficult problem found by Internet search.
                    // solved eventually using no = 100, me = 5,000.
                    // solution when seed = 577 (i.e., 577 attempts ~ 20 min.)
                    ProblemRowS = SampleProblems.impossibleProblem();
                    break;
            }

            return _problem;
        }

    }
}
