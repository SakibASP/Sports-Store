USE [SportsStore]
GO
/****** Object:  StoredProcedure [dbo].[GetProducts]    Script Date: 22-Nov-22 3:19:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROC [dbo].[GetProducts]
(
@product_id AS INT = NULL,
@cat_id AS INT = NULL,
@price AS INT = NULL,
@searchString VARCHAR(50) = NULL
)
AS
BEGIN
SET NOCOUNT ON;
BEGIN TRY

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	DECLARE @SqlQuery NVARCHAR(MAX);
	SET @SqlQuery= '  
	SELECT TOP 50000
		P.ProductID,PIM.AUTO_ID AS ProductImageID,P.Name ,P.Description,P.Price,P.Buying_Price,PIM.ImageData,PIM.ImageName,P.CREATED_BY
		,CASE WHEN LEN(P.Description)>10 THEN LEFT(P.Description,10) + ''...'' ELSE P.Description END AS ShortDesc
		,P.CREATED_DATE,P.CURRENT_STOCK,P.Cat_Id,C.CategoryName AS Category,PIM.IsCover,P.IsAvailabe
	FROM Products P
		LEFT JOIN ProductImages PIM ON P.ProductID = PIM.ProductID 
		LEFT JOIN Categories C ON P.Cat_Id = C.AUTO_ID
	WHERE (P.Cat_Id = @cat_id OR @cat_id IS NULL) AND (P.Price <= @price OR @price IS NULL) AND (P.ProductID = @product_id OR @product_id IS NULL)
		AND (UPPER(c.CategoryName) LIKE ''%''+ UPPER(@searchString) +''%'' OR UPPER(P.Name) LIKE ''%''+ UPPER(@searchString) +''%''  OR @searchString IS NULL )'

  PRINT (@SqlQuery);
	EXECUTE sp_executesql @SqlQuery, N'@product_id AS INTEGER,@cat_id AS INTEGER,@price AS INTEGER, @searchString AS VARCHAR(50)',@product_id,@cat_id,@price,@searchString;

END TRY
    BEGIN CATCH

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState) WITH SETERROR;

        RETURN -1;

    END CATCH;
END;

-- EXEC GetProducts 