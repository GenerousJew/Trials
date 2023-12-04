using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewDesktop.Classes
{
    internal class StatusHistory
    {
        public int Id { get; set; }
        public Nullable<int> TaskId { get; set; }
        public Nullable<int> OldStatusId { get; set; }
        public string OldStatusName { get; set; }
        public Nullable<int> NewStatusId { get; set; }
        public string NewStatusName { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string UpdateTimeText { get { return ((DateTime)UpdateTime).ToString("d"); } }
    }
}
