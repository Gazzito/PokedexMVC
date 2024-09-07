namespace PokedexMVC.Models
{
    public class AssignRoleViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public List<string> AvailableRoles { get; set; } = new List<string>();
        public List<string> AssignedRoles { get; set; } = new List<string>();
    }
}