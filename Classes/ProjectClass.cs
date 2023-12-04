using NewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewAPI.Classes
{
    public class ProjectClass
    {
        public int Id { get; set; }
        public string FullTitle { get; set; }
        public string ShortTitle { get; set; }
        public byte[] Icon { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> DeletedTime { get; set; }
        public Nullable<System.DateTime> StartScheduledDate { get; set; }
        public Nullable<System.DateTime> FinishScheduledDate { get; set; }
        public string Description { get; set; }
        public Nullable<int> CreatorEmployeeId { get; set; }
        public Nullable<int> ResponsibleEmployeeId { get; set; }

        public ProjectClass(Project project)
        {
            Id = project.Id;
            FullTitle = project.FullTitle;
            ShortTitle = project.ShortTitle;
            Icon = project.Icon;
            CreatedTime = project.CreatedTime;
            DeletedTime = project.DeletedTime;
            StartScheduledDate = project.StartScheduledDate;
            FinishScheduledDate = project.FinishScheduledDate;
            Description = project.Description;
            CreatorEmployeeId = project.CreatorEmployeeId;
            ResponsibleEmployeeId = project.ResponsibleEmployeeId;
        }
    }
}