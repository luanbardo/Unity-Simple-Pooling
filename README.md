# Unity Simple Pooling

**Unity Simple Pooling** is a tiny, no-friction object pooling helper for Unity.

It gives you a single static API to spawn and despawn prefabs without setting up a full pooling framework.
No editor windows, no custom inspectors, just `Spawn` and `Despawn`.

---

## Installation

### Option 1: Unity Package (recommended)

1. Download the `.unitypackage` from the Releases page
2. Double-click it or drag it into Unity
3. Click Import

Done.

### Option 2: Manual

Copy the `SimplePoolingSystem` folder into your Unity project.

---

## Why this exists

Unity's built-in instantiation is fine for small projects, but repeated `Instantiate` / `Destroy` calls can:
- create spikes (GC and CPU)
- make simple VFX feel laggy
- add boilerplate everywhere

This system gives you a tiny, consistent API for pooling in small or medium projects.

---

## What problem does this solve?

Common cases:
- "Spawn 20 bullets per second without GC spikes."
- "Play particle-like VFX that I want to reuse."
- "Keep my code simple without a full pooling framework."

It pools by prefab, keeps inactive instances, and reuses them when needed.

---

## What's included?

Three simple scripts:
- `SimplePooling` (static API)
- `SimplePool` (internal pool implementation)
- `SimplePooledInstance` (marker component automatically added at runtime)

---

## How to use

### Basic spawn

```csharp
using SimplePoolingSystem;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    private void Fire()
    {
        SimplePooling.Spawn(bulletPrefab, transform.position, transform.rotation);
    }
}
```

### Despawn

```csharp
SimplePooling.Despawn(bulletInstance);
```

### Spawn with parent

```csharp
SimplePooling.Spawn(effectPrefab, parentTransform);
```

### Component-friendly spawn

```csharp
private Projectile bulletPrefab;

private void Fire()
{
    Projectile instance = SimplePooling.Spawn(bulletPrefab, transform.position, transform.rotation);
}
```

---

## Optional callbacks

If your pooled objects implement these methods, they will be invoked automatically:

- `OnSpawned`
- `OnDespawned`

This uses `SendMessage`, so the methods are optional.

---

## Limitations

- No pre-warm or max-size control
- Uses `SendMessage` (string-based)
- Not thread-safe
- If you call `Destroy` on an instance manually, it is removed from the pool (it will not be reused)

---

## Requirements

- Unity 2021 or newer
- No external packages

---

## License

MIT License  
Use it however you want.

---

## Final note

This tool is intentionally simple.
If you need pooling analytics, async preloading, or complex rules, a larger framework may be a better fit.
