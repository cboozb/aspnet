using ODataActionsSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Routing;
using System.Web.Http.OData.Routing.Conventions;

namespace ODataActionsSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            ODataModelBuilder modelBuilder = new ODataConventionModelBuilder();
            var moviesEntitySet = modelBuilder.EntitySet<Movie>("Movies");
            moviesEntitySet.EntityType.Ignore(m => m.TimeStamp);    // Don't expose timestamp to clients

            // CheckOut is transient action, because it is not available when the item is already checked out.
            ActionConfiguration checkout = modelBuilder.Entity<Movie>().TransientAction("CheckOut");

            // Provide a function that returns a link to the action, when the action is available, or
            // returns null when the action is not available.
            checkout.HasActionLink(ctx =>
            {
                Movie movie = ctx.EntityInstance as Movie;

                // Note: In some cases, checking whether the action is available may be a relatively expensive 
                // operation. You should avoid performing expensive checks in loops (i.e., when serializing a
                // feed). Instead, simply mark the action as available by including the action link. 

                // The SkipExpensiveAvailabilityChecks is a flag that says whether to skip expensive checks.
                // If this flag is true AND your availability check is true, then you should skip the check.

                // In this sample, the check is not really expensive, but we honor the flag to show how it works.
                
                if (ctx.SkipExpensiveAvailabilityChecks || movie.IsCheckedOut == false)
                {
                    return new Uri(ctx.Url.ODataLink(
                        new EntitySetPathSegment(ctx.EntitySet),
                        new KeyValuePathSegment(movie.ID.ToString()),
                        new ActionPathSegment(checkout.Name)));
                }
                else
                {
                    // The movie is checked out, so do not return an action link.
                    return null;
                }
            }, followsConventions: true);   // "followsConventions" means the action follows OData conventions.
            checkout.ReturnsFromEntitySet<Movie>("Movies");

            // ReturnMovie is always bindable. If the movie is not checked out, the action is a no-op.
            var returnAction = modelBuilder.Entity<Movie>().Action("Return");
            returnAction.ReturnsFromEntitySet<Movie>("Movies");

            // CheckOutMany is bound to a collection, instead of a single entity.
            // Also, it takes a collection parameter and returns a collection.
            ActionConfiguration checkoutMany = modelBuilder.Entity<Movie>().Collection.Action("CheckOutMany");
            checkoutMany.CollectionParameter<int>("Movies");
            checkoutMany.ReturnsCollectionFromEntitySet<Movie>("Movies");

            // CreateMovie is a non-bindable action. You invoke it from the service root: ~/odata/CreateMovie
            ActionConfiguration createMovie = modelBuilder.Action("CreateMovie");
            createMovie.Parameter<string>("Title");
            createMovie.ReturnsFromEntitySet<Movie>("Movies");

            // Add a custom route convention for non-bindable actions.
            // (Web API does not have a built-in routing convention for non-bindable actions.)
            IList<IODataRoutingConvention> conventions = ODataRoutingConventions.CreateDefault();
            conventions.Insert(0, new NonBindableActionRoutingConvention("NonBindableActions"));

            // Map the OData route.
            Microsoft.Data.Edm.IEdmModel model = modelBuilder.GetEdmModel();
            config.Routes.MapODataRoute("ODataRoute", "odata", model, new DefaultODataPathHandler(), conventions);
        }
    }
}
