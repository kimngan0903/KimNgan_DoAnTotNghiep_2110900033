CREATE DATABASE	OfficePlantCare
GO

USE OfficePlantCare
GO

--- BẢNG KHÁCH HÀNG
CREATE TABLE Customers (
	CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    Phone NVARCHAR(15) UNIQUE NOT NULL,
    Address NVARCHAR(500) NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
	Status NVARCHAR(50) DEFAULT N'Hoạt động' CHECK (Status IN (N'Hoạt động', N'Không hoạt động', N'Bị khóa'))
);

--- BẢNG QUẢN TRỊ VIÊN
CREATE TABLE Admins (
    AdminId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME,
    Avatar NVARCHAR(MAX),
	Status NVARCHAR(50) DEFAULT N'Hoạt động' CHECK (Status IN (N'Hoạt động', N'Không hoạt động'))
);

--- BẢNG NHÂN VIÊN
CREATE TABLE Staffs(
    StaffId INT PRIMARY KEY IDENTITY(1,1),
    StaffName NVARCHAR(255) NOT NULL,
    Position NVARCHAR(100),
    Phone NVARCHAR(20) UNIQUE NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
	Avatar NVARCHAR(255),
    BirthDate DATE,
    HireDate DATETIME DEFAULT GETDATE(),
    Salary DECIMAL(18,2),
	Status NVARCHAR(50) DEFAULT N'Đang làm' CHECK (Status IN (N'Đang làm', N'Nghỉ việc', N'Bị sa thải'))
);

--- BẢNG DỊCH VỤ
CREATE TABLE Services(
    ServiceId INT PRIMARY KEY IDENTITY(1,1),
    ServiceName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) CHECK (Price >= 0),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME,
	Status NVARCHAR(50) DEFAULT N'Đang hoạt động' CHECK (Status IN (N'Đang hoạt động', N'Tạm dừng'))
);

--- BẢNG DANH MỤC
CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Image NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
	Status NVARCHAR(50) DEFAULT N'Đang hoạt động' CHECK (Status IN (N'Đang hoạt động', N'Tạm dừng'))
);

--- BẢNG SẢN PHẨM
CREATE TABLE Products(
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    CategoryId INT FOREIGN KEY REFERENCES Categories(CategoryId),
    ProductName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) CHECK (Price >= 0),
    MaintenanceGuide NVARCHAR(MAX),
    StockQuantity INT,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME,
	Status NVARCHAR(50) DEFAULT N'Còn hàng' CHECK (Status IN (N'Còn hàng', N'Hết hàng'))
);

--- BẢNG HỢP ĐỒNG
CREATE TABLE Contracts (
    ContractId INT IDENTITY(1,1) PRIMARY KEY,
    ContractCode AS ('HD' + RIGHT('0000' + CAST(ContractId AS NVARCHAR), 4)) PERSISTED,  -- Tự động tạo mã hợp đồng dạng CT0001, CT0002,...
    CustomerId INT NOT NULL FOREIGN KEY REFERENCES Customers(CustomerId),
    ContractDuration INT NOT NULL CHECK (ContractDuration IN (1, 3, 6, 12, 24)), -- Chỉ cho phép các giá trị hợp lệ
    StartDate DATE NOT NULL CHECK (StartDate >= GETDATE()),  -- Không cho phép ngày bắt đầu nhỏ hơn hôm nay
    EndDate AS DATEADD(MONTH, ContractDuration, StartDate) PERSISTED, -- Tự động tính ngày kết thúc
    ServiceFee DECIMAL(18,2) NOT NULL CHECK (ServiceFee >= 0), -- Phí dịch vụ mỗi tháng
    TotalAmount AS (ServiceFee * ContractDuration) PERSISTED, -- Tự động tính tổng tiền hợp đồng
    Status NVARCHAR(50) DEFAULT N'Đang hiệu lực' CHECK (Status IN (N'Đang hiệu lực', N'Hết hạn', N'Hủy')),
    CreatedAt DATETIME DEFAULT GETDATE(),  -- Ghi nhận ngày tạo hợp đồng
    UpdatedAt DATETIME DEFAULT GETDATE() -- Tự động cập nhật khi có thay đổi
);

--- BẢNG LIÊN HỆ
CREATE TABLE Contacts (
    ContactId INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255),
    Phone NVARCHAR(20),
    Address NVARCHAR(255),
    Description NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
	Status NVARCHAR(50) DEFAULT N'Chưa xử lý' CHECK (Status IN (N'Chưa xử lý', N'Đang xử lý', N'Hoàn thành'))
);

