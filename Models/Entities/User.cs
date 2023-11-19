using Azure.Identity;

namespace MagazinAE.Models.Entities
{
    public class User
    {
        public User()
        {
            Username = string.Empty;
            Password = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string LastName {  get; set; }
        public string FirstName { get; set; }
        public DateTime? LastLogin { get; set; }

        public List<User> GetAll(MagazinAEContext context)
        {
            return context.Users.ToList();
        }

    }
}
