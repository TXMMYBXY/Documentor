using System.Text.Json.Serialization;

namespace DocumentFlowing.Client.Models;

public class User
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
    [JsonPropertyName("department")]
    public string Department { get; set; }
    [JsonPropertyName("role")]
    public Role RoleEntity { get; set; }
    [JsonPropertyName("id")]
    public int UserId { get; set; }
    public string RoleTitle => RoleEntity.Title;
}