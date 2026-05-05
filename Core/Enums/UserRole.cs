using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Documentor.Core.Enums;

public enum UserRole
{
    [Display(Name = "Администратор")]
    Admin = 1,
    
    [Display(Name = "Начальник отдела")]
    Boss = 2,
    
    [Display(Name = "Сотрудник закупок")]
    Purchaser = 3,
    
    [Display(Name = "Сотрудник")]
    User = 4
}
