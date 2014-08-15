Use the migration sample in scenario:
You want to migrate a MVC4 project with simple membership settings to use identity API,
AND you want to upgrade the database scheme to identity scheme.

To view demo, follow the following steps:

1. Create a new MVC4 application. Run the application and register new user.
2. Run SimpleMembershipToIdentityMigration.sql script on the simple membership database
3. Install Microsoft.AspNet.Identity.EntityFramework 1.0.0 package
4. Copy Models/IdentityModels.cs to MVC4 project Models folder
5. Copy Controllers/AccountController.cs to override MVC4 project controller

After the migration, you are able to use default identity EF models and UserManager API 
to manage your memebership database.