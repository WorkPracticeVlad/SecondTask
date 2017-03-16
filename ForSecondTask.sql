CREATE TABLE OrganizationUnits
(
[Identity] nvarchar(225) NOT NULL PRIMARY KEY,
[Description] nvarchar(max),
[IsVirtual] bit,
[ParentIdentity] nvarchar(225)  /*FOREIGN KEY REFERENCES OrganizationUnits([Identity])*/
)
go
CREATE TABLE Properties
(
[Name] nvarchar(225) NOT NULL PRIMARY KEY,
[Type] nvarchar(max)
)
go
CREATE TABLE OrganizationUnitToProperties
(
[OrganizationUnitIdentity] nvarchar(225) NOT NULL FOREIGN KEY REFERENCES OrganizationUnits([Identity]),
[PropertyName] nvarchar(225) NOT NULL FOREIGN KEY REFERENCES Properties([Name]),
[Value] nvarchar(max),
CONSTRAINT PK_OrganizationUnitToProperty PRIMARY KEY NONCLUSTERED ([OrganizationUnitIdentity], [PropertyName]) 
)
go
create function fnGetOrganiztionUnitParent
(
@Identity as nvarchar(225)
)
returns nvarchar(225)
as
begin
declare @ParentIdentity nvarchar(225)
select @ParentIdentity=[ParentIdentity] from [dbo].[OrganizationUnits]
where [Identity]=@Identity
return @ParentIdentity
end
GO
CREATE FUNCTION fnGetIdentityByIdentiyTail
(
@IdentityTail as nvarchar(225)
)
returns nvarchar(225)
as
begin
declare @Identity nvarchar(225)
select @Identity=[Identity] from [dbo].[OrganizationUnits]
where [Identity] like '%'+@IdentityTail
return @Identity
end
GO
CREATE PROC SelectAllValuesForOrganizationUnitByIdentiyTail(@Identity as nvarchar(255))
AS
SET NOCOUNT ON;
BEGIN
set @Identity=[dbo].[fnGetIdentityByIdentiyTail](@Identity)
WHILE @Identity<>''
BEGIN
select [Identity],[PropertyName],[Value] from [dbo].[OrganizationUnits] 
left join [dbo].[OrganizationUnitToProperties] on
[dbo].[OrganizationUnits].[Identity]=[dbo].[OrganizationUnitToProperties].
[OrganizationUnitIdentity]
and [OrganizationUnitIdentity]=@Identity
where [Identity]=@Identity
set @Identity=[dbo].[fnGetOrganiztionUnitParent](@Identity);
END;
END;
SET NOCOUNT OFF;
GO
CREATE PROC SelectOrganizationUnitToAncestors(@Identity as nvarchar(255))
AS
SET NOCOUNT ON;
WHILE @Identity<>''
BEGIN
select * from [dbo].[OrganizationUnits]
where [Identity]=@Identity;
set @Identity=[dbo].[fnGetOrganiztionUnitParent](@Identity);
END;
SET NOCOUNT OFF;
GO
CREATE PROC RowsPerPage(@Page as int, @ItemsPerPage as int, @TableName as nvarchar(128), @ColumnToOrderBy as nvarchar(128))
AS
SET NOCOUNT ON;
BEGIN
DECLARE @SqlCommand NVARCHAR(MAX);
SET @SqlCommand= N'
SELECT  *
FROM '+ @TableName+N' 
ORDER BY '+@ColumnToOrderBy+N' 
OFFSET '+CAST(@Page*@ItemsPerPage-@ItemsPerPage AS NVARCHAR(10))+N' ROWS
FETCH NEXT '+CAST(@ItemsPerPage AS NVARCHAR(10))+N' ROWS ONLY;'
EXEC sp_executesql @SqlCommand
END;
SET NOCOUNT OFF;
GO
create function [dbo].[fnCountPages]
(
@Count as int,
@ItemsPerPage as int
)
returns int
as
begin
declare  @Remnant as int
declare @PageCount as int
set @Remnant=@Count%@ItemsPerPage;
set @PageCount=@Count/@ItemsPerPage;
if @Remnant!=0 
set @PageCount=@PageCount+1
return @PageCount
end
GO
create function fnCountPerTable
(
@TableName as nvarchar(128)
)
returns int
as
begin
declare @Count as int
SELECT @Count =SUM (row_count)
FROM sys.dm_db_partition_stats
WHERE object_id=OBJECT_ID(@TableName)   
AND (index_id=0 or index_id=1);
return @Count
end
GO
create function fnCountPagesInTable
(
@ItemsPerPage as int,
@TableName as nvarchar(128)
)
returns int
as
begin
declare @Count as int
declare @Remnant as int
select @Count= [dbo].[fnCountPerTable](@TableName)
return [dbo].[fnCountPages](@Count,@ItemsPerPage)
end
GO
CREATE PROC SelectAllFromTables
AS
SET NOCOUNT ON;
BEGIN
DECLARE @SqlCommand NVARCHAR(MAX);
SET @SqlCommand= N'EXEC sp_MSForEachTable "SELECT * FROM ?"'
EXEC sp_executesql @SqlCommand
END;
SET NOCOUNT OFF;
GO
CREATE PROC DeleteAllFromTables
AS
SET NOCOUNT ON;
BEGIN
DECLARE @SqlCommand NVARCHAR(MAX);
SET @SqlCommand= N'EXEC sp_MSForEachTable "DISABLE TRIGGER ALL ON ?"

