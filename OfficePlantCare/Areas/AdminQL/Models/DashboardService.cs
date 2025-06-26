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

                // Truy vấn các số liệu thống kê
                string query = @"
                    SELECT 
                        (SELECT COUNT(*) FROM Customers) AS KhachHang,
                        (SELECT COUNT(*) FROM Staffs) AS NhanVien,
                        (SELECT COUNT(*) FROM Services) AS DichVu,
                        (SELECT COUNT(*) FROM Contracts) AS HopDong,
                        (SELECT COUNT(*) FROM Orders) AS DonHang,
                        (SELECT COUNT(*) FROM Feedbacks) AS DanhGia,
                        (SELECT COUNT(*) FROM News) AS TinTuc,
                        (SELECT COUNT(*) FROM CareSchedules) AS LichChamSoc
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
                        data.TinTuc = reader.GetInt32(6);
                        data.LichChamSoc = reader.GetInt32(7);
                    }
                }

                // Truy vấn lịch chăm sóc của ngày hiện tại với thông tin nhân viên
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                string careScheduleQuery = @"
                    SELECT 
                        cs.ScheduleId,
                        cs.ContractId,
                        cs.OrderId,
                        cs.StaffId,
                        cs.ScheduledDate,
                        cs.ScheduledTime,
                        cs.ActualDate,
                        cs.Duration,
                        cs.Status,
                        s.StaffName
                    FROM CareSchedules cs
                    LEFT JOIN Staffs s ON cs.StaffId = s.StaffId
                    WHERE cs.ScheduledDate = @Today
                ";

                using (SqlCommand cmd = new SqlCommand(careScheduleQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Today", today.ToString("yyyy-MM-dd"));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var careSchedule = new CareSchedule
                            {
                                ScheduleId = reader.GetInt32(0),
                                ContractId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                                OrderId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                                StaffId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                                ScheduledDate = DateOnly.FromDateTime(reader.GetDateTime(4)),
                                ScheduledTime = TimeOnly.FromTimeSpan(reader.GetTimeSpan(5)),
                                ActualDate = reader.IsDBNull(6) ? null : DateOnly.FromDateTime(reader.GetDateTime(6)),
                                Duration = reader.GetDecimal(7),
                                Status = reader.IsDBNull(8) ? null : reader.GetString(8),
                                // Gán thông tin Staff
                                Staff = reader.IsDBNull(9) ? null : new Staff
                                {
                                    StaffId = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                    StaffName = reader.GetString(9)
                                }
                            };
                            data.CareSchedulesToday.Add(careSchedule);
                        }
                    }
                }
            }

            return data;
        }
    }
}