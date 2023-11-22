using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace HiredWorkerManagement.Models
{ 
    public abstract class EntityBase
    {
        [Key]
        public int id { get; set; }
    }
      

    public class Skill
    {
        
        [Required, StringLength(30)]
        public string SkillName { get; set; }
        public virtual  ICollection<WorkerSkill> WorkerSkills { get; set; }=new List<WorkerSkill>();
    }

    public class Worker
    {
       
        [Required, StringLength(50), Display(Name = "Worker name")]
        public string WorkerName { get; set; }
        [Required, Column(TypeName = "money")]
        public decimal PayRate { get; set; } 
        [Required, StringLength(20)]
        public string Phone { get; set; } 
        [Required, StringLength(100)]
        public string Address { get; set; } 
        [Required, StringLength(50)]
        public string Picture { get; set; } 
        public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
        public virtual ICollection<WorkerSkill> WorkerSkills { get; set; } = new List<WorkerSkill>();
    }
    public class WorkLog
    {
        
        [Required, Column(TypeName = "date"), Display(Name = "Strat Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; } = DateTime.Today;
        [Display(Name = "End Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? EndDate { get; set; }
        [Required]
        public int WorkerId { get; set; }
        [ForeignKey(nameof(WorkerId))]
        public virtual Worker Worker { get; set; }
    }
    public class WorkerSkill
    {
        [Key, Column(Order =0), ForeignKey(nameof(Worker))]
        public int WorkerId { get; set; }
        [Key, Column(Order =1), ForeignKey(nameof(Skill))]
        public int SkillId { get; set; }
        public virtual Worker Worker { get; set; }
        public virtual Skill Skill { get; set; }
    }
    public class WorkerDbContext : DbContext
    {
        public WorkerDbContext() 
        {
            Database.SetInitializer(new WorkerDbInitializer());
        }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<WorkLog> WorkLogs { get; set; } 
        public DbSet<Skill> Skills { get; set; }
        public DbSet<WorkerSkill> WorkerSkills { get; set; }
    }
    public class WorkerDbInitializer : DropCreateDatabaseIfModelChanges<WorkerDbContext>
    {
        protected override void Seed(WorkerDbContext db)
        {
            Skill skill1 = new Skill { SkillName="Electrical Work"};
            Skill skill2 = new Skill { SkillName = "Plumbing" };
            Skill skill3 = new Skill { SkillName = "Pipe Fitting" };
            Skill skill4 = new Skill { SkillName = "Glass door fitting" };
            Skill skill5 = new Skill { SkillName = "Thai works" };
            db.Skills.AddRange(new[] { skill1, skill2, skill3, skill4, skill5 });
            Worker w1 = new Worker { WorkerName = "Abu Mia", Phone = "01711000000", Address = "Mirpur, Section 13, Dhaka", PayRate=2000.00M, Picture="e1.jpeg" };
            Worker w2 = new Worker { WorkerName = "Harun Ahmed", Phone = "01912000000", Address = "Mirpur, Section 1, Dhaka", PayRate=2100.00M, Picture="e2.jpeg" };
            Worker w3 = new Worker { WorkerName = "Shebul Ahmed", Phone = "01812000000", Address = "Mirpur, Section 1, Dhaka", PayRate=1950.00M, Picture = "e3.jpg" };
            w1.WorkerSkills.Add(new WorkerSkill { Skill = skill1 });
            w2.WorkerSkills.Add(new WorkerSkill { Skill = skill2 });
            w2.WorkerSkills.Add(new WorkerSkill { Skill=skill3 });
            w3.WorkerSkills.Add(new WorkerSkill { Skill = skill5 });

            w1.WorkLogs.Add(new WorkLog { StartDate= DateTime.Now.AddDays(-20), EndDate=DateTime.Now.AddDays(-14) });
            w1.WorkLogs.Add(new WorkLog { StartDate = DateTime.Now.AddDays(-7), EndDate = DateTime.Now.AddDays(-5) });
            w2.WorkLogs.Add(new WorkLog { StartDate = DateTime.Now.AddDays(-20), EndDate = DateTime.Now.AddDays(-14) });
            w2.WorkLogs.Add(new WorkLog { StartDate = DateTime.Now.AddDays(-7), EndDate = DateTime.Now.AddDays(-5) });
            w3.WorkLogs.Add(new WorkLog { StartDate = DateTime.Now.AddDays(-20), EndDate = DateTime.Now.AddDays(-14) });
            w3.WorkLogs.Add(new WorkLog { StartDate = DateTime.Now.AddDays(-2) });
            db.Workers.AddRange(new Worker[] { w1, w2, w3 });
            db.SaveChanges();
        }
    }
}