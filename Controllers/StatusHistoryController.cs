using NewAPI.Classes;
using NewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NewAPI.Controllers
{
    public class StatusHistoryController : ApiController
    {
        NewSession1Entities DataBase = new NewSession1Entities();

        public IHttpActionResult GetStatusHistoryByTaskId(int taskId)
        {
            var Histories = DataBase.StatusHistory.Where(x => x.TaskId == taskId);

            if(Histories.Count() > 0)
            {
                var HistoryList = Histories.ToList().ConvertAll(x => new StatusHistoryClass(x));

                return Ok(HistoryList);
            }
            else
            {
                return Ok(new List<StatusHistoryClass>());
            }
        }
    }
}