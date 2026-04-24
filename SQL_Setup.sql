-- ============================================================
-- AOGIRI — Платформа публикации частных объявлений
-- SQL-скрипт для SSMS (MS SQL Server)
-- ============================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'AogiriDB')
    DROP DATABASE AogiriDB;
GO

CREATE DATABASE AogiriDB;
GO

USE AogiriDB;
GO

-- ===== ТАБЛИЦЫ =====

CREATE TABLE Location_3 (
    LocationID  INT IDENTITY(1,1) PRIMARY KEY,
    City        VARCHAR(100) NOT NULL,
    Region      VARCHAR(100) NULL,
    Country     VARCHAR(100) NOT NULL
);
GO

CREATE TABLE Category_2 (
    CategoryID  INT IDENTITY(1,1) PRIMARY KEY,
    Name        VARCHAR(100) NOT NULL,
    IconClass   VARCHAR(50)  NOT NULL DEFAULT 'bi-tag'
);
GO

CREATE TABLE User_1 (
    UserID       INT IDENTITY(1,1) PRIMARY KEY,
    Login        VARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash VARCHAR(256) NOT NULL,
    Name         VARCHAR(100) NOT NULL,
    Email        VARCHAR(100) NULL,
    Phone        VARCHAR(20)  NOT NULL,
    RegDate      DATETIME     NOT NULL DEFAULT GETDATE(),
    Status       VARCHAR(20)  NOT NULL DEFAULT 'Active',  -- Active, Blocked
    [Role]       VARCHAR(20)  NOT NULL DEFAULT 'User',     -- User, Moderator, Admin
    AvatarUrl    VARCHAR(500) NULL
);
GO

CREATE TABLE Advertisement_4 (
    AdID            INT IDENTITY(1,1) PRIMARY KEY,
    Title           NVARCHAR(200)  NOT NULL,
    Description     NVARCHAR(MAX)  NULL,
    Price           MONEY          NOT NULL DEFAULT 0,
    PublishedDate   DATETIME       NOT NULL DEFAULT GETDATE(),
    Status          VARCHAR(20)    NOT NULL DEFAULT 'Draft',  -- Draft,Pending,Active,Rejected,Inactive,Expired,Deleted
    RejectionReason NVARCHAR(500)  NULL,
    ViewCount       INT            NOT NULL DEFAULT 0,
    ImageUrl        VARCHAR(500)   NULL,
    ExpiryDate      DATETIME       NULL,
    UserID          INT            NOT NULL,
    LocationID      INT            NOT NULL,
    CategoryID      INT            NOT NULL,
    CONSTRAINT FK_Ad_User     FOREIGN KEY (UserID)     REFERENCES User_1(UserID)     ON DELETE NO ACTION,
    CONSTRAINT FK_Ad_Location FOREIGN KEY (LocationID) REFERENCES Location_3(LocationID) ON DELETE NO ACTION,
    CONSTRAINT FK_Ad_Category FOREIGN KEY (CategoryID) REFERENCES Category_2(CategoryID) ON DELETE NO ACTION
);
GO

CREATE TABLE Message_5 (
    MessageID  INT IDENTITY(1,1) PRIMARY KEY,
    Text       NVARCHAR(MAX) NOT NULL,
    SentDate   DATETIME      NOT NULL DEFAULT GETDATE(),
    IsRead     BIT           NOT NULL DEFAULT 0,
    SenderID   INT           NOT NULL,
    ReceiverID INT           NOT NULL,
    AdID       INT           NULL,
    CONSTRAINT FK_Msg_Sender   FOREIGN KEY (SenderID)   REFERENCES User_1(UserID)        ON DELETE NO ACTION,
    CONSTRAINT FK_Msg_Receiver FOREIGN KEY (ReceiverID) REFERENCES User_1(UserID)        ON DELETE NO ACTION,
    CONSTRAINT FK_Msg_Ad       FOREIGN KEY (AdID)       REFERENCES Advertisement_4(AdID) ON DELETE SET NULL
);
GO

CREATE TABLE Favorite_6 (
    FavoriteID INT IDENTITY(1,1) PRIMARY KEY,
    AddTime    DATETIME NOT NULL DEFAULT GETDATE(),
    UserID     INT      NOT NULL,
    AdID       INT      NOT NULL,
    CONSTRAINT FK_Fav_User FOREIGN KEY (UserID) REFERENCES User_1(UserID)        ON DELETE CASCADE,
    CONSTRAINT FK_Fav_Ad   FOREIGN KEY (AdID)   REFERENCES Advertisement_4(AdID) ON DELETE CASCADE
);
GO

