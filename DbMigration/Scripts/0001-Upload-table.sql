
if OBJECT_ID('Upload') is null 
begin

CREATE TABLE [dbo].[Upload](
	[Id] [int] IDENTITY(1,1) primary key NOT NULL,
	[Filename] [nvarchar](100) NULL,
	[Url] [nvarchar](200) NULL,
	[Type] [varchar](10) NULL,
	[ParentId] int not null,
	[CreatedDate] datetime not null,
	[CreatedById] int null,
)
end 
