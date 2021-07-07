using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallCenterAPI.Models
{
    public class APINO12Header
    {
        public List<Detail12> Detail { get; set; }
    }


    public class Detail12
    {
        public string NoteCode { get; set; }
        public string Descr { get; set; }

    }
}