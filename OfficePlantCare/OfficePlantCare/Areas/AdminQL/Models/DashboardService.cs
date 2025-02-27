using Microsoft.Data.SqlClient;
using OfficePlantCare.Models;

namespace OfficePlantCare.Areas.AdminQL.Models
{
    public class DashboardService
    {
        private readonly string _connectionString;

        public DashboardService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OfficePlantCareConnection");
        }

        public Dashboard GetDashboardData()
        {
            var data = new Dashboard();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
    SELECT 
        (SELECT COUNT(*) FROM Customers) AS KhachHang,
        (SELECT COUNT(*) FROM Staffs) AS NhanVien,
        (SELECT COUNT(*) FROM Services) AS DichVu,
        (SELECT COUNT(*) FROM Contracts) AS HopDong,
        (SELECT COUNT(*) FROM Orders) AS DonHang,
        (SELECT COUNT(*) FROM Feedbacks) AS DanhGia,
        (SELECT COUNT(*) FROM ServiceRequests) AS YeuCauPhatSinh,
        (SELECT COUNT(*) FROM Partners) AS DoiTac

";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        data.KhachHang = reader.GetInt32(0);
                        data.NhanVien = reader.GetInt32(1);
                        data.DichVu = reader.GetInt32(2);
                        data.HopDong = reader.GetInt32(3);
                        data.DonHang = reader.GetInt32(4);
                        data.DanhGia = reader.GetInt32(5);
                        data.YeuCauPhatSinh = reader.GetInt32(6);
                        data.DoiTac = reader.GetInt32(7);
                        //data.DoanhThu = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8);
                        //data.ChiPhi = reader.IsDBNull(9) ? 0 : reader.GetDecimal(9);
                        //data.LoiNhuan = data.DoanhThu - data.ChiPhi;
                    }
                }
            }

            return data;
        }
    }
}

