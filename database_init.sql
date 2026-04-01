IF NOT EXISTS (
    SELECT name 
    FROM sys.databases 
    WHERE name = 'matchmaking_db'
)
BEGIN
    CREATE DATABASE matchmaking_db;
END;
GO

USE matchmaking_db;

DROP TABLE IF EXISTS Interactions
DROP TABLE IF EXISTS Matches
DROP TABLE IF EXISTS ProfilePreferences
DROP TABLE IF EXISTS Bids
DROP TABLE IF EXISTS Photos
DROP TABLE IF EXISTS Notifications
DROP TABLE IF EXISTS ProfileInterests
DROP TABLE IF EXISTS Profiles
DROP TABLE IF EXISTS DatingAdmin
DROP TABLE IF EXISTS SupportTicket

CREATE TABLE Profiles (
    userId INT PRIMARY KEY,
    [name] VARCHAR(50),
    gender VARCHAR(20),
    [location] VARCHAR(50),
    nationality VARCHAR(50),
    maxDistance INT,
    age INT,
    minPrefAge INT,
    maxPrefAge INT,
    bio VARCHAR(250),
    displayStarSign BIT,
    isArchived BIT,
    dateOfBirth DATETIME,
    loverType VARCHAR(50),
    isHotSeat BIT,
    boost BIT,
    boostDay INTEGER,
    hotSeatDay INTEGER
);
GO

CREATE TABLE Interactions (
    interactionId INT IDENTITY(1, 1) PRIMARY KEY,
    fromProfileId INT FOREIGN KEY REFERENCES Profiles(userId),
    toProfileId INT FOREIGN KEY REFERENCES Profiles(userId),
    [type] VARCHAR(50)
);
GO

CREATE TABLE Matches (
    matchId INT IDENTITY(1, 1) PRIMARY KEY,
    user1Id INT FOREIGN KEY REFERENCES Profiles(userId),
    user2Id INT FOREIGN KEY REFERENCES Profiles(userId)
);
GO

CREATE TABLE ProfilePreferences (
    userId INT FOREIGN KEY REFERENCES Profiles(userId),
    gender VARCHAR(50),
    PRIMARY KEY(userId, gender)
);
GO

CREATE TABLE Bids (
    bidId INT IDENTITY(1, 1) PRIMARY KEY,
    userId INT FOREIGN KEY REFERENCES Profiles(userId),
    bidSum INT
);
GO

CREATE TABLE Photos (
    photoId INT IDENTITY(1, 1) PRIMARY KEY,
    userId INT FOREIGN KEY REFERENCES Profiles(userId),
    [location] VARCHAR(200),
    profileOrderIndex INT
);
GO

CREATE TABLE Notifications (
    notificationId INT IDENTITY(1, 1) PRIMARY KEY,
    recipientId INT FOREIGN KEY REFERENCES Profiles(userId),
    fromId INT FOREIGN KEY REFERENCES Profiles(userId),
    [type] VARCHAR(50),
    isRead BIT,
    [timestamp] DATETIME,
    title VARCHAR(20),
    [description] VARCHAR(50)
);
GO

CREATE TABLE ProfileInterests (
    userId INT FOREIGN KEY REFERENCES Profiles(userId),
    interest VARCHAR(50),
    PRIMARY KEY(userId, interest)
);
GO

CREATE TABLE DatingAdmin (
    userId INT PRIMARY KEY
);
GO

CREATE TABLE SupportTicket (
    email VARCHAR(100) PRIMARY KEY,
    partnerName VARCHAR(100),
    certificateUrl VARCHAR(100),
    partnerPhotoUrl VARCHAR(100),
    isResolved BIT
);
GO

INSERT INTO DatingAdmin (userId) VALUES
(45);

SELECT *
FROM DatingAdmin

