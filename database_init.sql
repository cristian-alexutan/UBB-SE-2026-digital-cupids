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
    userId INT IDENTITY(1, 1) PRIMARY KEY,
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

-- Profiles (10 users)
INSERT INTO Profiles (gender, location, nationality, maxDistance, age, minPrefAge, maxPrefAge, bio, displayStarSign, isArchived, dateOfBirth, loverType, isHotSeat, boost, boostDay, hotSeatDay)
VALUES
('Male',       'Cluj-Napoca',  'Romanian',  100, 28, 22, 35, 'Software engineer who loves hiking and coffee.',         1, NULL, '1996-03-15', 'Deep Thinker',          NULL, NULL, NULL, NULL),
('Female',     'Bucuresti',    'Romanian',   50, 25, 23, 32, 'Artist and yoga instructor. Dog mom.',                   0, NULL, '1999-07-22', 'Social Explorer',        NULL, NULL, NULL, NULL),
('Male',       'Timisoara',    'Romanian',  200, 31, 25, 38, 'Finance guy by day, amateur chef by night.',             1, NULL, '1993-11-05', 'Stability Lover',        NULL, NULL, NULL, NULL),
('Female',     'Iasi',         'Romanian',   75, 27, 25, 34, 'Journalist. Lover of wine, books, and bad puns.',        1, NULL, '1997-04-18', 'Empathetic Connector',   NULL, NULL, NULL, NULL),
('Male',       'Brasov',       'Romanian',  300, 33, 27, 40, 'Architect with a passion for travel and photography.',   0, NULL, '1991-09-30', 'Adventure Seeker',       NULL, NULL, NULL, NULL),
('Female',     'Constanta',    'Romanian',  150, 29, 24, 36, 'Marine biology student. Swims every weekend.',           1, NULL, '1995-12-11', 'Adventure Seeker',       NULL, NULL, NULL, NULL),
('Non-Binary', 'Sibiu',        'Romanian',  500, 26, 22, 33, 'Music producer. Coffee shop regular.',                   0, NULL, '1998-02-28', 'Social Explorer',        NULL, NULL, NULL, NULL),
('Female',     'Oradea',       'Romanian',   80, 30, 26, 37, 'UX designer who loves art and hiking.',                  1,    1, '1994-06-03', 'Deep Thinker',           NULL, NULL, NULL, NULL),
('Male',       'Craiova',      'Romanian',  250, 35, 28, 42, 'Chef and football fan. Looking for something real.',     0, NULL, '1989-08-19', 'Stability Lover',        NULL, NULL, NULL, NULL),
('Other',      'Cluj-Napoca',  'Romanian',   60, 24, 18, 30, 'PhD student in literature. Pasta enthusiast.',           1, NULL, '2000-01-07', NULL,                     NULL, NULL, NULL, NULL);

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
INSERT INTO Photos (userId, location) VALUES
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

INSERT INTO Interactions (fromProfileId, toProfileId, [type]) VALUES
(1,  2,  'LIKE'),
(2,  1,  'LIKE'),
(1,  4,  'SUPER_LIKE'),
(4,  1,  'LIKE'),
(3,  6,  'LIKE'),
(6,  3,  'LIKE'),
(5,  8,  'SUPER_LIKE'),
(8,  5,  'LIKE'),
(7,  10, 'LIKE'),
(10, 7,  'LIKE'),
(9,  2,  'LIKE'),
(2,  9,  'PASS'),
(3,  4,  'PASS'),
(5,  6,  'LIKE'),
(6,  5,  'PASS'),
(7,  1,  'LIKE'),
(1,  7,  'PASS'),
(10, 3,  'SUPER_LIKE'),
(3,  10, 'PASS');

-- Matches
INSERT INTO Matches (user1Id, user2Id) VALUES
(1, 2),
(1, 4),
(3, 6),
(5, 8),
(7, 10);