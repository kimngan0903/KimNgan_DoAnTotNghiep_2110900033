using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        }
        // Phương thức kiểm tra vật tư có tái sử dụng được hay không
        private bool IsSupplyReusable(string supplyName)
        {
            var reusableSupplies = new List<string>
            {
                "Chậu nhựa",
                "Chậu sành/sứ",
                "Chậu composite",
                "Rọ nhựa trồng cây thủy sinh",
                "Dây buộc cây",
                "Kéo cắt tỉa",
                "Bình xịt tưới cây",
                "Bình tưới nước dung tích lớn",
                "Xẻng nhỏ làm vườn",
                "Găng tay bảo hộ",
                "Hệ thống tưới nhỏ giọt",
                "Đầu phun sương",
                "Ống dẫn nước tưới",
                "Đèn LED trang trí cây cảnh"
            };

            return reusableSupplies.Contains(supplyName);
        }
        // GET: AdminQL/CareSchedules
        // GET: AdminQL/CareSchedules
        public async Task<IActionResult> Index(string name, string status, DateOnly? scheduledDate, int page = 1)
        {
            int limit = 5;

            IQueryable<CareSchedule> query = _context.CareSchedules
                .Include(c => c.Staff)
                .Include(c => c.Contract)
                .Include(c => c.Order)
                .Include(c => c.Plant)
                .Include(c => c.Request)
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
            ViewBag.scheduledDate = scheduledDate?.ToString("yyyy-MM-dd");

            return View(pagedCareSchedules);
        }

        // GET: AdminQL/CareSchedules/Details/5
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
                .Include(c => c.Plant)
                .Include(c => c.Request)
                .Include(c => c.Staff)
                .Include(c => c.CareScheduleSupplies)
                    .ThenInclude(css => css.Supply)
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
            ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "PlantName");
            ViewData["RequestId"] = new SelectList(_context.ServiceRequests, "RequestId", "RequestId");
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffName");
            ViewData["Supplies"] = new MultiSelectList(_context.Supplies, "SupplyId", "SupplyName");
            return View();
        }

        // POST: AdminQL/CareSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScheduleId,ContractId,OrderId,RequestId,PlantId,StaffId,ScheduledDate,ScheduledTime,ActualDate,Duration,Status")] CareSchedule careSchedule, int[] selectedSupplies, decimal[] quantities)
        {
            if (ModelState.IsValid)
            {
                // Thêm CareSchedule vào cơ sở dữ liệu
                _context.Add(careSchedule);
                await _context.SaveChangesAsync();

                // Xử lý vật tư được chọn
                if (selectedSupplies != null && quantities != null && selectedSupplies.Length == quantities.Length)
                {
                    for (int i = 0; i < selectedSupplies.Length; i++)
                    {
                        var supplyId = selectedSupplies[i];
                        var quantityUsed = quantities[i];

                        // Tìm vật tư
                        var supply = await _context.Supplies.FindAsync(supplyId);
                        if (supply == null || supply.Quantity < quantityUsed)
                        {
                            ModelState.AddModelError("", $"Vật tư {supply?.SupplyName} không đủ số lượng để sử dụng.");
                            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractCode", careSchedule.ContractId);
                            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", careSchedule.OrderId);
                            ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "PlantName", careSchedule.PlantId);
                            ViewData["RequestId"] = new SelectList(_context.ServiceRequests, "RequestId", "RequestId", careSchedule.RequestId);
                            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffName", careSchedule.StaffId);
                            ViewData["Supplies"] = new MultiSelectList(_context.Supplies, "SupplyId", "SupplyName", selectedSupplies);
                            return View(careSchedule);
                        }

                        // Thêm vào CareScheduleSupply
                        var careScheduleSupply = new CareScheduleSupply
                        {
                            ScheduleId = careSchedule.ScheduleId,
                            SupplyId = supplyId,
                            QuantityUsed = quantityUsed,
                            Notes = "Sử dụng cho lịch chăm sóc"
                        };
                        _context.CareScheduleSupplies.Add(careScheduleSupply);

                        // Giảm số lượng vật tư nếu trạng thái là "Đang thực hiện"
                        if (careSchedule.Status == "Đang thực hiện")
                        {
                            supply.Quantity -= quantityUsed;
                            supply.UpdatedDate = DateTime.Now;
                            _context.Update(supply);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm lịch chăm sóc thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractCode", careSchedule.ContractId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", careSchedule.OrderId);
            ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "PlantName", careSchedule.PlantId);
            ViewData["RequestId"] = new SelectList(_context.ServiceRequests, "RequestId", "RequestId", careSchedule.RequestId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffName", careSchedule.StaffId);
            ViewData["Supplies"] = new MultiSelectList(_context.Supplies, "SupplyId", "SupplyName");
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
                .Include(cs => cs.CareScheduleSupplies)
                .FirstOrDefaultAsync(cs => cs.ScheduleId == id);
            if (careSchedule == null)
            {
                return NotFound();
            }
            ViewBag.ContractId = careSchedule.ContractId;
            ViewBag.OrderId = careSchedule.OrderId;
            ViewBag.RequestId = careSchedule.RequestId;
            //ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractCode", careSchedule.ContractId);
            //ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", careSchedule.OrderId);
            //ViewData["RequestId"] = new SelectList(_context.ServiceRequests, "RequestId", "RequestId", careSchedule.RequestId);
            ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "PlantName", careSchedule.PlantId);
            
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffName", careSchedule.StaffId);
            ViewData["Supplies"] = new MultiSelectList(_context.Supplies, "SupplyId", "SupplyName", careSchedule.CareScheduleSupplies.Select(css => css.SupplyId));
            return PartialView("_Edit", careSchedule);
        }

        // POST: AdminQL/CareSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,ContractId,OrderId,RequestId,PlantId,StaffId,ScheduledDate,ScheduledTime,ActualDate,Duration,Status")] CareSchedule careSchedule, int[] selectedSupplies, decimal[] quantities)
        {
            if (id != careSchedule.ScheduleId)
            {
                return NotFound();
            }

            // Chuẩn hóa giá trị null cho ContractId, OrderId, RequestId
            careSchedule.ContractId = careSchedule.ContractId == 0 ? (int?)null : careSchedule.ContractId;
            careSchedule.OrderId = careSchedule.OrderId == 0 ? (int?)null : careSchedule.OrderId;
            careSchedule.RequestId = careSchedule.RequestId == 0 ? (int?)null : careSchedule.RequestId;

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCareSchedule = await _context.CareSchedules
                        .Include(cs => cs.CareScheduleSupplies)
                        .FirstOrDefaultAsync(cs => cs.ScheduleId == id);

                    if (existingCareSchedule == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật giá trị mới từ form vào existingCareSchedule
                    existingCareSchedule.ContractId = careSchedule.ContractId;
                    existingCareSchedule.OrderId = careSchedule.OrderId;
                    existingCareSchedule.RequestId = careSchedule.RequestId;
                    existingCareSchedule.PlantId = careSchedule.PlantId;
                    existingCareSchedule.StaffId = careSchedule.StaffId;
                    existingCareSchedule.ScheduledDate = careSchedule.ScheduledDate;
                    existingCareSchedule.ScheduledTime = careSchedule.ScheduledTime;
                    existingCareSchedule.ActualDate = careSchedule.ActualDate;
                    existingCareSchedule.Duration = careSchedule.Duration;
                    existingCareSchedule.Status = careSchedule.Status;

                    // Cập nhật danh sách supplies
                    _context.CareScheduleSupplies.RemoveRange(existingCareSchedule.CareScheduleSupplies);
                    for (int i = 0; i < selectedSupplies.Length; i++)
                    {
                        existingCareSchedule.CareScheduleSupplies.Add(new CareScheduleSupply
                        {
                            ScheduleId = existingCareSchedule.ScheduleId,
                            SupplyId = selectedSupplies[i],
                            QuantityUsed = quantities[i]
                        });
                    }

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
                return RedirectToAction(nameof(Index));
            }

            // Populate ViewData để giữ lại giá trị trong form nếu có lỗi
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractCode", careSchedule.ContractId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", careSchedule.OrderId);
            ViewData["PlantId"] = new SelectList(_context.Plants, "PlantId", "PlantName", careSchedule.PlantId);
            ViewData["RequestId"] = new SelectList(_context.ServiceRequests, "RequestId", "RequestId", careSchedule.RequestId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffName", careSchedule.StaffId);
            ViewData["Supplies"] = new MultiSelectList(_context.Supplies, "SupplyId", "SupplyName", selectedSupplies);

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
                .Include(c => c.Plant)
                .Include(c => c.Request)
                .Include(c => c.Staff)
                .Include(c => c.CareScheduleSupplies)
                    .ThenInclude(css => css.Supply)
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
            var careSchedule = await _context.CareSchedules
                .Include(cs => cs.CareScheduleSupplies)
                .FirstOrDefaultAsync(cs => cs.ScheduleId == id);
            if (careSchedule != null)
            {
                // Khôi phục số lượng vật tư nếu trạng thái là "Đang thực hiện"
                if (careSchedule.Status == "Đang thực hiện")
                {
                    foreach (var css in careSchedule.CareScheduleSupplies)
                    {
                        var supply = await _context.Supplies.FindAsync(css.SupplyId);
                        if (supply != null)
                        {
                            supply.Quantity += css.QuantityUsed;
                            supply.UpdatedDate = DateTime.Now;
                            _context.Update(supply);
                        }
                    }
                }

                _context.CareSchedules.Remove(careSchedule);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa lịch chăm sóc thành công!";
            return RedirectToAction(nameof(Index));
        }


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
                .Include(cs => cs.CareScheduleSupplies)
                    .ThenInclude(css => css.Supply)
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

            // Cập nhật trạng thái của CareSchedules và xử lý số lượng vật tư
            foreach (var careSchedule in careSchedules)
            {
                // Ưu tiên 1: Nếu Order đã hoàn thành, CareSchedule chuyển từ "Đang thực hiện" sang "Hoàn thành"
                if (isOrderProcessed && careSchedule.Status == "Đang thực hiện")
                {
                    careSchedule.Status = "Hoàn thành";
                    _context.Update(careSchedule);
                    updated = true;

                    // Khôi phục số lượng vật tư tái sử dụng
                    foreach (var css in careSchedule.CareScheduleSupplies)
                    {
                        var supply = css.Supply;
                        if (IsSupplyReusable(supply.SupplyName))
                        {
                            supply.Quantity += css.QuantityUsed;
                            supply.UpdatedDate = DateTime.Now;
                            _context.Update(supply);
                        }
                    }
                }
                // Ưu tiên 2: Nếu Order đã xác nhận, CareSchedule chuyển sang "Đang thực hiện"
                else if (isOrderConfirmed && careSchedule.Status != "Đang thực hiện")
                {
                    careSchedule.Status = "Đang thực hiện";
                    _context.Update(careSchedule);
                    updated = true;

                    // Giảm số lượng vật tư
                    foreach (var css in careSchedule.CareScheduleSupplies)
                    {
                        var supply = css.Supply;
                        supply.Quantity -= css.QuantityUsed;
                        supply.UpdatedDate = DateTime.Now;
                        _context.Update(supply);
                    }
                }
            }

            if (updated)
            {
                await _context.SaveChangesAsync();
            }

            return updated;
        }

        public async Task<bool> UpdateCareScheduleStatusForServiceRequest(int requestId)
        {
            var serviceRequest = await _context.ServiceRequests
                .FirstOrDefaultAsync(sr => sr.RequestId == requestId);

            if (serviceRequest == null || serviceRequest.Status != "Đã xử lý")
            {
                return false;
            }

            var careSchedules = await _context.CareSchedules
                .Include(cs => cs.CareScheduleSupplies)
                    .ThenInclude(css => css.Supply)
                .Where(cs => cs.RequestId == requestId)
                .ToListAsync();

            bool updated = false;

            foreach (var careSchedule in careSchedules)
            {
                if (careSchedule.Status != "Hoàn thành")
                {
                    careSchedule.Status = "Hoàn thành";
                    _context.Update(careSchedule);
                    updated = true;

                    // Khôi phục số lượng vật tư tái sử dụng
                    foreach (var css in careSchedule.CareScheduleSupplies)
                    {
                        var supply = css.Supply;
                        if (IsSupplyReusable(supply.SupplyName))
                        {
                            supply.Quantity += css.QuantityUsed;
                            supply.UpdatedDate = DateTime.Now;
                            _context.Update(supply);
                        }
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
    }
}