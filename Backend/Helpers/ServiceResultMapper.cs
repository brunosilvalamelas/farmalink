using Backend.Services;

namespace Backend.Helpers;

/// <summary>
/// Provides methods for mapping service results to different types.
/// </summary>
public static class ServiceResultMapper
{
/// <summary>
/// Maps a ServiceResult of type T to a ServiceResult of type U using the provided mapping function.
/// </summary>
/// <typeparam name="T">The type of the data in the input ServiceResult.</typeparam>
/// <typeparam name="U">The type of the data in the output ServiceResult.</typeparam>
/// <param name="result">The input ServiceResult to map from.</param>
/// <param name="mapFunc">The function to apply to the data if it's not null.</param>
/// <returns>A new ServiceResult with the mapped data.</returns>
    public static ServiceResult<U> Map<T, U>(ServiceResult<T> result, Func<T, U> mapFunc)
    {
        return new ServiceResult<U>(
            result.ResultType,
            result.Message,
            result.Data != null ? mapFunc(result.Data) : default,
            result.Errors
        );
    }
}
