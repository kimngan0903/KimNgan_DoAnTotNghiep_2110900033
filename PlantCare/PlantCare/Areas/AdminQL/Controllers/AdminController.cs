using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlantCare.Models;

namespace PlantCare.Areas.AdminQL.Controllers
{
    public class AdminController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public AdminController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/Admins
        public async Task<IActionResult> Index()
        {
            return View(await _context.Admins.ToListAsync());
        }

        // GET: AdminQL/Admins/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName"); // Truyền danh sách Role
            return View();
        }

        // POST: AdminQL/Admins/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,Username,PasswordHash,Email,Avatar,Status,RoleId")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                // Thêm Admin
                _context.Add(admin);
                await _context.SaveChangesAsync();

                // Chỉ thêm vào Staff nếu Admin là nhân viên (RoleId = 2, 3, ... tùy theo hệ thống)
                if (admin.RoleId >= 2)
                {
                    var staff = new Staff
                    {
                        StaffName = admin.Username,
                        Email = admin.Email,
                        Phone = "N/A", // Có thể cập nhật sau
                        Position = "Nhân viên",
                        Status = "Đang làm",
                        RoleId = admin.RoleId
                    };
                    _context.Staffs.Add(staff);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }
        // GET: AdminQL/Admins/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var admin = await _context.ServiceCategories.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return PartialView("_Details", admin);
        }

        // GET: AdminQL/Admins/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var admin = await _context.ServiceCategories.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return PartialView("_Edit", admin);
        }

        // POST: AdminQL/Admins/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,Username,PasswordHash,Email,Avatar,Status,RoleId")] Admin admin)
        {
            if (id != admin.AdminId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();

                    // Kiểm tra xem Admin này có trong bảng Staff không
                    var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == admin.Email);

                    if (admin.RoleId >= 2) // Nếu Admin là nhân viên
                    {
                        if (staff != null)
                        {
                            // Cập nhật thông tin Staff
                            staff.StaffName = admin.Username;
                            staff.RoleId = admin.RoleId;
                            staff.Status = admin.Status;
                            _context.Staffs.Update(staff);
                        }
                        else
                        {
                            // Nếu chưa có trong Staff, thêm mới
                            var newStaff = new Staff
                            {
                                StaffName = admin.Username,
                                Email = admin.Email,
                                Phone = "N/A",
                                Position = "Nhân viên",
                                Status = "Đang làm",
                                RoleId = admin.RoleId
                            };
                            _context.Staffs.Add(newStaff);
                        }
                    }
                    else if (staff != null)
                    {
                        // Nếu Admin không còn là nhân viên nhưng có trong Staff, xóa khỏi Staffs
                        _context.Staffs.Remove(staff);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: AdminQL/Admins/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var admin = await _context.ServiceCategories.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return PartialView("_Delete", admin);
        }

        // POST: AdminQL/Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == admin.Email);

                if (staff != null)
                {
                    // Nếu Admin bị xóa là nhân viên, cập nhật trạng thái Staff thay vì xóa
                    staff.Status = "Nghỉ việc";
                    _context.Staffs.Update(staff);
                }

                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.AdminId == id);
        }
    }
}