CREATE TABLE ActivityLog_7 (
    LogID     INT IDENTITY(1,1) PRIMARY KEY,
    Action    NVARCHAR(200) NOT NULL,
    Details   NVARCHAR(MAX) NULL,
    Timestamp DATETIME      NOT NULL DEFAULT GETDATE(),
    Result    VARCHAR(20)   NOT NULL DEFAULT 'Success',
    UserID    INT           NULL,
    CONSTRAINT FK_Log_User FOREIGN KEY (UserID) REFERENCES User_1(UserID) ON DELETE SET NULL
);
GO

CREATE TABLE ModerationRule_8 (
    RuleID    INT IDENTITY(1,1) PRIMARY KEY,
    Phrase    NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- ===== SEED DATA =====

INSERT INTO Location_3 (City, Region, Country) VALUES
    ('Полоцк',  'Витебская область',  'Беларусь'),
    ('Минск',   'Минская область',    'Беларусь'),
    ('Витебск', 'Витебская область',  'Беларусь'),
    ('Гродно',  'Гродненская область','Беларусь'),
    ('Брест',   'Брестская область',  'Беларусь'),
    ('Гомель',  'Гомельская область', 'Беларусь'),
    ('Могилев', 'Могилевская область','Беларусь');
GO

INSERT INTO Category_2 (Name, IconClass) VALUES
    ('Транспорт',      'bi-car-front'),
    ('Недвижимость',   'bi-house'),
    ('Электроника',    'bi-laptop'),
    ('Одежда и обувь', 'bi-bag'),
    ('Работа',         'bi-briefcase'),
    ('Услуги',         'bi-tools'),
    ('Животные',       'bi-heart'),
    ('Хобби и спорт',  'bi-bicycle'),
    ('Мебель',         'bi-lamp'),
    ('Другое',         'bi-three-dots');
GO

-- Пароли хранятся как BCrypt-хэши. Для seed используем заранее хэшированные значения.
-- admin:admin9999 | moder:moder5678 | user1:1234
INSERT INTO User_1 (Login, PasswordHash, Name, Phone, Status, [Role], RegDate) VALUES
    ('admin', '$2a$11$K5XnhOjFsT.X7Bm6Fj2Y5OhMzqM/qb9V.oWH1xfCu0YG9eOW7K4y', 'Администратор', '+375291234567', 'Active', 'Admin',     '2025-01-01'),
    ('moder', '$2a$11$7Rmu4ARBXxPJeT5VCKlTou0aBO5FtAiIbqDbnwNqKKy4aTM0kYH5S', 'Модератор',     '+375297654321', 'Active', 'Moderator', '2025-01-01'),
    ('user1', '$2a$11$3Qzk5MYB/pU6smrLzKv0.u5m6K7mxwBhNLOgzj0h1jJCVL2xHkfJ6', 'Иван Иванов',   '+375291111111', 'Active', 'User',      '2025-01-02');
GO

INSERT INTO ModerationRule_8 (Phrase, CreatedAt) VALUES
    ('только пересылка', '2025-01-01'),
    ('бесплатно отдам',  '2025-01-01'),
    ('казино',           '2025-01-01');
GO

-- Тестовые объявления
INSERT INTO Advertisement_4 (Title, Description, Price, Status, UserID, LocationID, CategoryID, PublishedDate) VALUES
    ('Продам велосипед горный', 'Велосипед в отличном состоянии, цепь заменена, шины новые.', 450, 'Active', 3, 1, 8, '2025-11-01'),
    ('Сдам квартиру 2к',       'Уютная квартира в центре города, евроремонт, мебель.',      600, 'Active', 3, 2, 2, '2025-11-03'),
    ('iPhone 13 128GB',         'Apple iPhone 13, цвет midnight, комплект, без торга.',     1200, 'Active', 3, 1, 3, '2025-11-05'),
    ('Репетитор по математике', 'Опытный преподаватель, подготовка к ЦТ, ЕГЭ.',              25, 'Active', 3, 2, 6, '2025-11-07'),
    ('Щенки лабрадора',         'Чистокровные щенки с документами, 2 мес.',                 300, 'Pending',3, 3, 7, '2025-11-10');
GO

PRINT '✅ База данных AogiriDB успешно создана!';
PRINT '   Логины для входа:';
PRINT '   admin / admin9999  (Администратор)';
PRINT '   moder / moder5678  (Модератор)';
PRINT '   user1 / 1234       (Пользователь)';
GO
