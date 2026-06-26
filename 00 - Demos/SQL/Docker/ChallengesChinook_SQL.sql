/* ----------------------------------------------------------------------------- */
/*------------------------------ BASIC CHALLENGES -------------------------------*/
/* ----------------------------------------------------------------------------- */

-- List all customers (full name, customer id, and country) who are not in the USA
SELECT FirstName + ' ' + LastName,
        CustomerId, 
        Country
FROM dbo.Customer
WHERE Country != 'USA';

-- List all customers from Brazil
SELECT * 
FROM dbo.Customer
WHERE Country = 'Brazil';

-- List all sales agents -- SELECT * FROM employee WHERE title LIKE '%Agent%;
SELECT * 
FROM dbo.Employee
WHERE Title LIKE '%Agent%';

-- Retrieve a list of all countries in billing addresses on invoices
SELECT DISTINCT BillingCountry 
FROM dbo.Invoice;

-- Retrieve how many invoices there were in 2009?, and what was the sales total for that year?
SELECT 
    COUNT(InvoiceId) AS ManyInvoices,
    YEAR(InvoiceDate) AS _Year, 
    SUM(Total) AS SalesTotal
FROM dbo.Invoice
WHERE YEAR(InvoiceDate) = '2024'
GROUP BY YEAR(InvoiceDate);

-- (challenge: find the invoice count sales total for every year using one query)
SELECT
    YEAR(InvoiceDate) As _Year,
    COUNT(InvoiceId) AS CountSales,
    SUM(Total) AS SalesTotal
FROM dbo.Invoice
GROUP BY YEAR(InvoiceDate);

-- how many line items were there for invoice #37
SELECT COUNT(*) AS TotalLineItems 
FROM dbo.InvoiceLine
WHERE InvoiceId = 37;

-- how many invoices per country? BillingCountry  # of invoices 
SELECT
    COUNT(*) AS InvoicesPerCountry,
    BillingCountry
FROM dbo.Invoice
GROUP BY BillingCountry;

-- Retrieve the total sales per country, ordered by the highest total sales first.
SELECT
    SUM(Total) AS SalesPerCountry,
    BillingCountry
FROM dbo.Invoice
GROUP BY BillingCountry
ORDER BY SUM(Total) DESC;


/* ----------------------------------------------------------------------------- */
/*------------------------------ JOINS CHALLENGES -------------------------------*/
/* ----------------------------------------------------------------------------- */

-- Every Album by Artist
SELECT 
    al.*,
    ar.Name
FROM DBO.Album AS al
JOIN DBO.Artist AS ar ON ar.ArtistId = al.ArtistId
ORDER BY ar.Name ASC;

-- All songs of the rock genre
SELECT
    t.*
FROM dbo.Track AS t
JOIN dbo.Genre as g ON g.GenreId = t.GenreId
WHERE g.Name = 'rock';

-- Show all invoices of customers from brazil (mailing address not billing)
SELECT 
    i.*,
    c.FirstName + ' ' + c.LastName AS FullName
FROM dbo.Invoice AS i
JOIN dbo.Customer AS c ON c.CustomerId = i.CustomerId
where c.Country = 'Brazil'; 

-- Show all invoices together with the name of the sales agent for each one
SELECT
    i.*,
    e.FirstName + ' ' + e.LastName AS AgentFullName
FROM dbo.Invoice AS i
JOIN dbo.Employee AS e ON e.EmployeeId = i.CustomerId
JOIN dbo.Customer AS c ON i.CustomerId = c.CustomerId;

-- Which sales agent made the most sales in 2009?
SELECT
    e.FirstName + ' ' + e.LastName AS AgentFullName,
    SUM(i.Total) AS SalesTotal
FROM dbo.Employee AS e
JOIN dbo.Customer AS c ON c.SupportRepId = e.EmployeeId
JOIN dbo.Invoice AS i ON i.CustomerId = c.CustomerId
WHERE YEAR(i.InvoiceDate) = 2024
GROUP BY e.EmployeeId, e.FirstName, e.LastName
ORDER BY SalesTotal DESC;

