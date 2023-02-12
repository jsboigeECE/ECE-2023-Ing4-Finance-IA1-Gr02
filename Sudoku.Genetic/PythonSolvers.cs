using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;
using Sudoku.Shared;

namespace Sudoku.Genetic
{



	public class GeneticPythonNativeSolver : PythonSolverBase
	{


		public override Shared.SudokuGrid Solve(Shared.SudokuGrid s)
		{

			//using (Py.GIL())
			//{
			// create a Python scope
			using (PyModule scope = Py.CreateScope())
			{
				// convert the Person object to a PyObject
				PyObject pyCells = s.Cells.ToPython();

				// create a Python variable "person"
				scope.Set("instance", pyCells);

				string boardConverterCode = Ressources1.Board_py;
				scope.Exec(boardConverterCode);

				string CroisementConverterCode = Ressources1.CroisementFonction_py;
				scope.Exec(CroisementConverterCode);

				string GenerationGeneConverterCode = Ressources1.GenerationGene_py;
				scope.Exec(GenerationGeneConverterCode);

				string geneticSolverCode = Ressources1.GeneticSolverPython_py;
				scope.Exec(geneticSolverCode);
				var result = scope.Get("r");
				var managedResult = result.As<int[][]>();//.ToJaggedArray();

				//string numpyConverterCode = Ressources1.numpy_converter_py;
				//scope.Exec(numpyConverterCode);

				//string SelfConverterCode = Ressources1.SelfCallSolver1_py;
				//scope.Exec(SelfConverterCode);


				//var convertedResult = managedResult.Select(objList => objList.Select(o => o.As<int>()).ToArray()).ToArray();
				return new Shared.SudokuGrid() { Cells = managedResult };
			}
			//}

		}



		protected override void InitializePythonComponents()
		{
			InstallPipModule("numpy");
			base.InitializePythonComponents();
		}


	}



}