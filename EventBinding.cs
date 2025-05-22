using System;

namespace Amekhail.EventBus
{
    /// <summary>
    /// Internal interface that represents an event binding for events of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The event type implementing <see cref="IEvent"/>.</typeparam>
    internal interface IEventBinding<T>
    {
        /// <summary>
        /// Event callback that accepts the event instance as a parameter.
        /// </summary>
        Action<T> OnEvent { get; set; }

        /// <summary>
        /// Event callback with no parameters.
        /// </summary>
        Action OnEventNoArgs { get; set; }
    }

    /// <summary>
    /// Represents a binding to a specific event of type <typeparamref name="T"/>.
    /// Used by the <see cref="EventBus{T}"/> to dispatch events to registered listeners.
    /// </summary>
    /// <typeparam name="T">The event type implementing <see cref="IEvent"/>.</typeparam>
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> _onEvent = _ => { };
        private Action _onEventNoArgs = () => { };

        /// <inheritdoc/>
        Action<T> IEventBinding<T>.OnEvent
        {
            get => _onEvent;
            set => _onEvent = value;
        }

        /// <inheritdoc/>
        Action IEventBinding<T>.OnEventNoArgs
        {
            get => _onEventNoArgs;
            set => _onEventNoArgs = value;
        }

        /// <summary>
        /// Creates an event binding with a parameterized event callback.
        /// </summary>
        /// <param name="onEvent">The callback that receives the event instance.</param>
        public EventBinding(Action<T> onEvent) => _onEvent = onEvent;

        /// <summary>
        /// Creates an event binding with a no-argument event callback.
        /// </summary>
        /// <param name="onEventNoArgs">The callback to be invoked when the event is raised, without event data.</param>
        public EventBinding(Action onEventNoArgs) => _onEventNoArgs = onEventNoArgs;

        /// <summary>
        /// Adds a no-argument callback to the binding.
        /// </summary>
        /// <param name="onEvent">The callback to add.</param>
        public void Add(Action onEvent) => _onEventNoArgs += onEvent;

        /// <summary>
        /// Removes a no-argument callback from the binding.
        /// </summary>
        /// <param name="onEvent">The callback to remove.</param>
        public void Remove(Action onEvent) => _onEventNoArgs -= onEvent;

        /// <summary>
        /// Adds a callback that takes the event as a parameter to the binding.
        /// </summary>
        /// <param name="onEvent">The callback to add.</param>
        public void Add(Action<T> onEvent) => _onEvent += onEvent;

        /// <summary>
        /// Removes a parameterized callback from the binding.
        /// </summary>
        /// <param name="onEvent">The callback to remove.</param>
        public void Remove(Action<T> onEvent) => _onEvent -= onEvent;

        /// <summary>
        /// Adds a parameterized callback to the event binding using the '+' operator.
        /// </summary>
        /// <param name="binding">The event binding to modify.</param>
        /// <param name="callback">The callback to add to the binding.</param>
        /// <returns>The updated event binding with the added callback.</returns>
        public static EventBinding<T> operator +(EventBinding<T> binding, Action<T> callback)
        {
            binding.Add(callback);
            return binding;
        }

        /// <summary>
        /// Removes a parameterized callback from the event binding using the '-' operator.
        /// </summary>
        /// <param name="binding">The event binding to modify.</param>
        /// <param name="callback">The callback to remove from the binding.</param>
        /// <returns>The updated event binding with the callback removed.</returns>
        public static EventBinding<T> operator -(EventBinding<T> binding, Action<T> callback)
        {
            binding.Remove(callback);
            return binding;
        }

        /// <summary>
        /// Adds a no-argument callback to the event binding using the '+' operator.
        /// </summary>
        /// <param name="binding">The event binding to modify.</param>
        /// <param name="callback">The callback to add to the binding.</param>
        /// <returns>The updated event binding with the added callback.</returns>
        public static EventBinding<T> operator +(EventBinding<T> binding, Action callback)
        {
            binding.Add(callback);
            return binding;
        }

        /// <summary>
        /// Removes a no-argument callback from the event binding using the '-' operator.
        /// </summary>
        /// <param name="binding">The event binding to modify.</param>
        /// <param name="callback">The callback to remove from the binding.</param>
        /// <returns>The updated event binding with the callback removed.</returns>
        public static EventBinding<T> operator -(EventBinding<T> binding, Action callback)
        {
            binding.Remove(callback);
            return binding;
        }
    }
}