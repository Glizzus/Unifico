using Unifico.Core.Strategy;

namespace Unifico.Core.Plugins;

/// <summary>
///     A <see cref="IStrategy" /> that can be loaded from a plugin.
/// </summary>
public interface IStrategyPlugin : IStrategy
{
    // This is a marker interface.
}