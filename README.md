# Amekhail's EventBus System for Unity
## Overview
The `EventBus` system provides a simple mechanism for managing and dispatching events in Unity. Is uses `EventBinding` 
to link events to their subscribers, making it easy to handle parameterized and no-argument callbacks efficiently.

This lightweight and extensible solution is particularly useful for reducing dependencies between components and 
promoting a clean observer pattern in your Unity Projects.

## Features
- **Type-Safe Events**: Allows parameterized events with strict type checking.
- **No-Argument Callbacks**: Supports both parameterized (`Action<T>`) and no-argument (`Action`) event handling.
- **Operator Overloads**: Add and remove event bindings using `+` and `-` operators.
- **Centralized Event Management**: Simplified interface to handle event subscriptions.

## Getting Started
To begin, import the EventBus package into your Unity Project.

### 1. Create an Event Type
An event type must inherit from the `IEvent` interface. This ensures enforceable type safety in your bindings.
```csharp
public interface IEvent {}
```
An example of a custom event:
```csharp
public struct HealthChangedEvent : IEvent
{
    public int playerHealth;
}
```
### 2. Create an Event Binding and Register Callbacks
The `EventBinding<T>` class enables you to register callbacks for specific events.\
Here is how you use it:
```csharp
public class Player : MonoBehaviour
{
    public int Health = 10;
    
    EventBinding<HealthChangedEvent> playerHealthEvent;
    
    private void OnEnable() 
    {
        // Like this:
        playerHealthEvent = new EventBinding<HealthChangedEvent>(HandleHealthEvent);
        // Or like this:
        // playerHealthEvent += HandleHealthEvent;
        
        // Then Explicitly register (or Subscribe) the event on the EventBus
        EventBus<HealthChangedEvent>.Register(HandleHealthEvent);
    }
    
    private void OnDisable() 
    {
        // Explicitily UnRegister the event from the EventBus
        EventBus<HealthChangedEvent>.UnRegister(HandleHealthEvent);
    }
    
    private void HandleHealthEvent(HealthChangedEvent e) 
    {
        Health = e.playerHealth;
        Debug.Log($"Event Recieved! current health = {this.Health}.");
    } 
}
```

### 3. Dispatching Events
Leverage the `EventBus` system to centralize event dispatching. Here's an example of how to dispatch events.

```csharp
 EventBus<HealthChangedEvent>.Raise(new HealthChangedEvent 
 {
     playerHealth = 100; 
 });
```
**Output:**
```
Event Recieved! current health = 100.
```

## Use Case Example
One use case for this would allow your UI to easily access and display player data like health
### Health Component
```csharp
using UnityEngine;

public class Health : MonoBehaviour
{
    public int CurrentHealth { get; private set; } = 100;

    public void ApplyDamage(int damage)
    {
        CurrentHealth -= damage;
        EventBus<HealthChangedEvent>.Raise(new HealthChangedEvent(CurrentHealth));
    }
}
```
### UI Listener
```csharp
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    private EventBinding<HealthChangedEvent> _healthChangedBinding;

    private void Awake()
    {
        _healthChangedBinding = new EventBinding<HealthChangedEvent>();
        _healthChangedBinding += (HealthChangedEvent e) =>
        {
            Debug.Log($"Updating UI. New Health: {e.NewHealth}");
        };

        EventBus<HealthChangedEvent>.Register(_healthChangedBinding);
    }

    private void OnDestroy()
    {
        EventBus<HealthChangedEvent>.UnRegister(_healthChangedBinding);
    }
}

```
When the `Health` component applies damage, all subscribed listeners (like the `HealthUI`) are automatically notified.

## Best Practices
1. **Unbind Callbacks When Done:** Always unbind an event to prevent memory leaks or invalid callbacks. 
Make sure you unbind before an object is destroyed.
2. **Avoid Anonymous Functions for persistent Subscriptions:** Use named methods or delegates for better readability 
and easier unbinding.
```csharp
// THIS IS BAD FOR UNBINDING!
EventBus<HealthChangedEvent>.Register((e) => Debug.Log(e.playerHealth)); 
// You can't easily unregister this specific anonymous function later!
```

## Advanced Use Cases
### Dynamic Event Subscriptions
Dynamically bind and unbind events based on run time needs.
```csharp
EventBinding<PlayerJoinedEvent> playerJoinedBinding = new EventBinding<PlayerJoinedEvent();
playerJoinedBinding += OnPlayerJoined;
EventBus<PlayerJoinedEvent>.Register(playerJoinedBinding);

private void OnPlayerJoined(PlayerJoinedEvent e) 
{
    Debug.Log($"Player {e.PlayerName} Has Joined The Match!");
}
```


