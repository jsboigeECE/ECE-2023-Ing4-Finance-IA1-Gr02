using System;
using System.IO;
using System.Xml.Linq;

namespace SudokuTools
{
    public enum PuzzleStatus
    {
        Normal,
        Invalid,
        Complete
    }

    public enum SolutionStatus
    {
        Optimal,
        Close,
        Invalid
    }

    public enum ProblemLevels
    {
        Easy = 3,
        Medium = 5,
        Hard = 8,
        Insane = 13,
        Impossible = 21
    }


    public enum OrganismTypes
    {
        Worker = 0,
        Explorer = 1
    }

    /// <summary>
    /// Description of Constants.
    /// </summary>
    public static class Constants
    {
        public const int BlockSize = 3;
        public const int BoardSize = 9;
        public const string CellErrorMessage = "Choose a number 1 through 9";
    }

    /// <summary>
    /// Description of IPuzzleRepository.
    /// </summary>
    public interface IPuzzleRepository
    {
        XDocument LoadPuzzleSetupXDoc();

        XDocument LoadSavedGameXDoc();

        void SavePuzzleSetupXDoc(XDocument xDoc);

        void SaveSavedGameXDoc(XDocument xDoc);
    }

    /// <summary>
    /// Description of XmlFilePuzzleRepository.
    /// </summary>
    public class XmlFilePuzzleRepository : IPuzzleRepository
    {
        private const string puzzleSetupXmlString = "PuzzleSetup.xml";
        private const string savedGameXmlString = "SavedGame.xml";

        readonly private string puzzleSetupXmlPath = string.Empty;
        readonly private string savedGameXmlPath = string.Empty;

        public XmlFilePuzzleRepository()
        {
            puzzleSetupXmlPath = Path.Combine(AppDomain.CurrentDomain.GetData("APPBASE").ToString(), puzzleSetupXmlString);
            savedGameXmlPath = Path.Combine(AppDomain.CurrentDomain.GetData("APPBASE").ToString(), savedGameXmlString);
        }

        public XDocument LoadPuzzleSetupXDoc()
        {
            return XDocument.Load(puzzleSetupXmlPath);
        }

        public XDocument LoadSavedGameXDoc()
        {
            return XDocument.Load(savedGameXmlPath);
        }

        public void SavePuzzleSetupXDoc(XDocument xDoc)
        {
            xDoc.Save(puzzleSetupXmlPath);
        }

        public void SaveSavedGameXDoc(XDocument xDoc)
        {
            xDoc.Save(savedGameXmlPath);
        }
    }


    public interface IDisplayMatrix {
        void DisplayMatrix(int[][] matrix);
        void SplashView(SudokuTool.TestResult testResults, EvolutionStats stats, int[][] originalMatrix,
            int[][] bestMatrix);
    }
    public class ConsoleDisplayMatrix : IDisplayMatrix
    {
        public void DisplayMatrix(int[][] matrix)
        {
            for (var r = 0; r < 9; ++r)
            {
                if (r == 3 || r == 6) Console.WriteLine("");
                for (var c = 0; c < 9; ++c)
                {
                    if (c == 3 || c == 6) Console.Write(" ");
                    if (matrix[r][c] == 0)
                        Console.Write(" _");
                    else
                        Console.Write(" " + matrix[r][c]);
                }
                Console.WriteLine("");
            }
            Console.WriteLine("\n");
        }

        public void SplashView(SudokuTool.TestResult testResults, EvolutionStats stats, int[][] originalMatrix,
            int[][] bestMatrix)
        {
            Console.Clear();
            DisplayMatrix(originalMatrix);
            Console.WriteLine("\nSearching for Best solution: \n");
           

            if(testResults.isOptimal==false)
            {
                Console.ForegroundColor = (ConsoleColor.Red);
                Console.WriteLine(" Did not find optimal solution \nBest solution found: \n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Optimal solution found. \n");
            }

            DisplayMatrix(bestMatrix);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" -- Statistics -- ");
            Console.ForegroundColor = ConsoleColor.Yellow;

