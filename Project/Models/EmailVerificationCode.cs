namespace Project.Models
{
    public class EmailVerificationCode
    {
        public int Id { get; set; }

        

        public string Code { get; set; }

        public DateTime ExpireAt { get; set; }

        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }
        public AspNetUser User { get; set; }
    }
}
