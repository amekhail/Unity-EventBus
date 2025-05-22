using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Amekhail.EventBus
{
    /// <summary>
    /// Utility class for initializing, managing, and clearing event buses at runtime and in the Unity Editor.
    /// </summary>
    public static class EventBusUtil
    {
        /// <summary>
        /// Gets or sets the list of types that implement <see cref="IEvent"/>.
        /// </summary>
        public static IReadOnlyList<Type> EventTypes { get; set; }

        /// <summary>
        /// Gets or sets the list of instantiated generic EventBus types (e.g., <c>EventBus&lt;MyEvent&gt;</c>).
        /// </summary>
        public static IReadOnlyList<Type> EventBusTypes { get; set; }

#if UNITY_EDITOR
        /// <summary>
        /// Gets or sets the current play mode state in the Unity Editor.
        /// </summary>
        public static PlayModeStateChange PlayModeState { get; set; }

        /// <summary>
        /// Automatically called by Unity Editor when the editor loads. Subscribes to play mode change events.
        /// </summary>
        [InitializeOnLoadMethod]
        public static void InitializeEditor()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// Called when the Unity Editor play mode state changes. Clears all event buses when exiting play mode.
        /// </summary>
        /// <param name="playModeStateChange">The new play mode state.</param>
        private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            PlayModeState = playModeStateChange;
            if (playModeStateChange == PlayModeStateChange.ExitingPlayMode)
            {
                ClearAllBusses();
            }
        }
#endif

        /// <summary>
        /// Initializes the event bus system before the first scene loads at runtime.
        /// Discovers all event types and sets up corresponding event buses.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            EventTypes = PredefinedAssemblyUtil.GetTypes(typeof(IEvent));
            EventBusTypes = InitializeAllBusses();
        }

        /// <summary>
        /// Constructs all generic EventBus types based on discovered event types.
        /// </summary>
        /// <returns>A list of all constructed EventBus types.</returns>
        private static IReadOnlyList<Type> InitializeAllBusses()
        {
            List<Type> eventBusTypes = new List<Type>();

            var typedef = typeof(EventBus<>);
            foreach (var eventType in EventTypes)
            {
                var busType = typedef.MakeGenericType(eventType);
                eventBusTypes.Add(busType);
            }

            return eventBusTypes;
        }

        /// <summary>
        /// Calls the private <c>Clear</c> method on all discovered EventBus types to reset them.
        /// </summary>
        public static void ClearAllBusses()
        {
            for (int i = 0; i < EventBusTypes.Count; i++)
            {
                var busType = EventBusTypes[i];
                var clearMethod = busType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic);
                clearMethod.Invoke(null, null);
            }
        }
    }
}