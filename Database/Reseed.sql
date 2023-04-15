

DECLARE @MAXID BIGINT
SELECT @MAXID=MAX(ProductID) FROM [SportsStore].[dbo].[Products]
SET @MAXID=ISNULL(@MAXID,0)
DBCC CHECKIDENT ('[Products]', RESEED, @MAXID)

GO


DECLARE @MAXID BIGINT
SELECT @MAXID=MAX(AUTO_ID) FROM [SportsStore].[dbo].[Categories]
SET @MAXID=ISNULL(@MAXID,0)
DBCC CHECKIDENT ('[Categories]', RESEED, @MAXID)

GO

DECLARE @MAXID BIGINT
SELECT @MAXID=MAX(AUTO_ID) FROM [SportsStore].[dbo].ShippingDetails
SET @MAXID=ISNULL(@MAXID,0)
DBCC CHECKIDENT ('[ShippingDetails]', RESEED, @MAXID)

GO

DECLARE @MAXID BIGINT
SELECT @MAXID=MAX(AUTO_ID) FROM [SportsStore].[dbo].ShipmentOrders
SET @MAXID=ISNULL(@MAXID,0)
DBCC CHECKIDENT ('[ShipmentOrders]', RESEED, @MAXID)

GO

DECLARE @MAXID BIGINT
SELECT @MAXID=MAX(AUTO_ID) FROM [SportsStore].[dbo].ProductImages
SET @MAXID=ISNULL(@MAXID,0)
DBCC CHECKIDENT ('[ProductImages]', RESEED, @MAXID)