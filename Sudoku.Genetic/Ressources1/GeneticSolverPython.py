from typing import Callable
import time
#import Board
#import GenerationGene


class GeneticSolver():
    """
    A class holding all elements of genetic algorithm and
    responsible for running it.
    """
    def __init__(self,
                 board: Board,
                 max_generations: int,
                 population_size: int,
                 elitism: float,
                 drop_out: float,
                 crossover: float,
                 crossover_func: Callable) -> None:
        """
        Initialises GeneticSolver.
        Arguments:
            board {Board} -- Sudoku board.
            max_generations {int} -- maximal number of generations to run.
            population_size {int} -- number of chromosomes.
            elitism {float} -- coefficient indicating proportion of elite
            chromosomes.
            drop_out {float} -- coefficient  of dropped out chromosomes.
            crossover {float} -- probability of crossover.
            crossover_func {Callable[[np.array, np.array, float], np.array]} --
            function that will be use for crossover
        """
        self._board = board
        self._max_generations = max_generations
        self._generation = GenerationGene(population_size,
                                      self._board,
                                      elitism,
                                      drop_out,
                                      crossover,
                                      crossover_func)

    def run(self) -> None:
        """
        Function that executes genetic algorithm and prints progress and final
        result.
        """
        generation_no = 0
        start_time = time.time()
        while generation_no <= self._max_generations:
            fittest = self._generation.evolve()
            if generation_no % 100 == 0:
                print("-------------------------------------")
                print(f"Generation {generation_no} - fittest {fittest[1]}")
                print("-------------------------------------")
            if fittest[1] == 0:
                stop_time = time.time()
                run_time = stop_time - start_time
                print(f"Solution found in {generation_no} generations!")
                print(fittest[0])
                print(f"Elapsed time: {run_time: .3f}s")
                return fittest[0]
            generation_no += 1
        stop_time = time.time()
        run_time = stop_time - start_time
        print(f"Solution couldn't be found in {generation_no - 1} generations")
        print(f"Fitnes of best solution: {fittest[1]}")
        print(fittest[0])
        print(f"Elapsed time: {run_time: .3f}s")
        return fittest[0]


MAX_GENERATIONS = 1000
POPULATION_SIZE = 400
ELITISM_COEFF = 0.1
DROP_OUT_COEFF = 0.5
MUTATION_PROBABILITY = 0.2
CROSSOVER_PROBABILITY = (1.0 - MUTATION_PROBABILITY) / 2.0
        
        
board = Board()
board.load_board(instance)

algorithm = GeneticSolver(board,
                              MAX_GENERATIONS,
                              POPULATION_SIZE,
                              ELITISM_COEFF,
                              DROP_OUT_COEFF,
                              CROSSOVER_PROBABILITY,
                              triplets_crossover)
r=algorithm.run()