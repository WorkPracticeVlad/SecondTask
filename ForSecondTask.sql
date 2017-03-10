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

