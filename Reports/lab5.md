# Password storage. Hashing.

Для збереження паролів скоритаємось ASP.NET Core Identity шаблоном у Visual Studio.  
Паролі зберігаються у localdb MS SQL базі (має бути змінена для production env).  

Стандартний алгоритм хешування паролів, який використовує asp.net core (https://github.com/dotnet/aspnetcore/blob/release/6.0/src/Identity/Extensions.Core/src/PasswordHasher.cs#L29):
```
PBKDF2 with HMAC-SHA256, 128-bit salt, 256-bit subkey, 10000 iterations.
```

Цей алгоритм не є достатньо безпечним для зберігання паролів станом на 2021 рік.  
Скористаємось рекомендаціями OWASP cheat sheet:  
```
Use Argon2id with a minimum configuration of 15 MiB of memory, an iteration count of 2, and 1 degree of parallelism.
```

Для хешування паролів скористаємося відкритою бібліотекою Argon2id на базі libsodium (https://github.com/scottbrady91/ScottBrady91.AspNetCore.Identity.Argon2PasswordHasher). 
За замовченням вона виконистовує такі налаштування алгоритму:
```
32 MiB of memory, 4 iterations, 1 degree of parallelism.
```
Що є безпечнішим ніж рекомендовані параметри, тому залишимо стандартні.  

Змінимо алгоритм хешування у конфігурації проекту:  
```
builder.Services.AddScoped<IPasswordHasher<IdentityUser>, Argon2PasswordHasher<IdentityUser>>();
```

Сторінка авторизації:  
![image](https://user-images.githubusercontent.com/20458905/145714204-84d55971-bcd7-4a9a-a52e-39906e08b57b.png)

Сторінка, яка доступна тільки авторизованим користувачам:  
![image](https://user-images.githubusercontent.com/20458905/145714310-e4463e27-4db8-48eb-a20c-b9e902388961.png)

Вигляд захешованих паролів у базі даних:  
![image](https://user-images.githubusercontent.com/20458905/145714421-0ab8557f-6875-47a8-b28a-91a5f41da246.png)

