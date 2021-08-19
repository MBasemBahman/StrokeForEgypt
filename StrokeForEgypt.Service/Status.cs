namespace StrokeForEgypt.Service
{
    public class Status
    {
        public Status()
        {
        }

        public Status(bool Success)
        {
            this.Success = Success;
            ErrorMessage = "";
        }

        public bool Success { get; private set; } = false;

        public string ErrorMessage { get; set; } = "Error Message!";

        public string ExceptionMessage { get; set; } = "";
    }
}
