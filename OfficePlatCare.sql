CREATE DATABASE OfficePlantCare
GO

USE [OfficePlantCare]
GO
/****** Object:  Table [dbo].[Admins]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admins](
	[AdminId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](255) NOT NULL,
	[PasswordHash] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[Avatar] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NULL,
	[RoleId] [int] NOT NULL,
	[Address] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[AdminId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Banners]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Banners](
	[BannerId] [int] IDENTITY(1,1) NOT NULL,
	[Image] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[BannerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CareScheduleInventory]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CareScheduleInventory](
	[CareScheduleInventoryId] [int] IDENTITY(1,1) NOT NULL,
	[CareScheduleId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[QuantityUsed] [int] NOT NULL,
	[Notes] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[CareScheduleInventoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Staffs]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Staffs](
	[StaffId] [int] IDENTITY(1,1) NOT NULL,
	[StaffName] [nvarchar](255) NOT NULL,
	[Position] [nvarchar](100) NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Avatar] [nvarchar](255) NULL,
	[BirthDate] [date] NULL,
	[HireDate] [datetime] NULL,
	[Salary] [decimal](18, 2) NULL,
	[Status] [nvarchar](50) NULL,
	[RoleId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[StaffId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Services]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Services](
	[ServiceId] [int] IDENTITY(1,1) NOT NULL,
	[ServiceName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
	[CategoryServiceId] [int] NULL,
	[Image] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ServiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE ServicePrices (
    PriceId INT IDENTITY(1,1) PRIMARY KEY,
    ServiceId INT,              -- Dịch vụ nào
    ServiceType NVARCHAR(50),   -- "Lẻ", "3 tháng", "6 tháng", "12 tháng"
    TreeSize NVARCHAR(50),      -- "Nhỏ", "Trung bình", "Lớn"
    OfficeSize NVARCHAR(50),    -- "Dưới 50m²", "50-100m²", "Trên 100m²"
    DurationInMonths INT NULL,
	NumberOfTrees INT NULL,
	Price DECIMAL(15,2),        -- Giá của gói
    FOREIGN KEY (ServiceId) REFERENCES Services(ServiceId)
);
/****** Object:  Table [dbo].[PaymentMethods]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentMethods](
	[PaymentMethodId] [int] IDENTITY(1,1) NOT NULL,
	[MethodName] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentMethodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Contracts]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contracts] (
    [ContractId] INT IDENTITY(1,1) NOT NULL,
    [ContractCode] AS ('HD' + RIGHT('0000' + CONVERT([nvarchar], [ContractId]), 4)) PERSISTED,
    [CustomerId] INT NOT NULL,
    [ServiceId] INT NOT NULL FOREIGN KEY REFERENCES [dbo].Services([ServiceId]), -- Lấy theo dịch vụ
    [FixedPrice] DECIMAL(18,2) NOT NULL, -- Giá dịch vụ tại thời điểm ký hợp đồng
    [DurationUnit] NVARCHAR(10) NOT NULL DEFAULT 'Month', -- 'Month' hoặc 'Year'
    [Duration] INT NOT NULL, -- Số tháng hoặc năm của hợp đồng
    [StartDate] DATE NOT NULL,
    [EndDate] AS (CASE 
                    WHEN DurationUnit = 'Month' THEN DATEADD(MONTH, Duration, StartDate)
                    WHEN DurationUnit = 'Year' THEN DATEADD(YEAR, Duration, StartDate)
                  END) PERSISTED, -- Tự động tính ngày kết thúc
    [PaymentMethodId] INT NULL FOREIGN KEY REFERENCES [dbo].PaymentMethods([PaymentMethodId]),
    [TotalAmount] AS (FixedPrice * Duration) PERSISTED, -- Tổng tiền tự động tính
    [PaidAmount] DECIMAL(18,2) NULL DEFAULT 0, 
    [RemainingAmount] DECIMAL(18,2) NULL, -- Thành cột bình thường
    [Status] NVARCHAR(50),
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    [UpdatedAt] DATETIME DEFAULT GETDATE(),
    PRIMARY KEY CLUSTERED ([ContractId] ASC)
);
-- Tạo trigger để cập nhật RemainingAmount
CREATE TRIGGER trg_UpdateRemainingAmount
ON [dbo].[Contracts]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE c
    SET RemainingAmount = c.FixedPrice * c.Duration - c.PaidAmount
    FROM [dbo].[Contracts] c
    INNER JOIN inserted i ON c.ContractId = i.ContractId;
END;
/****** Object:  Table [dbo].[CareSchedules]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CareSchedules] (
    [ScheduleId] INT IDENTITY(1,1) PRIMARY KEY,
    [ContractId] INT NOT NULL,
    [StaffId] INT NULL,
    [ScheduledDate] DATE NOT NULL, -- Ngày dự kiến chăm sóc
    [ActualDate] DATE NULL,        -- Ngày thực tế chăm sóc
    [Duration] DECIMAL(5,2) NOT NULL CHECK (Duration > 0), -- Thời lượng chăm sóc (giờ)
    [Status] NVARCHAR(50),
);

/****** Object:  Table [dbo].[Contacts]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contacts](
	[ContactId] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](255) NULL,
	[Phone] [nvarchar](20) NULL,
	[Address] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


/****** Object:  Table [dbo].[Customers]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerName] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](15) NOT NULL,
	[Address] [nvarchar](500) NULL,
	[CustomerType] [nvarchar](255) NULL,
	[RoleId] [int] NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedbacks]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedbacks](
	[FeedbackId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NULL,
	[ServiceId] [int] NULL,
	[FeedbackDate] [datetime] NULL,
	[Rating] [int] NULL,
	[Content] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[FeedbackId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InventoryItems]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryItems](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[ItemName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Unit] [nvarchar](50) NULL,
	[Price] [decimal](18, 2) NULL,
	[StockQuantity] [int] NOT NULL,
	[ItemType] [nvarchar](50) NOT NULL,
	[Status] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[News]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[News](
	[NewsId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Image] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
	[Content] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[NewsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[OrderDetailId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[ServiceId] [int] NULL,
	[Address] [nvarchar](255) NULL,
	[Quantity] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
    [TotalAmount] DECIMAL(18,2) NULL, -- Không dùng PERSISTED
	[Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
UPDATE OrderDetails
SET TotalAmount = Quantity * (SELECT Price FROM ServicePrices WHERE ServicePrices.PriceId = OrderDetails.ServiceId);
/****** Object:  Table [dbo].[Orders]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NULL,
	[StaffId] [int] NULL,
    [OrderDate] DATETIME DEFAULT GETDATE(),
    [TotalPrice] DECIMAL(18,2) NULL, -- Không dùng PERSISTED
	[Discount] DECIMAL(18,2) NULL DEFAULT 0,
	[PaymentMethodId] [int] NOT NULL,
	[PaymentDate] [datetime] NULL,
	[PaymentStatus] [nvarchar](20) NULL,
	[Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
UPDATE Orders
SET TotalPrice = (
    SELECT SUM(TotalAmount) FROM OrderDetails WHERE OrderDetails.OrderId = Orders.OrderId
);

/****** Object:  Table [dbo].[Partners]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Partners](
	[PartnerId] [int] IDENTITY(1,1) NOT NULL,
	[PartnerName] [nvarchar](255) NOT NULL,
	[Logo] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[PartnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Roles]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceCategories]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceCategories](
	[CategoryServiceId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryServiceName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryServiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceDescriptions]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceDescriptions](
	[DescriptionId] [int] IDENTITY(1,1) NOT NULL,
	[ServiceId] [int] NOT NULL,
	[Image] [nvarchar](max) NULL,
	[StepNumber] [int] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[DescriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceInventory]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceInventory](
	[ServiceInventoryId] [int] IDENTITY(1,1) NOT NULL,
	[ServiceId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[QuantityRequired] [decimal](18, 2) NOT NULL,
	[Notes] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ServiceInventoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceRequests]    Script Date: 2/22/2025 2:43:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ServiceRequests](
	[RequestID] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleID] [int] NULL,
	[ServiceID] [int] NULL,
	[CustomerId] [int] NULL,
	[Quantity] [int] NOT NULL,
    [PriceId] [int] NOT NULL FOREIGN KEY ([PriceId]) REFERENCES [dbo].[ServicePrices] ([PriceId]), -- Chỉ lưu PriceId, không lưu giá
    [TotalAmount] DECIMAL(18,2) NULL, -- Sẽ cập nhật bằng trigger
	[Notes] [nvarchar](max) NULL,
	[RequestDate] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
 PRIMARY KEY CLUSTERED ([RequestID] ASC)
) ON [PRIMARY]
GO
CREATE TRIGGER trg_UpdateTotalAmount
ON ServiceRequests
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE sr
    SET sr.TotalAmount = sr.Quantity * sp.Price
    FROM ServiceRequests sr
    JOIN ServicePrices sp ON sr.PriceId = sp.PriceId
    WHERE sr.TotalAmount IS NULL OR sr.TotalAmount <> sr.Quantity * sp.Price;
END;
GO



SET IDENTITY_INSERT [dbo].[Admins] ON 

INSERT [dbo].[Admins] ([AdminId], [Username], [PasswordHash], [Email], [CreatedDate], [UpdatedDate], [Avatar], [Status], [RoleId], [Address]) VALUES (1, N'Kim Ngân', N'090303', N'ngan@plantcare.com', CAST(N'2025-02-05T14:42:01.827' AS DateTime), NULL, N'/assets/images/admin/2.jpg', N'Hoạt động', 1, N'Hải Phòng')
INSERT [dbo].[Admins] ([AdminId], [Username], [PasswordHash], [Email], [CreatedDate], [UpdatedDate], [Avatar], [Status], [RoleId], [Address]) VALUES (2, N'Ruby', N'123456', N'ruby@nhanvien.com', CAST(N'2025-02-06T20:30:54.427' AS DateTime), NULL, NULL, N'Hoạt động', 2, N'Hà Nội')
SET IDENTITY_INSERT [dbo].[Admins] OFF
GO
SET IDENTITY_INSERT [dbo].[Banners] ON 

INSERT [dbo].[Banners] ([BannerId], [Image], [Title], [CreatedDate], [UpdatedDate], [Status], [Description]) VALUES (1, N'/img/banner/banner_trang_chu_1.jpg', N'Xanh Đúng Chất – Văn Phòng Đẳng Cấp Với Dịch Vụ Cây Cảnh Chuyên Nghiệp', CAST(N'2025-02-20T16:38:47.880' AS DateTime), NULL, N'Hiện', N'Biến không gian làm việc thành một khu vườn thu nhỏ, nâng tầm đẳng cấp doanh nghiệp với dịch vụ cho thuê cây xanh cao cấp. Cây đẹp, hợp phong thủy, chăm sóc trọn gói – văn phòng luôn xanh, tinh thần luôn tươi mới!')
INSERT [dbo].[Banners] ([BannerId], [Image], [Title], [CreatedDate], [UpdatedDate], [Status], [Description]) VALUES (2, N'/img/banner/banner_trang_chu_2.jpg', N'Chăm Sóc Cây Cảnh Văn Phòng – Giữ Lá Xanh, Giữ Nguồn Cảm Hứng', CAST(N'2025-02-20T16:41:02.843' AS DateTime), NULL, N'Hiện', N'Cây không chỉ để trang trí mà còn tạo sinh khí cho môi trường làm việc. Dịch vụ chăm sóc chuyên nghiệp đảm bảo cây luôn khỏe mạnh, xanh tốt, giúp văn phòng tràn đầy sức sống và năng lượng tích cực.')
INSERT [dbo].[Banners] ([BannerId], [Image], [Title], [CreatedDate], [UpdatedDate], [Status], [Description]) VALUES (3, N'/img/banner/banner_trang_chu_3.jpg', N'Thiết Kế Cảnh Quan Văn Phòng – Định Hình Phong Cách, Kiến Tạo Không Gian Xanh', CAST(N'2025-02-20T16:41:21.003' AS DateTime), NULL, N'Hiện', N'Một văn phòng đẹp không thể thiếu mảng xanh hài hòa. Chúng tôi mang đến giải pháp thiết kế và thi công cây xanh đẳng cấp, giúp doanh nghiệp xây dựng hình ảnh chuyên nghiệp, ấn tượng ngay từ cái nhìn đầu tiên!')
SET IDENTITY_INSERT [dbo].[Banners] OFF
GO
SET IDENTITY_INSERT [dbo].[Customers] ON 

INSERT [dbo].[Customers] ([CustomerId], [CustomerName], [Email], [Phone], [Address], [CustomerType], [RoleId], [PasswordHash], [CreatedDate], [Status]) VALUES (2, N'Kim Ngân', N'ngan@gmail.com', N'0932426854', N'Hải Phòng', NULL, 3, N'8888', CAST(N'2025-02-19T18:19:15.577' AS DateTime), N'Hoạt động')
INSERT [dbo].[Customers] ([CustomerId], [CustomerName], [Email], [Phone], [Address], [CustomerType], [RoleId], [PasswordHash], [CreatedDate], [Status]) VALUES (5, N'Ruby', N'ngan123@gmail.com', N'0839238321', N'Hải Phòng', NULL, 4, N'1111', CAST(N'2025-02-19T19:23:34.957' AS DateTime), N'Hoạt động')
INSERT [dbo].[Customers] ([CustomerId], [CustomerName], [Email], [Phone], [Address], [CustomerType], [RoleId], [PasswordHash], [CreatedDate], [Status]) VALUES (6, N'Kim Sa', N'sa@gmail.com', N'0839432742', N'Vĩnh Phúc', NULL, 5, N'6666', CAST(N'2025-02-20T19:42:12.887' AS DateTime), N'Hoạt động')
SET IDENTITY_INSERT [dbo].[Customers] OFF
GO
SET IDENTITY_INSERT [dbo].[News] ON 

INSERT [dbo].[News] ([NewsId], [Title], [Description], [Image], [CreatedDate], [UpdatedDate], [Status], [Content]) VALUES (1, N'Top 10 loại cây nên trồng trong nhà dễ chăm dễ sống', N'Việc đặt cây trồng trong nhà có rất nhiều ích lợi có thể kể đến như loại bỏ chất độc hại, giảm tia bức xạ từ thiết bị công nghệ, giảm hiệu ứng nhà kính trong tòa nhà, văn phòng, và đem lại môi trường lành mạnh cho mọi người.', NULL, CAST(N'2025-02-07T16:33:34.167' AS DateTime), NULL, N'Hiển thị', N'Hiện nay ô nhiễm không khí ngày càng trở nên nghiêm trọng hơn bởi vì hiệu ứng đô thị, nhiều khu công nghiệp mọc lên cùng lượng khí thải từ các phương tiện giao thông sinh ra. Trong đó lượng bụi mịn từ đó sinh ra có kích thước rất nhỏ và dễ tấn công và gây ảnh hưởng nghiêm trọng đến sức khỏe của bạn. Việc trồng cây giúp lọc không khí, cũng như sẽ giúp cải thiện không gian sống của bạn. * Dưới đây là top 10 loại cây trồng trong nhà không những giúp lọc không khí hiệu quả mà còn vừa dễ chăm và dễ sống. - Cây Dương xỉ : là loài cây giúp bạn loại bỏ formaldehyde trong không khí và nhiều kim loại độc hại khác ảnh hưởng tới sức khỏe như asen, thủy ngân. Dương xỉ cũng giúp làm giảm bớt tia bức xạ phát ra từ máy tính. Đặt các chậu dương xỉ nhỏ ở ban công, cửa sổ, khu vực làm việc sẽ giúp cho phòng có nguồn không khí trong lành hơn và giảm thiểu tác động của các sản phẩm thiết bị công nghệ  - Cây Vạn Niên Thanh : Là loại cây không quá khó trồng, không chỉ giúp cho không gian được tô điểm màu xanh mát, mà còn cải thiện tính thẩm mỹ của khu vực. Các nhà khoa học đã nghiên cứu và cho thấy loại cây này giúp giảm các chất độc hại như benzen, formaldehyde, thủy ngân.. và còn có khả năng kiểm soát các tế bào ung thư vô cùng hiệu quả.  - Cây Lưỡi Hổ : Là loại cây được ưu tiên trồng tại ban công, phòng ngủ, phòng làm việc, cây lưỡi hổ có khả năng thực hiện quá trình chuyển đổi CO2 thành O2 liên tục, cũng như thanh lọc các chất độc hại, rất có lợi cho khu vực mà bạn mong muốn. Là dạng thân cây mọng nước, do vậy cây lưỡi hổ cũng có tác dụng như cây nha đam, giúp sát khuẩn, kháng viêm. Bạn có thể dùng lõi cây để làm chất giảm sưng đau và dùng cho làm đẹp rất tiện lợi.  - Cây Nha Đam: cũng giống như cây lưỡi hổ là loại cây có khả năng chuyển đổi khí CO2, giúp bầu không khí trong lành hơn. Là dạng cây không cần nhiều dinh dưỡng để phát triển, vậy nên có thể đặt cây tại các khu vực thiếu sáng, thiếu nước cây vẫn có thể tồn tại rất lâu. Khi cây xuất hiện đốm nâu trên thân bạn sẽ nhận biết được mức ô nhiễm không khí trong nơi bạn sống và có thể kịp thời xử lý.  - Cây Thường Xuân: là loại cây họ leo, tốc độ phát triển nhanh và mạnh mà không mất nhiều công chăm sóc. Ngoài tác dụng lọc bụi bẩn và chất độc hại. cây còn có tác dụng làm đẹp không gian rất hiệu quả khi có thể phủ nhanh 1 góc trong không gian.   - Cây Ngũ gia bì: được nhiều người yêu thích và lựa chọn, cây có tác dụng lớn khi đem lại lợi ích cho sức khỏe, cây giúp giảm stress và cải thiện giấc ngủ rất tốt. Không những vậy trong môi trường có nhiều côn trùng cây cũng có tác dụng xua đuổi rất hiệu quả.  - Cây Cọ: ngoài việc lọc không khí cây còn hấp thụ các tia bức xạ phát ra từ thiết bị công nghệ, giúp cải thiện khả năng tập trung và ghi nhớ, nên thường được lựa chọn trong văn phòng và tòa nhà.  - Cây Trúc mây: không chỉ dễ trồng, đẹp mắt, cây không chỉ hợp đặt trong nội thất mà cả ngoại thất cũng vẫn rất đẹp. Cây có nhu cầu nước thấp nên thường sống khỏe trong nhiều loại môi trường. Cây có tác dụng lọc các chất gây hại cho hệ hô hấp và các loại mùi độc hại rất tốt.  - Cây Tuyết tùng: cây cần nhiều nước nhưng cũng là loại dễ sống và phát triển ổn định. Cây tuyết tùng có tác dụng loại bỏ bụi bẩn, giúp giảm căng thẳng, giảm đau đầu.  - Cây Kim tiền: ngoài việc được yêu thích bởi ý nghĩa đặc biệt hút tiền tài, hút lộc cho chủ. Cây Kim tiền trồng trong phòng còn giúp không gian thêm ấm cúng và giúp lọc được không khí trong môi trường.  * Trên đây là tổng hợp của Cây cảnh Tầm Nhìn Châu Á về top các loại cây giúp lọc không khí, vừa bền mà vẫn làm đẹp không gian. Hi vọng chia sẻ này sẽ giúp bạn lựa chọn được các loại cây ưng ý khi để trang trí không gian làm việc, phòng của bạn giúp chất lượng cuộc sống tốt hơn.')
INSERT [dbo].[News] ([NewsId], [Title], [Description], [Image], [CreatedDate], [UpdatedDate], [Status], [Content]) VALUES (2, N'Tư vấn, lựa chọn các loại cây văn phòng cho đầu năm mới', N'Trang trí văn phòng bằng cây cảnh không chỉ tạo điểm nhấn thú vị mà còn giúp cải thiện không khí làm việc, tạo cảm giác thư giãn và nâng cao sự sáng tạo của nhân viên. Đặc biệt, vào dịp đầu năm mới, việc chọn lựa cây cảnh phù hợp sẽ mang đến may mắn và sự mới mẻ cho môi trường làm việc.', NULL, CAST(N'2025-02-07T16:34:42.843' AS DateTime), NULL, N'Hiển thị', N'Trầu bà Nam mỹ (Monstera): Loại cây này có hình dạng lá độc đáo và tượng, tạo điểm nhấn sinh động cho không gian văn phòng. Monstera cũng không cần quá nhiều ánh sáng và dễ chăm sóc. Cây Monstera thích ánh sáng nhẹ và cần được tưới nước đều đặn nhưng hạn chế để đất không quá ẩm ướt hoặc khô hanh. Lưu ý: Xử lý lá bị bệnh và lấy bớt lá cũ để giữ cho cây luôn xanh tươi.  Vạn niên thanh rủ: Cây Vạn niên thanh rủ có thể treo hoặc để trên bàn làm việc, mang đến cảm giác xanh mát và thư giãn. Đặc biệt, giúp lọc không khí và tạo không gian trong lành.     Lưỡi hổ (Snake Plant): Loại cây này có khả năng lọc không khí tốt và không cần nhiều ánh sáng. Lưỡi hổ mang lại sự sang trọng và tinh tế cho văn phòng. Lưỡi hổ ít cần ánh sáng và dễ chăm sóc, mặc dù có thể sống trong nhiều điều kiện khác nhau. Lưu ý: Tránh tưới quá nhiều nước và để đất khô trước khi tưới lại để tránh gốc mục nước.   Kim phát tài (Kim tiền): Là một lựa chọn phổ biến để trang trí văn phòng bởi vẻ đẹp và ý nghĩa may mắn mà nó mang lại. Cây kim tiền được tin rằng mang lại may mắn và tài lộc cho chủ nhân. Giúp làm sạch không khí và tạo không gian làm việc thêm sảng khoái.  Cây kim tiền là loại cây rất dễ chăm sóc, phù hợp cho những người không có nhiều kinh nghiệm trong việc trồng câthích ánh sáng nhưng nên tránh nắng trực tiếp vào mùa hè. - Đặt cây ở nơi có ánh sáng tự nhiên nhưng không quá sáng. - Tưới nước cho cây kim tiền khi đất hoàn toàn khô, khoảng 1-2 lần mỗi tuần là đủ. Tránh để đất ẩm ướt quá lâu để tránh gốc mục nước. - Chọn đất thông thoáng, giàu chất hữu cơ như đất trồng cây cảnh hoặc đất trồng rau sạch. Để cây kim tiền ở nơi có không gian đủ để phát triển và tránh đặt gần máy lạnh hoặc các nguồn nhiệt khác. - Dùng vải sạch ướt lau nhẹ nhàng lá cây để giữ cho lá sạch và hấp thụ ánh sáng tốt hơn.   Hãy lựa chọn các loại cây trang trí phù hợp với không gian và điều kiện ánh sáng trong văn phòng của bạn để tạo nên một môi trường làm việc thú vị, sáng tạo và đem đến may mắn cho cả công ty trong năm mới!')
INSERT [dbo].[News] ([NewsId], [Title], [Description], [Image], [CreatedDate], [UpdatedDate], [Status], [Content]) VALUES (3, N'Nâng cao chất lượng không gian làm việc với cây xanh', N'Không gian văn phòng không chỉ có mỗi bàn làm việc mới cần có cây xanh, mà hầu như mọi khu vực đều nên có. Mỗi một vị trí sẽ có cách bố trí không giống nhau bởi phải dựa vào tính chất khu vực mà lựa chọn loại cây, kích thước và hướng đặt sao cho hợp lý và có tính thẩm mỹ.', NULL, CAST(N'2025-02-07T16:35:52.727' AS DateTime), NULL, N'Hiển thị', N'I. Sự quan trọng của việc sử dụng cây xanh trong văn phòng làm việc   1. Giảm căng thẳng và tăng năng suất làm việc - phúc lợi làm việc cho nhân viên ​8 tiếng làm việc mỗi ngày, đặc biệt là làm với máy tính thường xuyên, không tránh khỏi việc váng đầu, mệt mỏi và đau nhức mắt. Một văn phòng có thật nhiều cây xanh sẽ giúp cho bạn có thêm nhiều góc thư giãn, thả mắt vào những chậu cây xanh ngắt, và nghỉ ngơi đôi chút, nạp thêm năng lượng để tiếp tục làm việc.  Một chậu cây xanh tuy bé nhưng đây là phúc lợi tinh thần vô hình dành cho nhân viên của mình, vì vậy văn phòng dù bé hay nhỏ nên chọn bố trí cây xanh để động viên tinh thần làm việc của nhân viên.   Cây xanh giúp giảm căng thằng, tăng năng suất làm việc    Cây xanh, một phúc lợi dành cho nhân viên  2. Tạo không gian làm việc trong lành Tác dụng chính của cây xanh là cung cấp oxy để có 1 không gian trong lành. Đặt cây cảnh trong văn phòng, dù không thể nào mang lại cảm giác dễ chịu tuyệt đối như ở vườn cây hay rừng cây xanh mát, nhưng vẫn đảm bảo yếu tố thoải mái hơn nhiều so với văn phòng chỉ có nội thất và thiết bị, máy móc làm việc. Hoạt động và làm việc trong bầu không khí trong lành thời gian dài, sức khỏe của người làm việc sẽ được cải thiện đáng kể so với lúc làm việc căng thẳng, mệt mỏi với không gian đơn giản lúc ban đầu.   Cây xanh nội thất giúp cải thiện chất lượng bầu không khí  3. Yếu tố nhỏ nhưng rất cần thiết để tôn thêm vẻ đẹp thẩm mỹ của văn phòng Bạn không thể nào phủ nhận tầm quan trọng của cây cảnh văn phòng trong toàn bộ tổng thể nội thất được bố trí. Màu xanh tươi mát của cây cối bao giờ cũng khác biệt, tự nhiên và sinh động – vốn là điều mà nội thất nhân tạo không thể sánh bằng. Sự hòa hợp giữa vẻ đẹp nhân tạo của nội thất và vẻ đẹp tự nhiên, sống động mà cây cảnh mang đến hẳn sẽ tạo nên không gian văn phòng đầy tính thẩm mỹ. Làm việc trong văn phòng được bố trí đẹp đẽ, hợp lý và ấn tượng các loại cây cảnh, có ai lại chẳng yêu thích. Cây cảnh còn giúp văn phòng mang một vẻ đẹp khác với sự tinh tế, chỉn chu và hiện đại.   Cây xanh giúp tôn thêm vẻ đẹp thẩm mỹ cho văn phòng  4. Mang đến tài lộc và thuận lợi khi kinh doanh Với một số công ty tin vào phong thủy có khả năng ảnh hưởng đến hoạt động kinh doanh thường lựa chọn đặt niềm tin khá nhiều vào cây cảnh - những điều vốn xuất phát từ tự nhiên. Cây cảnh văn phòng, tùy vào từng loại cụ thể có thể mang đến may mắn và tài lộc cho công ty, thúc đẩy sự phát triển thuận lợi các hoạt động mà doanh nghiệp đang thực hiện.   Ngoài giá trị thẩm mỹ, cây xanh còn có ý nghĩa về mặt phong thủy  5. Phụ kiên cây xanh nâng cao giá trị thương hiệu doanh nghiệp Khi khách hàng hay đối tác có dịp ghé đến tham quan không gian làm việc của công ty, hẳn sẽ chú ý rất nhiều đến thiết kế và cách bố trí văn phòng. Việc lựa chọn loại cây và bố trí hợp lý sẽ mang đến không gian làm việc chuyên nghiệp, tinh tế, chất lượng. Cơ sở vật chất được đảm bảo thì hoạt động làm việc mới đạt được hiệu quả. Từ đây, khách hàng và đối tác càng đánh giá cao chất lượng hoạt động và thương hiệu doanh nghiệp có độ nhận diện cao hơn.   Cây xanh vị trí lễ tân công ty  II. Gợi ý một số cách setup văn phòng xanh Không gian văn phòng không chỉ có mỗi bàn làm việc mới cần có cây xanh, mà hầu như mọi khu vực đều nên có. Mỗi một vị trí sẽ có cách bố trí không giống nhau bởi phải dựa vào tính chất khu vực mà lựa chọn loại cây, kích thước và hướng đặt sao cho hợp lý và có tính thẩm mỹ.   - Cây xanh bày trí bàn làm việc - tủ tài liệu Đây là cách bố trí phố biến quen thuộc, giúp giảm stress nhanh chóng, cực kì thu hút, bắt mắt và mang đến may mắn cho chủ nhân. Nên chọn cây có kích thước nhỏ để không chiếm không gian bàn làm việc, làm hạn chế tầm nhìn, gây khó khăn khi làm việc với hồ sơ, giấy tờ, máy tính và trong hoạt động giao tiếp. Tránh lựa chọn những loại cây có vẻ  ngoài gai góc, xù xì như xương rồng vì chúng có khả năng gây ra thương tích. Dây leo cũng nên tránh vì chúng phát triển nhanh, có khả năng cản trở không gian làm việc. Không nên đặt những loại cây có màu sắc quá sặc sỡ dẫn đến mất tập trung. Loại cây cảnh bố trí trên bàn làm việc phải là loại cây xanh tốt, có nhiều mầm lộc, có khả năng phát triển tốt trong môi trường bóng râm bởi văn phòng hạn chế tiếp xúc với ánh nắng mặt trời, có nơi còn có điều hòa máy lạnh.   Cây xanh đặt hộp tủ tài liệu    Cây xanh nội thất giúp thanh lọc không khí  - Cây xanh bày trí các góc phòng làm việc Văn phòng sẽ có rất nhiều khoảng không gian trống không đặt nội thất, nếu đặt cây xanh vào đấy có thể che lấp đi khoảng trống, điểm khuất (xấu), đồng thời tạo điểm nhấn nổi bật. Kích thước của các chậu cảnh tất nhiên sẽ to hơn so với chậu để bàn, tuy nhiên, nên căn cứ vào vị trí khoảng trống đó to hay nhỏ mà quyết định kích thước cây phù hợp. Thông thường, các văn phòng sẽ bố trí một số loại cây có thân dài, lá không xum xuê để tránh cản trở lối đi nhưng vẫn đảm bảo không gian xanh mát.   Kim phát tài ghép kết hợp    Sản phẩm xanh được thiết kế sáng tạo  - Cây xanh bày trí dọc hàng lang Trong không gian làm việc, hành lang là một phần quan trọng quyết định đến thái độ đánh giá của đối tác, khách hàng về công ty. Hành lang được bày trí ấn tượng với cách bố trí cây xanh hợp lý tạo không gian di chuyển làm mọi người cảm thấy thoải mái, dễ chịu. Nếu trồng cây xanh ở hành lang có thể làm tăng độ ẩm không khí lên 50%, đây là lý do văn phòng nên bố trí nhiều cây xanh dọc hành lang, lối đi. Đa số các công ty hiện nay được bao phủ bởi nhiệt độ điều hòa, có cây xanh sẽ giúp làm dịu không khí, da không bị khô.    - Cây xanh rủ treo - tiếc kiệm không gian Với một số không gian làm việc rộng lớn có thiết kế theo hướng mở, hoặc không gian văn phòng có diện tích khiêm tốn, việc sử dụng các biện pháp không gian bằng các sản phẩm treo không chỉ tối ưu sử dụng mà còn tạo nên sự đặc biệt trong cách bày trí cây xanh trong văn phòng của bạn.  - Cây xanh bày trí phòng họp   Phòng họp cũng là nơi cần đến sự xuất hiện của cây cảnh vì đây là một trong những yếu tố quan trọng khi thiết kế phòng họp tinh tế, sang trọng, sạch sẽ. Một cuộc họp văn phòng thường có không khí căng thẳng và áp lực bởi nhiều vấn đề phát sinh có liên quan đến hoạt động công ty. Vì thế, bố trí thêm cây xanh ở phòng họp sẽ làm giảm áp lực tinh thần của người tham gia, giúp quá trình họp diễn ra thuận lợi hơn. Thông thường, cây xanh sẽ được bố trí ở một số góc trống trong phòng họp hoặc hộc giữa của bàn.   - Cây xanh bày trí phòng lãnh đạo Khi đối tác và khách hàng có dịp ghé đến tham quan công ty, phòng lãnh đạo và giám đốc là một trong những nơi quan trọng khi đánh giá. Vì thế, bên cạnh bố trí cây xanh phù hợp với vị trí còn phải đảm bảo tính thẩm mỹ cho tổng thể không gian làm việc đó. Những loại cây được đặt ở văn phòng lãnh đạo là cây có ý nghĩa về mặt phong thủy và sự quyền lực, giúp đem đến sự tốt đẹp, tài lộc và may mắn cho hoạt động công ty. Văn phòng lãnh đạo vô cùng quan trọng nên vị trí đặt cây cũng phải phù hợp với phong thủy như cây có tán lá nhỏ được đặt ở phía xa và cây có lá to hơn được đặt ở chính diện văn phòng để tạo sự chú ý.')
INSERT [dbo].[News] ([NewsId], [Title], [Description], [Image], [CreatedDate], [UpdatedDate], [Status], [Content]) VALUES (4, N'Cách chọn cây cho văn phòng truyền thống', N'Văn phòng truyền thống là kiểu văn phòng phổ biến ở Việt Nam và là sự lựa chọn tối ưu cho rất nhiều doanh nghiệp và công ty khác nhau bới tính tiết kiệm và tiện ích của chúng. Về cây xanh, để lựa chọn được 1 chậu cây ưng ý đặt vào văn phòng mang phong cách truyền thống cần phải cân nhắc đến yếu tố phong thuỷ, hợp lý, và tận dụng tối da các góc trống để trang trí nhưng không gây cảm giác chật chội thêm cho văn phòng. ', NULL, CAST(N'2025-02-07T16:36:59.770' AS DateTime), NULL, N'Hiển thị', N'1. Các chủng loại cây truyền thống: Thiết mộc lan, Vạn niên thanh cột, Kim tiền, Kim ngân,... Để nói đến cây xanh trang trí cho văn phòng truyền thống, Một trong những loại cây dễ dùng, dễ sử dụng phổ biến, đứng top đầu để lựa chọn chính là những dòng cây truyền thông như Thiết mộc lan, Vạn niên thanh cột, Kim ngân, Kim tiền,... Đây đều là những loại cây dễ trồng, dễ chăm sóc, có chức năng thanh lọc không khí tốt, đặc biệt là dễ dùng, dễ khắc phục điểm yếu cho văn phòng của công ty bạn.  Cây xanh đặt cạnh máy in trong văn phòng 2. Các loại cây kết hợp Đối vói các khu vực chụng, ta hoàn toàn có thể sử dụng các loại cây đặc biệt để làm đẹp thêm  khu vực không gian chung để có thêm điểm nhấn cho không gian truyền thồng. Thay vì đặt những chậu cây đơn sắc, ta có thể thay thế bằng chậu cây có nhiều màu sắc để tránh nhàm chán.   Khu vực chung tại sảnh toà nhà văn phòng truyền thống. 3. Tận dụng sự sáng tạo - các sản phẩm thiết kế  Ngày nay, cây xanh được trang trí một cách thông minh đi kèm cũng các sản phẩm thiết kế. Các hoạ tiết của các sản phảm thiết kế không chỉ làm cách điệu hoá các chi tiết đơn điệu của văn phòng truyền thống, còn mang sắc xanh tới. ')
INSERT [dbo].[News] ([NewsId], [Title], [Description], [Image], [CreatedDate], [UpdatedDate], [Status], [Content]) VALUES (5, N'Cách lựa chọn cây cho văn phòng hiện đại', N'Phong cách văn phòng hiện đại có đặc điểm không gian “mở”, không đi theo lối mòn  của các văn phòng truyền thống, vì vậy, chính vì vậy, việc đưa cây xanh vào văn phòng hiện đại với mục đích kết hợp tạo các vách ngăn không gian, tạo không gian xanh là rất cần thiết.', NULL, CAST(N'2025-02-07T16:37:40.700' AS DateTime), NULL, N'Hiển thị', N'1. Vách ngăn bằng cây xanh Phong cách văn phòng hiện đại có đặc điểm không gian “mở”, không đi theo lối mòn  của các văn phòng truyền thống, vì vậy, chính vì vậy, việc đưa cây xanh vào văn phòng hiện đại với mục đích kết hợp tạo các vách ngăn không gian, tạo không gian xanh là rất cần thiết  Khung đặt cây vừa giúp tiết kiệm không gian vừa mang lại hiệu quả trang trí cao  2. Chất liệu chậu đặt cây Các chậu trồng cây nên là từ chất liệu sứ, Composite nhẹ, các chậu cây nhỏ…để dễ dàng di chuyển và linh động.  Chậu sứ  Chậu Composite 3. Cách bố trí Việc bố trí cây cảnh ưu tiên kê xếp linh hoạt, năng động, sử dụng chủ yếu các sản phẩm nhỏ gọn để tiết kiệm không gian xanh văn phòng. Đặc biệt, đối với văn phòng hiện đại nên sử dụng các biện pháp “không gian xanh tiết kiệm diện tích” như các sản phẩm treo làm vách ngăn, hay khung tranh treo tường nghệ thuật là một giải pháp hữu hiệu.')
INSERT [dbo].[News] ([NewsId], [Title], [Description], [Image], [CreatedDate], [UpdatedDate], [Status], [Content]) VALUES (6, N'Cách tạo điểm nhấn cho văn phòng hiện đại', N'Thiên nhiên mang đến cho con người nhiều cảm hứng sáng tạo và giúp điều hòa, thanh lọc không khí xung quanh. Thay vì vùi mình vào 4 bức tường, việc đưa thiên nhiên vào trong không gian văn phòng và một quyết định tuyệt vời. Và việc đưa những chậu cây đơn thuần là quá đơn giản, thay vào đó việc thiết kế những hệ cây, hệ tiểu cảnh hay sắp đặt những chậu cây với nhau giúp tạo điểm nhấn mạnh mẽ, là dấu ấn riêng biệt cho không gian.', NULL, CAST(N'2025-02-07T16:38:21.217' AS DateTime), NULL, N'Hiển thị', N'Thiên nhiên mang đến cho con người nhiều cảm hứng sáng tạo và giúp điều hòa, thanh lọc không khí xung quanh. Thay vì vùi mình vào 4 bức tường, việc đưa thiên nhiên vào trong không gian văn phòng và một quyết định tuyệt vời. Và việc đưa những chậu cây đơn thuần là quá đơn giản, thay vào đó việc thiết kế những hệ cây, hệ tiểu cảnh hay sắp đặt những chậu cây với nhau giúp tạo điểm nhấn mạnh mẽ, là dấu ấn riêng biệt cho không gian. Dưới đây là một số hình ảnh mà Tầm Nhìn Châu Á đã thiết kế, tạo điểm nhấn bằng hệ tiểu cảnh cây cho văn phòng khách hàng')
INSERT [dbo].[News] ([NewsId], [Title], [Description], [Image], [CreatedDate], [UpdatedDate], [Status], [Content]) VALUES (7, N'Tips nhỏ tạo điểm nhấn cho văn phòng theo phong cách tối giản', N'Khái niệm tối giản hiện nay đang là một xu thế, ngày càng quen thuộc với rất nhiều người, nó là việc lược bỏ mọi thứ về hình thức cơ bản của chi tiết sự việc. Phong cách Minimalism được ứng dụng trong nghệ thuật, lối sống, kiến trúc và thiết kế nội thất.', NULL, CAST(N'2025-02-07T16:39:22.313' AS DateTime), NULL, N'Hiển thị', N'Ludwig Mies van der Rohe (1886 – 1969) là kiến trúc sư đại tài người Đức - cha đẻ của phong cách kiến trúc tối giản. Ông đặt nền móng cho phong cách này với không gian đơn giản, trong sạch, tinh tế, sử dụng những mặt phẳng, đường thẳng, đường vuông góc,..theo nguyên tắc "Less is more" (tạm dịch: ít là nhiều / đơn giản nhưng đầy đủ). Phong cách này vô cùng thịnh hành ở châu Âu – cái nôi của nội thất thế giới. Tại Châu Á phong cách này xuất hiện đầu tiên ở Nhật Bản. Bạn có thể tìm thấy phong cách này ở hầu hết công trình tại Nhật, kể cả đương đại lẫn truyền thống. Loại hình trang trí này thường được sử dụng trong những căn nhà hoặc văn phòng có diện tích hẹp. Khi chọn loại cây phù hợp cho phong cách nội thất này, những cây có kiểu dáng đơn giản, xinh xắn và màu sắc rực rỡ sẽ giúp làm giảm sự đơn điệu, tạo điểm nhấn mà không chiếm quá nhiều diện tích.   Những điểm lưu ý cho bạn khi chuẩn bị bố trí cây trong văn phòng tối giản để bố trí cây theo phong cách tối giản: - Màu sắc: không nên sử dụng quá 4 màu trong cùng một chậu cây, tốt nhất chỉ nên sử  dụng 2 màu:  1 màu chủ đạo và 1 màu nhấn. Trong đó những gam màu trung tính thường được sử dụng làm màu tường để tạo ra bức đệm hoàn hảo cho đồ nội thất khác. Chính vì vậy, khi lựa chọn cây xanh cho các văn phòng trên, việc lựa chọn màu sắc cây và chậu để hài hoà và cân xứng màu của nội thất là rất cần thiết. Với nguyên tắc "Less is more" - đơn giản hết mức có thể, có nghĩa cây là một điểm nhấn phụ cho không gian nội thất, không nên quá nổi bật.    Ví dụ: Đối với văn phòng tối giản, việc lựa chọn chậu màu trắng đơn thuần hoặc chậu trồng cây đơn giản, không dùng các chậu cây kết hợp hay cây có màu sắc quá màu mè. - Tips gợi ý một số loại cây dễ dùng, bền, và màu sắc phù hợp đối với tone màu sáng của văn phòng tối giản như: Phát tài núi, Đại lộc, Tài lộc, Kim phát tài,... - Loại chậu phù hợp: với màu sắc tone sáng của phong cách tối giản, chúng ta có thể sử dụng các chậu trắng men sứ có cùng màu sắc tone với văn phòng. Đối với văn phòng có màu sắc trầm, các chậu sành sứ hoặc composite màu đen trầm sẽ là lựa chọn phù hợp cho người sử dụng.    Chậu cây Phát tài núi. - Nội thất: bàn ghế, tủ, hay tủ tivi, màn hình chiếu,… luôn được hạn chế ở mức tối đa. Hầu hết đều có thiết kế đơn giản và mang hơi hướng của nội thất hiện đại châu Âu hoặc đơn giản như phong cách Nhật Bản. Ví dụ: Không gian phòng họp của văn phòng sẽ là vị trí vô tình làm hạn chế sự tối giản trong phong cách văn phòng, vì tiện nghi nội thất phải đáp ứng đủ nhu cầu sử dụng trong một không gian nhỏ hẹp.  Gợi ý mẹo nhỏ để khắc phục cho việc trang trí cây trong phòng họp theo phong cách tối giản:   - Sử dụng các chậu cây có kích thước phù hợp với không gian văn phòng tuỳ kích thước. Mẹo nhỏ hữu dụng đối với meeting room có không gian nhỏ, thay vì cố gắng cho 1 chậu cây đặt sàn, ta có thể đặt 1 chậu cây nhỏ đặt bàn xinh điểm xuyết chút sắc xanh sẽ hợp lý và tiết kiệm được không gian cho văn phòng của bạn     Mẫu chậu cây nhỏ đặt bàn  - Ánh sáng: sử dụng ánh sáng để tạo ra sự nhấn mạnh các khu vực quan trọng, thông qua hiệu ứng bóng đổ vào đồ nội thất hoặc qua tán cây, giúp tôn lên hình khối vật dụng và các thành phần khác trong kiến trúc. Ví dụ như: ánh sáng tự nhiên được lọc qua các bình phong lá chắn, rèm cửa hay các tán cây,... Chi tiết nhỏ giúp tôn lên sự sang trọng và tạo điểm nhấn một cách nghệ thuật cho văn phòng theo phong cách tối giản. ')
INSERT [dbo].[News] ([NewsId], [Title], [Description], [Image], [CreatedDate], [UpdatedDate], [Status], [Content]) VALUES (8, N'Một số lưu ý đặt cây xanh cho văn phòng hiện đại', N'Phong cách thiết kế văn phòng hiện đại, đang được rất nhiều các doanh nghiệp ưa chuộng. Không những thế các mẫu thiết kế còn giúp văn phòng trở nên sang trọng và độc đáo. Kết hợp cùng với máy móc thiết bị hiện đại giúp cho nhân viên có đời sống văn phòng', NULL, CAST(N'2025-02-07T16:40:05.183' AS DateTime), NULL, N'Hiển thị', N'Phong cách thiết kế văn phòng hiện đại, đang được rất nhiều các doanh nghiệp ưa chuộng. Không những thế các mẫu thiết kế còn giúp văn phòng trở nên sang trọng và độc đáo. Kết hợp cùng với máy móc thiết bị hiện đại giúp cho nhân viên có đời sống văn phòng tốt hơn, từ đó nâng cao hiệu quả giúp nhân viên làm việc có năng suất. Văn phòng hiện đại là không gian văn phòng được thiết kế không gian theo hướng hiện đại, đảm bảo sự linh hoạt và tiện ích đi kèm, phù hợp với các doanh nghiệp trong thời hiện đại. Mục tiêu của mô hình phòng làm việc hiện đại chính là để mang đến một không gian làm việc chung vừa năng suất vừa thú vị. Sự nhàm chán và quy củ sẽ bị loại bỏ dần ra khỏi mô hình văn phòng hiện đại.  1. Tận dụng, tiết kiệm những khoảng không gian có sẵn Để khắc phục các không gian thiết kế nội thất văn phòng hiện nay đều có diện tích hẹp thì giải pháp vách ngăn cây xanh, giá treo cây xanh gắn tường hay các giá treo trên không trung. Các chậu cây cảnh nhỏ xinh cho bàn làm việc. Các chậu cây đặt ở các góc văn phòng đều là những sự lựa chọn tuyệt vời.   2. Chọn cây phù hợp với điều kiện văn phòng Việc chọn cây cho các văn phòng, hãy chọn cây có khả năng phát triển tốt trong không gian ít ánh sáng tự nhiên, râm mát. Những loại cây có  tán lá xanh cũng được ưa chuộng để tạo ra luồng sinh khi mới cho thiết kế nội thất phòng làm việc hiện đại hiện nay.  3. Vị trí đặt cây hợp lý Khoảng cách đặt các chậu cây xanh với chỗ ngồi của nhân sự trong văn phòng cũng phải được tính toán cho thật phù hợp và thuận tiện để tránh việc bị đụng chạm hay bất tiện. Với tất cả những những lợi ích tuyệt vời mà cây xanh mang lại cho con người, các nhà quản lý, lãnh đạo ngày càng ưa chuộng giải pháp đưa cây xanh vào thiết kế văn phòng hiện đại của công ty mình.')
SET IDENTITY_INSERT [dbo].[News] OFF
GO
SET IDENTITY_INSERT [dbo].[Partners] ON 

INSERT [dbo].[Partners] ([PartnerId], [PartnerName], [Logo], [CreatedDate], [Description], [Status]) VALUES (1, N'Công ty TNHH Cây Xanh Việt Nam', NULL, CAST(N'2025-02-07T17:02:13.140' AS DateTime), N'Cung cấp cây cảnh nội thất, cây văn phòng, và các dịch vụ chăm sóc cây chuyên nghiệp.', N'Đang hợp tác')
INSERT [dbo].[Partners] ([PartnerId], [PartnerName], [Logo], [CreatedDate], [Description], [Status]) VALUES (2, N'Trung tâm Nghiên cứu & Phát triển Cây Xanh', NULL, CAST(N'2025-02-07T17:02:30.880' AS DateTime), N'Nghiên cứu, nhân giống và phát triển các loại cây phù hợp với môi trường văn phòng.', N'Đang hợp tác')
INSERT [dbo].[Partners] ([PartnerId], [PartnerName], [Logo], [CreatedDate], [Description], [Status]) VALUES (3, N'Công ty CP Cảnh Quan & Đô Thị Xanh', NULL, CAST(N'2025-02-07T17:02:50.540' AS DateTime), N'Thiết kế và thi công không gian xanh cho văn phòng, công ty và khu đô thị.', N'Đang hợp tác')
INSERT [dbo].[Partners] ([PartnerId], [PartnerName], [Logo], [CreatedDate], [Description], [Status]) VALUES (4, N'Dịch vụ Cho Thuê Cây Cảnh GreenLife', NULL, CAST(N'2025-02-07T17:03:04.523' AS DateTime), N'Chuyên cho thuê và chăm sóc cây cảnh văn phòng theo hợp đồng ngắn hạn và dài hạn.', N'Đang hợp tác')
INSERT [dbo].[Partners] ([PartnerId], [PartnerName], [Logo], [CreatedDate], [Description], [Status]) VALUES (5, N'Công ty TNHH Thiết Kế Cảnh Quan NatureSpace', NULL, CAST(N'2025-02-07T17:03:20.923' AS DateTime), N'Đơn vị tư vấn, thiết kế không gian xanh sáng tạo, kết hợp yếu tố phong thủy và thẩm mỹ.', N'Đang hợp tác')
INSERT [dbo].[Partners] ([PartnerId], [PartnerName], [Logo], [CreatedDate], [Description], [Status]) VALUES (6, N'Tập đoàn GreenTech - Công Nghệ Xanh', NULL, CAST(N'2025-02-07T17:03:35.717' AS DateTime), N'Ứng dụng công nghệ IoT trong giám sát và chăm sóc cây cảnh tự động.', N'Đang hợp tác')
INSERT [dbo].[Partners] ([PartnerId], [PartnerName], [Logo], [CreatedDate], [Description], [Status]) VALUES (7, N'Nhà Vườn An Phú', NULL, CAST(N'2025-02-07T17:03:49.563' AS DateTime), N'Cung cấp cây cảnh nhập khẩu và cây bản địa cho các văn phòng cao cấp.', N'Đang hợp tác')
INSERT [dbo].[Partners] ([PartnerId], [PartnerName], [Logo], [CreatedDate], [Description], [Status]) VALUES (8, N'Công ty TNHH Vườn Nhật Bản ZenGarden', NULL, CAST(N'2025-02-07T17:04:04.467' AS DateTime), N'Thiết kế sân vườn, tiểu cảnh theo phong cách Nhật Bản cho không gian văn phòng.', N'Đang hợp tác')
INSERT [dbo].[Partners] ([PartnerId], [PartnerName], [Logo], [CreatedDate], [Description], [Status]) VALUES (9, N'Trung tâm Ứng Dụng Nông Nghiệp Đô Thị', NULL, CAST(N'2025-02-07T17:04:18.847' AS DateTime), N'Phát triển các mô hình vườn tường xanh, vườn thủy canh trong văn phòng.', N'Đang hợp tác')
INSERT [dbo].[Partners] ([PartnerId], [PartnerName], [Logo], [CreatedDate], [Description], [Status]) VALUES (10, N'Công ty Cổ Phần Xây Dựng & Cây Xanh Thành Phố', NULL, CAST(N'2025-02-07T17:04:33.533' AS DateTime), N'Thi công hệ thống vườn đứng, vườn mái cho các tòa nhà văn phòng và khu thương mại.', N'Đang hợp tác')
SET IDENTITY_INSERT [dbo].[Partners] OFF
GO
SET IDENTITY_INSERT [dbo].[PaymentMethods] ON 

INSERT [dbo].[PaymentMethods] ([PaymentMethodId], [MethodName]) VALUES (1, N'Thanh toán bằng tiền mặt')
INSERT [dbo].[PaymentMethods] ([PaymentMethodId], [MethodName]) VALUES (2, N'Thanh toán qua ví điện tử')
INSERT [dbo].[PaymentMethods] ([PaymentMethodId], [MethodName]) VALUES (3, N'Thanh toán qua tài khoản ngân hàng')
INSERT [dbo].[PaymentMethods] ([PaymentMethodId], [MethodName]) VALUES (5, N'Thanh toán qua ứng dụng công ty')
SET IDENTITY_INSERT [dbo].[PaymentMethods] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([RoleId], [RoleName]) VALUES (1, N'Admin')
INSERT [dbo].[Roles] ([RoleId], [RoleName]) VALUES (5, N'Đặt dịch vụ')
INSERT [dbo].[Roles] ([RoleId], [RoleName]) VALUES (4, N'Ký hợp đồng')
INSERT [dbo].[Roles] ([RoleId], [RoleName]) VALUES (2, N'Nhân viên')
INSERT [dbo].[Roles] ([RoleId], [RoleName]) VALUES (3, N'Vãng lai')
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[ServiceCategories] ON 

INSERT [dbo].[ServiceCategories] ([CategoryServiceId], [CategoryServiceName], [Description]) VALUES (1, N'Dịch vụ bảo dưỡng định kỳ', N'- Kiểm tra, tưới nước, bón phân cho cây theo lịch cố định.
- Cắt tỉa cành, loại bỏ lá vàng, duy trì hình dáng cây.
-Kiểm tra và xử lý sâu bệnh, đảm bảo cây luôn xanh tốt.')
INSERT [dbo].[ServiceCategories] ([CategoryServiceId], [CategoryServiceName], [Description]) VALUES (2, N'Dịch vụ chăm sóc cây theo yêu cầu', N'- Cứu chữa cây bị héo, thay thế cây bị chết.
- Điều chỉnh vị trí, thay đổi chậu hoặc bố trí lại không gian cây xanh.
- Xử lý cây bị bệnh, bổ sung dưỡng chất đặc biệt theo yêu cầu.')
INSERT [dbo].[ServiceCategories] ([CategoryServiceId], [CategoryServiceName], [Description]) VALUES (3, N'Dịch vụ cắt tỉa & xử lý cây', N'- Cắt tỉa cành, tạo dáng cây bonsai hoặc theo phong cách mong muốn.
- Xử lý cây gãy, cây mục, thu gom và dọn dẹp cành lá rơi.
- Đảm bảo an toàn trong không gian văn phòng bằng cách kiểm tra và xử lý các cành có nguy cơ gãy.')
INSERT [dbo].[ServiceCategories] ([CategoryServiceId], [CategoryServiceName], [Description]) VALUES (4, N'Dịch vụ thiết kế & nâng cấp không gian xanh', N'- Thiết kế không gian xanh phù hợp với văn phòng.
- Cung cấp các loại cây phong thủy, tiểu cảnh nước hoặc tường cây xanh.
- Lắp đặt hệ thống tưới tự động và hệ thống chiếu sáng cho cây nội thất.')
INSERT [dbo].[ServiceCategories] ([CategoryServiceId], [CategoryServiceName], [Description]) VALUES (5, N'Gói dịch vụ cao cấp', N'- Tư vấn và cung cấp các giải pháp cây xanh theo phong thủy. Cung cấp cây nhập khẩu hoặc cây hiếm theo yêu cầu đặc biệt.
- Chăm sóc định kỳ với quy trình chuyên sâu, sử dụng phân bón và phương pháp chăm sóc cao cấp.')
SET IDENTITY_INSERT [dbo].[ServiceCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[ServiceDescriptions] ON 

INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) VALUES (1, 1, NULL, 1, N'Mô Tả Dịch Vụ', N'Dịch vụ cho thuê cây xanh văn phòng (ngắn hạn và dài hạn) được thiết kế để cung cấp các giải pháp cây cảnh cho các doanh nghiệp, giúp tạo không gian làm việc trong lành, nâng cao hiệu quả công việc và cải thiện tinh thần làm việc của nhân viên. Dịch vụ này gồm hai gói chính:
- Cho thuê ngắn hạn: Phù hợp với các sự kiện, hội nghị, triển lãm hoặc các chiến dịch marketing.
- Cho thuê dài hạn: Dành cho các văn phòng muốn duy trì không gian xanh liên tục, bao gồm việc thay cây định kỳ và chăm sóc cây.', NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) VALUES (2, 1, NULL, 2, N'Quy Trình Cung Cấp Dịch Vụ', N'Bước 1: Tư Vấn và Lựa Chọn Cây Cảnh
- Liên hệ khách hàng: Khách hàng sẽ liên hệ với công ty cung cấp dịch vụ qua điện thoại, email hoặc website.
- Tư vấn: Chuyên gia của công ty sẽ tư vấn cho khách hàng về các loại cây phù hợp với không gian văn phòng, phong thủy, và yêu cầu bảo dưỡng.
- Lựa chọn cây cảnh: Khách hàng có thể lựa chọn từ các loại cây như cây cảnh để bàn, cây để sàn, cây treo tường, cây xanh lọc không khí, hoặc các loài cây đặc biệt khác.
Bước 2: Thiết Kế và Đo Lường
- Đo đạc không gian văn phòng: Công ty sẽ cử nhân viên đến trực tiếp để đo đạc diện tích và xác định vị trí đặt cây.
- Thiết kế bố trí cây: Dựa trên thông tin thu thập được, công ty sẽ thiết kế một kế hoạch cây xanh tối ưu cho không gian văn phòng, đảm bảo tính thẩm mỹ và dễ dàng trong việc chăm sóc.
Bước 3: Cung Cấp Cây và Đưa Vào Sử Dụng
- Cung cấp cây: Sau khi lựa chọn cây và thiết kế bố trí, công ty sẽ tiến hành giao hàng và đưa cây vào văn phòng.
- Đặt cây: Cây sẽ được đặt vào vị trí đã định trong văn phòng theo thiết kế đã được phê duyệt.
- Bảo dưỡng ban đầu: Các cây được kiểm tra kỹ lưỡng về tình trạng sức khỏe và được chăm sóc trong những ngày đầu khi được đặt tại văn phòng.
Bước 4: Chăm Sóc và Duy Trì Cây
- Chăm sóc định kỳ: Dịch vụ bao gồm việc chăm sóc cây định kỳ, bao gồm tưới nước, bón phân, cắt tỉa, thay đất, và kiểm tra sức khỏe cây.
- Công cụ sử dụng: Sử dụng các công cụ chuyên dụng như bình tưới, kéo cắt tỉa, phân bón hữu cơ, và các dụng cụ chăm sóc khác.
- Thay cây khi cần thiết: Nếu cây có dấu hiệu không phát triển tốt, công ty sẽ thay thế cây mới trong thời gian ngắn nhất.
Bước 5: Kiểm Tra và Đánh Giá
- Kiểm tra định kỳ: Nhân viên sẽ kiểm tra tình trạng cây hàng tháng hoặc theo yêu cầu của khách hàng.
- Đánh giá phản hồi: Công ty sẽ yêu cầu khách hàng đánh giá dịch vụ chăm sóc cây và đưa ra phản hồi để cải thiện dịch vụ.
- Bước 6: Kết Thúc Hợp Đồng (Đối Với Thuê Ngắn Hạn)
- Thu hồi cây: Khi hợp đồng cho thuê ngắn hạn kết thúc, công ty sẽ tiến hành thu hồi cây cảnh khỏi văn phòng.
- Bảo dưỡng cây: Sau khi thu hồi, cây sẽ được bảo dưỡng lại và sẵn sàng cho các hợp đồng tiếp theo.', NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) VALUES (3, 1, NULL, 3, N'Thời Gian Dịch Vụ', N'- Thuê ngắn hạn: Thời gian cho thuê từ 1 tuần đến 6 tháng, phù hợp với các sự kiện hoặc chương trình đặc biệt.
- Thuê dài hạn: Thời gian cho thuê kéo dài từ 6 tháng đến 1 năm, có thể gia hạn theo yêu cầu của khách hàng.', NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) VALUES (4, 1, NULL, 4, N'Công Cụ và Thiết Bị', N'- Bình tưới tự động: Dành cho cây lớn hoặc cây yêu cầu tưới nước thường xuyên.
- Kéo cắt tỉa chuyên dụng: Sử dụng để cắt tỉa cành, lá khi cần thiết.
- Bón phân: Công ty sử dụng phân bón hữu cơ để đảm bảo cây phát triển khỏe mạnh.
- Dụng cụ kiểm tra độ ẩm đất: Dùng để theo dõi mức độ ẩm của đất giúp điều chỉnh việc tưới nước.
- Chậu cây: Chậu cây được lựa chọn dựa trên loại cây và không gian văn phòng, có thể là chậu xi măng, chậu sứ, hoặc chậu nhựa.', NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) VALUES (5, 1, NULL, 5, N'Chi Phí và Thanh Toán', N'- Chi phí: Chi phí dịch vụ sẽ được tính dựa trên loại cây, thời gian thuê, số lượng cây, và các dịch vụ đi kèm như chăm sóc và bảo dưỡng.
- Phương thức thanh toán: Khách hàng có thể thanh toán qua chuyển khoản ngân hàng, tiền mặt, hoặc thẻ tín dụng. Thường thì thanh toán được chia làm 2 đợt: Đợt đầu khi ký hợp đồng và đợt cuối khi kết thúc hợp đồng.', NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) VALUES (6, 1, NULL, 6, N'Các Lợi Ích Khi Sử Dụng Dịch Vụ', N'- Tiết kiệm chi phí: Không cần phải đầu tư vào cây cảnh và các công cụ chăm sóc.
- Không lo bảo dưỡng: Công ty chịu trách nhiệm hoàn toàn về việc chăm sóc cây.
- Tạo không gian làm việc thoải mái: Cây xanh giúp giảm căng thẳng, cải thiện sức khỏe và hiệu quả công việc.
- Linh hoạt trong thời gian thuê: Khách hàng có thể thay đổi thời gian thuê tùy theo nhu cầu.', NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) VALUES (7, 1, NULL, 7, N'Phản Hồi và Đánh Giá', N'Khách hàng có thể gửi phản hồi về chất lượng cây xanh và dịch vụ chăm sóc qua các kênh liên lạc của công ty. Đánh giá tích cực giúp công ty cải thiện dịch vụ, trong khi đánh giá tiêu cực sẽ được xem xét và khắc phục kịp thời.', NULL, NULL)


INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES 
(8, 2, NULL, 1, N'Mô Tả Dịch Vụ', 
N'- Dịch vụ này đảm bảo cây xanh trong văn phòng luôn phát triển tốt, xanh tươi và không bị sâu bệnh.' 
+ CHAR(10) + CHAR(13) + 
N'- Đội ngũ kỹ thuật viên sẽ thực hiện định kỳ việc kiểm tra, chăm sóc cây theo quy trình chuyên nghiệp, giúp duy trì vẻ đẹp tự nhiên của cây cảnh trong suốt thời gian thuê hoặc bảo dưỡng.', 
NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES 
(9, 2, NULL, 2, N'Quy trình thực hiện', 
N'Bước 1: Kiểm tra tình trạng cây xanh' 
+ CHAR(10) + CHAR(13) + 
N'- Tần suất: 1-2 lần/tuần (tùy vào gói dịch vụ).' 
+ CHAR(10) + CHAR(13) + 
N'- Nội dung kiểm tra:' 
+ CHAR(10) + CHAR(13) + 
N'   + Mức độ sinh trưởng, màu sắc lá, cành nhánh.' 
+ CHAR(10) + CHAR(13) + 
N'   + Dấu hiệu thiếu nước, thiếu dinh dưỡng hoặc cây bị bệnh.' 
+ CHAR(10) + CHAR(13) + 
N'   + Phát hiện sớm sâu bệnh, nấm mốc, côn trùng gây hại.' 
+ CHAR(10) + CHAR(13) + 
N'   + Đánh giá độ ẩm của đất để quyết định lượng nước tưới phù hợp.' 
+ CHAR(10) + CHAR(13) + 
N'- Dụng cụ sử dụng:' 
+ CHAR(10) + CHAR(13) + 
N'   + Găng tay bảo hộ.' 
+ CHAR(10) + CHAR(13) + 
N'   + Que thử độ ẩm đất.' 
+ CHAR(10) + CHAR(13) + 
N'   + Kính lúp cầm tay để kiểm tra sâu bệnh.' 
+ CHAR(10) + CHAR(13) + 
N'   + Nhật ký theo dõi cây cảnh.' 
+ CHAR(10) + CHAR(13) + 
N'Bước 2: Tưới nước theo nhu cầu của từng loại cây' 
+ CHAR(10) + CHAR(13) + 
N'- Tần suất: 2-4 lần/tuần (tùy loại cây và điều kiện môi trường).' 
+ CHAR(10) + CHAR(13) + 
N'- Nguyên tắc tưới:' 
+ CHAR(10) + CHAR(13) + 
N'   + Cây nội thất có nhu cầu nước ít hơn cây ngoài trời.' 
+ CHAR(10) + CHAR(13) + 
N'   + Tránh tưới quá nhiều gây úng rễ, chỉ tưới khi đất có dấu hiệu khô.' 
+ CHAR(10) + CHAR(13) + 
N'   + Sử dụng nước sạch, không có clo hoặc tạp chất gây hại.' 
+ CHAR(10) + CHAR(13) + 
N'- Dụng cụ sử dụng:' 
+ CHAR(10) + CHAR(13) + 
N'   + Bình tưới vòi dài hoặc bình phun sương.' 
+ CHAR(10) + CHAR(13) + 
N'   + Hệ thống tưới nhỏ giọt (đối với cây lớn).' 
+ CHAR(10) + CHAR(13) + 
N'   + Khăn lau lá (để lau bụi trên lá giúp cây quang hợp tốt hơn).' 
+ CHAR(10) + CHAR(13) + 
N'Bước 3: Bón phân định kỳ để cung cấp dinh dưỡng' 
+ CHAR(10) + CHAR(13) + 
N'- Tần suất: 2-4 tuần/lần (tùy vào loại cây và nhu cầu dinh dưỡng).' 
+ CHAR(10) + CHAR(13) + 
N'- Loại phân bón sử dụng:' 
+ CHAR(10) + CHAR(13) + 
N'   + Phân hữu cơ (phân trùn quế, phân vi sinh).' 
+ CHAR(10) + CHAR(13) + 
N'   + Phân NPK tan chậm dành cho cây cảnh.' 
+ CHAR(10) + CHAR(13) + 
N'   + Phân bón lá dạng phun để bổ sung vi lượng.' 
+ CHAR(10) + CHAR(13) + 
N'- Cách thực hiện:' 
+ CHAR(10) + CHAR(13) + 
N'   + Dùng dụng cụ xới nhẹ lớp đất mặt trước khi bón.' 
+ CHAR(10) + CHAR(13) + 
N'   + Bón lượng phân vừa đủ theo hướng dẫn, tránh bón quá nhiều gây sốc cây.' 
+ CHAR(10) + CHAR(13) + 
N'   + Sau khi bón phân, tưới nước để phân thẩm thấu vào đất.' 
+ CHAR(10) + CHAR(13) + 
N'- Dụng cụ sử dụng:' 
+ CHAR(10) + CHAR(13) + 
N'   + Xẻng nhỏ làm tơi đất.' 
+ CHAR(10) + CHAR(13) + 
N'   + Thước đo định lượng phân bón.' 
+ CHAR(10) + CHAR(13) + 
N'   + Bình tưới nước.' 
+ CHAR(10) + CHAR(13) + 
N'Bước 4: Phun thuốc phòng bệnh, diệt sâu rầy định kỳ' 
+ CHAR(10) + CHAR(13) + 
N'- Tần suất: 1-2 tháng/lần hoặc khi phát hiện sâu bệnh.' 
+ CHAR(10) + CHAR(13) + 
N'- Loại thuốc sử dụng:' 
+ CHAR(10) + CHAR(13) + 
N'   + Chế phẩm sinh học an toàn, không ảnh hưởng đến sức khỏe con người.' 
+ CHAR(10) + CHAR(13) + 
N'   + Thuốc phòng trừ nấm, vi khuẩn, côn trùng gây hại.' 
+ CHAR(10) + CHAR(13) + 
N'   + Dung dịch kích thích cây phát triển khỏe mạnh.' 
+ CHAR(10) + CHAR(13) + 
N'- Cách thực hiện:' 
+ CHAR(10) + CHAR(13) + 
N'   + Kiểm tra và xác định loại sâu bệnh trước khi xử lý.' 
+ CHAR(10) + CHAR(13) + 
N'   + Sử dụng thuốc đúng liều lượng, tránh lạm dụng hóa chất.' 
+ CHAR(10) + CHAR(13) + 
N'   + Phun vào thời điểm sáng sớm hoặc chiều mát để đạt hiệu quả tốt nhất.' 
+ CHAR(10) + CHAR(13) + 
N'- Dụng cụ sử dụng:' 
+ CHAR(10) + CHAR(13) + 
N'   + Bình phun thuốc áp suất thấp.' 
+ CHAR(10) + CHAR(13) + 
N'   + Khẩu trang, găng tay bảo hộ.' 
+ CHAR(10) + CHAR(13) + 
N'   + Dung dịch pha thuốc theo tỷ lệ chuẩn.', 
NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES 
(10, 2, NULL, 3, N'Lợi ích của dịch vụ', 
N'- Giúp cây luôn tươi tốt, tránh tình trạng héo úa, chết cây.' 
+ CHAR(10) + CHAR(13) + 
N'- Tiết kiệm thời gian cho doanh nghiệp, nhân viên không cần lo lắng về việc chăm sóc cây.' 
+ CHAR(10) + CHAR(13) + 
N'- Môi trường văn phòng xanh sạch hơn, tăng cường năng suất làm việc.' 
+ CHAR(10) + CHAR(13) + 
N'- Sử dụng phương pháp sinh học, an toàn cho con người và động vật nuôi.' 
+ CHAR(10) + CHAR(13) + 
N'- Theo dõi và xử lý kịp thời sâu bệnh, tránh lây lan trong không gian văn phòng.', 
NULL, NULL)

INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) VALUES 
(11, 3, NULL, 1, N'Mô Tả Dịch Vụ', 
N'Dịch vụ chăm sóc cây theo lịch đặt trước giúp đảm bảo cây xanh trong văn phòng luôn xanh tốt, khỏe mạnh và phát triển bền vững.' + CHAR(10) + CHAR(13) +
N'Khách hàng có thể đặt lịch chăm sóc định kỳ theo nhu cầu, bao gồm các gói dịch vụ linh hoạt từ hàng tuần, hàng tháng đến theo yêu cầu cụ thể.', NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES 
(12, 3, NULL, 2, N'Đối tượng khách hàng',
N'- Văn phòng, công ty, doanh nghiệp có cây xanh trong không gian làm việc.' + CHAR(10) + CHAR(13) +
N'- Khách hàng đã thuê cây xanh từ dịch vụ cho thuê cây cảnh văn phòng.' + CHAR(10) + CHAR(13) +
N'- Doanh nghiệp hoặc cá nhân sở hữu cây cảnh nhưng không có thời gian hoặc kiến thức chăm sóc.', NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES 
(13, 3, NULL, 3, N'Quy trình thực hiện dịch vụ',
N'Bước 1: Đặt lịch và khảo sát ban đầu' + CHAR(10) + CHAR(13) +
N'- Khách hàng liên hệ qua website, điện thoại hoặc email để đặt lịch chăm sóc.' + CHAR(10) + CHAR(13) +
N'- Nhân viên kỹ thuật đến khảo sát tình trạng cây xanh tại văn phòng.' + CHAR(10) + CHAR(13) +
N'- Ghi nhận số lượng, loại cây, mức độ phát triển và các vấn đề cần xử lý.' + CHAR(10) + CHAR(13) +
N'- Đề xuất lịch trình chăm sóc phù hợp (hàng tuần, hai tuần/lần, hàng tháng…).' + CHAR(10) + CHAR(13) +
N'- Xác nhận với khách hàng và tiến hành lên lịch thực hiện.' + CHAR(10) + CHAR(13) +
CHAR(10) + CHAR(13) +
N'Bước 2: Chuẩn bị công cụ và vật liệu' + CHAR(10) + CHAR(13) +
N'- Trước khi đến chăm sóc, nhân viên chuẩn bị đầy đủ công cụ cần thiết:' + CHAR(10) + CHAR(13) +
N'   + Dụng cụ cơ bản: Kéo tỉa cành, dao cắt, bay xới đất, găng tay làm vườn.' + CHAR(10) + CHAR(13) +
N'   + Thiết bị tưới nước: Bình xịt nước, bình phun sương, vòi tưới có điều chỉnh áp lực.' + CHAR(10) + CHAR(13) +
N'   + Dinh dưỡng cho cây: Phân bón hữu cơ, phân vi sinh, dung dịch kích thích tăng trưởng.' + CHAR(10) + CHAR(13) +
N'   + Thuốc bảo vệ thực vật: Chế phẩm sinh học phòng trừ sâu bệnh, thuốc trừ nấm tự nhiên.' + CHAR(10) + CHAR(13) +
N'- Vật tư khác: Giá thể, đất trồng bổ sung, chậu cây (nếu cần thay thế).' + CHAR(10) + CHAR(13), NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES 
(14, 3, NULL, 4, N'Thời gian thực hiện và tần suất chăm sóc',
N'- Dịch vụ có thể được đặt trước với các tùy chọn:' + CHAR(10) + CHAR(13) +
N'   + Chăm sóc hàng tuần: Phù hợp với cây cần bảo dưỡng thường xuyên.' + CHAR(10) + CHAR(13) +
N'   + Chăm sóc hai tuần/lần: Dành cho cây có mức bảo dưỡng trung bình.' + CHAR(10) + CHAR(13) +
N'   + Chăm sóc hàng tháng: Dành cho cây ít cần chăm sóc nhưng vẫn cần kiểm tra định kỳ.' + CHAR(10) + CHAR(13) +
N'   + Chăm sóc theo yêu cầu: Khi cây gặp vấn đề đột xuất cần xử lý ngay.' + CHAR(10) + CHAR(13) +
N'- Mỗi lần chăm sóc kéo dài từ 30 phút đến 2 giờ, tùy vào số lượng và tình trạng cây.', NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES 
(15, 3, NULL, 5, N'Lợi ích khi sử dụng dịch vụ',
N'- Duy trì cây xanh khỏe mạnh, tránh tình trạng cây chết do thiếu chăm sóc.' + CHAR(10) + CHAR(13) +
N'- Tiết kiệm thời gian cho khách hàng, không cần tự chăm cây.' + CHAR(10) + CHAR(13) +
N'- Tư vấn chuyên sâu từ đội ngũ kỹ thuật, đảm bảo cây phát triển bền vững.' + CHAR(10) + CHAR(13) +
N'- Không gian làm việc trong lành, đẹp mắt, nâng cao tinh thần làm việc.' + CHAR(10) + CHAR(13) +
N'- Dịch vụ linh hoạt, có thể đặt lịch định kỳ hoặc theo nhu cầu phát sinh.', NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES 
(16, 3, NULL, 6, N'Cam kết dịch vụ',
N'- Đội ngũ nhân viên chuyên nghiệp, giàu kinh nghiệm trong lĩnh vực cây cảnh.' + CHAR(10) + CHAR(13) +
N'- Sử dụng sản phẩm an toàn, thân thiện với môi trường.' + CHAR(10) + CHAR(13) +
N'- Cam kết chất lượng dịch vụ, đảm bảo sự hài lòng của khách hàng.', NULL, NULL)
INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES 
(17, 3, NULL, 7, N'Liên hệ đặt dịch vụ',
N'📍 Địa chỉ: 36 Lê Trọng Tấn, La Khê, Hà Đông, Hà Nội' + CHAR(10) + CHAR(13) +
N'📞 Hotline: 0373 993 662' + CHAR(10) + CHAR(13) +
N'📧 Email: ngan36758@gmail.com' + CHAR(10) + CHAR(13) +
N'🌍 Website: ' + CHAR(10) + CHAR(13) +
N'👉 Đặt lịch ngay hôm nay để không gian văn phòng luôn tràn đầy sức sống!', NULL, NULL);

INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES (18, 4, NULL, 1, N'Mô Tả Dịch Vụ', 
N'Dịch vụ cứu chữa cây suy yếu, cây héo úa giúp khôi phục sức sống cho cây cảnh trong văn phòng, đảm bảo cây phát triển khỏe mạnh, xanh tươi và duy trì vẻ đẹp tự nhiên.' 
+ CHAR(10) + CHAR(13) + 
N'Dịch vụ này phù hợp với các doanh nghiệp, văn phòng sử dụng cây xanh nhưng gặp tình trạng cây bị vàng lá, rụng lá, héo úa, hoặc ngừng phát triển do môi trường, cách chăm sóc không phù hợp hoặc bệnh lý.', 
NULL, NULL);

INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES (19, 4, NULL, 2, N'Quy trình thực hiện dịch vụ',
N'Bước 1: Khảo sát và đánh giá tình trạng cây' + CHAR(10) + CHAR(13) + 
N'- Thời gian: 15 - 30 phút/cây (tùy mức độ suy yếu)' + CHAR(10) + CHAR(13) + 
N'- Công cụ: Đèn soi lá, kính lúp chuyên dụng, nhiệt ẩm kế, dụng cụ đo pH đất, bộ xét nghiệm sâu bệnh, sổ ghi chú.' + CHAR(10) + CHAR(13) + 
N'- Nội dung thực hiện:' + CHAR(10) + CHAR(13) + 
N'  + Quan sát tổng thể cây: tình trạng lá, thân, rễ, đất trồng.' + CHAR(10) + CHAR(13) + 
N'  + Kiểm tra độ ẩm đất, pH đất và các yếu tố vi sinh.' + CHAR(10) + CHAR(13) + 
N'  + Xác định nguyên nhân gây suy yếu: thiếu nước, thừa nước, sâu bệnh, thiếu dinh dưỡng, ánh sáng kém, môi trường không phù hợp.' 
+ CHAR(10) + CHAR(13) + CHAR(10) + CHAR(13) + 
N'Bước 2: Xử lý nguyên nhân và phục hồi cây' + CHAR(10) + CHAR(13) + 
N'- Thời gian: 1 - 3 ngày tùy mức độ suy yếu' + CHAR(10) + CHAR(13) + 
N'- Công cụ: Kéo cắt tỉa, thuốc trừ sâu sinh học, phân bón hữu cơ, đất trồng cải tạo, chậu mới (nếu cần), dung dịch kích thích ra rễ, bình xịt nước.' + CHAR(10) + CHAR(13) + 
N'- Nội dung thực hiện:' + CHAR(10) + CHAR(13) + 
N'  + Nếu cây bị thiếu nước: Điều chỉnh chế độ tưới phù hợp, bổ sung dưỡng chất giúp cây hồi phục.' + CHAR(10) + CHAR(13) + 
N'  + Nếu cây bị thừa nước, úng rễ: Thay đất mới, cắt bỏ rễ hỏng, sử dụng thuốc chống nấm và kích thích mọc rễ.' + CHAR(10) + CHAR(13) + 
N'  + Nếu cây bị sâu bệnh: Loại bỏ lá hư hại, xử lý sâu bệnh bằng thuốc trừ sâu sinh học hoặc thiên địch.' + CHAR(10) + CHAR(13) + 
N'  + Nếu cây thiếu dinh dưỡng: Bổ sung phân bón hữu cơ phù hợp theo từng loại cây.' + CHAR(10) + CHAR(13) + 
N'  + Nếu môi trường ánh sáng không phù hợp: Di chuyển cây đến vị trí có ánh sáng tốt hơn hoặc sử dụng đèn LED chuyên dụng cho cây cảnh.', 
NULL, NULL);

INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES (20, 4, NULL, 3, N'Thời gian thực hiện dịch vụ',
N'- Dịch vụ khẩn cấp: 4 - 8 giờ kể từ khi tiếp nhận yêu cầu (đối với cây có nguy cơ chết cao).' + CHAR(10) + CHAR(13) + 
N'- Dịch vụ thông thường: Xử lý trong vòng 24 - 48 giờ.' + CHAR(10) + CHAR(13) + 
N'- Theo dõi sau dịch vụ: 1 - 2 tuần để đảm bảo cây phục hồi tốt.', 
NULL, NULL);

INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES (21, 4, NULL, 4, N'Đối tượng khách hàng',
N'- Các văn phòng có cây cảnh bị suy yếu do điều kiện môi trường không phù hợp.' + CHAR(10) + CHAR(13) + 
N'- Các công ty đã thuê cây xanh nhưng cây bị xuống sức và cần được phục hồi.' + CHAR(10) + CHAR(13) + 
N'- Cá nhân hoặc doanh nghiệp muốn cứu cây quý, cây có giá trị cao khỏi nguy cơ chết.', 
NULL, NULL);

INSERT [dbo].[ServiceDescriptions] ([DescriptionId], [ServiceId], [Image], [StepNumber], [Title], [Content], [CreatedDate], [UpdatedDate]) 
VALUES (22, 4, NULL, 5, N'Cam kết dịch vụ',
N'- Kiểm tra kỹ nguyên nhân và tư vấn giải pháp phù hợp.' + CHAR(10) + CHAR(13) + 
N'  + Sử dụng phương pháp tự nhiên, an toàn, không ảnh hưởng đến sức khỏe nhân viên trong văn phòng.' + CHAR(10) + CHAR(13) + 
N'  + Theo dõi tình trạng cây sau khi phục hồi để đảm bảo hiệu quả dài hạn.' + CHAR(10) + CHAR(13) + 
N'  + Nếu cây không thể phục hồi, hỗ trợ thay thế cây mới với chi phí ưu đãi.' + CHAR(10) + CHAR(13) + 
N'- Dịch vụ cứu chữa cây suy yếu, cây héo úa giúp duy trì không gian xanh tươi mát cho văn phòng, nâng cao chất lượng môi trường làm việc và thể hiện sự chuyên nghiệp, quan tâm đến thiên nhiên của doanh nghiệp.', 
NULL, NULL);


SET IDENTITY_INSERT [dbo].[ServiceDescriptions] OFF
GO
SET IDENTITY_INSERT [dbo].[Services] ON 

INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (1, N'Cho thuê cây xanh văn phòng (ngắn hạn & dài hạn)', N'Dịch vụ cho thuê cây xanh văn phòng ngắn hạn và dài hạn cung cấp giải pháp cây xanh cho không gian làm việc, giúp tạo môi trường trong lành, tăng cường hiệu quả công việc. Khách hàng có thể lựa chọn thuê cây cảnh phù hợp với diện tích và phong cách văn phòng của mình, với các gói thuê linh hoạt từ ngắn hạn (từ vài tuần đến vài tháng) đến dài hạn (nhiều tháng hoặc năm). Dịch vụ bao gồm việc cung cấp cây xanh, chăm sóc, và thay thế khi cần thiết, giúp duy trì không gian xanh mát, sinh động cho văn phòng mà không cần lo lắng về việc chăm sóc.', CAST(N'2025-02-17T16:20:41.277' AS DateTime), NULL, N'Đang hoạt động', 1, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (2, N'Kiểm tra, tưới nước, bón phân, phun thuốc phòng bệnh', NULL, CAST(N'2025-02-17T16:21:09.833' AS DateTime), NULL, N'Đang hoạt động', 1, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (3, N'Chăm sóc cây theo lịch đặt trước', NULL,  CAST(N'2025-02-17T16:21:24.610' AS DateTime), NULL, N'Đang hoạt động', 2, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (4, N'Cứu chữa cây suy yếu, cây héo úa', NULL,  CAST(N'2025-02-17T16:21:36.977' AS DateTime), NULL, N'Đang hoạt động', 2, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (5, N'Xử lý cây trong trường hợp khẩn cấp (bệnh, sâu bệnh, gãy đổ)', NULL,  CAST(N'2025-02-17T16:21:45.470' AS DateTime), NULL, N'Đang hoạt động', 2, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (6, N'Cắt tỉa cành, nhánh cây tạo dáng', NULL,  CAST(N'2025-02-17T16:21:53.937' AS DateTime), NULL, N'Đang hoạt động', 3, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (7, N'Xử lý cành cây gãy, mục rỗng', NULL,  CAST(N'2025-02-17T16:22:02.870' AS DateTime), NULL, N'Đang hoạt động', 3, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (8, N'Đốn hạ cây theo yêu cầu, xử lý gốc cây', NULL,  CAST(N'2025-02-17T16:22:11.680' AS DateTime), NULL, N'Đang hoạt động', 3, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (9, N'Thiết kế tiểu cảnh trong văn phòng', NULL,  CAST(N'2025-02-17T16:22:22.020' AS DateTime), NULL, N'Đang hoạt động', 4, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (10, N'Cải tạo & bổ sung cây xanh cho không gian làm việc', NULL,  CAST(N'2025-02-17T16:22:31.267' AS DateTime), NULL, N'Đang hoạt động', 4, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (11, N'Cung cấp, trồng mới cây cảnh theo yêu cầu', NULL,  CAST(N'2025-02-17T16:22:40.283' AS DateTime), NULL, N'Đang hoạt động', 4, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (12, N'Dịch vụ chăm sóc chuyên sâu cho cây quý hiếm', NULL,  CAST(N'2025-02-17T16:22:48.217' AS DateTime), NULL, N'Đang hoạt động', 5, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (13, N'Tư vấn & thiết kế không gian xanh theo phong thủy', NULL,  CAST(N'2025-02-17T16:22:56.113' AS DateTime), NULL, N'Đang hoạt động', 5, NULL)
INSERT [dbo].[Services] ([ServiceId], [ServiceName], [Description], [CreatedDate], [UpdatedDate], [Status], [CategoryServiceId], [Image]) VALUES (14, N'Gói dịch vụ chăm sóc toàn diện theo yêu cầu khách hàng', NULL,  CAST(N'2025-02-17T16:23:04.140' AS DateTime), NULL, N'Đang hoạt động', 5, NULL)
SET IDENTITY_INSERT [dbo].[Services] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Admins__A9D105343D7EFDAE]    Script Date: 2/22/2025 2:43:38 PM ******/
ALTER TABLE [dbo].[Admins] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Admins__A9D10534A66FEAA3]    Script Date: 2/22/2025 2:43:38 PM ******/
ALTER TABLE [dbo].[Admins] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Customer__5C7E359E38B44984]    Script Date: 2/22/2025 2:43:38 PM ******/
ALTER TABLE [dbo].[Customers] ADD UNIQUE NONCLUSTERED 
(
	[Phone] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Customer__5C7E359EE40DA5D6]    Script Date: 2/22/2025 2:43:38 PM ******/
