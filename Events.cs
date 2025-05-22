namespace Amekhail.EventBus
{
    /// <summary>
    /// Marker interface to group or identify multiple event types as a category of events.
    /// Inherits from <see cref="IEvent"/>.
    /// </summary>
    public interface Events : IEvent {}

    /// <summary>
    /// A simple test event used to demonstrate event dispatching.
    ///
    /// Consider implementing <see cref="IEvent"/> as a <c>struct</c> for performance reasons:
    /// - Structs are value types, typically allocated on the stack, reducing GC pressure.
    /// - Ideal for small, short-lived events used in high-frequency systems like event buses.
    ///
    /// Note: Avoid boxing and ensure structs remain small and immutable for optimal performance.
    /// </summary>
    public struct TestEvent : IEvent {}
}