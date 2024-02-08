namespace ApplicationCore.Business.Dtos;

public class ContactPreferenceDto
{
    public int Id { get; set; }
    public string PreferredContactMethod { get; set; } = null!;
}
