import gmpy

M = 2**32
def crack_a(X):
     mod_inv_a = gmpy.invert((X[0]-X[1]), M)
     found_a = ((X[1] - X[2])*mod_inv_a)%M
     return found_a

def crack_c(a, X):
    found_c = (X[2] - a*X[1])%M
    return found_c

def lcg(prev, a, c):
    return int((prev * a + c) % M)

print('Enter x1-x3 pseudorandom values generated by LCG')
print('x1:')
x1 = int(input())
print('x2:')
x2 = int(input())
print('x3:')
x3 = int(input())
X = [x1, x2, x3]
a = crack_a(X)
c = crack_c(a, X)
predict = lcg(x3, a, c)
print('found a = ' + str(a))
print('found c = ' + str(c))
print('next value: ' + str(predict))