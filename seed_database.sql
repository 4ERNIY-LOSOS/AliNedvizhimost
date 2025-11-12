USE AliNedvizhimost;
GO

-- Clear existing data from tables in the correct order to respect foreign key constraints
-- ON DELETE CASCADE will handle deleting properties when a user is deleted.
DELETE FROM Users;
GO

-- Insert a default Admin user and capture their new UserId
DECLARE @AdminUserId INT;
DECLARE @CustomerUserId INT;

INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role)
VALUES (N'admin@alinadvizhimost.com', N'$2a$11$1dxrOWMLg.K5OH7ktbxoeeYvYY92bbZ7nfSvGwK5GxcSLOp7/FqpW', N'Админ', N'Админов', N'Admin');
SET @AdminUserId = SCOPE_IDENTITY();

INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role)
VALUES (N'user@alinadvizhimost.com', N'$2a$11$1dxrOWMLg.K5OH7ktbxoeeYvYY92bbZ7nfSvGwK5GxcSLOp7/FqpW', N'Пользователь', N'Пользователев', N'Customer');
SET @CustomerUserId = SCOPE_IDENTITY();

-- Insert sample properties linked to the newly created Admin user
INSERT INTO Properties (Title, Address, Price, Area, Rooms, Description, UserId, Status)
VALUES
(N'Продам 3-комнатную квартиру', N'Проспект Вернадского, 78, Москва', 25000000.00, 120.5, 3, N'Просторная квартира в центре города. Недавно отремонтирована.', @AdminUserId, N'Активно'),
(N'Продам уютный дом с участком', N'Улица Ленина, 12, Санкт-Петербург', 45000000.00, 200.0, 4, N'Уютный семейный дом с большим двором и гаражом.', @AdminUserId, N'Активно'),
(N'Элитная вилла с бассейном', N'Рублевское шоссе, 101, Московская область', 65000000.00, 350.7, 5, N'Роскошная вилла с бассейном и потрясающим видом на лес.', @AdminUserId, N'Активно');

-- Insert sample properties linked to the newly created Customer user
INSERT INTO Properties (Title, Address, Price, Area, Rooms, Description, UserId, Status)
VALUES
(N'Студия в новостройке', N'Улица Пушкина, 5, Казань', 7000000.00, 30.0, 1, N'Светлая студия в новом жилом комплексе. Отличный вариант для инвестиций.', @CustomerUserId, N'Активно'),
(N'1-комнатная квартира у метро', N'Невский проспект, 1, Санкт-Петербург', 12000000.00, 45.0, 1, N'Квартира в шаговой доступности от метро. Развитая инфраструктура.', @CustomerUserId, N'Активно');
GO

PRINT 'Database seeded with test data.';
GO
