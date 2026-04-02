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
    userId INT FOREIGN KEY REFERENCES Profiles(userId) ON DELETE CASCADE,
    gender VARCHAR(50),
    PRIMARY KEY(userId, gender)
);
GO

CREATE TABLE Bids (
    bidId INT IDENTITY(1, 1) PRIMARY KEY,
    userId INT FOREIGN KEY REFERENCES Profiles(userId) ON DELETE CASCADE,
    bidSum INT
);
GO

CREATE TABLE Photos (
    photoId INT IDENTITY(1, 1) PRIMARY KEY,
    userId INT FOREIGN KEY REFERENCES Profiles(userId) ON DELETE CASCADE,
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
    userId INT FOREIGN KEY REFERENCES Profiles(userId) ON DELETE CASCADE,
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

INSERT INTO Profiles 
(userId, [name], gender, location, nationality, maxDistance, age, minPrefAge, maxPrefAge, bio, displayStarSign, isArchived, dateOfBirth, loverType, isHotSeat, boost, boostDay, hotSeatDay)
VALUES
(1,  'Andrei Pop',         'MALE',       'Cluj-Napoca', 'Romanian', 70,  27, 22, 35, 'Hiking and photography are my therapy. Weekends on summits, weekdays in logistics. Learning guitar badly but enthusiastically.',                        1, 0, '1997-03-14', 'ADVENTURE_SEEKER',     0, 0, NULL, NULL),
(2,  'Maria Ionescu',      'FEMALE',     'Cluj-Napoca', 'Romanian', 60,  24, 20, 30, 'Art history student, coffee snob, cinema lover. I can talk for hours about film or why brutalist architecture is underrated.',                           0, 0, '2000-07-22', 'DEEP_THINKER',         0, 0, NULL, NULL),
(3,  'Bogdan Muresan',     'MALE',       'Dej',         'Romanian', 80,  31, 25, 38, 'I know everyone at the party and remember your birthday. Events organiser who lives for bringing people together.',                                      1, 0, '1993-11-05', 'SOCIAL_EXPLORER',      0, 0, NULL, NULL),
(4,  'Elena Vlad',         'FEMALE',     'Cluj-Napoca', 'Romanian', 55,  26, 22, 33, 'Speech therapist who believes listening is an art. I show care through small gestures. Slow mornings, markets and honest conversations.',                 1, 0, '1998-05-18', 'EMPATHETIC_CONNECTOR', 0, 0, NULL, NULL),
(5,  'Razvan Chis',        'MALE',       'Dej',         'Romanian', 90,  34, 28, 42, 'Civil engineer. I renovated my own house, grow tomatoes and make my own țuică. Not flashy — just genuine.',                                             0, 0, '1990-09-30', 'STABILITY_LOVER',      0, 0, NULL, NULL),
(6,  'Ioana Sabau',        'FEMALE',     'Cluj-Napoca', 'Romanian', 65,  22, 18, 28, 'Med student surviving on coffee and spite. Into yoga, indie films and debating whether a hot dog is a sandwich.',                                        0, 0, '2002-01-11', NULL,                   0, 0, NULL, NULL),
(7,  'Alexandru Moga',     'MALE',       'Cluj-Napoca', 'Romanian', 70,  29, 24, 36, 'Rock climber and photographer. Software dev by week, summit chaser by weekend. Two local photo exhibitions — weirdly proud of that.',                    1, 0, '1995-06-23', 'ADVENTURE_SEEKER',     0, 0, NULL, NULL),
(8,  'Diana Botar',        'FEMALE',     'Dej',         'Romanian', 75,  28, 23, 35, 'Librarian with three cats and a reading list I''ll never finish. Quiet until I''m not — get me on the right topic and I won''t stop.',                  1, 0, '1996-12-03', 'DEEP_THINKER',         0, 0, NULL, NULL),
(9,  'Mihai Suciu',        'MALE',       'Cluj-Napoca', 'Romanian', 60,  33, 27, 40, 'Startup co-founder into urban cycling and behavioural economics. Looking for someone curious and not afraid of deep conversations.',                      0, 0, '1991-08-17', 'SOCIAL_EXPLORER',      0, 0, NULL, NULL),
(10, 'Raluca Ferenczi',    'FEMALE',     'Dej',         'Romanian', 70,  25, 20, 31, 'Dance instructor with a weakness for mountains and terrible puns. I will eventually try to teach you salsa. Life''s too short for bad energy.',           1, 0, '1999-04-09', 'ADVENTURE_SEEKER',     0, 0, NULL, NULL),
(11, 'Alex Toma',          'NON_BINARY', 'Cluj-Napoca', 'Romanian', 80,  27, 21, 34, 'Visual artist in mixed media. I collect experiences obsessively. Queer, neurodivergent, done with surface-level — let''s be real.',                     0, 0, '1997-10-25', 'DEEP_THINKER',         0, 0, NULL, NULL),
(12, 'Cristina Moldovan',  'FEMALE',     'Cluj-Napoca', 'Romanian', 65,  30, 25, 38, 'Trainee psychologist. I will notice your attachment style — kindly. Trail running, wild swimming and cooking for people I care about.',                  1, 0, '1994-02-28', 'EMPATHETIC_CONNECTOR', 0, 0, NULL, NULL),
(13, 'Tudor Balint',       'MALE',       'Dej',         'Romanian', 85,  36, 30, 44, 'Structural engineer and single dad to a funny seven-year-old. Weekends mean football, pancakes and cartoons — and I genuinely love it.',                 0, 0, '1988-07-04', 'STABILITY_LOVER',      0, 0, NULL, NULL),
(14, 'Sonia Achim',        'FEMALE',     'Cluj-Napoca', 'Romanian', 50,  23, 19, 29, 'Literature student writing poetry at 2am. I haunt bookshops and jazz bars. An old soul who feels things very thoroughly.',                               1, 0, '2001-09-13', NULL,                   0, 0, NULL, NULL),
(15, 'Radu Marginean',     'MALE',       'Cluj-Napoca', 'Romanian', 75,  32, 26, 39, 'Sunday league footballer who then spends hours cooking something elaborate. IT project manager — organised enough to plan a decent date.',                0, 0, '1992-05-01', 'SOCIAL_EXPLORER',      0, 0, NULL, NULL),
(16, 'Jordan Nistor',      'NON_BINARY', 'Dej',         'Romanian', 90,  29, 23, 37, 'Bassist in a post-rock band. I think slowly, feel deeply, communicate directly. Pan, non-binary, done with surface-level anything.',                     1, 0, '1995-12-19', 'EMPATHETIC_CONNECTOR', 0, 0, NULL, NULL),
(17, 'Gabriela Popa',      'FEMALE',     'Dej',         'Romanian', 70,  31, 26, 39, 'Personal trainer who still loves mornings. Competitive at workouts and board games equally. I cook healthy food that actually tastes good.',              1, 0, '1993-03-27', 'ADVENTURE_SEEKER',     0, 0, NULL, NULL),
(18, 'Vlad Cosma',         'MALE',       'Cluj-Napoca', 'Romanian', 65,  28, 23, 35, 'UX designer, small circle, deep friendships. Philosophy, chess at midnight, cycling everywhere. Introverted but intentional.',                           0, 0, '1996-11-08', 'DEEP_THINKER',         0, 0, NULL, NULL),
(19, 'Simona Rus',         'FEMALE',     'Cluj-Napoca', 'Romanian', 60,  35, 28, 43, 'Accountant with her life in order. I meal prep, water my plants and want someone consistent and kind. Drama-free, please.',                              1, 0, '1989-06-15', 'STABILITY_LOVER',      0, 0, NULL, NULL),
(20, 'Ionut Dragomir',     'MALE',       'Dej',         'Romanian', 80,  26, 21, 33, 'Motorsport fan, traveller, eats everything. Watches every F1 race live. Easygoing until you insult Senna — then we have a problem.',                    0, 0, '1998-02-20', NULL,                   0, 0, NULL, NULL);
GO

INSERT INTO ProfileInterests (userId, interest) VALUES
(1, 'Hiking'),
(1, 'Photography'),
(1, 'Travel'),
(1, 'Music'),
(1, 'Coffee'),
(1, 'Football'),

(2, 'Art'),
(2, 'Cinema'),
(2, 'Coffee'),
(2, 'Reading'),
(2, 'Architecture'),
(2, 'Writing'),

(3, 'Football'),
(3, 'Music'),
(3, 'Travel'),
(3, 'Cooking'),
(3, 'Coffee'),
(3, 'Gaming'),

(4, 'Yoga'),
(4, 'Cooking'),
(4, 'Reading'),
(4, 'Music'),
(4, 'Art'),
(4, 'Photography'),

(5, 'Cooking'),
(5, 'History'),
(5, 'Football'),
(5, 'Wine'),
(5, 'Dogs'),
(5, 'Finance'),

(6, 'Yoga'),
(6, 'Cinema'),
(6, 'Biology'),
(6, 'Coffee'),
(6, 'Swimming'),
(6, 'Reading'),
(6, 'Music'),

(7, 'Hiking'),
(7, 'Photography'),
(7, 'Travel'),
(7, 'Gaming'),
(7, 'Coffee'),
(7, 'Design'),

(8, 'Reading'),
(8, 'Writing'),
(8, 'Art'),
(8, 'Music'),
(8, 'Coffee'),
(8, 'Cinema'),

(9, 'Finance'),
(9, 'Travel'),
(9, 'Coffee'),
(9, 'Design'),
(9, 'Hiking'),
(9, 'Gaming'),

(10, 'Music'),
(10, 'Travel'),
(10, 'Hiking'),
(10, 'Dogs'),
(10, 'Yoga'),
(10, 'Photography'),
(10, 'Dancing'),

(11, 'Art'),
(11, 'Design'),
(11, 'Music'),
(11, 'Photography'),
(11, 'Cinema'),
(11, 'Vinyl Records'),

(12, 'Hiking'),
(12, 'Swimming'),
(12, 'Cooking'),
(12, 'Reading'),
(12, 'Music'),
(12, 'Dogs'),
(12, 'Biology'),

(13, 'Football'),
(13, 'Cooking'),
(13, 'History'),
(13, 'Dogs'),
(13, 'Finance'),
(13, 'Music'),

(14, 'Reading'),
(14, 'Writing'),
(14, 'Music'),
(14, 'Cinema'),
(14, 'Vinyl Records'),
(14, 'Art'),
(14, 'Coffee'),

(15, 'Football'),
(15, 'Cooking'),
(15, 'Travel'),
(15, 'Music'),
(15, 'Gaming'),
(15, 'Coffee'),

(16, 'Music'),
(16, 'Vinyl Records'),
(16, 'Art'),
(16, 'Cinema'),
(16, 'Writing'),
(16, 'Coffee'),
(16, 'Design'),

(17, 'Hiking'),
(17, 'Cooking'),
(17, 'Dogs'),
(17, 'Travel'),
(17, 'Swimming'),
(17, 'Music'),

(18, 'Design'),
(18, 'Reading'),
(18, 'History'),
(18, 'Coffee'),
(18, 'Vinyl Records'),
(18, 'Cinema'),
(18, 'Photography'),

(19, 'Cooking'),
(19, 'Wine'),
(19, 'Reading'),
(19, 'Finance'),
(19, 'Yoga'),
(19, 'Dogs'),

(20, 'Travel'),
(20, 'Football'),
(20, 'Coffee'),
(20, 'Gaming'),
(20, 'Music'),
(20, 'Cooking'),
(20, 'Photography');
GO

INSERT INTO ProfilePreferences (userId, gender) VALUES
(1,  'FEMALE'),
(2,  'MALE'),
(3,  'FEMALE'),
(4,  'MALE'),
(4,  'FEMALE'),
(5,  'FEMALE'),
(6,  'MALE'),
(6,  'FEMALE'),
(7,  'FEMALE'),
(8,  'MALE'),
(8,  'FEMALE'),
(9,  'FEMALE'),
(10, 'MALE'),
(11, 'MALE'),
(11, 'FEMALE'),
(11, 'NON_BINARY'),
(12, 'MALE'),
(12, 'FEMALE'),
(13, 'FEMALE'),
(14, 'MALE'),
(15, 'FEMALE'),
(16, 'MALE'),
(16, 'FEMALE'),
(16, 'NON_BINARY'),
(17, 'MALE'),
(18, 'FEMALE'),
(19, 'MALE'),
(19, 'FEMALE'),
(20, 'FEMALE');
GO

INSERT INTO Photos (userId, location, profileOrderIndex) VALUES
(1,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(1,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(2,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(2,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(3,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(3,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(4,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(4,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(5,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(5,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(6,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(6,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(7,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(7,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(8,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(8,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(9,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(9,  'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(10, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(10, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(11, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(11, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(12, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(12, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(13, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(13, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(14, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(14, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(15, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(15, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(16, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(16, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(17, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(17, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(18, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(18, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(19, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(19, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1),
(20, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\placeholder.png', 0),
(20, 'C:\UBB-SE-2026-digital-cupids\matchmaking\matchmaking\Assets\map.jpg',         1);
GO
