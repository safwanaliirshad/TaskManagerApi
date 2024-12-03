using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface ITaskService
    {
        Task<EntityModel.Task> CreateTask(EntityModel.Task task);
        Task<List<EntityModel.Task>> GetTask( string userId);
        Task<List<EntityModel.Task>> GetAllTasks();
        Task<EntityModel.Task> UpdateTask(EntityModel.Task task, string userId, string role);
        Task<bool> DeleteTask(int id);
    }
}
