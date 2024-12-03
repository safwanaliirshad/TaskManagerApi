using Infrastructure;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly UserManager<IdentityUser> _userManager;

        public TasksController(ITaskService taskService, UserManager<IdentityUser> userManager)
        {
            _taskService = taskService;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTask([FromBody] EntityModel.TaskViewModel task)
        {
            await _taskService.CreateTask(new EntityModel.Task { Id = task.Id, Title = task.Title, Description = task.Description, Status = task.Status, AssignedUserId = (await _userManager.FindByEmailAsync(task.AssignedUserEmail)).Id });
            return Ok(task);
        }
        [Route("GetAssignedTask")]
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAssignedTask()
        {
            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var task = await _taskService.GetTask(userId);
            return Ok(task);
        }

        [Route("GetAllTasks")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTasks()
        {

            return Ok(await _taskService.GetAllTasks());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskModel taskUpdates)
        {
            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var role = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            var updateDetail = new EntityModel.Task
            {
                Id = id,
                Title = taskUpdates.Title,
                Description = taskUpdates.Description,
                Status = taskUpdates.Status
            };
            var task = await _taskService.UpdateTask(updateDetail, userId, role);

            if (role == "User" && task.AssignedUserId != userId)
                return Forbid();

            return Ok(task);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskService.DeleteTask(id);
            if (!task) return NotFound();
            return Ok();
        }
    }
}
