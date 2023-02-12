from __future__ import annotations
from typing import Callable, List
import numpy as np
from random import shuffle, choices, random, randint
#import Board


class GenerationGene():
    """
    Class holding population of chromosomes and responsible for executing
    genetic algorithm's main loop.
    """
    SHUFFLE_NO = 3
    DEFAULT_FITNESS = 10000

    def __init__(self,
                 size: int,
                 board: Board,
                 elite: float,
                 drop_out: float,
                 crossover: float,
                 crossover_func: Callable) -> None:
        """
        Initialises Generation class.
        Arguments:
            size {int} -- size of population.
            board {Board} -- sudoku board.
            elite {float} -- elitism coefficient.
            drop_out {float} -- drop out coefficient.
            crossover {float} -- crossover coefficient.
            crossover_func {Callable[[np.array, np.array, float], np.array]} --
            function that will be use for crossover
        """
        self._elite = elite
        self._drop_out = drop_out
        self._crossover = crossover
        self._board = board
        self._size = size
        self._crossover_func = crossover_func
        self._population = [self.generate_chromosome()
                            for _ in range(self._size)]

    def _generate_row(self, row_no: int) -> List[int]:
        """
        Generates random row by shuffling sample number of times defined by
        constant SHUFFLE_NO.
        Returns:
            List[int] -- random row with correct fixed values.
        """
        sample = [1, 2, 3, 4, 5, 6, 7, 8, 9]
        known_values = self._board.board[row_no]
        for _ in range(GenerationGene.SHUFFLE_NO):
            shuffle(sample)
        return [known_values[i] if i > 0 else sample[i] for i in range(9)]

    def generate_chromosome(self) -> List:
        """
        Generates random chromosome.
        Returns:
            List[np.array, default_fitness] -- Randomly generated chromosome
            with default fitness assigned.
        """
        return [np.array([self._generate_row(row_no) for row_no in range(9)],
                         dtype=np.int8), GenerationGene.DEFAULT_FITNESS]

    def compute_chromosome_fitness(self, sample_no: int) -> None:
        """
        Function for computing chromosome's fittness.
        The fitter the chromosome is the lower the score.
        Arguments:
            sample_no {int} -- number of chromosome which fitness should be
            calculated
        """
        def calc_penalty(sum_in_nine: int) -> int:
            return abs(45 - sum_in_nine)

        def calc_duplicates(array) -> int:
            return 9 - np.unique(array).shape[0]

        incorrect_fixed_value_penalty = 90
        duplicate_penalty = 40
        fitness = 0
        # ? This reshape is needed???
        sample = np.reshape(self._population[sample_no][0], (9, 9))
        v_calc_penalty = np.vectorize(calc_penalty)
        # incorrect sum in column
        fitness = np.sum(v_calc_penalty(np.sum(sample, axis=0)))
        # incorrect sum in row
        fitness += np.sum(v_calc_penalty(np.sum(sample, axis=1)))
        # incorrect sum in square
        for row in range(0, 9, 3):
            for col in range(0, 9, 3):
                fitness += abs(45 - np.sum(sample[row:row+3, col:col+3]))
        # TODO it will be removed in the future.
        # Right now it's needed to compare with previous runs.
        # incorrect known item
        for row, line in enumerate(self._board.board):
            for col, val in enumerate(line):
                if val != 0 and sample[row, col] != val:
                    fitness += incorrect_fixed_value_penalty
        # duplicated values
        for row in range(9):
            fitness += calc_duplicates(sample[row, :]) * duplicate_penalty
        for col in range(9):
            fitness += calc_duplicates(sample[:, col]) * duplicate_penalty
        for row in range(0, 9, 3):
            for col in range(0, 9, 3):
                fitness += calc_duplicates(sample[row:row+3, col:col+3].
                                           flatten()) * duplicate_penalty
        self._population[sample_no][1] = fitness

    def get_fittest(self) -> List:
        """
        Returns fittest element.
        Returns:
            List[np.array, int] -- fittest chromosome with its fitness.
        """
        return self._population[0]

    def _compute_population_fitness(self) -> List:
        """
        Computes fitness of whole population using function compute_chromosome
        fitness and sorts them in descending order.
        Returns:
            List[np.array, int] -- fittest chromosome with its fitness.
        """
        def use_fitness(elem: List) -> int:
            return elem[1]

        for chromosome_no in range(self._size):
            self.compute_chromosome_fitness(chromosome_no)
        self._population.sort(key=use_fitness)
        return self.get_fittest()

    def _get_elite(self) -> List:
        """
        Returns list of most elite chromosomes from population. Number of elite
        chromosomes is defined by variable _elite.
        Returns:
            List[[np.array, default_fitness]] -- List of most elite chromosomes
            with default fitness assigned.
        """
        elite = []
        for chromosome_no in range(int(self._size * self._elite)):
            elite.append([self._population[chromosome_no][0],
                          GenerationGene.DEFAULT_FITNESS])
        return elite

    def _select_fittest(self) -> List:
        """
        Selects 2 random parents from the fittest chromosomes in population.
        Number of fittest chromosomes is defined by variable _drop_out.
        Returns:
            List[np.array] -- List of parents.
        """
        chromosome_count = int(self._size * self._drop_out)
        parents_count = 2
        fittest = choices(self._population[0:chromosome_count],
                          k=parents_count)
        return [item[0] for item in fittest]

    def _create_child(self, function, parent1, parent2,
                      crossover_probability: float):
        """
        Creates child using crossover or mutation (it depends on probability)
        Arguments:
            function {Callable[[np.array, np.array, float], np.array]} --
            function for doing crossover of chromosomes.
            parent1 {np.array} -- first parent.
            parent2 {np.array} -- second parent.
            crossover_probability {float} -- probability level for chosing
            either parent1 or parent2.
        Returns:
            np.array -- newly created child.
        """
        # TODO temporary solution to check if it works
        child = np.empty((9, 9), dtype=np.int8)
        for row in range(9):
            random_number = random()
            if random_number <= crossover_probability:
                child[row, :] = parent1[row, :].copy()
            elif random_number <= (2.0 * crossover_probability):
                child[row, :] = parent2[row, :].copy()
            else:
                child[row, :] = np.array(self._generate_row(row),
                                         dtype=np.int8).copy()
        return child
        # return function(parent1, parent2, crossover_probability)

    def evolve(self) -> List:
        """
        Function for executing main loop of genetic algorithm which is:
        1. calculation of population fitnes,
        2. chose elite chromosomes,
        3. select fittest parents,
        4. create new children using crossover or mutation.
        Returns:
            List[np.array, int] -- fittest chromosome with its fitness.
        """
        new_population = []
        fittest_chromosome = self._compute_population_fitness()
        # elitism
        new_population += self._get_elite()
        # selection, crossover, mutation
        for elem in range(int(self._size * (1.0 - self._elite))):
            fittest_parents = self._select_fittest()
            new_population.append([self._create_child(
                self._crossover_func,
                *fittest_parents,
                self._crossover),
                                   GenerationGene.DEFAULT_FITNESS])
        self._population = new_population.copy()
        return fittest_chromosome