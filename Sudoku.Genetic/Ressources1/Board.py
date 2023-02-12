from typing import List


class Board():
    """
    Class for holding already filled sudoku board elements.
    """
    def __init__(self) -> None:
        """
        Empty initialises Board.
        """
        self._board = []

    @property
    def board(self) -> List[List[int]]:
        """
        Items getter.
        Returns:
            List[List[int]] -- returns list of already filled elements, empty
            elemnts have value 0.
        """
        return self._board

    def inject_board(self) -> None:
        """
        Injects predefined sudoku board.
        """
        # Board taken from this page, section "Easiest":
        # https://dingo.sbs.arizona.edu/~sandiway/sudoku/examples.html
        board = []
        board.append([0, 0, 0, 2, 6, 0, 7, 0, 1])
        board.append([6, 8, 0, 0, 7, 0, 0, 9, 0])
        board.append([1, 9, 0, 0, 0, 4, 5, 0, 0])
        board.append([8, 2, 0, 1, 0, 0, 0, 4, 0])
        board.append([0, 0, 4, 6, 0, 2, 9, 0, 0])
        board.append([0, 5, 0, 0, 0, 3, 0, 2, 8])
        board.append([0, 0, 9, 3, 0, 0, 0, 7, 4])
        board.append([0, 4, 0, 0, 5, 0, 0, 3, 6])
        board.append([7, 0, 3, 0, 1, 8, 0, 0, 0])

        self._board = board.copy()

    def load_board(self, rowArray) -> None:
        """
        Injects external sudoku board.
        """
        #self._board = [cell for row in rowList for cell in row]
        self._board = [row for row in rowArray]