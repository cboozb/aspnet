using Owin;
using Owin.Types;
using System.Collections.Generic;

namespace BranchingPipelines
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.UseType<AddBreadCrumbMiddleware>("start-of-the-line");

            builder.MapPath("/branch1", builder1 =>
            {
                builder1.UseType<AddBreadCrumbMiddleware>("took-branch1");

                // Nesting paths, e.g. /branch1/branch2
                builder1.MapPath("/branch2", builder2 =>
                {
                    builder2.UseType<AddBreadCrumbMiddleware>("took-branch2");
                    builder2.UseType<DisplayBreadCrumbs>();
                });

                MapIfIE(builder1);
                builder1.UseType<DisplayBreadCrumbs>();
            });

            // Only full segments are matched, so /branch1 does not match /branch100
            builder.MapPath("/branch100", builder1 =>
            {
                builder1.UseType<AddBreadCrumbMiddleware>("took-branch100");
                builder1.UseType<DisplayBreadCrumbs>();
            });

            MapIfIE(builder);

            builder.UseType<AddBreadCrumbMiddleware>("no-branches-taken");
            builder.UseType<DisplayBreadCrumbs>();
        }

        private void MapIfIE(IAppBuilder builder)
        {
            builder.MapPredicate(IsIE, builder2 =>
            {
                builder2.UseType<AddBreadCrumbMiddleware>("took-IE-branch");
                builder2.UseType<DisplayBreadCrumbs>();
            });
        }

        private bool IsIE(IDictionary<string, object> environment)
        {
            OwinRequest request = new OwinRequest(environment);
            return request.GetHeader("User-Agent").Contains("MSIE");
        }
    }
}