--- BẢNG LỊCH CHĂM SÓC
CREATE TABLE CareSchedules (
    ScheduleId INT IDENTITY(1,1) PRIMARY KEY,
    ContractId INT NOT NULL FOREIGN KEY REFERENCES Contracts(ContractId),
	StaffId INT FOREIGN KEY REFERENCES Staffs(StaffId),
    ScheduledDate DATETIME DEFAULT GETDATE(),  -- Ngày dự kiến chăm sóc
	ActualDate DATETIME NULL, -- Ngày thực tế thực hiện
    Duration INT NOT NULL CHECK (Duration > 0),  -- Thời gian chăm sóc (phút)
    Status NVARCHAR(50) DEFAULT N'Chưa thực hiện' CHECK (Status IN (N'Chưa thực hiện', N'Đang thực hiện', N'Hoàn thành'))
);

--- BẢNG PHƯƠNG THỨC THANH TOÁN
CREATE TABLE PaymentMethods (
    PaymentMethodId INT PRIMARY KEY IDENTITY(1,1),
    MethodName NVARCHAR(255) NOT NULL UNIQUE -- Ví dụ: 'Tiền mặt', 'Thẻ tín dụng', 'Mã QR', 'Chuyển khoản'
);

--- BẢNG ĐƠN HÀNG
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId),
    StaffId INT FOREIGN KEY REFERENCES Staffs(StaffId),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) CHECK (TotalAmount >= 0),
	Discount DECIMAL(18,2) DEFAULT 0 CHECK (Discount >= 0),
	PaymentMethodId INT NOT NULL FOREIGN KEY REFERENCES PaymentMethods(PaymentMethodId), -- Tham chiếu đến bảng PaymentMethods
    PaymentDate DATETIME DEFAULT GETDATE(), 
    PaymentStatus NVARCHAR(20) CHECK (PaymentStatus IN ('Chưa thanh toán', 'Đã thanh toán', 'Hoàn tiền')) DEFAULT 'Chưa thanh toán',
    Status NVARCHAR(50) DEFAULT N'Chờ xử lý' CHECK (Status IN (N'Chờ xử lý', N'Đang thực hiện', N'Đã hoàn thành', N'Đã hủy'))
);

--- BẢNG CHI TIẾT ĐƠN HÀNG
CREATE TABLE OrderDetails (
    OrderDetailId INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT FOREIGN KEY REFERENCES Orders(OrderId),
    ServiceId INT FOREIGN KEY REFERENCES Services(ServiceId),
    ProductId INT FOREIGN KEY REFERENCES Products(ProductId),
	Address NVARCHAR(255),
    Quantity INT NOT NULL CHECK (Quantity > 0),
    Price DECIMAL(18,2) NOT NULL,
    TotalPrice AS (Quantity * Price) PERSISTED,
    Status NVARCHAR(50) DEFAULT N'Đang chờ xác nhận' CHECK (Status IN (N'Đang chờ xác nhận', N'Đã xác nhận'))
);

--- BẢNG ĐÁNH GIÁ
CREATE TABLE Feedbacks (
    FeedbackId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId),
    ProductId INT FOREIGN KEY REFERENCES Products(ProductId),
    ServiceId INT FOREIGN KEY REFERENCES Services(ServiceId),
    FeedbackDate DATETIME DEFAULT GETDATE(),
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Content NVARCHAR(MAX),
    Status NVARCHAR(50) DEFAULT N'Chờ duyệt' CHECK (Status IN (N'Chờ duyệt', N'Đã duyệt'))
);

--- BẢNG BANNER
CREATE TABLE Banners (
    BannerId INT PRIMARY KEY IDENTITY(1,1),
    Image NVARCHAR(MAX),
    Title NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME,
	Status NVARCHAR(50) DEFAULT N'Hiện' CHECK (Status IN (N'Hiện', N'Ẩn', N'Tạm dừng'))
);

--- BẢNG TIN TỨC
CREATE TABLE News (
    NewsId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Image NVARCHAR(255),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME,
	Status NVARCHAR(50) DEFAULT N'Hiển thị' CHECK (Status IN (N'Hiển thị', N'Ẩn'))
);

--- BẢNG ĐỐI TÁC
CREATE TABLE Partners (
    PartnerId INT PRIMARY KEY IDENTITY(1,1),
    PartnerName NVARCHAR(255) NOT NULL,
    Logo NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE(),
    Description NVARCHAR(MAX),
	Status NVARCHAR(50) DEFAULT N'Đang hoạt động' CHECK (Status IN (N'Đang hoạt động', N'Không hoạt động'))
);

