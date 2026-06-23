-- Create database in the library LibraryDB
CREATE DATABASE LibraryDB;
GO

-- Take an avive DB to the following querys
USE LibraryDB;
GO

CREATE TABLE test_table (
    id int IDENTITY
)
GO

-- Erase Table, CAUTION with this element is only for test
DROP TABLE dbo.test_table;
GO