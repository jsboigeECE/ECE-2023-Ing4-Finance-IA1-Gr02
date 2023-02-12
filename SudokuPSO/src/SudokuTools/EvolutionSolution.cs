using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SudokuTools
{
    public class EvolutionSolution
    {
        private static Random _rnd = new Random();
        private static StringBuilder _sb = new StringBuilder();
        private static int[][] OriginalMatrix;
        private readonly EvolutionStats _stats;
        private readonly IDisplayMatrix displayVector;

        /// <summary>
        /// pass in a IDisplayMatrix if you want the tests to output some information about the solution.
        /// </summary>
        /// <param name="disp">IDisplayMatrix instance to be used in output (write your own for logging, consoles, debugging, etc.)</param>
        public EvolutionSolution(IDisplayMatrix disp, EvolutionStats stats = null)
        {
            displayVector = disp;
            _stats = stats ?? new EvolutionStats();
        }

        public EvolutionSolution(EvolutionStats stats)
        {
            _stats = stats ?? new EvolutionStats();
        }

        public EvolutionSolution()
        {
            _stats = new EvolutionStats();
        }

        public EvolutionStats Stats
        {
            get { return _stats; }
        }

        //  private List<Organism> Hive;
        //  private OrganismBase TheBest;

        /// <summary>
        /// Attempts find an optimal solution to a sudoku puzzle with the a hive of organisms
        /// The organisms each have a total number of allowed Epochs (generations) to find the optimal solution. 
        /// If after the max epochs is reached without an optimal solution, they are culled and another hive is started within the maximum number of specified extinctions
        /// NOTE: Combinatorial evolution doesn't guarantee and optimal solution will be found.
        /// </summary>
        /// <param name="problem">int[][]</param>
        /// <param name="numOrganisms">how many virtual organism are in the hive at a given time, each representing a possible solution</param>
        /// <param name="maxEpochs">number of iterations (generations) each organism can run through to find the optimal solution</param>
        /// <param name="maxExtinctions">if an optimal solution isn't found after the maxEpochs, all organisms are killed and the cycle is restarted if not more than maxExtinctions.</param>
        /// <returns>The best solution (int[][]) found within the specified time.</returns>
        public async Task<int[][]> SolveB4ExtinctCount_Async(int[][] problem, int numOrganisms, int maxEpochs, int maxExtinctions)
        {
            OriginalMatrix = SudokuTool.DuplicateMatrix(problem);
            // wrapper for SolveEvo()
            var err = int.MaxValue;
            var seed = 0;
            int[][] bestSolution = null;
            var extinctions = 0;
            int epochs = 0;
            // generate a new hive of organisms with a number of epochs to try for an error-free solution or until we've killed them off too often
            while (err != 0 && extinctions < maxExtinctions)
            {
                _rnd = new Random(seed);
                bestSolution = await SolveAsync(problem, numOrganisms, maxEpochs).ConfigureAwait(true);
                err = SudokuTool.Errors(bestSolution);

                extinctions++; // keep track of the attempts
                Debug.WriteLine($"{err} errors prior to {extinctions} extinction level events");
                ++seed;// increment the seed
            }

            _stats.extinctions = extinctions;
            return bestSolution;
        }



        /// <summary>
        /// Core engine for finding a solution. Running this is going to spawn a hive of organisms (some percentage of workers, the rest explorers)
        /// 
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="numOrganisms"></param>
        /// <param name="maxEpochs"></param>
        /// <returns></returns>
        private async Task<int[][]> SolveAsync(int[][] problem, int numOrganisms, int maxEpochs)
        {
            // start fresh
            var Hive = new List<Organism>(new Organism[numOrganisms]);

            // the best organisms in each epoch will be stored here
            var TheBest = new OrganismBase(0, null, Int32.MaxValue, 0);

            // initialize combinatorial Organisms with 90% of them workers and the rest explorers
            // Explorers always get a new random matrix to test against.
            var numWorker = (int)(numOrganisms * 0.90);
            var numExplorer = numOrganisms - numWorker;


            // fill the hive with new organisms having a random puzzle and known error count and also if it was the best yet
            for (var i = 0; i < numOrganisms; ++i)
            {
                var orgType = i < numWorker ? OrganismTypes.Worker : OrganismTypes.Explorer;

                // get a new random puzzle and count the errors
                int[][] rndMatrix = await Task.FromResult(SudokuTool.GetRandomMatrix(problem, _rnd));
                var currentErrors = await Task.FromResult(SudokuTool.Errors(rndMatrix));

                // create a new organism, give it the random puzzle and errors
                Hive[i] = new Organism(orgType, rndMatrix, currentErrors, 0);
                _stats.organismsBorn++;

                // track the best so far puzzle and copy it to TheBest
                if (currentErrors < TheBest.Error)
                {
                    TheBest.Error = currentErrors;
                    TheBest.Matrix = await Task.FromResult(SudokuTool.DuplicateMatrix(rndMatrix));
                    _stats.randomWasTheBest++; // this should average out at some point?
                    // Splash();
                }
            }

            // main loop - start the generations (epochs)
            var epoch = 0;
            while (epoch < maxEpochs)
            {
                // keep a little log for every 1000 generations
                if (epoch % 1000 == 0)
                {
                    Debug.WriteLine("epoch = " + epoch);
                    Debug.WriteLine("best error = " + TheBest.Error);
                }

                // if we somehow have no errors (optimal solution!) then just stop now.
                if (TheBest.Error == 0) // solution found
                    break;

                // process each organism and give explorers random solutions and evolve workers
                // (with for loop)...why not foreach? because if it ages out we want to put a new organism in place
                for (var i = 0; i < numOrganisms; ++i)
                {
                    // we treat Explorers differently than Workers. Explorers always get a new random matrix to test against. Workers evolve, mutate, age +/- or die of old age
                    switch (Hive[i].Type)
                    {
                        case OrganismTypes.Explorer:
                            // give it a new random matrix
                            int[][] rndMatrix = await Task.FromResult(SudokuTool.GetRandomMatrix(problem, _rnd));
                            Hive[i].Matrix = await Task.FromResult(SudokuTool.DuplicateMatrix(rndMatrix));
                            Hive[i].Error = await Task.FromResult(SudokuTool.Errors(rndMatrix));

                            if (Hive[i].Error < TheBest.Error)
                            {
                                TheBest.Error = Hive[i].Error;
                                TheBest.Matrix = SudokuTool.DuplicateMatrix(rndMatrix);
                                _stats.randomWasTheBest++;
                                // Splash();
                            }
                            break;
                        case OrganismTypes.Worker:

                            // mutate the matrix, pick a random block and pick two random cells and swap them.
                            int[][] evolving = await Task.FromResult(SudokuTool.EvolveMatrixAsync(problem, Hive[i].Matrix, _rnd));
                            var evolutionErrors = await Task.FromResult(SudokuTool.Errors(evolving));

                            var probability = _rnd.NextDouble();
                            var rareMutation = probability < 0.001;
                            bool evoWorked = evolutionErrors < Hive[i].Error;

                            // if the evolution is better (natural selection) or if this is a rareMutation, take it
                            if (evoWorked || rareMutation) // better, or a mistake
                            {
                                // positive result to evolution provides refreshed genepool and resets age
                                if (evoWorked)
                                {
                                    Hive[i].Age = 0;
                                    _stats.evolutionWorked++;
                                }

                                // if you didn't get the better end of the deal
                                if (rareMutation && !evoWorked)
                                    _stats.mutationFailed++;

                                Hive[i].Matrix = await Task.FromResult(SudokuTool.DuplicateMatrix(evolving)); // by value
                                Hive[i].Error = evolutionErrors;

                                if (Hive[i].Error < TheBest.Error)
                                {
                                    TheBest.Error = Hive[i].Error;
                                    TheBest.Matrix = await Task.FromResult(SudokuTool.DuplicateMatrix(evolving));
                                    _stats.bestMutationsEver++;
                                    // Splash();
                                }
                            }
                            else // evolved code is not better and there was no chance for forced mutation
                            {
                                // age this organism
                                Hive[i].Age++;

                                if (Hive[i].Age > 1000) // die - if so, get a random to replace
                                {
                                    _stats.diedOfOldAge++;

                                    // create a new replacement for the hive.
                                    var m = await Task.FromResult(SudokuTool.GetRandomMatrix(problem, _rnd));
                                    var me = await Task.FromResult(SudokuTool.Errors(m));
                                    Hive[i] = new Organism(0, m, me, 0);
                                    _stats.organismsBorn++;
                                }
                            }
                            break;

                    }

                } // each organism


                // find the index of the best worker
                var bestWkrIndex = await Task.FromResult(SudokuTool.GetBestWorkerIndex(Hive, numWorker));
                var bestWorker = Hive[bestWkrIndex];

                // find the index of the best explorer
                var bestExpIndex = await Task.FromResult(SudokuTool.GetBestExplorerIndex(Hive, numWorker));
                var bestExplorer = Hive[bestExpIndex];

                // find the index of the worst worker
                var worstWkrIndex = await Task.FromResult(SudokuTool.GetWorstWorkerIndex(Hive, numWorker));


                // have a  50/50 chance for each block in 2nd organism will be blended into 1st organism 
                var babyOrg = await Task.FromResult(SudokuTool.MatingResult(bestWorker.Matrix, bestExplorer.Matrix, 0.50, _rnd));
                _stats.matingPairs++;
                var babyErr = await Task.FromResult(SudokuTool.Errors(babyOrg));
                var genNext = new Organism(OrganismTypes.Worker, babyOrg, babyErr, 0)
                {
                    // generations are only incremented if one of the parents have a GeneMarker
                    Generation = incrementGeneration(bestWorker, bestExplorer),
                    // GeneMarker = "GenX" // might use this if you happened to want to see details later.
                }; _stats.organismsBorn++;

                // replace the worst worker with the next generation of worker
                Hive[worstWkrIndex] = genNext;
                _stats.culledCount++;

                if (babyErr < TheBest.Error)
                {
                    TheBest.Error = babyErr;
                    TheBest.Matrix = await Task.FromResult(SudokuTool.DuplicateMatrix(babyOrg));
                    _stats.bestBabies++;
                    // Splash();
                }
                ++epoch;
                _stats.epochsCompleted++;
            } // while


            return TheBest.Matrix;
        } // SolveEvo









        /// <summary>
        /// Just use this to get the next generation number by adding 1 to the latest generation's gen number.... only if either orgs have a GeneMarker
        /// </summary>
        /// <param name="workerOrganism"></param>
        /// <param name="explorerOrganism"></param>
        /// <returns></returns>
        private static int incrementGeneration(Organism workerOrganism, Organism explorerOrganism)
        {
            if ((workerOrganism.GeneMarker != string.Empty || explorerOrganism.GeneMarker != string.Empty))
            {
                var highest = workerOrganism.Generation < explorerOrganism.Generation
                    ? explorerOrganism.Generation : workerOrganism.Generation;

                return highest + 1;
            }
            return 1;
        }



    }
}
