using Microsoft.Ajax.Utilities;
using NewAPI.Classes;
using NewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NewAPI.Controllers
{
    public class TaskController : ApiController
    {
        NewSession1Entities DataBase = new NewSession1Entities();

        public IHttpActionResult GetTasks()
        {
            var TaskList = DataBase.Task.ToList().ConvertAll(x => new TaskClass(x));

            return Ok(TaskList);
        }

        public IHttpActionResult GetTaskByProjectId(int projectId) 
        {
            var TaskList = DataBase.Task.Where(x => x.ProjectId == projectId).ToList().ConvertAll(x => new TaskClass(x));

            return Ok(TaskList);
        }

        public IHttpActionResult DeleteTask(int id) 
        {
            var task = DataBase.Task.FirstOrDefault(x => x.Id == id);

            if(task != null)
            {
                foreach (var item in DataBase.Task.Where(x => x.PreviousTaskId == id))
                {
                    item.PreviousTaskId = task.PreviousTaskId;
                }
                
                DataBase.Task.Remove(task);
                DataBase.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        public IHttpActionResult PutTask(Task task)
        {
            var oldTask = DataBase.Task.FirstOrDefault(x => x.Id == task.Id);

            if (oldTask != null)
            {
                oldTask.ProjectId = task.ProjectId;
                oldTask.FullTitle = task.FullTitle;
                oldTask.ShortTitle = task.ShortTitle;
                oldTask.Description = task.Description;
                oldTask.ExecuriveEmployeeId = task.ExecuriveEmployeeId;

                if(oldTask.StatusId != task.StatusId)
                {
                    DataBase.StatusHistory.Add(new StatusHistory() 
                    {
                        TaskId = task.Id,
                        UpdateTime = DateTime.Now,
                        OldStatusId = oldTask.StatusId,
                        NewStatusId = task.StatusId
                    });

                    oldTask.StatusId = task.StatusId;
                }
                
                oldTask.CreatedTime = task.CreatedTime;
                oldTask.UpdatedTime = task.UpdatedTime;
                oldTask.DeletedTime = task.DeletedTime;
                oldTask.Deadline = task.Deadline;
                oldTask.StartActualTime = task.StartActualTime;
                oldTask.FinishActualTime = task.FinishActualTime;
                oldTask.PreviousTaskId = task.PreviousTaskId;

                DataBase.SaveChanges();
                return Ok();
            }

            return NotFound();
            
        }

        public IHttpActionResult PostTask(Task task)
        {
            DataBase.Task.Add(task);
            DataBase.SaveChanges();

            return Ok();
        }
    }
}