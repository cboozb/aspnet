using ODataActionsSample.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;

namespace ODataActionsSample.Controllers
{
    public class MoviesController : EntitySetController<Movie, int>
    {
        private MoviesContext db = new MoviesContext();

        private bool TryCheckoutMovie(Movie movie)
        {
            if (movie.IsCheckedOut)
            {
                return false;
            }
            else
            {
                // To check out a movie, set the due date.
                movie.DueDate = DateTime.Now.AddDays(7);
                return true;
            }
        }

        public override IQueryable<Movie> Get()
        {
            return db.Movies;
        }

        protected override Movie GetEntityByKey(int key)
        {
            return db.Movies.Find(key);
        }

        #region Action methods

        [HttpPost]
        public Movie CheckOut([FromODataUri] int key)
        {
            var movie = GetEntityByKey(key);
            if (movie == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            if (!TryCheckoutMovie(movie))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            return movie;
        }

        [HttpPost]
        public Movie Return([FromODataUri] int key)
        {
            var movie = GetEntityByKey(key);
            if (movie == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            movie.DueDate = null;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            return movie;
        }

        [HttpPost]
        public ICollection<Movie> CheckOutMany(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // Client passes a list of movie IDs to check out.
            var movieIDs = parameters["Movies"] as ICollection<int>;

            var results = new List<Movie>();
            foreach (Movie movie in db.Movies.Where(m => movieIDs.Contains(m.ID)))
            {
                if (TryCheckoutMovie(movie))
                {
                    results.Add(movie);                    
                }
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // Return a list of the movies that were checked out.
            return results;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
