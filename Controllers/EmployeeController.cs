using NewAPI.Classes;
using NewAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NewAPI.Controllers
{
    public class EmployeeController : ApiController
    {
        public NewSession1Entities DataBase = new NewSession1Entities();
        public IHttpActionResult GetEmployees()
        {
            return Ok(DataBase.Employee.ToList().ConvertAll(x => new EmployeeClass(x)));
        }
    }
}