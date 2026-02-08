namespace hrms.Dto.Response.Error
{
    public class AlreadyExists
    {
        public String Message { get; set; }
        public AlreadyExists(string Message) {
            this.Message = Message;
        }
    }
}