-- How many customers are assigned to each sales agent?
SELECT
    e.EmployeeId,
    COUNT(c.SupportRepId) AS NumOfCustomers
FROM dbo.Customer as c
JOIN dbo.Employee AS e ON e.EmployeeId = c.SupportRepId
GROUP BY EmployeeId;

-- Which track was purchased the most in 2024?
SELECT TOP 1
    t.TrackId,
    t.Name,
    SUM(il.Quantity) AS TotalPurchased,
    YEAR(i.InvoiceDate) AS _Year
FROM dbo.Track AS t
JOIN dbo.InvoiceLine AS il ON il.TrackId = t.TrackId
JOIN dbo.Invoice AS i ON i.InvoiceId = il.InvoiceId
WHERE YEAR(i.InvoiceDate) = 2024
GROUP BY t.TrackId, t.Name, YEAR(i.InvoiceDate)
ORDER BY TotalPurchased DESC;

-- Show the top three best selling artists.
SELECT TOP 3
    a.*,
    SUM(il.UnitPrice*il.Quantity) AS SalesTotal
FROM dbo.Artist AS a
JOIN dbo.Album AS al ON al.ArtistId = a.ArtistId
JOIN dbo.Track AS t ON t.AlbumId = al.AlbumId
JOIN dbo.InvoiceLine AS il ON il.TrackId = t.TrackId
GROUP BY a.ArtistId, a.Name
ORDER BY SalesTotal DESC;

-- Which customers have the same initials as at least one other customer?
SELECT 
    CustomerId, 
    FirstName + ' ' + LastName AS FullName,
    SUBSTRING(FirstName, 1, 1) + SUBSTRING(LastName, 1, 1) AS Initials
FROM dbo.Customer
WHERE SUBSTRING(FirstName, 1, 1) + SUBSTRING(LastName, 1, 1) IN (
    SELECT SUBSTRING(FirstName, 1, 1) + SUBSTRING(LastName, 1, 1)
    FROM dbo.Customer
    GROUP BY SUBSTRING(FirstName, 1, 1) + SUBSTRING(LastName, 1, 1)
    HAVING COUNT(*) > 1
)
ORDER BY Initials;

-- Which countries have the most invoices?
SELECT TOP 3
    BillingCountry,
    COUNT(InvoiceId) AS TotalInvoices
FROM dbo.Invoice
GROUP BY BillingCountry
ORDER BY TotalInvoices DESC;

-- Which city has the customer with the highest sales total?
SELECT TOP 1
    c.City,
    SUM(i.Total) AS SalesTotal
FROM dbo.Customer AS c
JOIN dbo.Invoice AS i ON i.CustomerId = c.CustomerId
GROUP BY c.City
ORDER BY SalesTotal DESC;

-- Who is the highest spending customer?
SELECT TOP 1
    c.CustomerId,
    c.FirstName + ' ' + c.LastName AS CustomerName,
    SUM(i.Total) AS TotalSpent
FROM dbo.Customer AS c
JOIN dbo.Invoice AS i ON i.CustomerId = c.CustomerId
GROUP BY c.CustomerId, c.FirstName, c.LastName
ORDER BY TotalSpent DESC;

-- Return the email and full name of of all customers who listen to Rock.
SELECT DISTINCT
    c.Email,
    c.FirstName + ' ' + c.LastName AS FullName
FROM dbo.Customer AS c
JOIN dbo.Invoice AS i ON i.CustomerId = c.CustomerId
JOIN dbo.InvoiceLine AS il ON il.InvoiceId = i.InvoiceId
JOIN dbo.Track AS t ON t.TrackId = il.TrackId
JOIN dbo.Genre AS g ON g.GenreId = t.GenreId
WHERE g.Name = 'Rock';

-- Which artist has written the most Rock songs?
SELECT TOP 1
    a.ArtistId,
    a.Name AS ArtistName,
    COUNT(t.TrackId) AS TotalRockSongs
