using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AttributeRoutingSample.Models;

namespace AttributeRoutingSample.Controllers
{
    /// <summary>
    /// This controller demonstrates the following:
    /// - Name for the route attributes defined at controller and action levels. This name can be used to 
    ///   generate links just like how you can with conventional routes
    /// </summary>
    [RoutePrefix("Students")]
    [Route("{action}/{id:int}", Name = "StudentsRoute")]
    public class StudentsController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: /Students/
        // GET: /Students/Index
        [Route(Name = "GetAllStudentsView")]
        [Route("Index")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Students.ToListAsync());
        }

        // GET: /Students/Details/5
        public async Task<ActionResult> Details(int id)
        {
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: /Students/Create
        [Route("Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FirstName,MiddleName,LastName")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: /Students/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: /Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FirstName,MiddleName,LastName")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: /Students/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: /Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Student student = await db.Students.FindAsync(id);
            db.Students.Remove(student);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
