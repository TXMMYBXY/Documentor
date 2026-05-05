using System.Text.Json.Serialization;
using Documentor.Application.Api.Models;

namespace Documentor.Application.Api.Admin.Dtos.User;

public class EmployeeDto
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    [JsonPropertyName("role")]
    public Role RoleEntity { get; set; }
    
    public string Role
    {
        get
        {
            switch (RoleEntity.Title)
            {
                case "Admin":
                    return "Администратор";
                case "Boss":
                    return "Начальник закупок";
                case "Purchaser":
                    return "Сотрудник закупок";
                case "Employee":
                    return "Сотрудник";
                
                default:
                    return "Неизвестная роль";
            }
        }
    }
}