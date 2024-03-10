

Create table LoaiSP
(
	MaLoai Integer NOT NULL,
	TenLoai Nvarchar2(50) NULL,
Primary Key (MaLoai)
);  
Create table QuyenHan
(
	MaQH Integer NOT NULL,
	TenQH Nvarchar2(50) NULL,
Primary Key (MaQH)
); 


Create table LoaiHD
(
	MaLoaiHD Integer NOT NULL,
	TenLoaiHD Nvarchar2(50) NULL,
Primary Key (MaLoaiHD)
); 

Create table HinhThucTT
(
	MaHTTT Integer  NOT NULL,
	TenHTTT Nvarchar2(50) NULL,
Primary Key (MaHTTT)
); 

Create table SanPham
(
	MaSP Integer generated always as identity NOT NULL,
	TenSP Nvarchar2(200) NULL,
	Gia Decimal NULL,
	SLT Integer NULL,
	Mota Nvarchar2(200) NULL,
	Hinh VarChar2(100) NULL,
	MaLoai Integer NOT NULL,
Primary Key (MaSP),
CONSTRAINT SanPham_FK foreign key(MaLoai) references LoaiSP (MaLoai)
); 


Create table NguoiDung
(
	TenDangNhap Varchar2(200) NOT NULL,
	MatKhau Varchar2(30) NULL,
	TenND Nvarchar2(200) NULL,
	NgaySinh Date NULL,
	DiaChi Nvarchar2(300) NULL,
	SDT VarChar2(11) NULL,
	Mail Varchar2(100) NULL,
Primary Key (TenDangNhap)
); 
Create table HoaDon
(
	MaHD Integer generated always as identity NOT NULL,
	ThoiGianDat Date NULL,
	ThoiGianGiaoDuKien Date NULL,
    ThanhTien Float NULL,
	DiaChi Nvarchar2(250) NULL,
	SDTNhan Varchar2(10) NULL,
	TenDangNhap Varchar2(200) NOT NULL,
    MaHTTT Integer NOT NULL,
Primary Key (MaHD),
CONSTRAINT DonHang_FK foreign key(TenDangNhap) references NguoiDung (TenDangNhap),
CONSTRAINT DonHang_FK1 foreign key(MaHTTT) references HinhThucTT (MaHTTT)
); 

--Create table HoaDonTT
--(
--	MaHD Integer generated always as identity NOT NULL,
--	NgayLapHD Date NULL,
--	ThanhTien Float NULL,
--	DiaChi Nvarchar2(250) NULL,
--	SDTNhan Varchar2(10) NULL,
--	MaDH Integer NOT NULL,
--	
--Primary Key (MaHD),
--CONSTRAINT HoaDonTT foreign key(MaDH) references DonHang (MaDH),
--
--);



Create table ChiTietHoaDon
(
	MaSP Integer NOT NULL,
	MaHD Integer NOT NULL,
	SoLuong Integer NULL,
	Gia Decimal NULL,
Primary Key (MaSP,MaHD),
CONSTRAINT ChiTietDonHang_FK foreign key(MaSP) references SanPham (MaSP),
    CONSTRAINT ChiTietDonHang1_FK foreign key(MaHD) references HoaDon (MaHD)
) ;


Create table NhanVien
(
	TenTaiKhoan Varchar2(200) NOT NULL,
	Pass Varchar2(30) NULL,
	TenNhanVien Nvarchar2(50) NULL,
	NS Date NULL,
	SoDT Varchar2(10) NULL,
	DiaChi Nvarchar2(30) NULL,
	Mail Varchar2(30) NULL,
	MaQH Integer NOT NULL,
Primary Key (TenTaiKhoan),
CONSTRAINT NhanVien_FK foreign key(MaQH) references QuyenHan (MaQH) 
); 

Create table HopDong
(
	MaHopDong VARCHAR2(200) NOT NULL,
    ThoiGianLap Date NULL,
    ND NVARCHAR2(200) NULL,
    ChiPhiDuPhong NVARCHAR2(200) NULL,
	TenDangNhap VARCHAR2(200) NOT NULL,
	TenTaiKhoan VARCHAR2(30) NOT NULL,			
	MaLoaiHD Integer NOT NULL,
Primary Key (MaHopDong),
CONSTRAINT Hopdong_FK foreign key(TenDangNhap) references NguoiDung (TenDangNhap),
    CONSTRAINT Hopdong1_FK foreign key(TenTaiKhoan) references NhanVien (TenTaiKhoan),
    CONSTRAINT Hopdong2_FK  foreign key(MaLoaiHD) references LoaiHD (MaLoaiHD)
); 


Create table HDTTHopDong
(
	MaHDTTHopDong Integer NOT NULL,
	MaHopDong Varchar2(200) NOT NULL,
	MaHTTT Integer NOT NULL,
	SoTienCoc Float NULL,
	SoTienConLai Float NULL,
Primary Key (MaHDTTHopDong),
CONSTRAINT HDTTHopdong_FK foreign key(MaHopDong) references HopDong (MaHopDong),
    CONSTRAINT HDTTHopdong1_FK foreign key(MaHTTT) references HinhThucTT (MaHTTT)
);

drop table "SYSTEM"."LOAISP" cascade constraints
drop table "SYSTEM"."LOAIHD" cascade constraints
drop table "SYSTEM"."QUYENHAN" cascade constraints 
drop table "SYSTEM"."HINHTHUCTT" cascade constraints 
drop table "SYSTEM"."SANPHAM" cascade constraints 
drop table "SYSTEM"."NGUOIDUNG" cascade constraints 
drop table "SYSTEM"."HOADON" cascade constraints 
drop table "SYSTEM"."CHITIETHOADON" cascade constraints 
drop table "SYSTEM"."NHANVIEN" cascade constraints 
drop table "SYSTEM"."HOPDONG" cascade constraints 
drop table "SYSTEM"."HDTTHOPDONG" cascade constraints 




