using Microsoft.Owin.StaticFiles;
using Owin;

namespace StaticFiles
{
    // Note that in the Web.Config the native IIS static file module has been disabled.
    public class Startup
    {
        // Invoked once at startup to configure your application.
        public void Configuration(IAppBuilder builder)
        {
            // Allow directory browsing from a specific dir.
            builder.UseFileServer(options =>
            {
                options.WithRequestPath("/browse");
                options.WithPhysicalPath("public");
                options.WithDirectoryBrowsing();
            });

            // Allow file access to a specific dir.
            builder.UseFileServer(options =>
            {
                options.WithRequestPath("/non-browse");
                options.WithPhysicalPath("protected");
            });

            // Serve default files out of the specified directory for the root url.
            builder.UseFileServer(options =>
            {
                options.WithRequestPath("/");
                options.WithPhysicalPath("protected");
                // options.WithDefaultFileNames(new[] { "index.html" }); // Already included by default.
            });

            builder.UseType<ServeSpecificPage>(@"protected\CustomErrorPage.html");
        }
    }
}