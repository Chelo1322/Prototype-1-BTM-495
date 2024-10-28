CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL,
    Role VARCHAR(20) CHECK (Role IN ('Manager', 'Waitress', 'Cook')),
    PasswordHash VARCHAR(255) NOT NULL
);
