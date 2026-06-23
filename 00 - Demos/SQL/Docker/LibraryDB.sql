USE LibraryDB;
CREATE TABLE dbo.Author
(
    -- Column-name  data-type    constrains(optional)
    AuthorId INT IDENTITY(1,1) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    BirthYear INT NULL, -- NULL here signifies that we INTEND for this to maybe be null

    -- After Idefine my columns, datatypes and basic constraints
    -- I can optionally add some nemed constrains. If I don't name constrains,
    -- nothing breaks BUT  I can make my life easier and make error messages more later
    -- functional/readable by explicitly naming my constrains
    CONSTRAINT PK_Author PRIMARY KEY (AuthorId),
    
    -- When someone tries to add an Author, make sure that BirthYear is either NULL or BETWEEN 300 & 2050
    CONSTRAINT CK_Author_BirthYear CHECK (BirthYear IS NULL OR BirthYear BETWEEN 300 AND 2050)
    -- if you THINK that you migth need to alter a table's constrains
    -- you should name them. It makes running ALTER TABLE commands easier Later
);
GO -- including my GO batch statment for MS SQL Server

-- SELECT * FROM dbo.Author

USE LibraryDB;
CREATE TABLE dbo.Member
(
    -- This is perhaps faster but not best practice. For exaple: lets say Iwant to change from MemberID as my PK
    -- to email as my PK. By doing 
    MemberId INT IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Fun fact, Identity will not reuse any nuimbers even if deleted
    FirstName VARCHAR(50) NOT NULL,
    LasName VARCHAR(50) NOT NULL,
    Email VARCHAR(125) NOT NULL UNIQUE,
    -- Using a DEFAULT constraint, if no value is provided, the built in GETDATE() function gets a value to satisfy the column
    JoinedDate DATE NOT NULL DEFAULT(GETDATE()),
);
GO

-- Book is our Largest table so far
USE LibraryDB;
CREATE TABLE dbo.Book
(
    BookId INT IDENTITY(1,1) NOT NULL,
    Title VARCHAR(200) NOT NULL,
    ISBN CHAR(13) NOT NULL,  -- UNIQUE
    PublishedYear INT NULL,
    CategoryName VARCHAR(60) NOT NULL CONSTRAINT DF_Book_CategoryName DEFAULT ('General'),
    AuthorId INT NOT NULL, --This will be a foreign key, we'll set the FK constraint below
    TotalCopies INT NOT NULL CONSTRAINT DF_Book_TotalCopies DEFAULT(1),
    AvailableCopies INT NOT NULL CONSTRAINT DF_Book_AvailabeCopies DEFAULT(1),
    -- More named constraints below
    CONSTRAINT PK_Book PRIMARY KEY (BookId),
    CONSTRAINT UQ_Book_ISBN UNIQUE(ISBN),
    
    -- Setting our first Foreign key constraint
    -- We need to tell the SQL engine, what column in this table is getting the FK constraint
    -- as well as what column in another existing table that FK points to
    CONSTRAINT FK_Book_Author FOREIGN KEY (AuthorId) REFERENCES dbo.Author(AuthorId),

    -- The final thing Iwant to do is enforce some logical rules about Available and Total copies
    -- AvailableCopies CANNOT exceed TotalCopies
    CONSTRAINT CK_Book_Copies CHECK(TotalCopies >= AvailableCopies)
)
GO

-- loan -- Two forign keys
-- represents the library loaning a book to a member
USE LibraryDB;
CREATE TABLE dbo.Loan
(
    LoanId INT IDENTITY(1,1) NOT NULL, -- PK
    BookId INT NOT NULL, -- FK
    MemberId INT NOT NULL, -- FK
    -- Date stamp for when the book was lent to the member
    LoanDate DATE NOT NULL CONSTRAINT DF_Loan_LoanDate DEFAULT(GETDATE()),
    DueDate DATE NOT NULL,
    ReturnDate DATE NULL,

    -- More named constraints below
    CONSTRAINT PK_Loan PRIMARY KEY (LoanId),
    -- Note: Technically, FK columns don't have to match the column in the table they are a PK in.
    CONSTRAINT FK_Loan_Book FOREIGN KEY (BookId) REFERENCES dbo.Book (BookId),
    CONSTRAINT FK_Loan_Member FOREIGN KEY (MemberId) REFERENCES dbo.Member (MemberId),
    CONSTRAINT CK_Loan_Dates CHECK(DueDate >=LoanDate) -- Due date
)
GO

-- Now that I've created the tables, using CREATE (DDL), how can I edit the tables themselves?

-- Let's add a column to an existing table, lets use dbo.Book
ALTER TABLE dbo.Book ADD Edition INT NOT NULL CONSTRAINT DF_Book_Edition DEFAULT(1);

-- We can get more granular, and not just add a new column, we can edit things about existing columns
-- I can use this ALTER TABLE + ALTER COLUMN to add or edit constraints
ALTER TABLE dbo.Book ALTER COLUMN Title VARCHAR(250) NOT NULL;

-- Ideadlly, we would never have to ALTER stuff. when possible, do it in CREATE. IN the real world,
-- you'll need to ALTER things about the tables in schema. Once you have data in a table,  you're
-- stuck ALTER-ing it. Prior to giving a table any data, it is often easier to drop the table and
-- edit the CREATE statement for it.

-- DROP and Truncate
-- DROP: removes a table entirely. Data is lost, the structure is also gone. Like it never existed.
DROP TABLE dbo.Loan;

-- TRUNCATE: removes all the data (rows) in table, leaves behind the structure.
TRUNCATE TABLE dbo.Loan;