namespace OfficePlantCare.Areas.AdminQL.Models
{
    public class Dashboard
    {
        public int KhachHang { get; set; }
        public int NhanVien { get; set; }
        public int DichVu { get; set; }
        public int YeuCauPhatSinh { get; set; }

        public int HopDong { get; set; }
        public int DonHang { get; set; }
        public int DanhGia { get; set; }
        public int DoiTac { get; set; }

        // Thêm các chỉ số thống kê mới
        public decimal DoanhThu { get; set; }
        public decimal ChiPhi { get; set; }
        public decimal LoiNhuan { get; set; }
    }

}