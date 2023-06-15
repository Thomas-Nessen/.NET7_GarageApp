USE GarageDB;

DROP TABLE IF EXISTS [Garage];

CREATE TABLE [Garage] (
[id] INT identity primary key,
[name] VARCHAR(30) UNIQUE,
[car_id] VARCHAR(30) UNIQUE,
[parking_spot] VARCHAR(30) UNIQUE NULL,
[parked_ind] BIT, 
[start_park_time] DATETIME2 NULL,
[end_park_time] DATETIME2 NULL,
);

INSERT INTO [Garage] VALUES
('Frenk', 'AB-123-C', 'A1', 1, '2023-3-28 10:58:12', '2023-3-28 12:12:12'),
('Thomas', 'ZZ-999-Z', NULL, 0, NULL, NULL),
('Koen', 'KO-333-N', 'A2', 1, '2023-3-27 14:23:52', '2023-3-28 08:10:12'),
('Klaartje', 'RZ-023-R', 'B3', 1, '2023-3-28 10:58:12', '2023-3-28 12:12:12')