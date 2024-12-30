namespace api_intranet_surfland.Models; 
public class DTOPerson {
    public int? PersonId { get; set; }
    public string? Name { get; set; }
    public string? Document { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool? Foreigner { get; set; }
}