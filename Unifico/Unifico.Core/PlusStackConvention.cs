namespace Unifico.Core;

/// <summary>
///     Common conventions for how to handle plus stacks.
/// </summary>
public enum PlusStackConvention
{
    /// <summary>
    ///     Plus stacks are banned and never occur.
    ///     This is the official UNO ruling.
    /// </summary>
    Banned,

    /// <summary>
    ///     Plus Two's can only be stacked on Plus Two's and Plus Four's can be stacked on anything.
    /// </summary>
    Conservative,

    /// <summary>
    ///     Similar to <see cref="Conservative" />, but Plus Two's can be stacked on Plus Four's if the colors match.
    /// </summary>
    Liberal
}