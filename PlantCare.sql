CREATE DATABASE PlantCare
GO

USE DATABASE PlantCare
GO

--- BẢNG KHÁCH HÀNG
CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    CustomerName NVARCHAR(255) NOT NULL,
	Password NVARCHAR(255) NOT NULL,
    Address NVARCHAR(255),
    Phone NVARCHAR(20) UNIQUE NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    BirthDate DATE,
    CreatedDate DATETIME DEFAULT GETDATE(),
	Status NVARCHAR(50) DEFAULT N'Hoạt động' CHECK (Status IN (N'Hoạt động', N'Không hoạt động', N'Bị khóa'))
);

--- BẢNG QUẢN TRỊ VIÊN
CREATE TABLE Admins (
    AdminId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(255) NOT NULL,
    Password NVARCHAR(MAX) NOT NULL,
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
    Salary DECIMAL(18, 3),
	Status NVARCHAR(50) DEFAULT N'Đang làm' CHECK (Status IN (N'Đang làm', N'Nghỉ việc', N'Bị sa thải'))
);

--- BẢNG DỊCH VỤ
CREATE TABLE Services(
    ServiceId INT PRIMARY KEY IDENTITY(1,1),
    ServiceName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18, 3),
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
    Price DECIMAL(18, 3),
    MaintenanceGuide NVARCHAR(MAX),
    StockQuantity INT,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME,
	Status NVARCHAR(50) DEFAULT N'Còn hàng' CHECK (Status IN (N'Còn hàng', N'Hết hàng'))
);

--- BẢNG HỢP ĐỒNG
CREATE TABLE Contracts (
    ContractId INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    StartDate DATE NOT NULL, 
    EndDate DATE NOT NULL,
    MonthlyFee DECIMAL(18, 3),
    Status NVARCHAR(50) DEFAULT N'Đang hiệu lực' CHECK (Status IN (N'Đang hiệu lực', N'Hết hạn', N'Hủy')),
    CreatedDate DATETIME DEFAULT GETDATE()
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
    CareScheduleId INT PRIMARY KEY IDENTITY(1,1),
    ContractID INT FOREIGN KEY (ContractID) REFERENCES Contracts(ContractID),
    StaffId INT FOREIGN KEY REFERENCES Staffs(StaffId),
    ScheduledDate DATETIME NOT NULL,
    Notes NVARCHAR(MAX),
    Status NVARCHAR(50) DEFAULT N'Chờ xác nhận' CHECK (Status IN (N'Chờ xác nhận', N'Đang thực hiện', N'Hoàn thành')),
    DailyCareFee DECIMAL(18,3) DEFAULT 0 -- Chi phí hàng ngày ngoài hợp đồng
);

--- BẢNG PHƯƠNG THỨC THANH TOÁN
CREATE TABLE PaymentMethods(
    PaymentId INT PRIMARY KEY IDENTITY(1,1),
    PaymentMethod NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
	Status NVARCHAR(50) DEFAULT N'Đang hoạt động' CHECK (Status IN (N'Đang hoạt động', N'Tạm dừng'))
);

--- BẢNG ĐƠN HÀNG
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId),
    StaffId INT FOREIGN KEY REFERENCES Staffs(StaffId),
    PaymentId INT FOREIGN KEY REFERENCES PaymentMethods(PaymentId),
    Address NVARCHAR(255),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 3) DEFAULT 0,
    Status NVARCHAR(50) DEFAULT N'Chờ xử lý' CHECK (Status IN (N'Chờ xử lý', N'Đang thực hiện', N'Đã hoàn thành', N'Đã hủy')),
    AdditionalServices NVARCHAR(MAX), -- Danh sách các dịch vụ phát sinh ngoài hợp đồng
    TotalAdditionalCosts DECIMAL(18,3) DEFAULT 0 -- Tổng chi phí phát sinh ngoài hợp đồng
);

--- BẢNG CHI TIẾT ĐƠN HÀNG
CREATE TABLE OrderDetails (
    OrderDetailId INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT FOREIGN KEY REFERENCES Orders(OrderId),
    ServiceId INT FOREIGN KEY REFERENCES Services(ServiceId),
    ProductId INT FOREIGN KEY REFERENCES Products(ProductId),
    Quantity INT NOT NULL,
    Price DECIMAL(18, 3) NOT NULL,
    TotalPrice AS (Quantity * Price),
    Status NVARCHAR(50) DEFAULT N'Đang chờ xác nhận' CHECK (Status IN (N'Đang chờ xác nhận', N'Đã xác nhận')),
    AdditionalServiceCost DECIMAL(18,3) DEFAULT 0 -- Chi phí phát sinh cho dịch vụ ngoài hợp đồng
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
    RequestID INT PRIMARY KEY IDENTITY,
    ScheduleID INT FOREIGN KEY (ScheduleID) REFERENCES CareSchedules(CareScheduleId),
    ServiceID INT FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID),
    Quantity INT NOT NULL,
    Price DECIMAL(18,3) NOT NULL,
    TotalAmount AS (Quantity * Price),
    Notes NVARCHAR(MAX),
    Status NVARCHAR(50),
    RequestType NVARCHAR(100), -- Loại yêu cầu phát sinh (Thêm cành, Chăm sóc thêm, v.v.)
    AdditionalCosts DECIMAL(18,3) DEFAULT 0 -- Chi phí phát sinh ngoài hợp đồng
);


-- BẢNG NGUYÊN LIỆU
CREATE TABLE Materials (
    MaterialId INT PRIMARY KEY IDENTITY,
    MaterialName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Unit NVARCHAR(50), -- Đơn vị (ví dụ: kg, lít)
    Price DECIMAL(18, 2) NOT NULL, -- Giá mỗi đơn vị
    StockQuantity INT NOT NULL, -- Số lượng tồn kho
    CategoryId INT FOREIGN KEY REFERENCES Categories(CategoryId), 
    CreatedDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT N'Có sẵn' CHECK (Status IN (N'Có sẵn', N'Hết hàng', N'Đang chờ bổ sung', N'Bị hư hại'))
);


-- BẢNG DỊCH VỤ VẬT LIỆU
CREATE TABLE ServiceMaterials (
    ServiceMaterialId INT PRIMARY KEY IDENTITY,
    ServiceId INT NOT NULL FOREIGN KEY REFERENCES Services(ServiceId),
    MaterialId INT NOT NULL FOREIGN KEY REFERENCES Materials(MaterialId),
    QuantityRequired DECIMAL(18, 2) NOT NULL, -- Số lượng nguyên liệu cần cho dịch vụ
    Notes NVARCHAR(MAX)
);


