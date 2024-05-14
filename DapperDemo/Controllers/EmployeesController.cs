using DapperDemo.Models;
using DapperDemo.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DapperDemo.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ICompanyRepository _compRepo;
        private readonly IEmployeeRepository _empRepo;

        [BindProperty]
        public Employee Employee { get; set; }

        public EmployeesController(ICompanyRepository compRepo, IEmployeeRepository empRepo)
        {
            _compRepo = compRepo;
            _empRepo = empRepo;
        }

        public async Task<IActionResult> Index()
        {
            return View(_empRepo.GetAll());
        }

        public IActionResult Create()
        {
            IEnumerable<SelectListItem> companyList = _compRepo.GetAll()
                .Select(s => new SelectListItem {
                    Text = s.Name,
                    Value = s.CompanyId.ToString()
                });
            ViewBag.CompanyList = companyList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePOST()
        {
            if (ModelState.IsValid)
            {
                _empRepo.Add(Employee);                
                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IEnumerable<SelectListItem> companyList = _compRepo.GetAll()
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.CompanyId.ToString()
                });
            ViewBag.CompanyList = companyList;

            Employee = _empRepo.Find(id.GetValueOrDefault());
            if (Employee == null)
            {
                return NotFound();
            }
            return View(Employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (id != Employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _empRepo.Update(Employee);
                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _empRepo.Remove(id.GetValueOrDefault());
            return RedirectToAction(nameof(Index));
        }
        
    }
}
