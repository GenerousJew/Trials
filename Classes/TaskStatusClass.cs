using NewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewAPI.Classes
{
    public class TaskStatusClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ColorHex { get; set; }

        public TaskStatusClass(TaskStatus taskStatus)
        {
            Id = taskStatus.Id;
            Name = taskStatus.Name;
            ColorHex = taskStatus.ColorHex;
        }
    }
}