-- Profiles (10 users)
INSERT INTO Profiles 
(userId, [name], gender, location, nationality, maxDistance, age, minPrefAge, maxPrefAge, bio, displayStarSign, isArchived, dateOfBirth, loverType, isHotSeat, boost, boostDay, hotSeatDay)
VALUES
(1, 'Andrei Popescu',   'MALE',       'Cluj-Napoca',       'Romanian', 100, 28, 22, 35, 'Software engineer who loves hiking and coffee.', 1, 0, '1996-03-15', 'DEEP_THINKER', NULL, NULL, NULL, NULL),
(2, 'Maria Ionescu',    'FEMALE',     'Bucuresti',  'Romanian', 50, 25, 23, 32, 'Artist and yoga instructor. Dog mom.',          0, 0, '1999-07-22', 'SOCIAL_EXPLORER', NULL, NULL, NULL, NULL),
(3, 'Alex Dumitrescu',  'MALE',       'Timisoasa',      'Romanian', 200, 31, 25, 38, 'Finance guy by day, amateur chef by night.',    1, 0, '1993-11-05', 'STABILITY_LOVER', NULL, NULL, NULL, NULL),
(4, 'Elena Marinescu',  'FEMALE',     'Iasi',       'Romanian', 75, 27, 25, 34, 'Journalist. Lover of wine, books, and bad puns.',1, 0, '1997-04-18', 'EMPATHETIC_CONNECTOR', NULL, NULL, NULL, NULL),
(5, 'Radu Stan',        'MALE',       'Brasov',     'Romanian', 300, 33, 27, 40, 'Architect with a passion for travel and photography.',0, 0, '1991-09-30', 'ADVENTURE_SEEKER', NULL, NULL, NULL, NULL),
(6, 'Ioana Petrescu',   'FEMALE',     'Constanta',  'Romanian', 150, 29, 24, 36, 'Marine biology student. Swims every weekend.', 1, 0, '1995-12-11', 'ADVENTURE_SEEKER', NULL, NULL, NULL, NULL),
(7, 'Sam Luca',         'NON_BINARY', 'Sibiu',      'Romanian', 500, 26, 22, 33, 'Music producer. Coffee shop regular.',        0, 0, '1998-02-28', 'SOCIAL_EXPLORER', NULL, NULL, NULL, NULL),
(8, 'Ana Pop',          'FEMALE',     'Bihor',      'Romanian', 80, 30, 26, 37, 'UX designer who loves art and hiking.',        1, 1, '1994-06-03', 'DEEP_THINKER', NULL, NULL, NULL, NULL),
(9, 'Mihai Georgescu',  'MALE',       'Craiova',       'Romanian', 250, 35, 28, 42, 'Chef and football fan. Looking for something real.',0, 0, '1989-08-19', 'STABILITY_LOVER', NULL, NULL, NULL, NULL),
(10,'Alex Morgan',      'OTHER',      'Cluj-Napoca',       'Romanian', 60, 24, 18, 30, 'PhD student in literature. Pasta enthusiast.', 1, 0, '2000-01-07', NULL, NULL, NULL, NULL, NULL),

