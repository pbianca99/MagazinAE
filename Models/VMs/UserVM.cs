using MagazinAE.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MagazinAE.Models.VMs
{
    public class UserVM
    {
        public UserVM()
        {
            Username = string.Empty;
            Password = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(256)]
        public string Username { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(256)]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(256)]
        public string LastName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(256)]
        public string FirstName { get; set; }

        public DateTime? LastLogin { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(256)]
        public string ConfirmPassword { get; set; }

        public static User VMUserToUser(UserVM vm)
        {
            var user = new User();

            user.FirstName = vm.FirstName;
            user.Username = vm.Username;
            user.Password = vm.Password;
            user.LastName = vm.LastName;
       
            return user;
        }

        public UserVM UserToUserVM(User? user)
        {
            if (user == null)
                return new UserVM();

            var vm = new UserVM();

            vm.Id = user.Id;
            vm.Username = user.Username;
            vm.Password = user.Password;
            vm.FirstName = user.FirstName;
            vm.LastName = user.LastName;
          
            
            return vm;
        }

    }
}
