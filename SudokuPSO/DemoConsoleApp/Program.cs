using SudokuTools;
using System;

namespace DemoConsoleApp
{
    class Program
    {
        private static EvolutionSolution _evo;
        private static IDisplayMatrix _disp;
        private static EvolutionStats _stats;
        private static ProblemLevels _thisLevel = ProblemLevels.Medium;
        static void Main(string[] args)
        {

            Console.WriteLine("For the following parameters, enter a value or press ENTER to take defaults.");
            Console.WriteLine(" --------------- ");

            Console.WriteLine("Please select puzzle difficulty");
            Console.WriteLine($"E (easy), M (medium), H (hard), I (insane), U (impossible)  *default {_thisLevel}*");
            string reqDifficulty = Console.ReadLine();
            if (reqDifficulty == null)
            {
                _thisLevel = ProblemLevels.Medium;
            }
            else
            {
                switch (reqDifficulty.ToUpperInvariant())
                {
                    case "E":
                        _thisLevel = ProblemLevels.Easy;
                        break;
                    case "M":
                        _thisLevel = ProblemLevels.Medium;
                        break;
                    case "H":
                        _thisLevel = ProblemLevels.Hard;
                        break;
                    case "I":
                        _thisLevel = ProblemLevels.Insane;
                        break;
                    case "U":
                        _thisLevel = ProblemLevels.Impossible;
                        break;
                    default:
                        _thisLevel = ProblemLevels.Medium;
                        break;
                }
            }
            Console.WriteLine($" --LEVEL SET TO {_thisLevel}-- ");

            int mo = 100;
            int mep = 5000;
            int me = 20;
            Console.WriteLine("  ");
            Console.WriteLine($"Please enter a number of hive organisms to use (50 - 200) *default {mo}*");
            var maxOrganisms = Console.ReadLine();
            if (!int.TryParse(maxOrganisms, out mo) || mo < 50 || mo > 200)
            {
                mo = 100;
                Console.WriteLine($" DEFAULT TO {mo} ");
            }

            Console.WriteLine("  ");
            Console.WriteLine($"Please enter a number Epochs for the organisms to work the problem (2000 - 5000) *default {mep}*");
            var maxEpochs = Console.ReadLine();

            if (!int.TryParse(maxEpochs, out mep) || mep < 2000 || mep > 5000)
            {
                mep = 5000;
                Console.WriteLine($" DEFAULT TO {mep} ");
            }

            Console.WriteLine("  ");
            Console.WriteLine($"Please enter a number Mass Extinctions to allow if all Epochs fail to resolve the puzzle (10 - 30) *default {me}*");
            var maxExtinctions = Console.ReadLine();
            if (!int.TryParse(maxExtinctions, out me) || me < 10 || me > 30)
            {
                me = 20;
                Console.WriteLine($" DEFAULT TO {me} ");
            }

            Console.WriteLine("  ");
            Console.WriteLine($"You have chosen a {_thisLevel} puzzle.");
            Console.WriteLine($"A hive of {mo} Organisms will spawn and work for {mep} Epochs.");
            Console.WriteLine($"If they fail to find an optimal solution, the hive will go extinct and another will spawn. (up to {me} times)");
            Console.WriteLine("   ");

            Console.WriteLine(" ---- Press Enter to Begin ----- ");
            Console.ReadLine();

            long startTime = DateTime.UtcNow.Ticks;
            _disp = new ConsoleDisplayMatrix();
            _stats = new EvolutionStats();
            _evo = new EvolutionSolution(_disp, _stats);


            Problem problem = ProblemGenerator.GetProblem(_thisLevel);
            Console.WriteLine("Starting problem resolution for a " + _thisLevel + " puzzle.");

            int[][] result = _evo.SolveB4ExtinctCount_Async(problem.Rows, mo, mep, me).Result;


            var testResults = SudokuTool.Test(result);
            long stopTime = DateTime.UtcNow.Ticks;


            _disp.SplashView(testResults, _evo.Stats, problem.Rows, result);


            Console.WriteLine();
            Console.ForegroundColor = (ConsoleColor.Cyan);
            Console.WriteLine("This run took: " + TimeSpan.FromTicks(stopTime - startTime).TotalSeconds.ToString("#.##") + " seconds");

            Console.ReadLine();
        }
    }
}
