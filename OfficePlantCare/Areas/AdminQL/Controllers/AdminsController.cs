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
    public class AdminsController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public AdminsController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/Admins
        public async Task<IActionResult> Index(string name, int? roleId, int page = 1)
        {
            // Số ghi trên 1 trang
            int limit = 5;
            TempData["CurrentPage"] = page;
            // Tạo query cơ bản
            IQueryable<Admin> query = _context.Admins
                                              .Include(a => a.Role)
                                              .OrderBy(c => c.AdminId);
            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Username.Contains(name));
            }
            // Nếu có tham số roleId trên URL, thêm điều kiện lọc
            if (roleId.HasValue)
            {
                query = query.Where(c => c.RoleId == roleId.Value);
            }
            // Chuyển query sang danh sách
            var admin = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedAdmin = admin.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;
            ViewBag.selectedRoleId = roleId;

            // Truyền danh sách Role để hiển thị trong dropdown
            ViewBag.Roles = new SelectList(await _context.Roles.ToListAsync(), "RoleId", "RoleName", roleId);
            return View(pagedAdmin);
        }

        // GET: AdminQL/Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return PartialView("_Details", admin);
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AdminId == id);

            if (admin == null)
            {
                return NotFound();
            }

            // Chuẩn bị dữ liệu cho dropdown RoleId (nếu cần trong form Edit)
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", admin.RoleId);

            return View(admin);
        }
        // GET: AdminQL/Admins/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: AdminQL/Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,Username,PasswordHash,Email,CreatedDate,UpdatedDate,Avatar,Status,RoleId,Address")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh trước
                var files = HttpContext.Request.Form.Files;
                if (files.Any() && files[0].Length > 0)
                {
                    var file = files[0];
                    var fileName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\admin", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn vào Admin
                    admin.Avatar = "/img/admin/" + fileName;
                }

                // Thêm Admin vào database
                _context.Add(admin);
                await _context.SaveChangesAsync();

                // Nếu RoleId >= 2 thì thêm vào Staff
                if (admin.RoleId >= 2)
                {
                    var staff = new Staff
                    {
                        StaffName = admin.Username,
                        Email = admin.Email,
                        Phone = "", // Có thể cập nhật sau
                        Position = "",
                        Status = "Đang làm",
                        RoleId = admin.RoleId,
                        Avatar = admin.Avatar 
                    };
                    _context.Staffs.Add(staff);
                    await _context.SaveChangesAsync();

                    // Thêm thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Thêm mới Admin và Nhân viên thành công!";
                }

                int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
                return RedirectToAction(nameof(Index), new { page = currentPage });
            }

            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", admin.RoleId);
            return View(admin);
        }

        // GET: AdminQL/Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                                .Include(a => a.Role)
                               .FirstOrDefaultAsync(o => o.AdminId == id);

            if (admin == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", admin.RoleId);
            return PartialView("_Edit", admin);
        }

        // POST: AdminQL/Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,Username,PasswordHash,Email,CreatedDate,UpdatedDate,Avatar,Status,RoleId,Address")] Admin admin)
        {
            if (id != admin.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    if (files.Any() && files[0].Length > 0)
                    {
                        var file = files[0];
                        var fileName = file.FileName;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\admin", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            admin.Avatar = "/img/admin/" + fileName;
                        }
                    }
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                    // Thêm thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Cập nhật Admin thành công!";

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
                                Phone = "",
                                Position = "",
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
                int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
                return RedirectToAction(nameof(Index), new { page = currentPage });
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", admin.RoleId);
            return View(admin);
        }

        // GET: AdminQL/Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AdminId == id);
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
                _context.Admins.Remove(admin);
            }

            await _context.SaveChangesAsync();
            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Xóa Admin thành công!";
            int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
            return RedirectToAction(nameof(Index), new { page = currentPage });
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.AdminId == id);
        }
    }
}
