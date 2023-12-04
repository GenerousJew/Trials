using NewAPI.Classes;
using NewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NewAPI.Controllers
{
    public class TaskStatusController : ApiController
    {
        NewSession1Entities DataBase = new NewSession1Entities();

        public IHttpActionResult GetTaskStatuses()
        {
            var TaskStatusList = DataBase.TaskStatus.ToList().ConvertAll(x => new TaskStatusClass(x));

            return Ok(TaskStatusList);
        }
    }
}