using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace GermanWhistWebPage.Models
{
    public class HumanPlayer : Player
    {
        [ForeignKey("IdentityUserId")]
        public string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; }
        public override string Name  => IdentityUser.UserName;

    }
}
