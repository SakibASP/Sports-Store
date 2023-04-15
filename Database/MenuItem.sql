USE [SportsStore]
GO
SET IDENTITY_INSERT [dbo].[MenuItem] ON 
GO
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (1, N'SYSTEM ADMIN', N'#', NULL, 1)
GO
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (10, N'Manage Products', N'/SystemAdmin/Index', 1, 1)
GO
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (11, N'Manage Categories', N'/Category/Index', 1, 1)
GO
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (12, N'Maintain Users', N'/MaintainUsers/Index', 1, 1)
GO
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (13, N'Maintain UserRoles', N'/RoleManager/Index', 1, 1)
GO
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (14, N'Reset Password', N'/Identity/Account/ResetPassword', 1, 1)
GO
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (15, N'Maintain Admin Rights', N'/AdminRights/Index', 1, 1)
GO
SET IDENTITY_INSERT [dbo].[MenuItem] OFF
GO

SET IDENTITY_INSERT [dbo].[MenuItem] ON 
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (2, N'ORDERS', N'/ShippingDetails/Index', NULL, 1)
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (3, N'REPORTS', N'#', NULL, 1)
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (20, N'Category Report', N'/Report/CategoryReport', 3, 1)
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (21, N'Product Report', N'/Report/ProductReport', 3, 1)
SET IDENTITY_INSERT [dbo].[MenuItem] OFF

SET IDENTITY_INSERT [dbo].[MenuItem] ON
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (22, N'Buy History', N'/Report/PeriodicalBuy', 3, 1)
INSERT [dbo].[MenuItem] ([MenuId], [MenuName], [MenuUrl], [MenuParentId], [Active]) VALUES (23, N'Sell History', N'/Report/PeriodicalSell', 3, 1)
SET IDENTITY_INSERT [dbo].[MenuItem] OFF




