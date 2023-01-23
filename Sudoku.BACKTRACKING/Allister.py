import math

def affichage(grille):
    print(" -------------------------\n")
    for i in range(0, 9):
        print(" | ", end="")
        for j in range(0, 9):

            print("{} ".format(grille[i][j]) if (math.fmod((j+1), 3)) else "{} | ".format(grille[i][j]), end="")
        print("\n")
        if not(math.fmod((i+1), 3)):
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
    _i = int(i-(math.fmod(i, 3))) # ou encore : _i = 3*(i/3), _j = 3*(j/3);
    _j = int(j-(math.fmod(j, 3)))
    i = int(_i)
    while i < _i+3:
        j = _j
        while j < _j+3:
            if grille[i][j] == k:
                return False
            j += 1
        i += 1
    return True

def estValide(grille, position):
    if (position == 9*9):
        return True

    i = int(position / 9)
    j = int(position%9)
    #print("i:", type(i))
    #print("j:", type(j))

    if (grille[i][j] != 0):
        return Globals.estValide(grille, position+1)

    for k in range(1, 10):
        if Globals.absentSurLigne(k, grille, i) and Globals.absentSurColonne(k, grille, j) and Globals.absentSurBloc(k, grille, i, j):
            grille[i][j] = k

            if Globals.estValide(grille, position+1):
                return True
    grille[i][j] = 0

    return False



grille = [[9, 0, 0, 1, 0, 0, 0, 0, 5], [0, 0, 5, 0, 9, 0, 2, 0, 1], [8, 0, 0, 0, 4, 0, 0, 0, 0], [0, 0, 0, 0, 8, 0, 0, 0, 0], [0, 0, 0, 7, 0, 0, 0, 0, 0], [0, 0, 0, 0, 2, 6, 0, 0, 9], [2, 0, 0, 3, 0, 0, 0, 0, 6], [0, 0, 0, 2, 0, 0, 9, 0, 0], [0, 0, 1, 9, 0, 4, 5, 7, 0]]

print("Grille vide :\n")
grille = affichage(grille)

estValide(grille, 0)

print("Grille complétée :\n")
grilleComplete = affichage(grille)

print(grilleComplete)