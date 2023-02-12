using SudokuTools;
using System;
using System.Diagnostics;
using Xunit;

namespace ToolsTests
{
    public class SolvePuzzleTests
    {
        private static Random _rnd = new Random(0);
        private EvolutionSolution _evo;
        private readonly IDisplayMatrix _disp = new DebugDisplayMatrix();
        
        [Theory]
        [InlineData(ProblemLevels.Easy, 100, 5000, 20)]
        [InlineData(ProblemLevels.Medium, 100, 5000, 20)]
        [InlineData(ProblemLevels.Hard, 150, 5000, 20)]
        [InlineData(ProblemLevels.Insane, 150, 5000, 20)]
        [InlineData(ProblemLevels.Insane, 150, 5000, 20)]
        public void CAN_I_SOLVE_VARIOUS_PROBLEMS(ProblemLevels diff, int organisms, int maxEpochs, int maxExtinctions)
        {
            Problem problem = ProblemGenerator.GetProblem(diff);
            _evo = new EvolutionSolution(_disp);
            Debug.WriteLine($"Attempting solution of {diff.ToString()} with {organisms} organisms \n");
            int[][] proposedSolution = _evo.SolveB4ExtinctCount_Async(problem.Rows, organisms, maxEpochs, maxExtinctions).Result;
            var testResult = SudokuTool.Test(proposedSolution);
            var resultStatus = testResult.resultStatus;
            var resultErrors = testResult.errorCount;
            bool optimalResult = testResult.isOptimal;
            var stats = _evo.Stats;
            Debug.WriteLine(resultStatus);
            // show some stats
            System.Diagnostics.Debug.WriteLine($@"Final Status: {testResult.resultStatus}
Errors: {testResult.errorCount}
-- Stats -- 
| Epochs (years): {stats.epochsCompleted} 
| Mass Extinctions (resets): {stats.extinctions} 
| Organisms Created: {stats.organismsBorn} | Died of Old Age (>999 years): {stats.diedOfOldAge} 
| Mutation Fails: {stats.mutationFailed} | Evolution Successes: {stats.evolutionWorked} 
| Worst Replaced by Mating Results: {stats.culledCount} | Mating Results Were Best: {stats.bestBabies} 
| Rare Mutation Was Best: {stats.bestMutationsEver} | Random Solution Was Best: {stats.randomWasTheBest} ");

            var mutationvsevolution = ((double)stats.bestMutationsEver / stats.evolutionWorked);
            System.Diagnostics.Debug.WriteLine(
                $@"Evolution that turned out for the best: {mutationvsevolution:P1}");

            System.Diagnostics.Debug.WriteLine(
                $@"Mating that resulted in KHAN: {stats.bestBabies / stats.matingPairs * 100}%");
            Debug.WriteLine(resultErrors == 0 ? "Optimal solution found. \n" : "Did not find optimal solution \n");
            Debug.WriteLineIf(resultErrors != 0, "\nBest solution found: \n");
            _disp.DisplayMatrix(proposedSolution);
            Assert.True(optimalResult);

        }



    }
}
