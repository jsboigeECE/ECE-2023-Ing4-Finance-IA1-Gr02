import numpy as np
from random import random, shuffle, randint, sample


# TODO Test this functions to check if everything works as expected!!!
def single_cell_crossover(parent1, parent2, cross_probability: float):
    """
    Crossover point is single cell.
    Arguments:
        parent1 {np.array} -- first parent.
        parent2 {np.array} -- second parent.
        cross_probability {float} -- crossover probability level for taking
        elements from first parent.
    Returns:
        {np.array} -- newly created child
    """
    child = np.empty((9, 9), dtype=np.int8)
    for row in range(9):
        for col in range(9):
            random_number = random()
            if random_number <= cross_probability:
                child[row, col] = parent1[row, col].copy()
            elif random_number <= (2.0 * cross_probability):
                child[row, col] = parent2[row, col].copy()
            else:
                child[row, col] = randint(1, 9)
    return child


def row_crossover(parent1, parent2, cross_probability: float):
    """
    Crossover point is single row.
    Arguments:
        parent1 {np.array} -- first parent.
        parent2 {np.array} -- second parent.
        cross_probability {float} -- crossover probability level for taking
        elements from first parent.
    Returns:
        {np.array} -- newly created child
    """
    child = np.empty((9, 9), dtype=np.int8)
    for row in range(9):
        random_number = random()
        if random_number <= cross_probability:
            child[row, :] = parent1[row, :].copy()
        elif random_number <= (2.0 * cross_probability):
            child[row, :] = parent2[row, :].copy()
        else:
            sample = [1, 2, 3, 4, 5, 6, 7, 8, 9]
            shuffle(sample)
            child[row, :] = sample
    return child


def square_crossover(parent1, parent2, cross_probability: float):
    """
    Crossover point is single sqare(9x9).
    Arguments:
        parent1 {np.array} -- first parent.
        parent2 {np.array} -- second parent.
        cross_probability {float} -- crossover probability level for taking
        elements from first parent.
    Returns:
        {np.array} -- newly created child
    """
    child = np.empty((9, 9), dtype=np.int8)
    for row in range(0, 9, 3):
        for col in range(0, 9, 3):
            random_number = random()
            if random_number <= cross_probability:
                child[row:row+3, col:col+3] = parent1[
                                                row:row+3, col:col+3].copy()
            elif random_number <= (2.0 * cross_probability):
                child[row:row+3, col:col+3] = parent2[
                                                row:row+3, col:col+3].copy()
            else:
                sample = [1, 2, 3, 4, 5, 6, 7, 8, 9]
                shuffle(sample)
                child[row:row+3, col:col+3] = np.reshape(
                    np.array(sample, dtype=np.int8), (3, 3)).copy()
    return child


def triplets_crossover(parent1, parent2, cross_probability: float):
    """
    Crossover point is triplet (3 elements) in row.
    Arguments:
        parent1 {np.array} -- first parent.
        parent2 {np.array} -- second parent.
        cross_probability {float} -- crossover probability level for taking
        elements from first parent.
    Returns:
        {np.array} -- newly created child
    """
    # TODO try to flatten it!!!
    child = np.empty((9, 9), dtype=np.int8)
    for row in range(9):
        for col in range(0, 9, 3):
            random_number = random()
            if random_number <= cross_probability:
                child[row, col:col+3] = parent1[row, col:col+3].copy()
            elif random_number <= (2.0 * cross_probability):
                child[row, col:col+3] = parent2[row, col:col+3].copy()
            else:
                avail_elements = [1, 2, 3, 4, 5, 6, 7, 8, 9]
                shuffle(avail_elements)
                child[row, col:col+3] = np.array(sample(avail_elements, 3),
                                                 dtype=np.int8).copy()
    return child