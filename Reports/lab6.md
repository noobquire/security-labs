# Sensitive information storage
ASP.NET Core містить вбудовані інтерфейси для захисту персональних даних: IPersonalDataProtector, ILookupProtectorKeyRing, ILookupProtector.  
IPersonalDataProtector, ILookupProtectorKeyRing містять логіку шифрування та розшифровки персональних даних для отримання користувачем.  
ILookupProtectorKeyRing містить логіку доступу та збереження ключів шифрування.  
Реалізуємо їх на на основі симетричного шифрування AES 256 HMAC SHA 512.  

У production-сценарії ключі AES варто зберігати у менеджері секретів (AWS Secrets Manager/Azure Key Vault).  
Для спрощення нашої реалізації збережемо ключі на диску у вигляді файлу, окремо від бази даних.  
![image](https://user-images.githubusercontent.com/20458905/145720388-3f2f7c56-d220-4204-9967-98babbe8cc23.png)

Тепер можна побачити, що значення, які зберігаються у базі даних зашифровані.  
![image](https://user-images.githubusercontent.com/20458905/145720755-9f6ca87d-c672-49af-9553-1a9bd80a5193.png)

У разі зливу бази даних користувачів дані будуть у безпеці.  
Однак, якщо отримати доступ і до ключа шифрування, зловмисник зможе розшифрувати персональні дані.  
