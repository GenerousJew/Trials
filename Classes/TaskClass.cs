using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewDesktop.Classes
{
    internal class TaskClass
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
        public string DeadlineText 
        { 
            get 
            { 
                if (this.DeletedTime == null)
                {
                    return string.Empty;
                }
                return ((DateTime)Deadline).ToString("g"); 
            } 
        }
        public Nullable<System.DateTime> StartActualTime { get; set; }
        public Nullable<System.DateTime> FinishActualTime { get; set; }
        public Nullable<int> PreviousTaskId { get; set; }
        public List<int> ObserverIdList { get; set; }
    }
}
