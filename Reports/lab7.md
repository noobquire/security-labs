# 7. TLS configuration
Отримаємо безкоштовний SSL сертифікат з https://zerossl.com.
![image](https://user-images.githubusercontent.com/20458905/141698080-0b6608b8-a8bc-406c-8625-0c74dba5f95f.png)
![image](https://user-images.githubusercontent.com/20458905/141698096-8e14bb5c-9e2f-453f-b950-2e7766d4fc86.png)
![image](https://user-images.githubusercontent.com/20458905/141698108-f08c4a4e-4d11-4651-8440-b0f232a898f6.png)
Завантажимо файл сертифіката та переглянемо деталі. Бачимо, що ZeroSSL використовує RSA шифрування з алгоритмом хешування SHA-384:

![image](https://user-images.githubusercontent.com/20458905/141698150-b7f128fe-ed8a-4b48-9a7b-1c56d638f867.png)

Конкатенуємо головний сертифікат та сертифікат CA у один файл:
```
$ cat certificate.crt ca_bundle.crt >> bundle.crt
```

Скопіюємо сертифікат та приватний ключ на сервер:
```
$ scp -r certs pi@oleksiilytvynov.tk:~/certs
```

Перейменуємо та скопіюємо сертифікат та приватний ключ до /etc/ssl
```
$ cd certs
$ sudo cp bundle.crt /etc/ssl/certs/oleksiilytvynov.tk.crt
$ sudo cp private.key /etc/ssl/certs/oleksiilytvynov.tk.key
```

Відредагуємо конфігурацію віртуального хоста nginx:
```
$ cat /etc/nginx/sites-enabled/oleksiilytvynov.tk
server {
        listen 443 ssl http2 default_server;
        listen [::]:443 ssl http2 default_server;

        ssl on;
        ssl_certificate /etc/ssl/certs/oleksiilytvynov.tk.crt;
        ssl_certificate_key /etc/ssl/private/oleksiilytvynov.tk.key;

        index index.html index.htm index.nginx-debian.html;

        server_name oleksiilytvynov.tk www.oleksiilytvynov.tk;
        root /var/www/oleksiilytvynov.tk/html;

        location / {
                try_files $uri $uri/ =404;
        }
}
```
Також відредагуємо дефолтну конфігурацію для перенаправлення http запитів на https:
```
$ cat /etc/nginx/sites-enabled/default
server {
        listen 80 default_server;
        server_name _;
        return 301 https://$host$request_uri;
}
```

Протестуємо файли конфігурації та перезавантажимо сервер:
```
$ sudo nginx -t
nginx: the configuration file /etc/nginx/nginx.conf syntax is ok
nginx: configuration file /etc/nginx/nginx.conf test is successful
$ sudo systemctl restart nginx
```

Перевіримо роботу https через браузер:

![image](https://user-images.githubusercontent.com/20458905/141698565-31006509-9d3f-4eed-a951-f54ee38ea143.png)
