namespace SudokuTools
{

    // extended organism, carries information for taging ('GeneMarker') and counting the generation 
    public class Organism : OrganismBase
    {
        public string GeneMarker { get; set; }
        public int Generation = 0;

        public Organism(OrganismTypes type, int[][] m, int error, int age, string tag = "") : base(type, m, error, age)
        {
            GeneMarker = tag;
        }
    }

    /// <summary>
    /// All that's needed for an organism
    /// </summary>
    public class OrganismBase
    {
        public OrganismTypes Type;  // 0 = worker, 1 = explorer
        public int[][] Matrix { get; set; }
        public int Error;
        public int Age;

        public OrganismBase(OrganismTypes type, int[][] m, int error, int age)
        {
            Type = type;
            Matrix = m;
            Error = error;
            Age = age;
        }
    }
}
