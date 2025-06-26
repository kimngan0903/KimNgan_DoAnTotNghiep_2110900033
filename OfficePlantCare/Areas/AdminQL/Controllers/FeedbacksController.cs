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
    public class FeedbacksController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public FeedbacksController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/Feedbacks
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            // Số ghi trên 1 trang
            int limit = 5;
            TempData["CurrentPage"] = page;

            // Tạo query cơ bản
            IQueryable<Feedback> query = _context.Feedbacks
                                         .Include(c => c.Customer)
                                         .Include(c => c.Service)
                                        .OrderBy(c => c.Customer.CustomerName);

            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Customer.CustomerName.Contains(name));
            }

            // Chuyển query sang danh sách
            var feedback = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pageFeedback = feedback.ToPagedList(page, limit);
            var officePlantCareContext = _context.Feedbacks.Include(f => f.Customer).Include(f => f.Service);
            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;

            return View(pageFeedback);
        }
        // GET: AdminQL/Feedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Customer)
                .Include(f => f.Service)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return PartialView("_Details", feedback);
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Customer)
                .Include(f => f.Service)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }
        // GET: AdminQL/Feedbacks/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName");
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName");
            return View();
        }

        // POST: AdminQL/Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FeedbackId,CustomerId,ServiceId,FeedbackDate,Rating,Content,Status")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm đánh giá thành công!";
                int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
                return RedirectToAction(nameof(Index), new { page = currentPage });
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", feedback.CustomerId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", feedback.ServiceId);
            return View(feedback);
        }

        // GET: AdminQL/Feedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", feedback.CustomerId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", feedback.ServiceId);
            return PartialView("_Edit", feedback);
        }

        // POST: AdminQL/Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FeedbackId,CustomerId,ServiceId,FeedbackDate,Rating,Content,Status")] Feedback feedback)
        {
            if (id != feedback.FeedbackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                    // Thêm thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Cập nhật đánh giá thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.FeedbackId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", feedback.CustomerId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", feedback.ServiceId);
            return View(feedback);
        }

        // GET: AdminQL/Feedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Customer)
                .Include(f => f.Service)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", feedback);
        }

        // POST: AdminQL/Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
            }

            await _context.SaveChangesAsync();
            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Xóa đánh giá thành công!";
            int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
            return RedirectToAction(nameof(Index), new { page = currentPage });
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(e => e.FeedbackId == id);
        }
    }
}
