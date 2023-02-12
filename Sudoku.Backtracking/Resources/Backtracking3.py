from timeit import default_timer



#Function to check if a digit can be placed in the given block
def possible(row,col,digit,instance):
    
    for i in range(0,9):
        if instance[row][i] == digit:
            return False
    for i in range(0,9):
        if instance[i][col] == digit:
            return False
    square_row = (row//3)*3
    square_col = (col//3)*3
    for i in range(0,3):
        for j in range(0,3):
            if instance[square_row+i][square_col+j] == digit:
                return False    
    return True

def solve(instance):
    
    for row in range(9):
        for col in range(9):
            if instance[row][col] == 0:
                for digit in range(1,10):
                    if possible(row,col,digit,instance):
                        instance[row][col] = digit
                        solve(instance)
                        instance[row][col] = 0  #Backtrack step                  
                return 
    print('\nThe Solution')   
        

start = default_timer()
solve(instance)
r=instance  
duration = default_timer() - start
print("Le temps de résolution est de : ", duration, " seconds as a floating point value")

#wall-clock time duration in seconds as a floating point value