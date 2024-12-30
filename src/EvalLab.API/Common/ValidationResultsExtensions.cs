using System.ComponentModel.DataAnnotations;

namespace EvalLab.API.Common;

static class ValidationResultsExtensions
{
  public static Dictionary<string, string[]> ToErrors(this List<ValidationResult> results) => results.ToDictionary(r => r.MemberNames.First(), r => new[] { r.ErrorMessage! });
}