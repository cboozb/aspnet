ODataPagingSample
----------------

This sample illustrates server-driven paging using OData. In particular, it stores a table
of books in a SQL Server Compact database and exposes the table as an OData entity set using a
WebAPI controller that derives from EntitySetController. The controller uses Entity Framework
as an object relational mapper and the client uses datajs to make OData requests in JavaScript
and Knockout for refreshing the page elements.

The server controls the page size by using the ResultLimit property on the [Queryable] attribute.
Modifying this value allows the server to change the page size for the client without requiring
any changes to the client itself.
 
This sample is provided as part of the ASP.NET Web Stack sample repository at
http://aspnet.codeplex.com/

For more information about the samples, please see
http://go.microsoft.com/fwlink/?LinkId=261487