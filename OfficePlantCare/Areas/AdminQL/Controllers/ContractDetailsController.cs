using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Models;
using X.PagedList.Extensions;

namespace OfficePlantCare.Areas.AdminQL.Controllers
{
    public class ContractDetailsController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public ContractDetailsController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/ContractDetails
        //public async Task<IActionResult> Index()
        //{
        //    var officePlantCareContext = _context.ContractDetails.Include(c => c.Contract).Include(c => c.Price).Include(c => c.Service);
        //    return View(await officePlantCareContext.ToListAsync());
        //}
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            // Số ghi trên 1 trang
            int limit = 5;
            TempData["CurrentPage"] = page;

            // Tạo query cơ bản
            IQueryable<ContractDetail> query = _context.ContractDetails
                                                .Include(cd => cd.Contract)
                                                .Include(cd => cd.Price)
                                                .Include(cd => cd.Service)
                                                .OrderBy(cd => cd.Contract.ContractCode);

            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Contract.ContractCode.Contains(name));
            }

            // Chuyển query sang danh sách
            var contractDetails = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedContractDetails = contractDetails.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;

            return View(pagedContractDetails);
        }
        // GET: AdminQL/ContractDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractDetail = await _context.ContractDetails
                .Include(c => c.Contract)
                .Include(c => c.Price)
                .Include(c => c.Service)
                .FirstOrDefaultAsync(m => m.ContractDetailId == id);
            if (contractDetail == null)
            {
                return NotFound();
            }

            return PartialView("_Details", contractDetail);
        }

        // GET: AdminQL/ContractDetails/Create
        public IActionResult Create()
        {
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractCode");
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId");
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName");
            return View();
        }

        // POST: AdminQL/ContractDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractDetailId,ContractId,ServiceId,Address,PriceId,TotalAmount,Notes,Status")] ContractDetail contractDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contractDetail);
                await _context.SaveChangesAsync();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm mới chi tiết hợp đồng thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractCode", contractDetail.ContractId);
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId", contractDetail.PriceId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", contractDetail.ServiceId);
            return View(contractDetail);
        }

        // GET: AdminQL/ContractDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractDetail = await _context.ContractDetails
                                               .Include(cd => cd.Contract)
                                                .Include(cd => cd.Price)
                                                .Include(cd => cd.Service)
                                              .FirstOrDefaultAsync(o => o.ContractDetailId == id); 
            if (contractDetail == null)
            {
                return NotFound();
            }
            ViewData["ServiceName"] = contractDetail.Service?.ServiceName;
            ViewData["ContractCode"] = contractDetail.Contract?.ContractCode;
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractCode", contractDetail.ContractId);
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId", contractDetail.PriceId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", contractDetail.ServiceId);
            return PartialView("_Edit", contractDetail);
        }

        // POST: AdminQL/ContractDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractDetailId,ContractId,ServiceId,Address,PriceId,TotalAmount,Notes,Status")] ContractDetail contractDetail)
        {
            if (id != contractDetail.ContractDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contractDetail);
                    await _context.SaveChangesAsync();
                    // Thêm thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Chỉnh sửa chi tiết hợp đồng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractDetailExists(contractDetail.ContractDetailId))
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
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractCode", contractDetail.ContractId);
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId", contractDetail.PriceId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", contractDetail.ServiceId);
            return View(contractDetail);
        }

        // GET: AdminQL/ContractDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractDetail = await _context.ContractDetails
                .Include(c => c.Contract)
                .Include(c => c.Price)
                .Include(c => c.Service)
                .FirstOrDefaultAsync(m => m.ContractDetailId == id);
            if (contractDetail == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", contractDetail);
        }

        // POST: AdminQL/ContractDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contractDetail = await _context.ContractDetails.FindAsync(id);
            if (contractDetail != null)
            {
                _context.ContractDetails.Remove(contractDetail);
            }

            await _context.SaveChangesAsync();
            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Xóa chi tiết hợp đồng thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool ContractDetailExists(int id)
        {
            return _context.ContractDetails.Any(e => e.ContractDetailId == id);
        }
    }
}
