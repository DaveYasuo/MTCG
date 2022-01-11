namespace ServerModule.Responses
{
    public enum Status
    {
        Ok = 200,
        Created = 201,
        NoContent = 204,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        Conflict = 409,
        InternalServerError = 500,
        NotImplemented = 501,
        HttpVersionNotSupported = 505
    }
}