-- BẢNG XỬ LÝ YÊU CẦU PHÁT SINH
CREATE TABLE ServiceRequests (
    RequestID INT PRIMARY KEY IDENTITY(1,1),
    ScheduleID INT FOREIGN KEY REFERENCES CareSchedules(ScheduleId),
    ServiceID INT FOREIGN KEY REFERENCES Services(ServiceID),
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId), -- Ai gửi yêu cầu?
	ProcessedBy INT FOREIGN KEY REFERENCES Admins(AdminId),
    Quantity INT NOT NULL CHECK (Quantity > 0),
    Price DECIMAL(18,2) NOT NULL CHECK (Price >= 0),
    TotalAmount AS (Quantity * Price) PERSISTED, -- Tính toán tự động
    Notes NVARCHAR(MAX) NULL,
    RequestDate DATETIME DEFAULT GETDATE(), -- Ngày tạo yêu cầu
    UpdatedAt DATETIME DEFAULT GETDATE(), -- Lần cập nhật cuối
    Status NVARCHAR(50) DEFAULT N'Chờ xử lý' CHECK (Status IN (N'Chờ xử lý', N'Đã xử lý', N'Từ chối'))
);

-- BẢNG KHO VẬT TƯ (GỘP NGUYÊN LIỆU & CÔNG CỤ)
CREATE TABLE InventoryItems (
    ItemId INT PRIMARY KEY IDENTITY(1,1),
    ItemName NVARCHAR(255) NOT NULL,  -- Tên vật tư (Nguyên liệu hoặc Công cụ)
    Description NVARCHAR(MAX),        -- Mô tả chi tiết
    Unit NVARCHAR(50),                -- Đơn vị tính (kg, lít, cái, bộ, ...)
    Price DECIMAL(18, 2) CHECK (Price >= 0), -- Giá mỗi đơn vị (áp dụng cho nguyên liệu)
    StockQuantity INT NOT NULL CHECK (StockQuantity >= 0), -- Số lượng tồn kho
    ItemType NVARCHAR(50) NOT NULL CHECK (ItemType IN (N'Nguyên liệu', N'Công cụ')), -- Phân loại
    CategoryId INT FOREIGN KEY REFERENCES Categories(CategoryId), -- Danh mục vật tư
    Status NVARCHAR(50) DEFAULT N'Có sẵn' CHECK (Status IN (N'Có sẵn', N'Hết hàng', N'Đang chờ bổ sung', N'Bị hư hại')),
    CreatedDate DATETIME DEFAULT GETDATE(),  
    UpdatedDate DATETIME
);

-- BẢNG LIÊN KẾT GIỮA VẬT TƯ & DỊCH VỤ
CREATE TABLE ServiceInventory (
    ServiceInventoryId INT PRIMARY KEY IDENTITY(1,1),
    ServiceId INT NOT NULL FOREIGN KEY REFERENCES Services(ServiceId),
    ItemId INT NOT NULL FOREIGN KEY REFERENCES InventoryItems(ItemId),
    QuantityRequired DECIMAL(18, 2) NOT NULL CHECK (QuantityRequired > 0), -- Số lượng cần cho dịch vụ
    Notes NVARCHAR(MAX) -- Ghi chú
);

-- BẢNG LIÊN KẾT GIỮA VẬT TƯ & LỊCH CHĂM SÓC
CREATE TABLE CareScheduleInventory (
    CareScheduleInventoryId INT PRIMARY KEY IDENTITY (1,1),
    CareScheduleId INT NOT NULL FOREIGN KEY REFERENCES CareSchedules(ScheduleId),
    ItemId INT NOT NULL FOREIGN KEY REFERENCES InventoryItems(ItemId),
    QuantityUsed INT NOT NULL CHECK (QuantityUsed > 0), -- Số lượng vật tư đã sử dụng
    Notes NVARCHAR(MAX) -- Ghi chú
);


CREATE TRIGGER trg_CareSchedules_ActualDate_Check
ON CareSchedules
AFTER INSERT, UPDATE
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM inserted 
        WHERE ActualDate IS NOT NULL AND ActualDate < ScheduledDate
    )
    BEGIN
        RAISERROR ('ActualDate không thể nhỏ hơn ScheduledDate!', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;


ALTER TABLE ServiceInventory ADD CONSTRAINT FK_ServiceInventory_Service 
FOREIGN KEY (ServiceId) REFERENCES Services(ServiceId) ON DELETE CASCADE;
