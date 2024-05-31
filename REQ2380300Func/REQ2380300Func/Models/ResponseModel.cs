using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REQ2380300Func.Models
{
    public class ResponseModel
    {
        public Apiversion apiVersion { get; set; }
        public Destsystem destSystem { get; set; }
        public Businessdata businessData { get; set; }
    }

    public class Apiversion
    {
        public string apiName { get; set; }
        public string version { get; set; }
    }

    public class Destsystem
    {
        public Result result { get; set; }
    }

    public class Result
    {
        public string message { get; set; }
    }

    public class Businessdata
    {
        public Body body { get; set; }
    }

    public class Body
    {
        public string processId { get; set; }
    }

}
