using EvalLab.API.Data;

namespace EvalLab.API.Common;

record PageDto<T>(int PageNumber, int PageSize, int TotalPages, long TotalItems, T[] Items)
{
  public static PageDto<TTarget> FromPage<TSource, TTarget>(
        Page<TSource> page,
        Func<TSource, TTarget> converter)
  {
    return new PageDto<TTarget>(
      page.PageNumber,
      page.PageSize,
      page.TotalPages,
      page.TotalItems,
      [.. page.Items.Select(converter)]
    );
  }
}
