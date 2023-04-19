Use Hotel
CREATE TABLE Hotels (
    ID int PRIMARY KEY IDENTITY(1,1) NOT NULL ,
	HotelName varchar(255),
    Phone varchar(50),
    Address varchar(255),
    Rating float,
    Picture varbinary(max)
);

CREATE TABLE RoomTypes (
    ID int PRIMARY KEY IDENTITY(1,1) NOT NULL ,
    Name varchar(50)
);

CREATE TABLE Rooms (
    ID int PRIMARY KEY IDENTITY(1,1) NOT NULL ,
    Number varchar(50),
    [Type] int,
    Capacity int,
    FOREIGN KEY ([Type]) REFERENCES RoomTypes(ID) ON DELETE CASCADE
);

CREATE TABLE HotelRoom (
	ID int PRIMARY KEY IDENTITY(1,1) NOT NULL ,
    HotelID int,
    RoomID int,
    Price float,
    FOREIGN KEY (HotelID) REFERENCES Hotels(ID) ON DELETE CASCADE,
    FOREIGN KEY (RoomID) REFERENCES Rooms(ID) ON DELETE CASCADE
);