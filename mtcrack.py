import random, time
from randcrack import RandCrack

rc = RandCrack()

with open('values.txt', 'r') as fp:
    for line in fp.readlines():
        rc.submit(int(line))
    
print(rc.predict_randrange(0, 4294967295))