using NewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewAPI.Classes
{
    public class StatusHistoryClass
    {
        public int Id { get; set; }
        public Nullable<int> TaskId { get; set; }
        public Nullable<int> OldStatusId { get; set; }
        public string OldStatusName { get; set; }
        public Nullable<int> NewStatusId { get; set; }
        public string NewStatusName { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }

        public StatusHistoryClass(StatusHistory history) 
        {
            Id = history.Id;
            TaskId = history.TaskId;
            OldStatusId = history.OldStatusId;
            OldStatusName = history.TaskStatus.Name;
            NewStatusId = history.NewStatusId;
            NewStatusName = history.TaskStatus1.Name;
            UpdateTime = history.UpdateTime;
        }
    }
}