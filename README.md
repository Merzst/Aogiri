# 🌑 AOGIRI — Платформа публикации частных объявлений

> ASP.NET Core 8 · Razor Pages · Entity Framework Core · MS SQL Server (SSMS)

---

## 🚀 Быстрый старт

### 1. Требования
- **Visual Studio 2022** (или VS Code + .NET SDK 8)
- **SQL Server** + **SSMS** (любая редакция: Express, Developer, Standard)
- **.NET SDK 8.0**

---

### 2. Настройка базы данных

#### Вариант A — SQL скрипт (рекомендуется)
1. Откройте **SSMS**, подключитесь к серверу
2. Откройте файл `SQL_Setup.sql`
3. Выполните скрипт (`F5`)
4. БД `AogiriDB` будет создана с тестовыми данными

#### Вариант B — EF Migrations
```bash
cd Aogiri
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

### 3. Строка подключения

Отредактируйте `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=AogiriDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Примеры строк подключения:**
```
Server=localhost\SQLEXPRESS;Database=AogiriDB;Trusted_Connection=True;TrustServerCertificate=True;
Server=.;Database=AogiriDB;Trusted_Connection=True;TrustServerCertificate=True;
Server=(localdb)\MSSQLLocalDB;Database=AogiriDB;Trusted_Connection=True;
```

---

### 4. Запуск

```bash
cd Aogiri
dotnet run
```

Или в Visual Studio: `F5`

Откройте браузер: **https://localhost:7xxx**

---

## 👤 Тестовые аккаунты

| Логин   | Пароль     | Роль          |
|---------|------------|---------------|
| admin   | admin9999  | Администратор |
| moder   | moder5678  | Модератор     |
| user1   | 1234       | Пользователь  |

---

## 📁 Структура проекта

```
Aogiri/
├── Data/
│   └── ApplicationDbContext.cs     ← EF Core контекст + seed данных
├── Models/
│   ├── User.cs                     ← Пользователь
│   ├── Advertisement.cs            ← Объявление
│   ├── Category.cs                 ← Категория
│   ├── Location.cs                 ← Местоположение
│   ├── Message.cs                  ← Сообщение
│   └── Favorite.cs                 ← Избранное + Логи + Правила
├── Services/
│   └── AuthService.cs              ← Аутентификация, регистрация, логирование
├── Pages/
│   ├── Index.cshtml                ← Главная страница (поиск + лента)
│   ├── Account/
│   │   ├── Login.cshtml            ← F2: Авторизация
│   │   ├── Register.cshtml         ← F1: Регистрация
│   │   ├── Cabinet.cshtml          ← F9: Личный кабинет
│   │   └── Profile.cshtml          ← F8: Профиль продавца
│   ├── Ads/
│   │   ├── Create.cshtml           ← F4: Публикация объявления
│   │   ├── Detail.cshtml           ← F5: Карточка объявления
│   │   └── Edit.cshtml             ← F6: Редактирование объявления
│   ├── Messages/
│   │   └── Index.cshtml            ← F10: Сообщения (чат)
│   ├── Favorites/
│   │   ├── Index.cshtml            ← F11: Избранное
│   │   └── Toggle.cshtml           ← Добавить/убрать из избранного
│   ├── Moderator/
│   │   └── Index.cshtml            ← F12/F13: Панель модератора
│   └── Admin/
│       └── Index.cshtml            ← F14: Панель администратора
├── wwwroot/
│   ├── css/aogiri.css              ← Все стили (тёмная тема Aogiri)
│   └── uploads/                    ← Загруженные изображения
├── appsettings.json                ← Строка подключения к БД
├── Program.cs                      ← Конфигурация приложения
└── SQL_Setup.sql                   ← SQL-скрипт для SSMS
```

---

## 🗄️ Таблицы базы данных

| Таблица            | Описание                          |
|--------------------|-----------------------------------|
| `User_1`           | Пользователи (все роли)           |
| `Category_2`       | Категории объявлений              |
| `Location_3`       | Города/регионы                    |
| `Advertisement_4`  | Объявления                        |
| `Message_5`        | Сообщения между пользователями    |
| `Favorite_6`       | Избранные объявления              |
| `ActivityLog_7`    | Логи действий пользователей       |
| `ModerationRule_8` | Правила автомодерации             |

---

## ✨ Реализованный функционал

### Роль «Пользователь»
- ✅ Регистрация и авторизация (BCrypt хэши)
- ✅ Личный кабинет с редактированием профиля
- ✅ Публикация объявлений с загрузкой фото
- ✅ Редактирование / деактивация / удаление своих объявлений
- ✅ Поиск по ключевым словам
- ✅ Фильтрация по категории, цене, городу
- ✅ Сортировка (новые / цена / популярность)
- ✅ Карточка объявления с счётчиком просмотров
- ✅ Внутренняя переписка (анонимный чат)
- ✅ Избранное (добавить / удалить)
- ✅ Просмотр профиля продавца

### Роль «Модератор»
- ✅ Очередь объявлений на модерации
- ✅ Одобрение / отклонение с причиной
- ✅ Блокировка / разблокировка пользователей
- ✅ Просмотр списка всех пользователей

### Роль «Администратор»
- ✅ Управление категориями (CRUD)
- ✅ Управление правилами автомодерации
- ✅ Просмотр / удаление логов действий
- ✅ Дашборд со статистикой платформы

### Система автомодерации
- ✅ Автоматическая проверка объявлений по запрещённым фразам
- ✅ При нахождении — объявление сразу получает статус «Отклонено»

---

## 🎨 Дизайн

Тёмная тема в стиле **Aogiri** (Tokyo Ghoul):
- Цветовая схема: чёрный + тёмно-красный (#8B0000)
- Bootstrap Icons для иконок
- Полностью адаптивный (мобильный + десктоп)
- Кастомный скроллбар
