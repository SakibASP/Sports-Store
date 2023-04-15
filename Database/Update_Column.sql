

ALTER TABLE Products
DROP COLUMN [ImageData],[ImageName]

GO

ALTER TABLE [dbo].[ProductImages]
Add IsCover INT NULL

GO

DROP INDEX IF EXISTS Unique_ProductImages
ON [SportsStore].[dbo].[ProductImages];
CREATE UNIQUE INDEX Unique_ProductImages  
ON [dbo].[ProductImages](ProductID,IsCover);

GO

DROP INDEX IF EXISTS UNIQUE_PRODUCTS
ON [SportsStore].[dbo].[Products];
CREATE UNIQUE INDEX UNIQUE_PRODUCTS  
ON [dbo].[Products](Cat_Id,Name);

GO

