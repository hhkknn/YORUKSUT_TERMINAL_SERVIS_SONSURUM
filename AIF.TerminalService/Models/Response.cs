using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.Models
{
    public class Response
    {
        public DataTable _list { get; set; } 

        public int Val { get; set; }

        public string Desc { get; set; }
    }
}