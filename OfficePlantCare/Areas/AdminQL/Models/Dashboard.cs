using OfficePlantCare.Models;

namespace OfficePlantCare.Areas.AdminQL.Models
{
    public class Dashboard
    {
        public int KhachHang { get; set; }
        public int NhanVien { get; set; }
        public int DichVu { get; set; }
        public int TinTuc { get; set; }
        public int HopDong { get; set; }
        public int DonHang { get; set; }
        public int DanhGia { get; set; }
        public int LichChamSoc { get; set; }

        // Thêm danh sách lịch chăm sóc của ngày hiện tại
        public List<CareSchedule> CareSchedulesToday { get; set; } = new List<CareSchedule>();
    }
}