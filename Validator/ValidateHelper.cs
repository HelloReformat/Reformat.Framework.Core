using System.ComponentModel.DataAnnotations;
using Reformat.Framework.Core.Exceptions;
using Reformat.Framework.Core.Validator.Domains;

namespace Reformat.Framework.Core.Validator;

/// <summary>
/// 验证辅助
/// 用于参数传递时进行参数校验
/// </summary>
public static class ValidateHelper
{
    /// <summary>
    /// DTO 模型校验
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void ValidateData(this object value)
    {
        var result = new ValidResult();
        try
        {
            var validationContext = new ValidationContext(value);
            var results = new List<ValidationResult>();
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(value, validationContext, results, true);
            result.IsVaild = isValid;
 
            if (!isValid)
            {
                foreach (ValidationResult? item in results)
                {
                    result.ErrorMembers.Add(new ErrorMember()
                    {
                        ErrorMessage = item.ErrorMessage,
                        ErrorMemberName = item.MemberNames.FirstOrDefault()
                    });
                }
                throw new ValidateException(result);
            }
        }
        catch (ValidationException ex)
        {
            result.IsVaild = false;
            result.ErrorMembers = new List<ErrorMember>
            {
                new ErrorMember()
                {
                    ErrorMessage = ex.Message,
                    ErrorMemberName = "Internal error"
                }
            };
            throw new ValidateException(result);
        }
    }
}