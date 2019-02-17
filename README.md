# More Gizmos

A proof-of-concept-quality library for Unity 2017+ that expands the number of
deferred debug gizmos that can be drawn.

Here's what you'd have available with Unity's offerings and this library:

- UnityEngine.Debug
  - DrawLine
  - DrawRay
- MoreGizmos.GizmosEx
  - DrawSphere
  - DrawCube
  - DrawCircle
  - DrawSquare
  - DrawCustomGizmo(GizmoDraw)

These are intended to be only drawn during run-time by collecting and deferring
the drawing of said debug gizmos to an instance of `GizmosEx` which in turn
renders them in its own `OnDrawGizmos` message.

Custom gizmos types can be derived from `GizmoDraw` and implement the required
methods that will be called to prepare and render the gizmo.

## Quick Usage

Attach an instance of `GizmosEx` to a GameObject in the scene. After doing so,
try adding something to the effect of the following code:

```c#
public class ExampleUsage : MonoBehaviour
{
  private void Update()
  {
    // draws a RED CUBE at 0,0,0, with a scale of ONE for each dimension
    GizmosEx.DrawCube(Vector3.zero, Vector3.one, Color.red);
  }
}
```

## Roadmap

- [ ] Support different gizmo rendering backends (i.e. don't depend on Unity's gizmo message)
  - [ ] Provide base type to allow custom rendering backends API (e.g. `IGizmoDrawer`)
- [ ] Optimize for memory and reduce garbage generated
- [ ] Add persistent draw calls that stay for _x_ seconds
- [ ] Add support for rendering at run-time
- [ ] Add support for drawing lines and gizmos
- [ ] Automatically strip out gizmos in development builds
  - [ ] Add special case for gizmos available at run-time (like `Trace` gizmos)
- [ ] Add categories/labels to support filtering / selective rendering
- [ ] Add gizmo for screen-space text / world-space text
- [ ] Publish packages
  - [ ] "*.unitypackage"-style pages
  - [ ] "Unity Package Manager"-style packages

## License

Copyright (c) 2019 Terry Nguyen - MIT License

More information can be found by reading the [LICENSE](LICENSE.md) file in this
repository.