using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewDesktop.Classes
{
    internal class Project
    {
        public int Id { get; set; }
        public string FullTitle { get; set; }
        public string ShortTitle { get; set; }
        public byte[] Icon { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        public DateTime? StartScheduledDate { get; set; }
        public DateTime? FinishScheduledDate { get; set; }
        public string Description { get; set; }
        public int? CreatorEmployeeId { get; set; }
        public int? ResponsibleEmployeeId { get; set; }
    }
}
