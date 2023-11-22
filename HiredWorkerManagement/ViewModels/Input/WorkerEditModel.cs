using HiredWorkerManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace HiredWorkerManagement.ViewModels.Input
{
    public class WorkerEditModel
    {
        public int WorkerId { get; set; }
        [Required, StringLength(50), Display(Name = "Worker name")]
        public string WorkerName { get; set; }
        [Required, Column(TypeName = "money")]
        public decimal PayRate { get; set; }
        [Required, StringLength(20)]
        public string Phone { get; set; }
        [Required, StringLength(100)]
        public string Address { get; set; }
        
        public HttpPostedFileBase Picture { get; set; }
        public virtual List<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
        public virtual ICollection<int> SkillId { get; set; } = new List<int>();
    }
}