namespace hrms.Dto.Response.Error
{
    public class AlreadyExists
    {
        public String message { get; set; }
        public AlreadyExists(string message) {
            this.message = message;
        }
    }
}
