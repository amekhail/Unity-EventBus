using System;
using System.Collections.Generic;
using UnityEngine;

namespace Amekhail.EventBus 
{
    /// <summary>
    /// A static event bus for broadcasting and subscribing to events of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The event type. Must implement <see cref="IEvent"/>.</typeparam>
    public static class EventBus<T> where T : IEvent
    {
        /// <summary>
        /// Stores all registered event bindings for the event type <typeparamref name="T"/>.
        /// </summary>
        private static readonly HashSet<IEventBinding<T>> _bindings = new HashSet<IEventBinding<T>>();
        
        /// <summary>
        /// Registers an <see cref="EventBinding{T}"/> to receive events of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="binding">The event binding to register.</param>
        public static void Register(EventBinding<T> binding) => _bindings.Add(binding);

        /// <summary>
        /// Unregisters an <see cref="EventBinding{T}"/> from the event bus.
        /// </summary>
        /// <param name="binding">The event binding to remove.</param>
        public static void Unregister(EventBinding<T> binding) => _bindings.Remove(binding);

        /// <summary>
        /// Raises an event of type <typeparamref name="T"/> to all registered bindings.
        /// </summary>
        /// <param name="e">The event instance to broadcast.</param>
        public static void Raise(T e)
        {
            // Take a snapshot to prevent errors if bindings are modified during iteration.
            var snapshot = new HashSet<IEventBinding<T>>(_bindings);

            foreach (var binding in snapshot)
            {
                binding.OnEvent.Invoke(e);
                binding.OnEventNoArgs.Invoke();
            }
        }

        /// <summary>
        /// Clears all registered event bindings.
        /// </summary>
        /// <remarks>This method is private and currently not accessible from outside the class.</remarks>
        private static void Clear()
        {
            _bindings.Clear();
        }
    }
}