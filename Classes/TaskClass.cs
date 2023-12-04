using NewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewAPI.Classes
{
    public class TaskClass
    {
        public int Id { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string FullTitle { get; set; }
        public string ShortTitle { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public Nullable<int> ExecuriveEmployeeId { get; set; }
        public string ExecuriveEmployeeFullName { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string StatusName { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> UpdatedTime { get; set; }
        public Nullable<System.DateTime> DeletedTime { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }
        public Nullable<System.DateTime> StartActualTime { get; set; }
        public Nullable<System.DateTime> FinishActualTime { get; set; }
        public Nullable<int> PreviousTaskId { get; set; }
        public List<int> ObserverIdList { get; set; }

        public TaskClass(Task task)
        {
            Id = task.Id;
            ProjectId = task.ProjectId;
            FullTitle = task.FullTitle;
            ShortTitle = task.ShortTitle;
            Number = task.Project.ShortTitle + Convert.ToString(Id);
            Description = task.Description;
            ExecuriveEmployeeId = task.ExecuriveEmployeeId;
            ExecuriveEmployeeFullName = ((task.Employee.FirstName + " " + task.Employee.MiddleName).Trim() + " " + task.Employee.LastName).Trim();
            StatusId = task.StatusId;
            StatusName = task.TaskStatus.Name;
            CreatedTime = task.CreatedTime;
            UpdatedTime = task.UpdatedTime;
            DeletedTime = task.DeletedTime;
            Deadline = task.Deadline;
            StartActualTime = task.StartActualTime;
            FinishActualTime = task.FinishActualTime;
            PreviousTaskId = task.PreviousTaskId;
            ObserverIdList = task.Employee1.ToList().ConvertAll(x => x.Id);
        }
    }
}