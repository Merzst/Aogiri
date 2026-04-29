USE Aogiri; -- или AogiriDB, в зависимости от вашей БД

-- Сделать администратором
UPDATE User_1
SET Role = 'Admin', Status = 'Active'
WHERE Login = 'Merzst';

-- Сделать модератором
UPDATE User_1
SET Role = 'Moderator', Status = 'Active'
WHERE Login = ' ';

-- Проверка
SELECT UserID, Login, Name, Role, Status FROM User_1;