            // show some stats
            Console.WriteLine($@"Final Status: {testResults.resultStatus}
Errors: {testResults.errorCount}
-- Stats -- 
| Epochs (years): {stats.epochsCompleted} 
| Mass Extinctions (resets): {stats.extinctions} 
| Organisms Created: {stats.organismsBorn} | Died of Old Age (>999 years): {stats.diedOfOldAge} 
| Mutation Fails: {stats.mutationFailed} | Evolution Successes: {stats.evolutionWorked} 
| Worst Replaced by Mating Results: {stats.culledCount} | Mating Results Were Best: {stats.bestBabies} 
| Rare Mutation Was Best: {stats.bestMutationsEver} | Random Solution Was Best: {stats.randomWasTheBest} ");

            var mutationvsevolution = ((double)stats.bestMutationsEver / stats.evolutionWorked);
           
            
              double ccRatio = (double)stats.culledCount / stats.organismsBorn;
            
  double meRatio = (double)stats.mutationFailed / stats.evolutionWorked;
           
            double mpbbRatio = (double)stats.bestBabies / stats.matingPairs;

          
            double rbbbRatio = (double)stats.randomWasTheBest / stats.bestMutationsEver;

        }
    }

    public class DebugDisplayMatrix : IDisplayMatrix
    {
        public void DisplayMatrix(int[][] matrix)
        {
            for (var r = 0; r < 9; ++r)
            {
                if (r == 3 || r == 6) System.Diagnostics.Debug.WriteLine("");
                for (var c = 0; c < 9; ++c)
                {
                    if (c == 3 || c == 6) System.Diagnostics.Debug.Write(" ");
                    if (matrix[r][c] == 0)
                        System.Diagnostics.Debug.Write(" _");
                    else
                        System.Diagnostics.Debug.Write(" " + matrix[r][c]);
                }
                System.Diagnostics.Debug.WriteLine("");
            }
            System.Diagnostics.Debug.WriteLine("\n");
        }

        public void SplashView(SudokuTool.TestResult testResults, EvolutionStats stats, int[][] originalMatrix,
            int[][] bestMatrix)
        {
           // System.Diagnostics.Debug.Clear();
            DisplayMatrix(originalMatrix);
            System.Diagnostics.Debug.WriteLine("\nSearching for Best solution: \n");
            //System.Diagnostics.Debug.ForegroundColor = ConsoleColor.Cyan;
            DisplayMatrix(bestMatrix);
           // Console.ForegroundColor = ConsoleColor.Green;
            System.Diagnostics.Debug.WriteLine(" -- Statistics -- ");
           // Console.ForegroundColor = ConsoleColor.Yellow;
            System.Diagnostics.Debug.WriteLine($"Epochs Completed: {stats.epochsCompleted}");
            System.Diagnostics.Debug.WriteLine($"Great Extinctions: {stats.extinctions}");
            System.Diagnostics.Debug.WriteLine($"{stats.diedOfOldAge} organisms aged-out.");
            System.Diagnostics.Debug.WriteLine($"{stats.culledCount} organisms culled.");

        }
    }

    public class EvolutionStats
    {
        public int extinctions;
        public int randomWasTheBest;
        /// <summary>
        /// how many times a mutation happened that was detremental
        /// </summary>
        public int mutationFailed;
        /// <summary>
        /// how many times a mutation or evolution was positive
        /// </summary>
        public int evolutionWorked;
        /// <summary>
        /// how many times evolution or mutation became the new alpha
        /// </summary>
        public int bestMutationsEver;

        public int diedOfOldAge;
        public int epochsCompleted;
        public int bestBabies;
        public int culledCount;
        public int organismsBorn;
        public int matingPairs;
    }

}
