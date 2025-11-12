USE AliNedvizhimost;
GO

-- Clear existing data from tables in the correct order to respect foreign key constraints
DELETE FROM Messages;
DELETE FROM PropertyOwner;
DELETE FROM PropertyStatus;
DELETE FROM PropertyInfo;
DELETE FROM Users;
GO

-- Insert a default Admin user and capture their new UserId
DECLARE @AdminUserId INT;
DECLARE @CustomerUserId INT;

INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role)
VALUES (N'admin@gmail.com', N'$2a$11$a8Gjg4kJanRRT1GkSuDB.OmyjyRIYJu9VbHO/ixtV0cULKzuoP4B2', N'Админ', N'Админов', N'Admin');
SET @AdminUserId = SCOPE_IDENTITY();

INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role)
VALUES (N'user@gmail.com', N'$2a$11$PcFnREApNuQdEtLgOil6KeT4SZJVq9w6HdqmHcYBC4t/HAqa9Vs/O', N'Пользователь', N'Пользователев', N'Customer');
SET @CustomerUserId = SCOPE_IDENTITY();

-- Insert sample properties linked to the newly created Admin user
DECLARE @PropertyId1 INT, @PropertyId2 INT, @PropertyId3 INT;

INSERT INTO PropertyInfo (Title, Address, Price, Area, Rooms, Description)
VALUES (N'Продам 3-комнатную квартиру', N'Проспект Вернадского, 78, Москва', 25000000.00, 120.5, 3, N'Просторная квартира в центре города. Недавно отремонтирована.');
SET @PropertyId1 = SCOPE_IDENTITY();
INSERT INTO PropertyStatus (PropertyId, Status) VALUES (@PropertyId1, N'Активно');
INSERT INTO PropertyOwner (PropertyId, UserId) VALUES (@PropertyId1, @AdminUserId);

INSERT INTO PropertyInfo (Title, Address, Price, Area, Rooms, Description)
VALUES (N'Продам уютный дом с участком', N'Улица Ленина, 12, Санкт-Петербург', 45000000.00, 200.0, 4, N'Уютный семейный дом с большим двором и гаражом.');
SET @PropertyId2 = SCOPE_IDENTITY();
INSERT INTO PropertyStatus (PropertyId, Status) VALUES (@PropertyId2, N'Активно');
INSERT INTO PropertyOwner (PropertyId, UserId) VALUES (@PropertyId2, @AdminUserId);

INSERT INTO PropertyInfo (Title, Address, Price, Area, Rooms, Description)
VALUES (N'Элитная вилла с бассейном', N'Рублевское шоссе, 101, Московская область', 65000000.00, 350.7, 5, N'Роскошная вилла с бассейном и потрясающим видом на лес.');
SET @PropertyId3 = SCOPE_IDENTITY();
INSERT INTO PropertyStatus (PropertyId, Status) VALUES (@PropertyId3, N'Активно');
INSERT INTO PropertyOwner (PropertyId, UserId) VALUES (@PropertyId3, @AdminUserId);

-- Insert sample properties linked to the newly created Customer user
DECLARE @PropertyId4 INT, @PropertyId5 INT;

INSERT INTO PropertyInfo (Title, Address, Price, Area, Rooms, Description)
VALUES (N'Студия в новостройке', N'Улица Пушкина, 5, Казань', 7000000.00, 30.0, 1, N'Светлая студия в новом жилом комплексе. Отличный вариант для инвестиций.');
SET @PropertyId4 = SCOPE_IDENTITY();
INSERT INTO PropertyStatus (PropertyId, Status) VALUES (@PropertyId4, N'Активно');
INSERT INTO PropertyOwner (PropertyId, UserId) VALUES (@PropertyId4, @CustomerUserId);

INSERT INTO PropertyInfo (Title, Address, Price, Area, Rooms, Description)
VALUES (N'1-комнатная квартира у метро', N'Невский проспект, 1, Санкт-Петербург', 12000000.00, 45.0, 1, N'Квартира в шаговой доступности от метро. Развитая инфраструктура.');
SET @PropertyId5 = SCOPE_IDENTITY();
INSERT INTO PropertyStatus (PropertyId, Status) VALUES (@PropertyId5, N'Активно');
INSERT INTO PropertyOwner (PropertyId, UserId) VALUES (@PropertyId5, @CustomerUserId);
GO

PRINT 'Database seeded with test data.';
GO
