namespace Cantus.Models
{
    // Models/User.cs
    public class User
    {
        public int Id { get; set; }
        public string Auth0Id { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
    }

}
