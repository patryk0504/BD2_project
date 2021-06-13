use projektBD
go

-- tabela points
create table Points (ID int IDENTITY(1,1) PRIMARY KEY, point dbo.Point);

-- tabela polygons
create table Polygons (ID int IDENTITY(1,1) PRIMARY KEY, polygon dbo.Polygon);