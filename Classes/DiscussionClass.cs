using NewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewAPI.Classes
{
    public class DiscussionClass
    {
        public int Id { get; set; }
        public Nullable<int> TaskId { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public string Text { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Task Task { get; set; }

        public DiscussionClass(Discussion discussion)
        {
            Id = discussion.Id;
            TaskId = discussion.TaskId;
            EmployeeId = discussion.EmployeeId;
            Text = discussion.Text;
            Employee = discussion.Employee;
            Task = discussion.Task;
        }
    }
}