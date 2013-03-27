using ODataActionsSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;

namespace ODataActionsSample.Controllers
{
    // Controller for handling non-bindable actions.
    [ODataFormatting]
    public class NonBindableActionsController : ApiController
    {
        MoviesContext db = new MoviesContext();

        [HttpPost]
        public Movie CreateMovie(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            string title = parameters["Title"] as string;

            Movie movie = new Movie()
            {
                Title = title
            };

            db.Movies.Add(movie);
            db.SaveChanges();
            return movie;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}