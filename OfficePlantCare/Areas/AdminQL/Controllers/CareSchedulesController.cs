using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using OfficePlantCare.Models;
using X.PagedList.Extensions;

namespace OfficePlantCare.Areas.AdminQL.Controllers
{
    public class CareSchedulesController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public CareSchedulesController(OfficePlantCareContext context)
        {
            _context = context;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Cấu hình giấy phép EPPlus
        }

        // GET: AdminQL/CareSchedules
        public async Task<IActionResult> Index(string name, string status, DateOnly? scheduledDate, int page = 1)
        {
            int limit = 5;
            TempData["CurrentPage"] = page;

            IQueryable<CareSchedule> query = _context.CareSchedules
                .Include(c => c.Staff)
                .Include(c => c.Contract)
                .Include(c => c.Order)
                .OrderBy(c => c.Status);

            // Lọc theo trạng thái (status)
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status.Contains(status));
            }

            // Lọc theo ScheduledDate
            if (scheduledDate.HasValue)
            {
                query = query.Where(c => c.ScheduledDate == scheduledDate.Value);
            }

            // Lọc theo tên (keyword)
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Status.Contains(name));
            }

            var careSchedules = await query.ToListAsync();
            var pagedCareSchedules = careSchedules.ToPagedList(page, limit);

            // Lưu các giá trị lọc để hiển thị lại trên form và pagination
            ViewBag.keyword = name;
            ViewBag.status = status; // Ensure status is stored in ViewBag
            ViewBag.scheduledDate = scheduledDate?.ToString("dd/MM/yyyy");

            return View(pagedCareSchedules);
        }

        // GET: AdminQL/CareSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careSchedule = await _context.CareSchedules
                .Include(c => c.Contract)
                .Include(c => c.Order)
                .Include(c => c.Staff)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (careSchedule == null)
            {
                return NotFound();
            }

            return PartialView("_Details", careSchedule);
        }

        // GET: AdminQL/CareSchedules/Create
        public IActionResult Create()
        {
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractCode");
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
            ViewData["StaffId"] = new SelectList(_context.Staffs.Where(s => s.Position == "Nhân viên chăm sóc cây"), "StaffId", "StaffName");
            return View();
        }

        // POST: AdminQL/CareSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScheduleId,ContractId,OrderId,StaffId,ScheduledDate,ScheduledTime,ActualDate,Duration,Status")] CareSchedule careSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(careSchedule);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm lịch chăm sóc thành công!";
                int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
                return RedirectToAction(nameof(Index), new { page = currentPage });
            }

            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractCode", careSchedule.ContractId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", careSchedule.OrderId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffName", careSchedule.StaffId);
            return View(careSchedule);
        }

        // GET: AdminQL/CareSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careSchedule = await _context.CareSchedules
            .Include(cs => cs.Contract)
            .Include(cs => cs.Order).ThenInclude(o => o.Customer)
            .FirstOrDefaultAsync(cs => cs.ScheduleId == id);
            if (careSchedule == null)
            {
                return NotFound();
            }
            ViewBag.ContractId = careSchedule.ContractId; // ID thực
            ViewBag.ContractCode = careSchedule.Contract?.ContractCode; // để hiển thị

            ViewBag.OrderId = careSchedule.OrderId;
            ViewBag.OrderCustomer = careSchedule.Order?.Customer?.CustomerName;

            //ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractCode", careSchedule.ContractId);
            //ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", careSchedule.OrderId);
            ViewData["StaffId"] = new SelectList(_context.Staffs.Where(s => s.Position == "Nhân viên chăm sóc cây"), "StaffId", "StaffName");

            //ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffName");
            return PartialView("_Edit", careSchedule);
        }

    // POST: AdminQL/CareSchedules/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,ContractId,OrderId,StaffId,ScheduledDate,ScheduledTime,ActualDate,Duration,Status")] CareSchedule careSchedule)
        {
            if (id != careSchedule.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCareSchedule = await _context.CareSchedules
                       .FirstOrDefaultAsync(cs => cs.ScheduleId == id);

                    if (existingCareSchedule == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật giá trị mới từ form vào existingCareSchedule
                    existingCareSchedule.ContractId = careSchedule.ContractId;
                    existingCareSchedule.OrderId = careSchedule.OrderId;
                    existingCareSchedule.StaffId = careSchedule.StaffId;
                    existingCareSchedule.ScheduledDate = careSchedule.ScheduledDate;
                    existingCareSchedule.ScheduledTime = careSchedule.ScheduledTime;
                    existingCareSchedule.ActualDate = careSchedule.ActualDate;
                    existingCareSchedule.Duration = careSchedule.Duration;
                    existingCareSchedule.Status = careSchedule.Status;

                    _context.Update(existingCareSchedule);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật lịch chăm sóc thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CareScheduleExists(careSchedule.ScheduleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
                return RedirectToAction(nameof(Index), new { page = currentPage });
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractCode", careSchedule.ContractId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", careSchedule.OrderId);
            ViewData["StaffId"] = new SelectList(_context.Staffs.Where(s => s.Position == "Nhân viên chăm sóc cây"), "StaffId", "StaffName", careSchedule.StaffId);

            return View(careSchedule);
        }

        // GET: AdminQL/CareSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careSchedule = await _context.CareSchedules
                .Include(c => c.Contract)
                .Include(c => c.Order)
                .Include(c => c.Staff)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (careSchedule == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", careSchedule);
        }

        // POST: AdminQL/CareSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var careSchedule = await _context.CareSchedules.FindAsync(id);
            if (careSchedule != null)
            {
                _context.CareSchedules.Remove(careSchedule);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa lịch chăm sóc thành công!";
            int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
            return RedirectToAction(nameof(Index), new { page = currentPage });
        }

        // Trạng thái đơn hàng
        public async Task<bool> UpdateCareScheduleStatusForOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return false;
            }

            bool isOrderProcessed = order.Status == "Đã hoàn thành";
            bool isOrderConfirmed = order.Status == "Đã xác nhận";
            bool updated = false;

            // Lấy tất cả CareSchedules liên quan đến Order này
            var careSchedules = await _context.CareSchedules
                .Include(cs => cs.Contract)
                .Where(cs => cs.OrderId == orderId)
                .ToListAsync();

            // Cập nhật trạng thái của OrderDetails
            if (isOrderConfirmed)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    if (orderDetail.Status == "Đang chờ xác nhận")
                    {
                        orderDetail.Status = "Đã xác nhận";
                        _context.Update(orderDetail);
                        updated = true;
                    }
                }
            }
            // Cập nhật nhân viên trong CareSchedules nếu nhân viên trong đơn hàng thay đổi
            foreach (var careSchedule in careSchedules)
            {
                if (careSchedule.StaffId != order.StaffId)
                {
                    careSchedule.StaffId = order.StaffId; // Cập nhật StaffId
                    _context.Update(careSchedule); // Cập nhật CareSchedule
                    updated = true;
                }
            }
            // Cập nhật trạng thái của CareSchedules và xử lý số lượng vật tư
            foreach (var careSchedule in careSchedules)
            {
                // Ưu tiên 1: Nếu Order đã hoàn thành, CareSchedule chuyển từ "Đang thực hiện" sang "Hoàn thành"
                if (isOrderProcessed && careSchedule.Status == "Đang thực hiện")
                {
                    careSchedule.Status = "Hoàn thành";
                    _context.Update(careSchedule);
                    updated = true;
                }
                // Ưu tiên 2: Nếu Order đã xác nhận, CareSchedule chuyển sang "Đang thực hiện"
                else if (isOrderConfirmed && careSchedule.Status != "Đang thực hiện")
                {
                    careSchedule.Status = "Đang thực hiện";
                    _context.Update(careSchedule);
                    updated = true;
                }
            }

            if (updated)
            {
                await _context.SaveChangesAsync();
            }

            return updated;
        }

        // Trạng thái hợp đồng
        public async Task<bool> UpdateCareScheduleStatusForContract(int contractId)
        {
            var contract = await _context.Contracts
                .Include(o => o.ContractDetails)
                .FirstOrDefaultAsync(o => o.ContractId == contractId);

            if (contract == null)
            {
                return false;
            }

            bool isContractProcessed = contract.Status == "Đang hiệu lực";
            bool updated = false;

            // Lấy tất cả CareSchedules liên quan đến Contract này
            var careSchedules = await _context.CareSchedules
                .Include(cs => cs.Contract)
                .Where(cs => cs.ContractId == contractId)
                .ToListAsync();

            // Cập nhật trạng thái của ContractDetails
            if (isContractProcessed)
            {
                foreach (var contractDetail in contract.ContractDetails)
                {
                    if (contractDetail.Status == "Đang chờ xác nhận")
                    {
                        contractDetail.Status = "Đã xác nhận";
                        _context.Update(contractDetail);
                        updated = true;
                    }
                }
            }         
            if (updated)
            {
                await _context.SaveChangesAsync();
            }

            return updated;
        }

        private bool CareScheduleExists(int id)
        {
            return _context.CareSchedules.Any(e => e.ScheduleId == id);
        }
        // Action để xuất danh sách lịch chăm sóc ra file Excel
        public async Task<IActionResult> ExportToExcel(string name, string status, DateOnly? scheduledDate)
        {
            // Lấy dữ liệu lịch chăm sóc với các điều kiện lọc giống như trong action Index
            IQueryable<CareSchedule> query = _context.CareSchedules
                .Include(c => c.Staff)
                .Include(c => c.Contract)
                .Include(c => c.Order)
                .OrderBy(c => c.Status);

            // Lọc theo trạng thái (status)
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status.Contains(status));
            }

            // Lọc theo ScheduledDate
            if (scheduledDate.HasValue)
            {
                query = query.Where(c => c.ScheduledDate == scheduledDate.Value);
            }

            // Lọc theo tên (keyword)
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Status.Contains(name));
            }

            var careSchedules = await query.ToListAsync();

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("LichChamSoc");

                // Định dạng tiêu đề
                worksheet.Cells[1, 1].Value = "DANH SÁCH LỊCH CHĂM SÓC";
                worksheet.Cells[1, 1, 1, 8].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Size = 16;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Định dạng tiêu đề cột
                worksheet.Cells[3, 1].Value = "Mã hợp đồng";
                worksheet.Cells[3, 2].Value = "Mã đơn hàng";
                worksheet.Cells[3, 3].Value = "Mã yêu cầu";
                worksheet.Cells[3, 4].Value = "Ngày thực hiện";
                worksheet.Cells[3, 5].Value = "Giờ thực hiện";
                worksheet.Cells[3, 6].Value = "Nhân viên";
                worksheet.Cells[3, 7].Value = "Trạng thái";
                worksheet.Cells[3, 8].Value = "Thời lượng (giờ)";

                // Định dạng tiêu đề cột
                worksheet.Cells[3, 1, 3, 8].Style.Font.Bold = true;
                worksheet.Cells[3, 1, 3, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[3, 1, 3, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                worksheet.Cells[3, 1, 3, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Ghi dữ liệu vào file Excel
                int row = 4;
                foreach (var item in careSchedules)
                {
                    worksheet.Cells[row, 1].Value = item.Contract?.ContractCode ?? "N/A";
                    worksheet.Cells[row, 2].Value = item.Order?.OrderId.ToString() ?? "N/A";
                    worksheet.Cells[row, 3].Value = item.ScheduledDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4].Value = item.ScheduledTime.ToString("HH:mm");
                    worksheet.Cells[row, 5].Value = item.Staff?.StaffName ?? "Chưa phân công";
                    worksheet.Cells[row, 6].Value = item.Status ?? "Không xác định";
                    worksheet.Cells[row, 7].Value = item.Duration;

                    row++;
                }

                // Tự động điều chỉnh độ rộng cột
                worksheet.Cells[3, 1, row - 1, 8].AutoFitColumns();

                // Định dạng viền cho bảng
                worksheet.Cells[3, 1, row - 1, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, 1, row - 1, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, 1, row - 1, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, 1, row - 1, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                // Chuyển file Excel thành mảng byte
                var stream = new MemoryStream(package.GetAsByteArray());

                // Trả file về client
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachLichChamSoc.xlsx");
            }
        }
    }
}
