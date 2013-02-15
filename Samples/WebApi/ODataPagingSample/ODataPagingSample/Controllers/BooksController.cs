using System;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;
using ODataPagingSample.Models;

namespace ODataPagingSample.Controllers
{
    // An implementation of EntitySetController for exposing the Books entity set using Entity Framework
    // The only action that's needed for this sample is Get(), but other methods are implemented as a demonstration
    public class BooksController : EntitySetController<Book, string>
    {
        BooksDbEntities _db = new BooksDbEntities();

        // The [Queryable] attribute allows this entity set to be queried using the OData syntax
        // The ResultLimit controls the maximum page size the server will send back to the client
        // Change the ResultLimit value to control the number of books that show up on each page
        [Queryable(ResultLimit=10)]
        public override IQueryable<Book> Get()
        {
            return _db.Books;
        }

        protected override string GetKey(Book entity)
        {
            return entity.ISBN;
        }

        protected override Book GetEntityByKey(string key)
        {
            return _db.Books.Where(book => book.ISBN == key).SingleOrDefault();
        }

        protected override Book CreateEntity(Book entity)
        {
            Book createdEntity = _db.Books.Add(entity);
            _db.SaveChanges();
            return createdEntity;
        }

        protected override Book UpdateEntity(string key, Book update)
        {
            if (GetEntityByKey(key) == null)
            {
                throw new InvalidOperationException("Invalid update: book does not exist.");
            }

            if (key != update.ISBN)
            {
                throw new InvalidOperationException("Invalid update: ISBN does not match.");
            }

            Book updatedEntity = _db.Books.Attach(update);
            _db.Entry(update).State = EntityState.Modified;
            _db.SaveChanges();
            return updatedEntity;
        }

        protected override Book PatchEntity(string key, Delta<Book> patch)
        {
            Book entityToPatch = GetEntityByKey(key);
            if (entityToPatch == null)
            {
                throw new InvalidOperationException("Invalid update: book does not exist.");
            }

            object patchISBN;
            if (patch.TryGetPropertyValue("ISBN", out patchISBN) && patchISBN != key)
            {
                throw new InvalidOperationException("Invalid patch: ISBN does not match.");
            }

            patch.Patch(entityToPatch);
            _db.Entry(entityToPatch).State = EntityState.Modified;
            _db.SaveChanges();
            return entityToPatch;
        }

        protected override void DeleteEntity(string key)
        {
            Book entityToDelete = GetEntityByKey(key);
            if (entityToDelete == null)
            {
                throw new InvalidOperationException("Invalid delete: book does not exist.");
            }

            _db.Books.Remove(entityToDelete);
            _db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
        }
    }
}