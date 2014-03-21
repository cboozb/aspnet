namespace ODataService
{
    /// <summary>
    /// A set of useful OData related errors.
    /// </summary>
    public static class ODataErrors
    {
        public static object EntityNotFound()
        {
            return new
            {
                Message = "The entity was not found.",
                ErrorCode = "Entity Not Found."
            };
        }

        public static object DeletingLinkNotSupported(string navigation)
        {
            return new
            {
                Message = string.Format("Deleting a '{0}' link is not supported.", navigation),
                MessageLanguage = "en-US",
                ErrorCode = "Deleting link failed."
            };
        }

        public static object CreatingLinkNotSupported(string navigation)
        {
            return new
            {
                Message = string.Format("Creating a '{0}' link is not supported.", navigation),
                MessageLanguage = "en-US",
                ErrorCode = "Creating link failed."
            };
        }
    }
}
