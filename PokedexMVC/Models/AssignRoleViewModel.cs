namespace PokedexMVC.Models
{
    // ViewModel for assigning roles to a user.
    public class AssignRoleViewModel
    {
        // The ID of the user to whom roles are being assigned.
        public string UserId { get; set; }

        // The email of the user to whom roles are being assigned.
        public string UserEmail { get; set; }

        // List of roles available for assignment to the user.
        public List<string> AvailableRoles { get; set; } = new List<string>();

        // List of roles currently assigned to the user.
        public List<string> AssignedRoles { get; set; } = new List<string>();
    }
}
