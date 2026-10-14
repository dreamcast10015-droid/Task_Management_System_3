using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Management_System.Data;
using Task_Management_System.Models;

namespace Task_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. عرض كل الموظفين (FR-08)
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return Ok(employees);
        }

        // 2. إضافة موظف جديد (FR-09)
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            // التحقق من صحة البيانات (الاسم والإيميل) (FR-12)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // إرجاع رسالة نجاح واضحة (FR-13)
            return Ok(new { message = "Employee added successfully", employee });
        }

        // 3. تعديل بيانات موظف (FR-10)
        [HttpPut("{id}")]
        public async Task<IActionResult> EditEmployee(int id, [FromBody] Employee updatedEmployee)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found" }); // رسالة إذا لم يتم العثور عليه (FR-13)

            employee.Name = updatedEmployee.Name;
            employee.Email = updatedEmployee.Email;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Employee updated successfully", employee });
        }

        // 4. حذف موظف (FR-11)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            // بنجيب الموظف مع المهام بتاعته عشان نتأكد
            var employee = await _context.Employees.Include(e => e.Tasks).FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound(new { message = "Employee not found" });

            // تطبيق شرط منع الحذف لو عنده مهام مسندة (FR-11)
            if (employee.Tasks != null && employee.Tasks.Any())
            {
                return BadRequest(new { message = "Cannot delete employee because they have assigned tasks." });
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Employee deleted successfully" });
        }
    }
}