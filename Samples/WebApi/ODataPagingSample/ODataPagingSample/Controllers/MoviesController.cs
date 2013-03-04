using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using ODataPagingSample.Models;

namespace ODataPagingSample.Controllers
{
    // An implementation of EntitySetController for exposing the Movies entity set using Entity Framework
    // The only action that's needed for this sample is Get(), but other methods are implemented as a demonstration
    public class MoviesController : EntitySetController<Movie, int>
    {
        MoviesDb _db = new MoviesDb();

        // The [Queryable] attribute allows this entity set to be queried using the OData syntax
        // The PageSize controls the maximum page size the server will send back to the client
        // Change the PageSize value to control the number of movies that show up on each page
        [Queryable(PageSize=10)]
        public override IQueryable<Movie> Get()
        {
            return _db.Movies;
        }

        protected override int GetKey(Movie entity)
        {
            return entity.ID;
        }

        protected override Movie GetEntityByKey(int key)
        {
            return _db.Movies.Find(key);
        }

        protected override Movie CreateEntity(Movie entity)
        {
            Movie createdEntity = _db.Movies.Add(entity);
            _db.SaveChanges();
            return createdEntity;
        }

        protected override Movie UpdateEntity(int key, Movie update)
        {
            if (GetEntityByKey(key) == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            if (key != update.ID)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid update: ID does not match."));
            }

            Movie updatedEntity = _db.Movies.Attach(update);
            _db.Entry(update).State = EntityState.Modified;
            _db.SaveChanges();
            return updatedEntity;
        }

        protected override Movie PatchEntity(int key, Delta<Movie> patch)
        {
            Movie entityToPatch = GetEntityByKey(key);
            if (entityToPatch == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            patch.Patch(entityToPatch);
            entityToPatch.ID = key;
            _db.Entry(entityToPatch).State = EntityState.Modified;
            _db.SaveChanges();
            return entityToPatch;
        }

        public override void Delete([FromODataUri] int key)
        {
            Movie entityToDelete = GetEntityByKey(key);
            if (entityToDelete == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _db.Movies.Remove(entityToDelete);
            _db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}