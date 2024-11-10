namespace BREADAPI.core
{
    
    public class NotFoundExeption: Exception 
    {
        public NotFoundExeption() { }
        public NotFoundExeption(string message): base(message) { }

    }

    public class BadRequestExeption : Exception
    {
        public BadRequestExeption() { }
        public BadRequestExeption(string message) : base(message) { }

    }

    public class ForbiddenExeption : Exception
    {
        public ForbiddenExeption() { }
        public ForbiddenExeption(string message) : base(message) { }

    }

}
