using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Documentor.Core.Enums;

public enum TemplateType
{
    [Display(Name = "Заявление")]
    Statement,
    
    [Display(Name = "Договор")]
    Contract
}