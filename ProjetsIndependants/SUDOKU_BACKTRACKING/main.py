import math

def affichage(grille):
    print(" -------------------------\n")
    for i in range(0, 9):
        print(" | ", end="")
        for j in range(0, 9):
            print("{} ".format(grille[i][j]) if (math.fmod((j + 1), 3)) else "{} | ".format(grille[i][j]), end="")
        print("\n")
        if not (math.fmod((i + 1), 3)):
            print(" -------------------------\n")
    print("\n\n")
    return grille


def absentSurLigne(k, grille, i):
    for j in range(0, 9):
        if grille[i][j] == k:
            return False
    return True


def absentSurColonne(k, grille, j):
    for i in range(0, 9):
        if grille[i][j] == k:
            return False
    return True


def absentSurBloc(k, grille, i, j):
    _i = int(i - (math.fmod(i, 3)))  # ou encore : _i = 3*(i/3), _j = 3*(j/3);
    _j = int(j - (math.fmod(j, 3)))
    i = int(_i)
    while i < _i + 3:
        j = _j
        while j < _j + 3:
            if grille[i][j] == k:
                return False
            j += 1
        i += 1
    return True


def estValide(grille, position):
    if (position == 9 * 9): #Si on est à la 82e case (on sort du tableau)
        return True

    #On récupère les coordonnées de la case
    i = int(position / 9)
    j = int(position % 9)
    # print("i:", type(i))
    # print("j:", type(j))


    if (grille[i][j] != 0): #Si la case est pas vide, on passe à la suivante (appel récursif)
        return estValide(grille, position + 1)

    for k in range(1, 10): #On dit qu elles sont les valeurs possibles
        if absentSurLigne(k, grille, i) and absentSurColonne(k, grille, j) and absentSurBloc(k, grille, i, j):
            grille[i][j] = k

            #On appelle récursivement la fonction estValide(), pour voir si ce choix est bon par la suite
            if estValide(grille, position + 1):
                return True

            #Tous les chiffres ont été testés, aucun est bon, on réinitialise la case
    grille[i][j] = 0

    return False


def facile(numeroSudoku):
    with open('Sudoku_Easy51.txt', 'r') as fichier:
        lignes = fichier.readlines()
        #print(lignes[numeroSudoku-1])
    return lignes[numeroSudoku-1]

def moyen(numeroSudoku):
    with open('Sudoku_hardest.txt', 'r') as fichier:
        lignes = fichier.readlines()
        #print(lignes[numeroSudoku - 1])
        lignes = [elem.replace('.', '0') for elem in lignes]
        #print(lignes[numeroSudoku - 1])
        #print(len(lignes[numeroSudoku - 1]))
    return lignes[numeroSudoku-1]

def difficile(numeroSudoku):
    with open('Sudoku_top95.txt', 'r') as fichier:
        lignes = fichier.readlines()
        #print(lignes[numeroSudoku - 1])
        lignes = [elem.replace('.', '0') for elem in lignes]
        #print(lignes[numeroSudoku - 1])
        #print(len(lignes[numeroSudoku - 1]))
    return lignes[numeroSudoku - 1]

def tradToGrilleAvecZeros(liste):
    grille = []
    grilleStr = []
    for i in range(0, 81):
        grilleStr.append(liste[i])

    '''
    for i in range(0, 9):
        liste = []
        for k in range(0, 9):
            liste = liste.append(lignes[])
    '''
    #print("grilleStr:", grilleStr, "\nlen =", len(grilleStr))

    for i in range(0, 9):
        grilleTemp = []
        for j in range(1, 10):
            grilleTemp.append(int(grilleStr[j+9*i-1]))
        grille.append(grilleTemp)

    #print("grille:", grille)

    return grille

def tradToGrilleAvecPoints(liste):
    grille = []
    grilleStr = []
    for i in range(0, 81):
        grilleStr.append(liste[i])

    '''
    for i in range(0, 9):
        liste = []
        for k in range(0, 9):
            liste = liste.append(lignes[])
    '''
    #print("grilleStr:", grilleStr, "\nlen =", len(grilleStr))

    for i in range(0, 9):
        grilleTemp = []
        for j in range(1, 10):
            grilleTemp.append(int(grilleStr[j + 9 * i - 1]))
        grille.append(grilleTemp)

    # print("grille:", grille)

    return grille

def resolutionSudoku(grille):
    print("\n\nChosen Puzzle:")
    grille = affichage(grille)

    estValide(grille, 0)

    print("Valid solution:")
    grilleComplete = affichage(grille)

    #Grille finale :
    #print(grilleComplete)



def main():
    while True:
        difficulte = int(input("Select difficulty: 1-Easy, 2-Medium, 3-Hard\n"))
        if difficulte == 1:
            numeroSudoku = int(input("Choose a puzzle index between 1 and 51\n"))
            liste = facile(numeroSudoku)
            grille = tradToGrilleAvecZeros(liste)
        elif difficulte == 2:
            numeroSudoku = int(input("Choose a puzzle index between 1 and 11\n"))
            liste = moyen(numeroSudoku)
            grille = tradToGrilleAvecPoints(liste)
        elif difficulte == 3:
            numeroSudoku = int(input("Choose a puzzle index between 1 and 95\n"))
            liste = difficile(numeroSudoku)
            grille = tradToGrilleAvecPoints(liste)
        else:
            print("ERREUR")

        # grille = [[9, 0, 0, 1, 0, 0, 0, 0, 5], [0, 0, 5, 0, 9, 0, 2, 0, 1], [8, 0, 0, 0, 4, 0, 0, 0, 0], [0, 0, 0, 0, 8, 0, 0, 0, 0], [0, 0, 0, 7, 0, 0, 0, 0, 0], [0, 0, 0, 0, 2, 6, 0, 0, 9], [2, 0, 0, 3, 0, 0, 0, 0, 6], [0, 0, 0, 2, 0, 0, 9, 0, 0], [0, 0, 1, 9, 0, 4, 5, 7, 0]]
        resolutionSudoku(grille)

main()