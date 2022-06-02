# BookStorageApp

Технологии : ASP.NET Core 5 MVC,
             Entity Framework Core,
             MS SQL Server,
             Bootstrap.

Интернет-каталог книг :
  - авторизация по ролям / регистрация,
  - аутентификация(с помощью куки)  
  - 4 основных страницы : 
      - каталог книг + фильтры / поиск
      - подробная информация о книге + комментарии + список глав
      - чтение главы + комментарии
      - профиль пользователя (список добавленных книг + все комментарии данного пользователя)

Гость имеет возможность :
    - просматривать книги
    - читать главы 
  
Зарегистрированный пользователь имеет возможность :
    - просматривать книги
    - читать главы
    - комментировать главы и книги
    - добавлять в профиль / удалять из него понравившиеся книги
    - редактировать профиль    
        
Администратор имеет возможность :
  -   добавлять / изменять / удалять книги, главы, комментарии 
  -   просматривать список пользоватлей, изменять роли, удалять пользователей