(11,'Adrian Pop',        'MALE',       'Cluj-Napoca', 'Romanian', 120, 28, 23, 35, 'Gym enthusiast and coffee lover.', 1, 0, '1996-04-12', 'ADVENTURE_SEEKER', NULL, NULL, NULL, NULL),
(12,'Bianca Muresan',    'FEMALE',     'Cluj-Napoca', 'Romanian', 80, 25, 22, 32, 'Bookworm and yoga fan.', 0, 0, '1999-09-21', 'DEEP_THINKER', NULL, NULL, NULL, NULL),
(13,'Cristian Radu',     'MALE',       'Cluj-Napoca', 'Romanian', 150, 31, 25, 38, 'Tech geek and gamer.', 1, 0, '1993-07-08', 'SOCIAL_EXPLORER', NULL, NULL, NULL, NULL),
(14,'Diana Moldovan',    'FEMALE',     'Cluj-Napoca', 'Romanian', 70, 24, 21, 30, 'Student who loves traveling.', 1, 0, '2000-02-17', 'EMPATHETIC_CONNECTOR', NULL, NULL, NULL, NULL),
(15,'Eduard Toma',       'MALE',       'Cluj-Napoca', 'Romanian', 200, 33, 27, 40, 'Entrepreneur and foodie.', 0, 0, '1991-11-03', 'STABILITY_LOVER', NULL, NULL, NULL, NULL),
(16,'Flavia Popescu',    'FEMALE',     'Cluj-Napoca', 'Romanian', 90, 27, 23, 34, 'Designer and plant lover.', 1, 0, '1997-06-25', 'DEEP_THINKER', NULL, NULL, NULL, NULL),
(17,'George Stan',       'MALE',       'Cluj-Napoca', 'Romanian', 180, 35, 28, 42, 'Photographer and traveler.', 0, 0, '1989-01-14', 'ADVENTURE_SEEKER', NULL, NULL, NULL, NULL),
(18,'Horia Iancu',       'MALE',       'Cluj-Napoca', 'Romanian', 100, 29, 24, 36, 'Runner and coffee addict.', 1, 0, '1995-03-30', 'SOCIAL_EXPLORER', NULL, NULL, NULL, NULL),
(19,'Irina Petru',       'FEMALE',     'Cluj-Napoca', 'Romanian', 60, 26, 22, 33, 'Marketing specialist and dog lover.', 1, 0, '1998-10-09', 'EMPATHETIC_CONNECTOR', NULL, NULL, NULL, NULL),
(20,'Ionut Badea',       'MALE',       'Cluj-Napoca', 'Romanian', 140, 32, 25, 39, 'Football fan and home chef.', 0, 0, '1992-05-19', 'STABILITY_LOVER', NULL, NULL, NULL, NULL),
(21,'Julia Rusu',        'FEMALE',     'Cluj-Napoca', 'Romanian', 75, 24, 21, 30, 'Art student and dreamer.', 1, 0, '2000-08-11', 'SOCIAL_EXPLORER', NULL, NULL, NULL, NULL),
(22,'Klaus Wagner',      'MALE',       'Cluj-Napoca', 'Romanian', 130, 34, 27, 41, 'Engineer who loves hiking.', 0, 0, '1990-12-22', 'ADVENTURE_SEEKER', NULL, NULL, NULL, NULL),
(23,'Larisa Enache',     'FEMALE',     'Cluj-Napoca', 'Romanian', 85, 28, 24, 36, 'Teacher and reader.', 1, 0, '1996-01-27', 'DEEP_THINKER', NULL, NULL, NULL, NULL),
(24,'Mihai Costin',      'MALE',       'Cluj-Napoca', 'Romanian', 160, 31, 25, 38, 'Startup enthusiast.', 0, 0, '1993-09-05', 'SOCIAL_EXPLORER', NULL, NULL, NULL, NULL),
(25,'Nicoleta Sandu',    'FEMALE',     'Cluj-Napoca', 'Romanian', 70, 25, 22, 32, 'Fitness and nutrition lover.', 1, 0, '1999-04-14', 'ADVENTURE_SEEKER', NULL, NULL, NULL, NULL),
(26,'Ovidiu Marin',      'MALE',       'Cluj-Napoca', 'Romanian', 120, 30, 24, 37, 'Cycling and photography.', 0, 0, '1994-02-02', 'ADVENTURE_SEEKER', NULL, NULL, NULL, NULL),
(27,'Paula Dobre',       'FEMALE',     'Cluj-Napoca', 'Romanian', 65, 26, 22, 33, 'Yoga instructor.', 1, 0, '1998-07-18', 'EMPATHETIC_CONNECTOR', NULL, NULL, NULL, NULL),
(28,'Razvan Ilie',       'MALE',       'Cluj-Napoca', 'Romanian', 140, 33, 26, 40, 'Finance guy who loves cooking.', 0, 0, '1991-06-06', 'STABILITY_LOVER', NULL, NULL, NULL, NULL),
(29,'Simona Luca',       'FEMALE',     'Cluj-Napoca', 'Romanian', 80, 27, 23, 34, 'Music and festivals.', 1, 0, '1997-03-12', 'SOCIAL_EXPLORER', NULL, NULL, NULL, NULL),
(30,'Tudor Neagu',       'MALE',       'Cluj-Napoca', 'Romanian', 150, 35, 28, 42, 'Traveler and storyteller.', 0, 0, '1989-10-25', 'ADVENTURE_SEEKER', NULL, NULL, NULL, NULL);


-- ProfileInterests
INSERT INTO ProfileInterests (userId, interest) VALUES
(1,  'Hiking'),        (1,  'Coffee'),       (1,  'Gaming'),
(2,  'Yoga'),          (2,  'Painting'),      (2,  'Dogs'),
(3,  'Cooking'),       (3,  'Finance'),       (3,  'Travel'),
(4,  'Writing'),       (4,  'Wine'),          (4,  'Cinema'),
(5,  'Photography'),   (5,  'Architecture'),  (5,  'Travel'),
(6,  'Swimming'),      (6,  'Biology'),       (6,  'Diving'),
(7,  'Music'),         (7,  'Coffee'),        (7,  'Vinyl Records'),
(8,  'Art'),           (8,  'Hiking'),        (8,  'Design'),
(9,  'Football'),      (9,  'Cooking'),       (9,  'Travel'),
(10, 'Reading'),       (10, 'Cooking'),       (10, 'History');

