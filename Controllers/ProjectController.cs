using NewAPI.Classes;
using NewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NewAPI.Controllers
{
    public class ProjectController : ApiController
    {
        NewSession1Entities DataBase = new NewSession1Entities();
        public IHttpActionResult GetProjects()
        {
            var ProjectList = DataBase.Project.ToList().ConvertAll(x => new ProjectClass(x));

            return Ok(ProjectList);
        }
    }
}