use projektBD;
go

--helper function to imitate vaarags input parameter
CREATE FUNCTION dbo.SplitInts
(
   @List       VARCHAR(MAX),
   @Delimiter  CHAR(1)
)
RETURNS TABLE
AS
   RETURN 
   (
       SELECT Item = CONVERT(INT, Item)
       FROM
       (
           SELECT Item = x.i.value('(./text())[1]', 'INT')
           FROM
           (
               SELECT [XML] = CONVERT(XML, '<i>' 
                    + REPLACE(@List, @Delimiter, '</i><i>') 
                    + '</i>').query('.')
           ) AS a
           CROSS APPLY
           [XML].nodes('i') AS x(i)
       ) AS y
       WHERE Item IS NOT NULL
   );
go

-- function to insert polygons using points id
CREATE PROCEDURE dbo.InsertPolygonFromPointsTable
    @List VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @Points VARCHAR(8000) 

    SELECT @Points = COALESCE(@Points + '/','') + point.ToString() FROM dbo.Points AS t
        INNER JOIN dbo.SplitInts(@List, ',') AS list
        ON t.ID = list.Item;
	
	insert into dbo.Polygons (polygon) values (@Points);
END
GO