ALTER TABLE [dbo].[Customers] ADD UNIQUE NONCLUSTERED 
(
	[Phone] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Customer__A9D10534C39188FB]    Script Date: 2/22/2025 2:43:38 PM ******/
ALTER TABLE [dbo].[Customers] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Customer__A9D10534DF55B680]    Script Date: 2/22/2025 2:43:38 PM ******/
ALTER TABLE [dbo].[Customers] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__PaymentM__218CFB1727D6045C]    Script Date: 2/22/2025 2:43:38 PM ******/
ALTER TABLE [dbo].[PaymentMethods] ADD UNIQUE NONCLUSTERED 
(
	[MethodName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Roles__8A2B6160A605AFFB]    Script Date: 2/22/2025 2:43:38 PM ******/
ALTER TABLE [dbo].[Roles] ADD UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Staffs__5C7E359EFE4A4C67]    Script Date: 2/22/2025 2:43:38 PM ******/
ALTER TABLE [dbo].[Staffs] ADD UNIQUE NONCLUSTERED 
(
	[Phone] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Staffs__A9D105342C79B1B8]    Script Date: 2/22/2025 2:43:38 PM ******/
ALTER TABLE [dbo].[Staffs] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Admins] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Admins] ADD  DEFAULT (N'Hoạt động') FOR [Status]
GO
ALTER TABLE [dbo].[Banners] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Banners] ADD  DEFAULT (N'Hiện') FOR [Status]
GO
ALTER TABLE [dbo].[CareSchedules] ADD  DEFAULT (getdate()) FOR [ScheduledDate]
GO
ALTER TABLE [dbo].[CareSchedules] ADD  DEFAULT (N'Chưa thực hiện') FOR [Status]
GO
ALTER TABLE [dbo].[Contacts] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Contacts] ADD  DEFAULT (N'Chưa xử lý') FOR [Status]
GO
ALTER TABLE [dbo].[Contracts] ADD  DEFAULT (N'Đang hiệu lực') FOR [Status]
GO
ALTER TABLE [dbo].[Customers] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Customers] ADD  DEFAULT (N'Hoạt động') FOR [Status]
GO
ALTER TABLE [dbo].[Feedbacks] ADD  DEFAULT (getdate()) FOR [FeedbackDate]
GO
ALTER TABLE [dbo].[Feedbacks] ADD  DEFAULT (N'Chờ duyệt') FOR [Status]
GO
ALTER TABLE [dbo].[InventoryItems] ADD  DEFAULT (N'Có sẵn') FOR [Status]
GO
ALTER TABLE [dbo].[InventoryItems] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[News] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[News] ADD  DEFAULT (N'Hiển thị') FOR [Status]
GO
ALTER TABLE [dbo].[OrderDetails] ADD  DEFAULT (N'Đang chờ xác nhận') FOR [Status]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (N'Chưa thanh toán') FOR [PaymentStatus]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (N'Chờ xử lý') FOR [Status]
GO
ALTER TABLE [dbo].[Partners] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Partners] ADD  DEFAULT (N'Đang hợp tác') FOR [Status]
GO
ALTER TABLE [dbo].[ServiceRequests] ADD  DEFAULT (getdate()) FOR [RequestDate]
GO
ALTER TABLE [dbo].[ServiceRequests] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[ServiceRequests] ADD  DEFAULT (N'Chờ xử lý') FOR [Status]
GO
ALTER TABLE [dbo].[Services] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Services] ADD  DEFAULT (N'Đang hoạt động') FOR [Status]
GO
ALTER TABLE [dbo].[Staffs] ADD  DEFAULT (getdate()) FOR [HireDate]
GO
ALTER TABLE [dbo].[Staffs] ADD  DEFAULT (N'Đang làm') FOR [Status]
GO
ALTER TABLE [dbo].[Admins]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
GO
ALTER TABLE [dbo].[CareScheduleInventory]  WITH CHECK ADD FOREIGN KEY([CareScheduleId])
REFERENCES [dbo].[CareSchedules] ([ScheduleId])
GO
ALTER TABLE [dbo].[CareScheduleInventory]  WITH CHECK ADD FOREIGN KEY([ItemId])
REFERENCES [dbo].[InventoryItems] ([ItemId])
GO
ALTER TABLE [dbo].[CareSchedules]  WITH CHECK ADD FOREIGN KEY([ContractId])
REFERENCES [dbo].[Contracts] ([ContractId])
GO
ALTER TABLE [dbo].[CareSchedules]  WITH CHECK ADD FOREIGN KEY([StaffId])
REFERENCES [dbo].[Staffs] ([StaffId])
GO
ALTER TABLE [dbo].[Contracts]  WITH CHECK ADD FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[Customers]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
GO
ALTER TABLE [dbo].[Feedbacks]  WITH CHECK ADD FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[Feedbacks]  WITH CHECK ADD FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Services] ([ServiceId])
GO
--ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([ContractId])
--REFERENCES [dbo].[Contracts] ([ContractId])
--GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[ServiceDescriptions]  WITH CHECK ADD  CONSTRAINT [FK_ServiceDescriptions_Service] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Services] ([ServiceId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ServiceDescriptions] CHECK CONSTRAINT [FK_ServiceDescriptions_Service]
GO
ALTER TABLE [dbo].[ServiceInventory]  WITH CHECK ADD FOREIGN KEY([ItemId])
REFERENCES [dbo].[InventoryItems] ([ItemId])
GO
ALTER TABLE [dbo].[ServiceInventory]  WITH CHECK ADD  CONSTRAINT [FK_ServiceInventory_Service] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Services] ([ServiceId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ServiceInventory] CHECK CONSTRAINT [FK_ServiceInventory_Service]
GO
ALTER TABLE [dbo].[ServiceRequests]  WITH CHECK ADD FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[ServiceRequests]  WITH CHECK ADD FOREIGN KEY([ScheduleID])
REFERENCES [dbo].[CareSchedules] ([ScheduleId])
GO
ALTER TABLE [dbo].[ServiceRequests]  WITH CHECK ADD FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Services] ([ServiceId])
GO
ALTER TABLE [dbo].[Services]  WITH CHECK ADD  CONSTRAINT [FK_ServiceCategories] FOREIGN KEY([CategoryServiceId])
REFERENCES [dbo].[ServiceCategories] ([CategoryServiceId])
GO
ALTER TABLE [dbo].[Services] CHECK CONSTRAINT [FK_ServiceCategories]
GO
ALTER TABLE [dbo].[Staffs]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
GO
ALTER TABLE [dbo].[Admins]  WITH CHECK ADD CHECK  (([Status]=N'Không hoạt động' OR [Status]=N'Hoạt động'))
GO
ALTER TABLE [dbo].[Banners]  WITH CHECK ADD CHECK  (([Status]=N'Tạm dừng' OR [Status]=N'Ẩn' OR [Status]=N'Hiện'))
GO
ALTER TABLE [dbo].[CareScheduleInventory]  WITH CHECK ADD CHECK  (([QuantityUsed]>(0)))
GO
ALTER TABLE [dbo].[CareSchedules]  WITH CHECK ADD CHECK  (([Duration]>(0)))
GO
ALTER TABLE [dbo].[CareSchedules]  WITH CHECK ADD CHECK  (([Status]=N'Hoàn thành' OR [Status]=N'Đang thực hiện' OR [Status]=N'Chưa thực hiện'))
GO
ALTER TABLE [dbo].[Contacts]  WITH CHECK ADD CHECK  (([Status]=N'Hoàn thành' OR [Status]=N'Đang xử lý' OR [Status]=N'Chưa xử lý'))
GO
ALTER TABLE [dbo].[Contracts]  WITH CHECK ADD CHECK  (([Status]=N'Đã hủy' OR [Status]=N'Đã hết hạn' OR [Status]=N'Đang hiệu lực'))
GO
ALTER TABLE [dbo].[Customers]  WITH CHECK ADD CHECK  (([CustomerType]=N'Ký hợp đồng' OR [CustomerType]=N'Đặt dịch vụ' OR [CustomerType]=N'Vãng lai'))
GO
ALTER TABLE [dbo].[Customers]  WITH CHECK ADD CHECK  (([Status]=N'Bị khóa' OR [Status]=N'Không hoạt động' OR [Status]=N'Hoạt động'))
GO
ALTER TABLE [dbo].[Feedbacks]  WITH CHECK ADD CHECK  (([Rating]>=(1) AND [Rating]<=(5)))
GO
ALTER TABLE [dbo].[Feedbacks]  WITH CHECK ADD CHECK  (([Status]=N'Đã duyệt' OR [Status]=N'Chờ duyệt'))
GO
ALTER TABLE [dbo].[InventoryItems]  WITH CHECK ADD CHECK  (([ItemType]=N'Công cụ' OR [ItemType]=N'Nguyên liệu'))
GO
ALTER TABLE [dbo].[News]  WITH CHECK ADD CHECK  (([Status]=N'Ẩn' OR [Status]=N'Hiển thị'))
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD CHECK  (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD CHECK  (([Status]=N'Đã xác nhận' OR [Status]=N'Đang chờ xác nhận'))
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD CHECK  (([Discount]>=(0)))
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD CHECK  (([PaymentStatus]=N'Hoàn tiền' OR [PaymentStatus]=N'Ðã thanh toán' OR [PaymentStatus]=N'Chưa thanh toán' OR [PaymentStatus]=N'Thanh toán 1 phần'))
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD CHECK  (([Status]=N'Chờ xác nhận' OR [Status]=N'Đã xác nhận' OR [Status]=N'Đã hoàn thành' OR [Status]=N'Hủy'))
GO
ALTER TABLE [dbo].[Partners]  WITH CHECK ADD CHECK  (([Status]=N'Đang hợp tác' OR [Status]=N'Ðang hợp tác' OR [Status]=N'Tạm dừng hợp tác' OR [Status]=N'Ngừng hợp tác' OR [Status]=N'Đối tác tiềm năng' OR [Status]=N'Đối tác chiến lược'))
GO
ALTER TABLE [dbo].[ServiceInventory]  WITH CHECK ADD CHECK  (([QuantityRequired]>(0)))
GO
ALTER TABLE [dbo].[ServiceRequests]  WITH CHECK ADD CHECK  (([Price]>=(0)))
GO
ALTER TABLE [dbo].[ServiceRequests]  WITH CHECK ADD CHECK  (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[ServiceRequests]  WITH CHECK ADD CHECK  (([Status]=N'Từ chối' OR [Status]=N'Đã xử lý' OR [Status]=N'Chờ xử lý'))
GO
ALTER TABLE [dbo].[Services]  WITH CHECK ADD CHECK  (([Status]=N'Tạm dừng' OR [Status]=N'Đang hoạt động'))
GO
ALTER TABLE [dbo].[Staffs]  WITH CHECK ADD CHECK  (([Status]=N'Bị sa thải' OR [Status]=N'Nghỉ việc' OR [Status]=N'Đang làm'))
GO

INSERT INTO [ServicePrices] ([ServiceId], [ServiceType], [TreeSize], [OfficeSize], [DurationInMonths], [NumberOfTrees], [Price]) 
VALUES
-- 🏢 Dịch vụ 1: Cho thuê cây xanh văn phòng (ngắn hạn & dài hạn)
(1, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 50000),
(1, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 70000),
(1, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 90000),
(1, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 70000),
(1, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 90000),
(1, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 120000),
(1, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 90000),
(1, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 120000),
(1, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 150000),

(1, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL, 1500000),
(1, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL, 2100000),
(1, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL, 2700000),
(1, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL, 2100000),
(1, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,2700000),
(1, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL, 3300000),
(1, '3 tháng', 'Lớn', 'Dưới 50m²', 3,NULL,2700000),
(1, '3 tháng', 'Lớn', '50-100m²', 3, NULL, 3300000),
(1, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,3900000),

(1, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 3000000),
(1, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 4200000),
(1, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 5400000),
(1, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 4200000),
(1, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 5400000),
(1, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 6600000),
(1, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 5400000),
(1, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 6600000),
(1, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 7800000),
-- 🪴 Dịch vụ 2: Kiểm tra, tưới nước, bón phân, phun thuốc phòng bệnh
(2, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 30000),
(2, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 40000),
(2, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 50000),
(2, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 50000),
(2, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 60000),
(2, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 70000),
(2, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 70000),
(2, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 90000),
(2, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 110000),
(2, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,900000),
(2, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,1200000),
(2, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,1500000),
(2, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,1500000),
(2, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,1800000),
(2, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,2100000),
(2, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,2100000),
(2, '3 tháng', 'Lớn', '50-100m²', 3, NULL,2700000),
(2, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,3300000),
(2, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 1800000),
(2, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 2400000),
(2, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 3000000),
(2, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 3000000),
(2, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 3600000),
(2, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 4200000),
(2, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 4200000),
(2, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 5400000),
(2, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 6600000),
(2, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 3600000),
(2, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 4800000),
(2, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 6000000),
(2, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 6000000),
(2, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 7200000),
(2, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 8400000),
(2, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 8400000),
(2, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 10800000),
(2, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 13200000),
--- 🪴 Dịch vụ 3: Chăm sóc cây theo lịch đặt trước
(3, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 35000),
(3, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 45000),
(3, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 55000),
(3, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 55000),
(3, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 65000),
(3, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 75000),
(3, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 75000),
(3, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 95000),
(3, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 115000),
(3, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,1050000),
(3, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,1350000),
(3, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,1650000),
(3, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,1650000),
(3, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,1950000),
(3, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,2250000),
(3, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,2250000),
(3, '3 tháng', 'Lớn', '50-100m²', 3, NULL,2850000),
(3, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,3450000),
(3, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 2100000),
(3, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 2700000),
(3, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 3300000),
(3, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 3300000),
(3, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 3900000),
(3, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 4500000),
(3, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 4500000),
(3, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 5700000),
(3, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 6900000),
(3, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 4200000),
(3, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 5400000),
(3, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 6600000),
(3, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 6600000),
(3, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 7800000),
(3, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 9000000),
(3, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 9000000),
(3, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 11400000),
(3, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 13800000),
--- 🌿 Dịch vụ 4: Cứu chữa cây suy yếu, cây héo úa
(4, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 40000),
(4, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 50000),
(4, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 60000),
(4, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 60000),
(4, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 70000),
(4, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 80000),
(4, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 80000),
(4, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 100000),
(4, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 120000),
(4, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,1200000),
(4, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,1500000),
(4, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,1800000),
(4, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,1800000),
(4, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,2100000),
(4, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,2400000),
(4, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,2400000),
(4, '3 tháng', 'Lớn', '50-100m²', 3, NULL,3000000),
(4, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,3600000),
(4, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 2400000),
(4, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 3000000),
(4, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 3600000),
(4, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 3600000),
(4, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 4200000),
(4, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 4800000),
(4, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 4800000),
(4, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 6000000),
(4, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 7200000),
(4, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 4800000),
(4, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 6000000),
(4, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 7200000),
(4, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 7200000),
(4, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 8400000),
(4, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 9600000),
(4, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 9600000),
(4, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 12000000),
(4, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 14400000),
--- 🌿 Dịch vụ 5: Xử lý cây trong trường hợp khẩn cấp (bệnh, sâu bệnh, gãy đổ)
(5, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 50000),
(5, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 60000),
(5, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 70000),
(5, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 70000),
(5, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 80000),
(5, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 90000),
(5, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 90000),
(5, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 110000),
(5, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 130000),
(5, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,1500000),
(5, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,1800000),
(5, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,2100000),
(5, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,2100000),
(5, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,2400000),
(5, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,2700000),
(5, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,2700000),
(5, '3 tháng', 'Lớn', '50-100m²', 3, NULL,3300000),
(5, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,3900000),
(5, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 3000000),
(5, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 3600000),
(5, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 4200000),
(5, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 4200000),
(5, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 4800000),
(5, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 5400000),
(5, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 5400000),
(5, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 6600000),
(5, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 7800000),
(5, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 6000000),
(5, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 7200000),
(5, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 8400000),
(5, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 8400000),
(5, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 9600000),
(5, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 10800000),
(5, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 10800000),
(5, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 13200000),
(5, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 15600000),
--- ✂ Dịch vụ 6: Cắt tỉa cành, nhánh cây tạo dáng
(6, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 40000),
(6, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 50000),
(6, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 60000),
(6, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 60000),
(6, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 70000),
(6, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 80000),
(6, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 80000),
(6, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 100000),
(6, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 120000),
(6, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,1200000),
(6, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,1500000),
(6, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,1800000),
(6, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,1800000),
(6, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,2100000),
(6, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,2400000),
(6, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,2400000),
(6, '3 tháng', 'Lớn', '50-100m²', 3, NULL,3000000),
(6, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,3600000),
(6, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 2400000),
(6, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 3000000),
(6, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 3600000),
(6, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 3600000),
(6, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 4200000),
(6, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 4800000),
(6, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 4800000),
(6, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 6000000),
(6, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 7200000),
(6, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 4800000),
(6, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 6000000),
(6, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 7200000),
(6, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 7200000),
(6, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 8400000),
(6, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 9600000),
(6, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 9600000),
(6, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 12000000),
(6, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 14400000),
--- 🌳 Dịch vụ 7: Xử lý cành cây gãy, mục rỗng
(7, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 50000),
(7, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 60000),
(7, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 70000),
(7, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 70000),
(7, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 80000),
(7, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 90000),
(7, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 90000),
(7, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 110000),
(7, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 130000),
(7, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,1500000),
(7, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,1800000),
(7, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,2100000),
(7, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,2100000),
(7, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,2400000),
(7, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,2700000),
(7, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,2700000),
(7, '3 tháng', 'Lớn', '50-100m²', 3, NULL,3300000),
(7, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,3900000),
(7, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 3000000),
(7, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 3600000),
(7, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 4200000),
(7, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 4200000),
(7, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 4800000),
(7, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 5400000),
(7, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 5400000),
(7, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 6600000),
(7, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 7800000),
(7, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 6000000),
(7, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 7200000),
(7, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 8400000),
(7, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 8400000),
(7, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 9600000),
(7, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 10800000),
(7, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 10800000),
(7, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 13200000),
(7, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 15600000),
--- 🌳 Dịch vụ 8: Đốn hạ cây theo yêu cầu, xử lý gốc cây
(8, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 70000),
(8, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 80000),
(8, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 90000),
(8, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 90000),
(8, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 100000),
(8, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 110000),
(8, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 110000),
(8, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 130000),
(8, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 150000),
(8, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,2100000),
(8, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,2400000),
(8, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,2700000),
(8, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,2700000),
(8, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,3000000),
(8, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,3300000),
(8, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,3300000),
(8, '3 tháng', 'Lớn', '50-100m²', 3, NULL,3900000),
(8, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,4500000),
(8, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 4200000),
(8, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 4800000),
(8, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 5400000),
(8, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 5400000),
(8, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 6000000),
(8, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 6600000),
(8, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 6600000),
(8, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 7800000),
(8, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 9000000),
(8, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 8400000),
(8, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 9600000),
(8, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 10800000),
(8, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 10800000),
(8, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 12000000),
(8, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 13200000),
(8, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 13200000),
(8, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 15600000),
(8, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 18000000),
--- 🌿 Dịch vụ 9: Thiết kế tiểu cảnh trong văn phòng
(9, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 500000),
(9, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 600000),
(9, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 700000),
(9, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 700000),
(9, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 800000),
(9, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 900000),
(9, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 900000),
(9, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 1100000),
(9, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 1300000),
(9, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,15000000),
(9, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,18000000),
(9, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,21000000),
(9, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,21000000),
(9, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,24000000),
(9, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,27000000),
(9, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,27000000),
(9, '3 tháng', 'Lớn', '50-100m²', 3, NULL,33000000),
(9, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,39000000),
(9, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 30000000),
(9, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 36000000),
(9, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 42000000),
(9, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 42000000),
(9, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 48000000),
(9, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 54000000),
(9, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 54000000),
(9, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 66000000),
(9, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 78000000),
(9, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 60000000),
(9, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 72000000),
(9, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 84000000),
(9, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 84000000),
(9, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 96000000),
(9, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 108000000),
(9, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 108000000),
(9, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 132000000),
(9, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 156000000),
--- 🌿 Dịch vụ 10: Cải tạo & bổ sung cây xanh cho không gian làm việc
(10, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 400000),
(10, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 500000),
(10, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 600000),
(10, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 600000),
(10, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 700000),
(10, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 800000),
(10, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 800000),
(10, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 1000000),
(10, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 1200000),
(10, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,12000000),
(10, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,15000000),
(10, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,18000000),
(10, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,18000000),
(10, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,21000000),
(10, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,24000000),
(10, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,24000000),
(10, '3 tháng', 'Lớn', '50-100m²', 3, NULL,30000000),
(10, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,36000000),
(10, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 24000000),
(10, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 30000000),
(10, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 36000000),
(10, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 36000000),
(10, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 42000000),
(10, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 48000000),
(10, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 48000000),
(10, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 60000000),
(10, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 72000000),
(10, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 48000000),
(10, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 60000000),
(10, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 72000000),
(10, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 72000000),
(10, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 84000000),
(10, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 96000000),
(10, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 96000000),
(10, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 120000000),
(10, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 144000000),
--- 🌿 Dịch vụ 11: Cung cấp, trồng mới cây cảnh theo yêu cầu
(11, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 50000),
(11, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 70000),
(11, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 90000),
(11, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 80000),
(11, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 100000),
(11, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 120000),
(11, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 150000),
(11, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 180000),
(11, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 220000),
(11, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,1500000),
(11, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,2100000),
(11, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,2700000),
(11, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,2400000),
(11, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,3000000),
(11, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,3600000),
(11, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,4500000),
(11, '3 tháng', 'Lớn', '50-100m²', 3, NULL,5400000),
(11, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,6600000),
(11, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 3000000),
(11, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 4200000),
(11, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 5400000),
(11, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 4800000),
(11, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 6000000),
(11, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 7200000),
(11, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 9000000),
(11, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 10800000),
(11, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 13200000),
(11, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 6000000),
(11, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 8400000),
(11, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 10800000),
(11, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 9600000),
(11, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 12000000),
(11, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 14400000),
(11, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 18000000),
(11, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 21600000),
(11, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 26400000),

---🌿 Dịch vụ 12: Dịch vụ chăm sóc chuyên sâu cho cây quý hiếm
(12, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 100000),
(12, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 130000),
(12, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 160000),
(12, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 150000),
(12, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 180000),
(12, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 210000),
(12, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 250000),
(12, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 300000),
(12, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 350000),
(12, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,3000000),
(12, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,3900000),
(12, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,4800000),
(12, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,4500000),
(12, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,5400000),
(12, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,6300000),
(12, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,7500000),
(12, '3 tháng', 'Lớn', '50-100m²', 3, NULL,9000000),
(12, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,10500000),
(12, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 6000000),
(12, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 7800000),
(12, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 9600000),
(12, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 9000000),
(12, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 10800000),
(12, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 12600000),
(12, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 15000000),
(12, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 18000000),
(12, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 21000000),
(12, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 12000000),
(12, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 15600000),
(12, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 19200000),
(12, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 18000000),
(12, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 21600000),
(12, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 25200000),
(12, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 30000000),
(12, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 36000000),
(12, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 42000000),

--- 🌿 Dịch vụ 13: Tư vấn & thiết kế không gian xanh theo phong thủy
(13, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 200000),
(13, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 250000),
(13, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 300000),
(13, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 300000),
(13, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 350000),
(13, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 400000),
(13, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 500000),
(13, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 600000),
(13, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 700000),
(13, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,6000000),
(13, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,750000),
(13, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,9000000),
(13, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,9000000),
(13, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,10500000),
(13, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,12000000),
(13, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,15000000),
(13, '3 tháng', 'Lớn', '50-100m²', 3, NULL,18000000),
(13, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL,21000000),
(13, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 12000000),
(13, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 15000000),
(13, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 18000000),
(13, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 18000000),
(13, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 21000000),
(13, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 24000000),
(13, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 30000000),
(13, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 36000000),
(13, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 42000000),
(13, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 24000000),
(13, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 30000000),
(13, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 36000000),
(13, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 36000000),
(13, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 42000000),
(13, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 48000000),
(13, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 60000000),
(13, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 72000000),
(13, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 84000000),

--- 🌿 Dịch vụ 14: Gói dịch vụ chăm sóc toàn diện theo yêu cầu khách hàng
(14, 'Lẻ', 'Nhỏ', 'Dưới 50m²', NULL, 1, 250000),
(14, 'Lẻ', 'Nhỏ', '50-100m²', NULL, 1, 300000),
(14, 'Lẻ', 'Nhỏ', 'Trên 100m²', NULL, 1, 350000),
(14, 'Lẻ', 'Trung bình', 'Dưới 50m²', NULL, 1, 350000),
(14, 'Lẻ', 'Trung bình', '50-100m²', NULL, 1, 400000),
(14, 'Lẻ', 'Trung bình', 'Trên 100m²', NULL, 1, 450000),
(14, 'Lẻ', 'Lớn', 'Dưới 50m²', NULL, 1, 550000),
(14, 'Lẻ', 'Lớn', '50-100m²', NULL, 1, 650000),
(14, 'Lẻ', 'Lớn', 'Trên 100m²', NULL, 1, 750000),
(14, '3 tháng', 'Nhỏ', 'Dưới 50m²', 3, NULL,7500000),
(14, '3 tháng', 'Nhỏ', '50-100m²', 3, NULL,9000000),
(14, '3 tháng', 'Nhỏ', 'Trên 100m²', 3, NULL,10500000),
(14, '3 tháng', 'Trung bình', 'Dưới 50m²', 3, NULL,10500000),
(14, '3 tháng', 'Trung bình', '50-100m²', 3, NULL,12000000),
(14, '3 tháng', 'Trung bình', 'Trên 100m²', 3, NULL,13500000),
(14, '3 tháng', 'Lớn', 'Dưới 50m²', 3, NULL,16500000),
(14, '3 tháng', 'Lớn', '50-100m²', 3, NULL,19500000),
(14, '3 tháng', 'Lớn', 'Trên 100m²', 3, NULL, 22500000),
(14, '6 tháng', 'Nhỏ', 'Dưới 50m²', 6, NULL, 15000000),
(14, '6 tháng', 'Nhỏ', '50-100m²', 6, NULL, 18000000),
(14, '6 tháng', 'Nhỏ', 'Trên 100m²', 6, NULL, 21000000),
(14, '6 tháng', 'Trung bình', 'Dưới 50m²', 6, NULL, 21000000),
(14, '6 tháng', 'Trung bình', '50-100m²', 6, NULL, 24000000),
(14, '6 tháng', 'Trung bình', 'Trên 100m²', 6, NULL, 27000000),
(14, '6 tháng', 'Lớn', 'Dưới 50m²', 6, NULL, 33000000),
(14, '6 tháng', 'Lớn', '50-100m²', 6, NULL, 39000000),
(14, '6 tháng', 'Lớn', 'Trên 100m²', 6, NULL, 45000000),
(14, '12 tháng', 'Nhỏ', 'Dưới 50m²', 12, NULL, 30000000),
(14, '12 tháng', 'Nhỏ', '50-100m²', 12, NULL, 36000000),
(14, '12 tháng', 'Nhỏ', 'Trên 100m²', 12, NULL, 42000000),
(14, '12 tháng', 'Trung bình', 'Dưới 50m²', 12, NULL, 42000000),
(14, '12 tháng', 'Trung bình', '50-100m²', 12, NULL, 48000000),
(14, '12 tháng', 'Trung bình', 'Trên 100m²', 12, NULL, 54000000),
(14, '12 tháng', 'Lớn', 'Dưới 50m²', 12, NULL, 66000000),
(14, '12 tháng', 'Lớn', '50-100m²', 12, NULL, 78000000),
(14, '12 tháng', 'Lớn', 'Trên 100m²', 12, NULL, 90000000);

UPDATE ServicePrices
SET ServiceType = N'Lẻ'
WHERE ServiceType = 'Lẻ'
UPDATE ServicePrices
SET ServiceType = N'3 tháng'
WHERE ServiceType = '3 tháng'
UPDATE ServicePrices
SET ServiceType = N'6 tháng'
WHERE ServiceType = '6 tháng'
UPDATE ServicePrices
SET ServiceType = N'12 tháng'
WHERE ServiceType = '12 tháng'

UPDATE ServicePrices
SET TreeSize = N'Nhỏ'
WHERE TreeSize = 'Nhỏ'
UPDATE ServicePrices
SET TreeSize = N'Trung bình'
WHERE TreeSize = 'Trung bình'
UPDATE ServicePrices
SET TreeSize = N'Lớn'
WHERE TreeSize = 'Lớn'

UPDATE ServicePrices
SET OfficeSize = N'Dưới 50m²'
WHERE OfficeSize = 'Dưới 50m²'
UPDATE ServicePrices
SET OfficeSize = N'Trên 50m²'
WHERE OfficeSize = 'Trên 50m²'

