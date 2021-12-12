# 4. Human-like password generator

## Генератор паролів

Створимо генератор паролів, в якому 10% - найменш складні та найбільш розповсюджені, 10% - повністю випадкові та 80% - найбільш схожі на ті, які використовують звичайні люди (загальні слова, дати, деякі літери написані великими або замінені на цифри). Згенеруємо 100k таких паролів та використаємо алгоритм SHA1 для хешування. 100 з них захешуємо за допомогою Argon2i (1GB, 4 Cores). Швидкість хешування одного паролю на середньостатистичній машині дорівнює приблизно 4-5 секунд, що робить буртфорс практично неможливим.

![image](https://user-images.githubusercontent.com/48530948/145720077-6b8515fe-c972-417e-9cab-48c5382c7bee.png)

## Підбір чужих хешів паролів за допомогою hashcat

Спробуємо підібрати паролі з списку 100 тис. md5 хешів слабких паролей.  
Використаємо wordlist атаку з 10 мільйонами найуживаніших паролей (https://github.com/danielmiessler/SecLists/blob/master/Passwords/Common-Credentials/10-million-password-list-top-1000000.txt)  
```
.\hashcat64.exe -a 0 -m 0 weakHashes.csv .\10-million-password-list-top-1000000.txt -o brokenWeakHashes.txt
```
Результат виконання:  
![image](https://user-images.githubusercontent.com/20458905/145692454-d330aabb-fd77-48dc-8c24-6f28eefe3055.png)

У результаті вдалося отримати 79474/100000 паролів  

## Брутфорс посилання на наступну лабу

Унизу документу з завданням знайшли 15 MD5 хешів, що складають посилання на гугл документ.  
Оскільки посилання складаються з випадкових base64 символів, використаємо брутфорс-атаку з base64 charset'ом:  
```
.\hashcat64.exe -a 3 -m0 -1 "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_:./" --increment --increment-min=4 --increment-max=7 hashes.txt ?1?1?1?1?1?1?1
```
```
-a 3 - режим брутфорсу
-m0 - md5 алгоритм
--increment - інкрементуємо довжину plaintext
--increment-min=4 - від 4 символів
--increment-max=7 - до 7 символів
-1 abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_:.\/ - символи, які зустрічаються у google docs посиланнях
hashes.txt - файл з хешами
?1?1?1?1?1?1?1 - маска, до 7 символів з чарсету
```

Отриманий результат:
![image](https://user-images.githubusercontent.com/20458905/145692266-721a6f6d-96dc-4364-8889-927b64d586dd.png)

Зберемо частини посилання:
https://docs.google.com/document/d/1w5zDaGqmhRpprxlX5t6xXrifRKnBZmJ862E72fFccKc
