using Python.Runtime;
using Sudoku.Shared;

namespace Sudoku.Bayesien;
public class BayesienPythonSolver : PythonSolverBase
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

			// create a Python variable "instance"
			scope.Set("instance", pyCells);


			this.AddNumpyConverterScript(scope);

			string code = Resources.Bayesien_py;
			scope.Exec(code);
			var result = scope.Get("r");
			var managedResult = result.As<int[,]>().ToJaggedArray();
			return new Shared.SudokuGrid() { Cells = managedResult };
		}
		//}
		
	}

	protected override void InitializePythonComponents()
	{
		//InstallPipModule("numpy", "1.21.6", true);
		//InstallPipModule("pymc3", "4.1.4", true);
		InstallPipModule("pymc");
		base.InitializePythonComponents();
	}



}
