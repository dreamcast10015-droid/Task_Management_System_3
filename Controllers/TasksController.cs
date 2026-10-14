using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Management_System.Data;
using Task_Management_System.Models;

namespace Task_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // 1. عرض كل المهام (FR-01)
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            // استخدام Include لجلب بيانات الموظف المسند للمهمة
            var tasks = await _context.Tasks.Include(t => t.Employee).ToListAsync();
            return Ok(tasks);
        }

        // 2. إضافة مهمة جديدة (FR-02)
        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] TaskItem task)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // التحقق من أن الموظف موجود في حال تم تمرير EmployeeId
            if (task.EmployeeId.HasValue)
            {
                var employeeExists = await _context.Employees.AnyAsync(e => e.Id == task.EmployeeId.Value);
                if (!employeeExists)
                    return BadRequest(new { message = "Assigned employee does not exist." });
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Task added successfully", task });
        }

        // 3. تعديل المهمة وتغيير حالتها وأولويتها وإسنادها (FR-03, FR-04, FR-05, FR-06)
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTask(int id, [FromBody] TaskItem updatedTask)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound(new { message = "Task not found" });

            // التحقق من وجود الموظف عند التعديل
            if (updatedTask.EmployeeId.HasValue)
            {
                var employeeExists = await _context.Employees.AnyAsync(e => e.Id == updatedTask.EmployeeId.Value);
                if (!employeeExists)
                    return BadRequest(new { message = "Assigned employee does not exist." });
            }

            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.Status = updatedTask.Status;
            task.Priority = updatedTask.Priority;
            task.EmployeeId = updatedTask.EmployeeId;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Task updated successfully", task });
        }

        // 4. حذف المهمة (FR-07)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound(new { message = "Task not found" });

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Task deleted successfully" });
        }
    }
}