namespace api_intranet_surfland.Models; 
public class DTOUser {
    public int? UserId { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }
    public int? AccessProfileId { get; set; }
    public int? StatusId { get; set; }
    public string? TemporaryPassword { get; set; }
    public int? PersonId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}