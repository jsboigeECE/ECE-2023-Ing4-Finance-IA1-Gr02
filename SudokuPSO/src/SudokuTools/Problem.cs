using System;

namespace SudokuTools
{
    public class Problem
    {
        private int[][] _problem;
        public Problem()
        {
            _problem = new int[Constants.BoardSize][];
            for (int i = 0; i < Constants.BlockSize; i++)
            {
                _problem[i] = new int[Constants.BlockSize];
            }

        }

        public int[][] Rows
        {
            get { return _problem; }
            set { _problem = value; }
        }


    }

    
}