-- ProfilePreferences
INSERT INTO ProfilePreferences (userId, gender) VALUES
(1,  'FEMALE'),
(2,  'MALE'),
(3,  'FEMALE'),
(4,  'MALE'),
(5,  'FEMALE'),
(6,  'MALE'),
(7,  'FEMALE'),
(7,  'NON_BINARY'),
(8,  'MALE'),
(8,  'FEMALE'),
(9,  'FEMALE'),
(10, 'MALE'),
(10, 'NON_BINARY');

-- Photos
INSERT INTO Photos (userId, location, profileOrderIndex) VALUES
(1,  'https://cdn.matchmaking.app/photos/user1_photo1.jpg', 1),
(1,  'https://cdn.matchmaking.app/photos/user1_photo2.jpg', 2),
(2,  'https://cdn.matchmaking.app/photos/user2_photo1.jpg', 1),
(3,  'https://cdn.matchmaking.app/photos/user3_photo1.jpg', 1),
(3,  'https://cdn.matchmaking.app/photos/user3_photo2.jpg', 2),
(4,  'https://cdn.matchmaking.app/photos/user4_photo1.jpg', 1),
(5,  'https://cdn.matchmaking.app/photos/user5_photo1.jpg', 1),
(6,  'https://cdn.matchmaking.app/photos/user6_photo1.jpg', 1),
(6,  'https://cdn.matchmaking.app/photos/user6_photo2.jpg', 2),
(7,  'https://cdn.matchmaking.app/photos/user7_photo1.jpg', 1),
(8,  'https://cdn.matchmaking.app/photos/user8_photo1.jpg', 1),
(9,  'https://cdn.matchmaking.app/photos/user9_photo1.jpg', 1),
(10, 'https://cdn.matchmaking.app/photos/user10_photo1.jpg', 1);

INSERT INTO ProfilePreferences (userId, gender) VALUES
-- Adrian Pop (11)
(11, 'FEMALE'),

-- Bianca Muresan (12)
(12, 'MALE'),

-- Cristian Radu (13)
(13, 'FEMALE'),

-- Diana Moldovan (14)
(14, 'MALE'),

-- Eduard Toma (15)
(15, 'FEMALE'),

-- Flavia Popescu (16)
(16, 'MALE'),

-- George Stan (17)
(17, 'FEMALE'),

-- Horia Iancu (18)
(18, 'FEMALE'),

-- Irina Petru (19)
(19, 'MALE'),

-- Ionut Badea (20)
(20, 'FEMALE'),

-- Julia Rusu (21)
(21, 'MALE'),

-- Klaus Wagner (22)
(22, 'FEMALE'),

-- Larisa Enache (23)
(23, 'MALE'),

-- Mihai Costin (24)
(24, 'FEMALE'),

-- Nicoleta Sandu (25)
(25, 'MALE'),

-- Ovidiu Marin (26)
(26, 'FEMALE'),

-- Paula Dobre (27)
(27, 'MALE'),

-- Razvan Ilie (28)
(28, 'FEMALE'),

-- Simona Luca (29)
(29, 'MALE'),

-- Tudor Neagu (30)
(30, 'FEMALE'),

-- Add a few more diverse preferences (optional realism)
(18, 'NON_BINARY'),
(21, 'NON_BINARY'),
(24, 'NON_BINARY'),
(27, 'FEMALE'),
(30, 'NON_BINARY');

GO

CREATE TRIGGER trg_DeleteProfile
ON Profiles
INSTEAD OF DELETE
AS
BEGIN
    DECLARE @id INT = (SELECT userId FROM deleted);

    DELETE FROM Notifications  WHERE recipientId = @id OR fromId   = @id;
    DELETE FROM Interactions   WHERE fromProfileId = @id OR toProfileId = @id;
    DELETE FROM Matches        WHERE user1Id = @id OR user2Id = @id;
    DELETE FROM ProfilePreferences WHERE userId = @id;
    DELETE FROM ProfileInterests   WHERE userId = @id;
    DELETE FROM Bids               WHERE userId = @id;
    DELETE FROM Photos             WHERE userId = @id;
    DELETE FROM DatingAdmin        WHERE userId = @id;
    DELETE FROM Profiles           WHERE userId = @id;
END;
GO

SELECT *
FROM Profiles