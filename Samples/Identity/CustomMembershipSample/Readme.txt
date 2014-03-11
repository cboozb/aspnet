**** Custom simple membership application ****

This is a sample application that demonstrates hooking in ASP.NET Identity in an existing application that used a custom membership provider implementation for user and role management.

1. Initial application
The application has a model for users that is created using database first approach. We first create the user table with the columns to hold the desired user information. For sake of simplicity the model used in this sample is not too complex. 
- Create a database AppDb.mdf in the App_data folder of the application
- Create a AppUsers and Addresses table to hold the user and address information
- Add a ADO.NET model in the application created from the above database

Now we create custom simple membership provider that creates users and stores them in the AppUser table. We also have a custom implementation used for password hashing before storing it to the database. 
- Create CustomProvider that extends MembershipProvider and implement the necessary methods.
- Configure this membership provider in the web.config

Result
- Registering and logging the user should use the CustomProvider and store the information in the AppUsers table