namespace Cantus.Models
{
    public class CantusUser
    {
        public int Id { get; set; }
        public string Auth0Id { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string Username { get; set; }
    }
}
