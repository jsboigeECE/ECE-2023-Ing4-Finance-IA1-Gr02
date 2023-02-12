using GeneticSharp;
using Sudoku.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Genetic
{

	public class GeneticSharpCellsSolver : GeneticSharpPermutationSolver
	{

		protected override ISudokuChromosome GetChromosome(SudokuBoard sBoard)
		{
			return new SudokuCellsChromosome(sBoard);
		}

	}


	



	public class GeneticSharpPermutationSolver : ISudokuSolver
    {
        public SudokuGrid Solve(SudokuGrid s)
        {

			var translatedSudoku = new SudokuBoard(s.Cells.Flatten());


			var populationSize = 200;

			var sudokuChromosome = GetChromosome(translatedSudoku);

			//var maxGenerationNb = 100;
			

			var fitness = new SudokuFitness(translatedSudoku);
			var selection = new EliteSelection();
			var crossover = new UniformCrossover();
			var mutation = new UniformMutation();

			
			var solved = false;
			do
			{
				var population = new Population(populationSize, populationSize, (ChromosomeBase)sudokuChromosome);
				var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
				{
					Termination = new OrTermination(new ITermination[]
				{
					new FitnessThresholdTermination(0),
					new FitnessStagnationTermination(25),
					//new GenerationNumberTermination(maxGenerationNb)
				})
				};
				ga.TaskExecutor = new GeneticSharp.TplTaskExecutor();
				ga.OperatorsStrategy = new TplOperatorsStrategy();

				ga.Start();

				
				solved = ga.Population.BestChromosome.Fitness == 0;

				if (solved)
				{
					var bestIndividual = ((ISudokuChromosome)ga.Population.BestChromosome);
					var bestSolution = bestIndividual.GetSudokus()[0];
					s.Cells = bestSolution.Cells.ToJaggedArray(9);
				}

				populationSize *= 3;

			}
			while(!solved);

			return s;
		}


		protected virtual ISudokuChromosome GetChromosome(SudokuBoard sBoard)
		{
			return new SudokuPermutationsChromosome(sBoard);
		}
		

    }


	/// <summary>
	/// Class that represents a Sudoku, fully or partially completed
	/// Holds a list of 81 int for cells, with 0 for empty cells
	/// Can parse strings and files from most common formats and displays the sudoku in an easy to read format
	/// </summary>
	public class SudokuBoard
	{

		/// <summary>
		/// The empty constructor assumes no mask
		/// </summary>
		public SudokuBoard()
		{
		}

		/// <summary>
		/// constructor that initializes the board with 81 cells
		/// </summary>
		/// <param name="cells"></param>
		public SudokuBoard(IEnumerable<int> cells)
		{
			var enumerable = cells.ToList();
			if (enumerable.Count != 81)
			{
				throw new ArgumentException("Sudoku should have exactly 81 cells", nameof(cells));
			}
			Cells = new List<int>(enumerable);
		}

		// We use a list for easier access to cells,

		/// <summary>
		/// Easy access by row and column number
		/// </summary>
		/// <param name="x">row number (between 0 and 8)</param>
		/// <param name="y">column number (between 0 and 8)</param>
		/// <returns>value of the cell</returns>
		public int GetCell(int x, int y)
		{
			return Cells[(9 * x) + y];
		}

		/// <summary>
		/// Easy setter by row and column number
		/// </summary>
		/// <param name="x">row number (between 0 and 8)</param>
		/// <param name="y">column number (between 0 and 8)</param>
		/// <param name="value">value of the cell to set</param>
		public void SetCell(int x, int y, int value)
		{
			Cells[(9 * x) + y] = value;
		}

		/// <summary>
		/// Sudoku cells are initialized with zeros standing for empty cells
		/// </summary>
		public IList<int> Cells { get; set; } = Enumerable.Repeat(0, 81).ToList();

		/// <summary>
		/// Displays a Sudoku in an easy to read format
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var lineSep = new string('-', 31);

			var output = new StringBuilder();
			output.Append(lineSep);
			output.AppendLine();

			for (int row = 1; row <= 9; row++)
			{
				// we start each line with |
				output.Append("| ");
				for (int column = 1; column <= 9; column++)
				{
					// we obtain the 81-cell index from the 9x9 row/column index
					var value = Cells[(row - 1) * 9 + (column - 1)];
					output.Append(value);
					//we identify boxes with | within lines
					output.Append(column % 3 == 0 ? " | " : "  ");
				}

				output.AppendLine();
				//we identify boxes with - within columns
				if (row % 3 == 0)
				{
					output.Append(lineSep);
				}

				output.AppendLine();
			}

			return output.ToString();
		}

		/// <summary>
		/// Parses a single Sudoku
		/// </summary>
		/// <param name="sudokuAsString">the string representing the sudoku</param>
		/// <returns>the parsed sudoku</returns>
		public static SudokuBoard Parse(string sudokuAsString)
		{
			return ParseMulti(new[] { sudokuAsString })[0];
		}

		/// <summary>
		/// Parses a file with one or several sudokus
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns>the list of parsed Sudokus</returns>
		public static List<SudokuBoard> ParseFile(string fileName)
		{
			return ParseMulti(File.ReadAllLines(fileName));
		}

		/// <summary>
		/// Parses a list of lines into a list of sudoku, accounting for most cases usually encountered
		/// </summary>
		/// <param name="lines">the lines of string to parse</param>
		/// <returns>the list of parsed Sudokus</returns>
		public static List<SudokuBoard> ParseMulti(string[] lines)
		{
			var toReturn = new List<SudokuBoard>();
			var cells = new List<int>(81);
			// we ignore lines not starting with a sudoku character
			foreach (var line in lines.Where(l => l.Length > 0
												 && IsSudokuChar(l[0])))
			{
				foreach (char c in line)
				{
					//we ignore lines not starting with cell chars
					if (IsSudokuChar(c))
					{
						if (char.IsDigit(c))
						{
							// if char is a digit, we add it to a cell
							cells.Add((int)Char.GetNumericValue(c));
						}
						else
						{
							// if char represents an empty cell, we add 0
							cells.Add(0);
						}
					}
					// when 81 cells are entered, we create a sudoku and start collecting cells again.
					if (cells.Count == 81)
					{
						toReturn.Add(new SudokuBoard() { Cells = new List<int>(cells) });
						// we empty the current cell collector to start building a new Sudoku
						cells.Clear();
					}

				}
			}

			return toReturn;
		}


		/// <summary>
		/// identifies characters to be parsed into sudoku cells
		/// </summary>
		/// <param name="c">a character to test</param>
		/// <returns>true if the character is a cell's char</returns>
		private static bool IsSudokuChar(char c)
		{
			return char.IsDigit(c) || c == '.' || c == 'X' || c == '-';
		}

	}




	public interface ISudokuChromosome
	{
		IList<SudokuBoard> GetSudokus();
	}


	/// <summary>
	/// Evaluates a sudoku chromosome for completion by counting duplicates in rows, columns, boxes, and differences from the target mask
	/// </summary>
	public class SudokuFitness : IFitness
	{
		/// <summary>
		/// The target Sudoku Mask to solve.
		/// </summary>
		private readonly SudokuBoard _targetSudokuBoard;

		public SudokuFitness(SudokuBoard targetSudokuBoard)
		{
			_targetSudokuBoard = targetSudokuBoard;
		}

		/// <summary>
		/// Evaluates a chromosome according to the IFitness interface. Simply reroutes to a typed version.
		/// </summary>
		/// <param name="chromosome"></param>
		/// <returns></returns>
		public double Evaluate(IChromosome chromosome)
		{
			return Evaluate((ISudokuChromosome)chromosome);
		}

		/// <summary>
		/// Evaluates a ISudokuChromosome by summing over the fitnesses of its corresponding Sudoku boards.
		/// </summary>
		/// <param name="chromosome">a Chromosome that can build Sudokus</param>
		/// <returns>the chromosome's fitness</returns>
		public double Evaluate(ISudokuChromosome chromosome)
		{
			List<double> scores = new List<double>();

			var sudokus = chromosome.GetSudokus();
			foreach (var sudoku in sudokus)
			{
				scores.Add(Evaluate(sudoku));
			}

			return scores.Sum();
		}

		/// <summary>
		/// Evaluates a single Sudoku board by counting the duplicates in rows, boxes
		/// and the digits differing from the target mask.
		/// </summary>
		/// <param name="testSudokuBoard">the board to evaluate</param>
		/// <returns>the number of mistakes the Sudoku contains.</returns>
		public double Evaluate(SudokuBoard testSudokuBoard)
		{
			// We use a large lambda expression to count duplicates in rows, columns and boxes
			var cells = testSudokuBoard.Cells.Select((c, i) => new { index = i, cell = c }).ToList();
			var toTest = cells.GroupBy(x => x.index / 9).Select(g => g.Select(c => c.cell)) // rows
			  .Concat(cells.GroupBy(x => x.index % 9).Select(g => g.Select(c => c.cell))) //columns
			  .Concat(cells.GroupBy(x => x.index / 27 * 27 + x.index % 9 / 3 * 3).Select(g => g.Select(c => c.cell))); //boxes
			var toReturn = -toTest.Sum(test => test.GroupBy(x => x).Select(g => g.Count() - 1).Sum()); // Summing over duplicates
			toReturn -= cells.Count(x => _targetSudokuBoard.Cells[x.index] > 0 && _targetSudokuBoard.Cells[x.index] != x.cell); // Mask
			return toReturn;
		}



	}


	/// <summary>
	/// This abstract chromosome accounts for the target mask if given, and generates an extended mask with cell domains updated according to original mask
	/// </summary>
	public abstract class SudokuChromosomeBase : ChromosomeBase, ISudokuChromosome
	{

		/// <summary>
		/// The target sudoku board to solve
		/// </summary>
		private readonly SudokuBoard _targetSudokuBoard;

		/// <summary>
		/// The cell domains updated from the initial mask for the board to solve
		/// </summary>
		private Dictionary<int, List<int>> _extendedMask;

		/// <summary>
		/// Constructor that accepts an additional extended mask for quick cloning
		/// </summary>
		/// <param name="targetSudokuBoard">the target sudoku to solve</param>
		/// <param name="extendedMask">The cell domains after initial constraint propagation</param>
		/// <param name="length">The number of genes for the sudoku chromosome</param>
		protected SudokuChromosomeBase(SudokuBoard targetSudokuBoard, Dictionary<int, List<int>> extendedMask, int length) : base(length)
		{
			_targetSudokuBoard = targetSudokuBoard;
			_extendedMask = extendedMask;
			CreateGenes();
		}


		/// <summary>
		/// The target sudoku board to solve
		/// </summary>
		public SudokuBoard TargetSudokuBoard => _targetSudokuBoard;

		/// <summary>
		/// The cell domains updated from the initial mask for the board to solve
		/// </summary>
		public Dictionary<int, List<int>> ExtendedMask
		{
			get
			{
				if (_extendedMask == null)
					BuildExtenedMask();

				return _extendedMask;
			}
		}

		private void BuildExtenedMask()
		{
			// We generate 1 to 9 figures for convenience
			var indices = Enumerable.Range(1, 9).ToList();
			var extendedMask = new Dictionary<int, List<int>>(81);
			if (_targetSudokuBoard != null)
			{
				//If target sudoku mask is provided, we generate an inverted mask with forbidden values by propagating rows, columns and boxes constraints
				var forbiddenMask = new Dictionary<int, List<int>>();
				List<int> targetList = null;
				for (var index = 0; index < _targetSudokuBoard.Cells.Count; index++)
				{
					var targetCell = _targetSudokuBoard.Cells[index];
					if (targetCell != 0)
					{
						//We parallelize going through all 3 constraint neighborhoods
						var row = index / 9;
						var col = index % 9;
						var boxStartIdx = (index / 27 * 27) + (index % 9 / 3 * 3);

						for (int i = 0; i < 9; i++)
						{
							//We go through all 9 cells in the 3 neighborhoods
							var boxtargetIdx = boxStartIdx + (i % 3) + ((i / 3) * 9);
							var targetIndices = new[] { (row * 9) + i, i * 9 + col, boxtargetIdx };
							foreach (var targetIndex in targetIndices)
							{
								if (targetIndex != index)
								{
									if (!forbiddenMask.TryGetValue(targetIndex, out targetList))
									{
										//If the current neighbor cell does not have a forbidden values list, we create it
										targetList = new List<int>();
										forbiddenMask[targetIndex] = targetList;
									}
									if (!targetList.Contains(targetCell))
									{
										// We add current cell value to the neighbor cell forbidden values
										targetList.Add(targetCell);
									}
								}
							}
						}
					}
				}

				// We invert the forbidden values mask to obtain the cell permitted values domains
				for (var index = 0; index < _targetSudokuBoard.Cells.Count; index++)
				{
					extendedMask[index] = indices.Where(i => !forbiddenMask[index].Contains(i)).ToList();
				}

			}
			else
			{
				//If we have no sudoku mask, 1-9 numbers are allowed for all cells
				for (int i = 0; i < 81; i++)
				{
					extendedMask.Add(i, indices);
				}
			}
			_extendedMask = extendedMask;
		}

		public abstract IList<SudokuBoard> GetSudokus();

	}




	/// <summary>
	/// This simple chromosome simply represents each cell by a gene with value between 1 and 9, accounting for the target mask if given
	/// </summary>
	public class SudokuCellsChromosome : SudokuChromosomeBase, ISudokuChromosome
	{


		public SudokuCellsChromosome() : this(null)
		{
		}

		/// <summary>
		/// Basic constructor with target sudoku to solve
		/// </summary>
		/// <param name="targetSudokuBoard">the target sudoku to solve</param>
		public SudokuCellsChromosome(SudokuBoard targetSudokuBoard) : this(targetSudokuBoard, null) { }

		/// <summary>
		/// Constructor with additional precomputed domains for faster cloning
		/// </summary>
		/// <param name="targetSudokuBoard">the target sudoku to solve</param>
		/// <param name="extendedMask">The cell domains after initial constraint propagation</param>
		public SudokuCellsChromosome(SudokuBoard targetSudokuBoard, Dictionary<int, List<int>> extendedMask) : base(targetSudokuBoard, extendedMask, 81)
		{
		}


		public override Gene GenerateGene(int geneIndex)
		{
			//If a target mask exist and has a digit for the cell, we use it.
			if (TargetSudokuBoard != null && TargetSudokuBoard.Cells[geneIndex] != 0)
			{
				return new Gene(TargetSudokuBoard.Cells[geneIndex]);
			}
			// otherwise we use a random digit amongts those permitted.
			var rnd = RandomizationProvider.Current;
			var targetIdx = rnd.GetInt(0, ExtendedMask[geneIndex].Count);
			return new Gene(ExtendedMask[geneIndex][targetIdx]);
		}

		public override IChromosome CreateNew()
		{
			return new SudokuCellsChromosome(TargetSudokuBoard, ExtendedMask);
		}

		/// <summary>
		/// Builds a single Sudoku from the 81 genes
		/// </summary>
		/// <returns>A Sudoku board built from the 81 genes</returns>
		public override IList<SudokuBoard> GetSudokus()
		{
			var sudoku = new SudokuBoard(GetGenes().Select(g => (int)g.Value));
			return new List<SudokuBoard>(new[] { sudoku });
		}
	}


	/// <summary>
	/// This more elaborated chromosome manipulates rows instead of cells, and each of its 9 gene holds an integer for the index of the row's permutation amongst all that respect the target mask.
	/// Permutations are computed once when a new Sudoku is encountered, and stored in a static dictionary for further reference.
	/// </summary>
	public class SudokuPermutationsChromosome : SudokuChromosomeBase, ISudokuChromosome
	{


		/// <summary>
		/// The list of row permutations accounting for the mask
		/// </summary>
		private IList<IList<IList<int>>> _targetRowsPermutations;


		/// <summary>
		/// This constructor assumes no mask
		/// </summary>
		public SudokuPermutationsChromosome() : this(null) { }

		/// <summary>
		/// Constructor with a mask sudoku to solve, assuming a length of 9 genes
		/// </summary>
		/// <param name="targetSudokuBoard">the target sudoku to solve</param>
		public SudokuPermutationsChromosome(SudokuBoard targetSudokuBoard) : this(targetSudokuBoard, 9) { }

		/// <summary>
		/// Constructor with a mask and a number of genes
		/// </summary>
		/// <param name="targetSudokuBoard">the target sudoku to solve</param>
		/// <param name="length">the number of genes</param>
		public SudokuPermutationsChromosome(SudokuBoard targetSudokuBoard, int length) : this(targetSudokuBoard, null, length) { }

		/// <param name="targetSudokuBoard">the target sudoku to solve</param>
		/// <param name="extendedMask">The cell domains after initial constraint propagation</param>
		/// <param name="length">The number of genes for the sudoku chromosome</param>
		public SudokuPermutationsChromosome(SudokuBoard targetSudokuBoard, Dictionary<int, List<int>> extendedMask, int length) : base(targetSudokuBoard, extendedMask, length) { }


		/// <summary>
		/// generates a chromosome gene from its index containing a random row permutation
		/// amongst those respecting the target mask. 
		/// </summary>
		/// <param name="geneIndex">the index for the gene</param>
		/// <returns>a gene generated for the index</returns>
		public override Gene GenerateGene(int geneIndex)
		{

			var rnd = RandomizationProvider.Current;
			//we randomize amongst the permutations that account for the target mask.
			var permIdx = rnd.GetInt(0, TargetRowsPermutations[geneIndex].Count);
			return new Gene(permIdx);
		}

		public override IChromosome CreateNew()
		{
			var toReturn = new SudokuPermutationsChromosome(TargetSudokuBoard, ExtendedMask, Length);
			return toReturn;
		}


		/// <summary>
		/// builds a single Sudoku from the given row permutation genes
		/// </summary>
		/// <returns>a list with the single Sudoku built from the genes</returns>
		public override IList<SudokuBoard> GetSudokus()
		{
			var listInt = new List<int>(81);
			for (int i = 0; i < 9; i++)
			{
				var perm = GetPermutation(i);
				listInt.AddRange(perm);
			}
			var sudoku = new SudokuBoard(listInt);
			return new List<SudokuBoard>(new[] { sudoku });
		}



		/// <summary>
		/// Gets the permutation to apply from the index of the row concerned
		/// </summary>
		/// <param name="rowIndex">the index of the row to permute</param>
		/// <returns>the index of the permutation to apply</returns>
		protected virtual List<int> GetPermutation(int rowIndex)
		{
			int permIDx = GetPermutationIndex(rowIndex);
			return GetPermutation(rowIndex, permIDx);
		}


		/// <summary>
		/// Gets the permutation for a row and given a permutation index, according to the corresponding row's available permutations
		/// </summary>
		/// <param name="rowIndex">the row index for the permutation</param>
		/// <param name="permIDx">the permutation index to retrieve</param>
		/// <returns></returns>
		protected virtual List<int> GetPermutation(int rowIndex, int permIDx)
		{

			// we use a modulo operator in case the gene was swapped:
			// It may contain a number higher than the number of available permutations. 
			var perm = TargetRowsPermutations[rowIndex][permIDx % TargetRowsPermutations[rowIndex].Count].ToList();
			return perm;
		}



		/// <summary>
		/// Gets the permutation to apply from the index of the row concerned
		/// </summary>
		/// <param name="rowIndex">the index of the row to permute</param>
		/// <returns>the index of the permutation to apply</returns>
		protected virtual int GetPermutationIndex(int rowIndex)
		{
			return (int)GetGene(rowIndex).Value;
		}


		/// <summary>
		/// This method computes for each row the list of digit permutations that respect the target mask, that is the list of valid rows discarding columns and boxes
		/// </summary>
		/// <param name="sudokuBoard">the target sudoku to account for</param>
		/// <returns>the list of permutations available</returns>
		public IList<IList<IList<int>>> GetRowsPermutations()
		{
			if (TargetSudokuBoard == null)
			{
				return UnfilteredPermutations;
			}

			// we store permutations to compute them once only for each target Sudoku
			if (!_rowsPermutations.TryGetValue(TargetSudokuBoard, out var toReturn))
			{
				// Since this is a static member we use a lock to prevent parallelism.
				// This should be computed once only.
				lock (_rowsPermutations)
				{
					if (!_rowsPermutations.TryGetValue(TargetSudokuBoard, out toReturn))
					{
						toReturn = GetRowsPermutationsUncached();
						_rowsPermutations[TargetSudokuBoard] = toReturn;
					}
				}
			}
			return toReturn;
		}

		private IList<IList<IList<int>>> GetRowsPermutationsUncached()
		{
			var toReturn = new List<IList<IList<int>>>(9);
			for (int i = 0; i < 9; i++)
			{
				var tempList = new List<IList<int>>();
				foreach (var perm in AllPermutations)
				{
					// Permutation should be compatible with current row extended mask domains
					if (Range9.All(j => ExtendedMask[i * 9 + j].Contains(perm[j])))
					{
						tempList.Add(perm);
					}
				}
				toReturn.Add(tempList);
			}

			return toReturn;
		}



		/// <summary>
		/// Produces 9 copies of the complete list of permutations
		/// </summary>
		public static IList<IList<IList<int>>> UnfilteredPermutations
		{
			get
			{
				if (!_unfilteredPermutations.Any())
				{
					lock (_unfilteredPermutations)
					{
						if (!_unfilteredPermutations.Any())
						{
							_unfilteredPermutations = Range9.Select(i => AllPermutations).ToList();
						}
					}
				}
				return _unfilteredPermutations;
			}
		}

		/// <summary>
		/// Builds the complete list permutations for {1,2,3,4,5,6,7,8,9}
		/// </summary>
		public static IList<IList<int>> AllPermutations
		{
			get
			{
				if (!_allPermutations.Any())
				{
					lock (_allPermutations)
					{
						if (!_allPermutations.Any())
						{
							_allPermutations = GetPermutations(Enumerable.Range(1, 9), 9);
						}
					}
				}
				return _allPermutations;
			}
		}

		/// <summary>
		/// The list of row permutations accounting for the mask
		/// </summary>
		public IList<IList<IList<int>>> TargetRowsPermutations
		{
			get
			{
				if (_targetRowsPermutations == null)
				{
					_targetRowsPermutations = GetRowsPermutations();
				}
				return _targetRowsPermutations;
			}
		}

		/// <summary>
		/// The list of compatible permutations for a given Sudoku is stored in a static member for fast retrieval
		/// </summary>
		private static readonly IDictionary<SudokuBoard, IList<IList<IList<int>>>> _rowsPermutations = new Dictionary<SudokuBoard, IList<IList<IList<int>>>>();

		/// <summary>
		/// The list of row indexes is used many times and thus stored for quicker access.
		/// </summary>
		private static readonly List<int> Range9 = Enumerable.Range(0, 9).ToList();

		/// <summary>
		/// The complete list of unfiltered permutations is stored for quicker access
		/// </summary>
		private static IList<IList<int>> _allPermutations = (IList<IList<int>>)new List<IList<int>>();
		private static IList<IList<IList<int>>> _unfilteredPermutations = (IList<IList<IList<int>>>)new List<IList<IList<int>>>();

		/// <summary>
		/// Computes all possible permutation for a given set
		/// </summary>
		/// <typeparam name="T">the type of elements the set contains</typeparam>
		/// <param name="list">the list of elements to use in permutations</param>
		/// <param name="length">the size of the resulting list with permuted elements</param>
		/// <returns>a list of all permutations for given size as lists of elements.</returns>
		static IList<IList<T>> GetPermutations<T>(IEnumerable<T> list, int length)
		{
			if (length == 1) return list.Select(t => (IList<T>)(new T[] { t }.ToList())).ToList();

			var enumeratedList = list.ToList();
			return (IList<IList<T>>)GetPermutations(enumeratedList, length - 1)
			  .SelectMany(t => enumeratedList.Where(e => !t.Contains(e)),
				(t1, t2) => (IList<T>)t1.Concat(new T[] { t2 }).ToList()).ToList();
		}



	}


}