EXEC sp_MSForEachTable "ALTER TABLE ? NOCHECK CONSTRAINT ALL"

EXEC sp_MSForEachTable "DELETE FROM ?"

EXEC sp_MSForEachTable "ALTER TABLE ? CHECK CONSTRAINT ALL"

EXEC sp_MSForEachTable "ENABLE TRIGGER ALL ON ?"'
EXEC sp_executesql @SqlCommand
END;
SET NOCOUNT OFF;
GO
CREATE PROC SelectAllFromTable( @TableName as nvarchar(128))
AS
SET NOCOUNT ON;
BEGIN
DECLARE @SqlCommand NVARCHAR(MAX);
SET @SqlCommand= N'SELECT * FROM '+ @TableName
EXEC sp_executesql @SqlCommand
END;
SET NOCOUNT OFF;
GO
CREATE PROC DeleteAllFromTable( @TableName as nvarchar(128))
AS
SET NOCOUNT ON;
BEGIN
DECLARE @SqlCommand NVARCHAR(MAX);
SET @SqlCommand= N'DELETE FROM '+ @TableName
EXEC sp_executesql @SqlCommand
END;
SET NOCOUNT OFF;
GO
CREATE TYPE [dbo].[OrganizationUnitsType] AS TABLE(
 [Identity] nvarchar(225) ,
[Description] nvarchar(max),
[IsVirtual] bit,
[ParentIdentity] nvarchar(225) 
)
GO
create procedure [dbo].[InsertOrganizationUnits]
(  
    @OrgUnits OrganizationUnitsType READONLY
)
AS
BEGIN
 SET NOCOUNT ON;
     INSERT [dbo].[OrganizationUnits]([Identity], [Description], [IsVirtual], [ParentIdentity]) 
     SELECT [Identity], [Description], [IsVirtual], [ParentIdentity] FROM @OrgUnits;
 SET NOCOUNT OFF;
END
GO
CREATE TYPE [dbo].[PropertiesType] AS TABLE(
[Name] nvarchar(225),
[Type] nvarchar(max) 
)
GO
create procedure [dbo].[InsertProperties]
(  
    @Properties PropertiesType READONLY
)
AS
BEGIN
 SET NOCOUNT ON;
     INSERT [dbo].[Properties]([Name], [Type]) 
     SELECT [Name], [Type] FROM @Properties;
 SET NOCOUNT OFF;
END
GO
CREATE TYPE [dbo].[OrganizationUnitToPropertiesType] AS TABLE(
[OrganizationUnitIdentity] nvarchar(225),
[PropertyName] nvarchar(225),
[Value] nvarchar(max)
)
GO
create procedure [dbo].[InsertOrganizationUnitToProperties]
(  
    @OrganizationUnitToProperties OrganizationUnitToPropertiesType READONLY
)
AS
BEGIN
 SET NOCOUNT ON;
     INSERT [dbo].[OrganizationUnitToProperties]([OrganizationUnitIdentity], [PropertyName], [Value]) 
     SELECT [OrganizationUnitIdentity], [PropertyName], [Value] FROM @OrganizationUnitToProperties;
 SET NOCOUNT OFF;
END
GO