FROM dbo.Artist AS a
JOIN dbo.Album AS al ON al.ArtistId = a.ArtistId
JOIN dbo.Track AS t ON t.AlbumId = al.AlbumId
JOIN dbo.Genre AS g ON g.GenreId = t.GenreId
WHERE g.Name = 'Rock'
GROUP BY a.ArtistId, a.Name
ORDER BY TotalRockSongs DESC;

-- Which artist has generated the most revenue?
SELECT TOP 1
    a.ArtistId,
    a.Name AS ArtistName,
    SUM(il.UnitPrice * il.Quantity) AS TotalRevenue
FROM dbo.Artist AS a
JOIN dbo.Album AS al ON al.ArtistId = a.ArtistId
JOIN dbo.Track AS t ON t.AlbumId = al.AlbumId
JOIN dbo.InvoiceLine AS il ON il.TrackId = t.TrackId
GROUP BY a.ArtistId, a.Name
ORDER BY TotalRevenue DESC;


/* ----------------------------------------------------------------------------- */
/*--------------------------- ADVANCED CHALLENGES -------------------------------*/
/* ----------------------------------------------------------------------------- */

-- solve these with a mixture of joins, subqueries, CTE, and set operators.
-- solve at least one of them in two different ways, and see if the execution
-- plan for them is the same, or different.

-- Form1 - boss employee (the one who reports to nobody)
SELECT 
    EmployeeId, 
    FirstName + ' ' + LastName AS BossName,
    ReportsTo
FROM dbo.Employee
WHERE ReportsTo IS NULL;

-- Form 2 - boss employee (the one who reports to nobody)
SELECT 
    EmployeeId, 
    FirstName + ' ' + LastName AS BossName
FROM dbo.Employee
EXCEPT
SELECT 
    EmployeeId, 
    FirstName + ' ' + LastName AS BossName
FROM dbo.Employee
WHERE ReportsTo IS NOT NULL;


/* ----------------------------------------------------------------------------- */
/*------------------------------ DML exercises ----------------------------------*/
/* ----------------------------------------------------------------------------- */

-- 1. insert two new records into the employee table.
INSERT INTO [dbo].[Employee] ([LastName], [FirstName], [Title], [ReportsTo], [BirthDate], [HireDate], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email]) VALUES 
    ('Mendoza', 'Carlos', 'IT Support', 1, '1990-05-12', GETDATE(), 'Av. Juárez 123', 'Guadalajara', 'Jalisco', 'Mexico', '44100', '+52 33 1234 5678', NULL, 'carlos.mendoza@chinookcorp.com'),
    ('Gómez', 'Ana', 'Sales Support', 1, '1993-08-25', GETDATE(), 'Calle Mayor 45', 'Madrid', 'Madrid', 'Spain', '28001', '+34 91 123 4567', NULL, 'ana.gomez@chinookcorp.com');

-- 2. insert two new records into the tracks table.
INSERT INTO [dbo].[Track] ([Name], [AlbumId], [MediaTypeId], [GenreId], [Composer], [Milliseconds], [Bytes], [UnitPrice]) VALUES
    (N'BlueRed Light', 239, 1, 1, N'U22', 225854, 7453704, 0.99),
    (N'NoSurrender', 239, 1, 1, N'U22', 333505, 11221406, 0.99);

-- 3. update customer Aaron Mitchell's name to Robert Walter
UPDATE [Customer]
SET [FirstName] = 'Robert', [LastName] = 'Walter'
WHERE [FirstName] LIKE '%Aron%' AND [LastName] LIKE 'Mitchell';

-- 4. delete one of the employees you inserted.
DELETE FROM [dbo].[Employee]
WHERE [FirstName] = 'Carlos' AND [LastName] = 'Mendoza';

-- 5. delete customer Robert Walter.
DELETE FROM [dbo].[Customer]
WHERE [FirstName] = 'Robert' AND [LastName] = 'Walter';