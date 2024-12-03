using Microsoft.EntityFrameworkCore;
using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EntityModel.Task> CreateTask(EntityModel.Task task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }
        
        public async Task<List<EntityModel.Task>> GetTask(string userId)
        {
            var task = await _context.Tasks
                .Where(t => t.AssignedUserId == userId).ToListAsync();
            return task;
        }

        public async Task<List<EntityModel.Task>> GetAllTasks()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<EntityModel.Task> UpdateTask(EntityModel.Task task, string userId, string role)
        {
            var existingTask = await _context.Tasks.FindAsync(task.Id);

            if (existingTask == null)
            {
                return null;
            }
            if (role == "Admin")
            {
                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.AssignedUserId = task.AssignedUserId;
                existingTask.Status = task.Status;
            }
            else if (role == "User" && existingTask.AssignedUserId == userId)
            {
                existingTask.Status = task.Status; 
            }
            else
            {
                return null; 
            }

            await _context.SaveChangesAsync();
            return existingTask;
        }

        public async Task<bool> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
      
    }
}
