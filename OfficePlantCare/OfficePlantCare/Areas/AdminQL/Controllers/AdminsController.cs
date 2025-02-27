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
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            // Số ghi trên 1 trang
            int limit = 5;

            // Tạo query cơ bản
            IQueryable<Admin> query = _context.Admins.OrderBy(c => c.AdminId);

            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Username.Contains(name));
            }

            // Chuyển query sang danh sách
            var admin = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedAdmin = admin.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;

            return View(pagedAdmin);
        }


        // GET: AdminQL/Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.Include(a => a.Role).FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }
            return PartialView("_Details", admin);
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
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\images\\admin", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        // Lưu đường dẫn vào Admin
                        admin.Avatar = "/assets/images/admin/" + fileName;
                    }
                }

                // Thêm Admin vào database
                _context.Add(admin);
                await _context.SaveChangesAsync();

                // Chỉ thêm vào Staff nếu Admin là nhân viên (RoleId >= 2)
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

        // GET: AdminQL/Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
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
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\images\\admin", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            admin.Avatar = "/assets/images/admin/" + fileName;
                        }
                    }
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.Include(a => a.Role).FirstOrDefaultAsync(m => m.AdminId == id);
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

        public IActionResult ServiceRequests()
        {
            var requests = _context.ServiceRequests
                                   .Include(sr => sr.Service)
                                   .Include(sr => sr.Customer)
                                   .ToList();
            return View(requests);
        }

    }
}
