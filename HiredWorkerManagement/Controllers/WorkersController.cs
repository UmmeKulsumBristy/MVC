using HiredWorkerManagement.Models;
using HiredWorkerManagement.ViewModels;
using HiredWorkerManagement.ViewModels.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using X.PagedList;
using X.PagedList.Mvc;
namespace HiredWorkerManagement.Controllers
{
    [Authorize]
    public class WorkersController : Controller
    {
        private readonly WorkerDbContext db = new WorkerDbContext();
        // GET: Workers
        public ActionResult Index(int pg=1)
        {
            List<WorkerViewModel> vm = new List<WorkerViewModel>();
            db.Workers.ToList().ForEach(w =>
            {
                var v = new WorkerViewModel
                {
                    WorkerId = w.WorkerId,
                    WorkerName = w.WorkerName,
                    Phone = w.Phone,
                    Address = w.Address,
                    PayRate = w.PayRate,
                    Picture = w.Picture
                };
                v.WorkLogs = w.WorkLogs.Select(wl => new WorkLogViewModel { WorkLogId = wl.WorkLogId, WorkerId = wl.WorkerId, WorkerName = wl.Worker.WorkerName, StartDate = wl.StartDate, EndDate = wl.EndDate, PayRate=wl.Worker.PayRate }).ToList();
                v.Skills = string.Join(",",w.WorkerSkills.Select(ws => ws.Skill.SkillName).ToList());
                vm.Add(v);
            });
            return View(vm.OrderBy(w=> w.WorkerId).ToPagedList(pg, 5));
        }
        public ActionResult Create()
        {
            var data = new WorkerInputModel();
            data.WorkLogs.Add(new WorkLog());
            ViewData["Skills"]= db.Skills.ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult Create(WorkerInputModel model, string act="")
        {
            if(act == "add")
            {
                model.WorkLogs.Add(new WorkLog());
                foreach(var v in ModelState.Values)
                {
                    v.Errors.Clear();
                }
            }
            if (act.StartsWith("remove"))
            {
                var index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.WorkLogs.RemoveAt(index);
                
                foreach (var v in ModelState.Values)
                {
                    v.Errors.Clear();
                }
            }
            if(act == "insert")
            {
                if(ModelState.IsValid)
                {
                    var worker = new Worker
                    {
                        WorkerName = model.WorkerName,
                        Phone = model.Phone,
                        Address = model.Address,
                        PayRate = model.PayRate

                    };
                    string ext = Path.GetExtension(model.Picture.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                    string savePath = Path.Combine(Server.MapPath("~/Pictures"), fileName);
                    model.Picture.SaveAs(savePath);
                    worker.Picture = fileName;
                    foreach(var l in model.WorkLogs)
                    {
                        worker.WorkLogs.Add(l);
                    }
                    foreach(var i in model.SkillId)
                    {
                        worker.WorkerSkills.Add(new WorkerSkill { SkillId = i });
                    }
                    db.Workers.Add(worker);
                    db.SaveChanges();
                    model = new WorkerInputModel();
                    model.WorkLogs.Add(new WorkLog());
                }
            }
            ViewData["Act"] = act;
            ViewData["Skills"] = db.Skills.ToList();
            Thread.Sleep(500);
            return PartialView("_CreatePartial", model);
        }
        public ActionResult AddSkill (string name)
        {
            var skill = new Skill { SkillName = name };
            db.Skills.Add(skill);
            db.SaveChanges();
            return Json(new { success = true, data = skill }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int id)
        {
            var w= db.Workers.FirstOrDefault(x=> x.WorkerId == id);
            if (w == null) return new HttpNotFoundResult();
            var data = new WorkerEditModel
            {
                WorkerId=w.WorkerId,
                WorkerName=w.WorkerName,
                Phone=w.Phone,
                Address=w.Address,
                PayRate=w.PayRate
                
            };
            data.WorkLogs = w.WorkLogs.ToList();
            foreach (var i in w.WorkerSkills)
            {
                data.SkillId.Add(i.SkillId);
            }
            ViewData["CurrentPic"] = w.Picture;
            ViewData["Skills"] = db.Skills.ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(WorkerEditModel model, string act = "")
        {
            var worker = db.Workers.FirstOrDefault(x => x.WorkerId == model.WorkerId);
            if(worker == null) return new HttpNotFoundResult();
           
            if (act == "add")
            {
                model.WorkLogs.Add(new WorkLog());
                foreach (var v in ModelState.Values)
                {
                    v.Errors.Clear();
                }
            }
            if (act.StartsWith("remove"))
            {
                var index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.WorkLogs.RemoveAt(index);

                foreach (var v in ModelState.Values)
                {
                    v.Errors.Clear();
                }
            }
            if (act == "update")
            {
                if (ModelState.IsValid)
                {

                    worker.WorkerName = model.WorkerName;
                    worker.Phone = model.Phone;
                    worker.Address = model.Address;
                    worker.PayRate = model.PayRate;
                    if(model.Picture != null)
                    {
                        string ext = Path.GetExtension(model.Picture.FileName);
                        string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = Path.Combine(Server.MapPath("~/Pictures"), fileName);
                        model.Picture.SaveAs(savePath);
                        worker.Picture = fileName;
                    }
                    var skills = db.WorkerSkills.Where(x => x.WorkerId == model.WorkerId).ToList();
                    for(var i = 0; i < skills.Count; i++)
                    {
                        db.WorkerSkills.Remove(skills[i]);
                        worker.WorkerSkills.Remove(skills[i]);
                    }
                    var logs = db.WorkLogs.Where(x => x.WorkerId == model.WorkerId).ToList();
                    for (var i = 0; i < logs.Count; i++)
                    {
                        db.WorkLogs.Remove(logs[i]);
                        worker.WorkLogs.Remove(logs[i]);
                    }
                    foreach(var s in model.SkillId)
                    {
                        worker.WorkerSkills.Add(new WorkerSkill { SkillId = s });
                    }
                    foreach (var l in model.WorkLogs)
                    {
                        worker.WorkLogs.Add(l);
                    }
                    db.SaveChanges();
                    
                }
            }
            ViewData["Act"] = act;
            ViewData["Skills"] = db.Skills.ToList();
          
            return PartialView("_EditPartial", model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var d = db.Workers.FirstOrDefault(x => x.WorkerId == id);
            if (d == null) return new HttpNotFoundResult();
            db.Workers.Remove(d);
            db.SaveChanges();
            return Json(new { success = true, id });
        }
    }
}