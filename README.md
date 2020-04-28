# MoreGizmos

![A example of a square gizmo being drawn](https://user-images.githubusercontent.com/4968773/52925428-3a178e80-32e6-11e9-853c-1c1f64938636.gif)

A proof-of-concept-quality library for Unity 2017+ that expands the number of
debug gizmos that can be drawn.

Here's what you'd have available with Unity's offerings and this library:

- [UnityEngine.Gizmos](https://docs.unity3d.com/ScriptReference/Gizmos.html) - usable in OnDrawGizmos, editor-only
  - DrawCube
  - DrawFrustum
  - DrawGUITexture
  - DrawIcon
  - DrawLine
  - DrawMesh
  - DrawRay
  - DrawSphere
  - DrawWireCube
  - DrawWireMesh
  - DrawWireSphere
- [UnityEngine.Debug](https://docs.unity3d.com/ScriptReference/Debug.html) - usable anywhere, editor-only
  - DrawRay
  - DrawLine
- MoreGizmos.GizmosEx - usable anywhere, editor-only (for now)
  - DrawSphere
  - DrawCube
  - DrawCircle
  - DrawSquare
  - DrawCustomGizmo - accepts objects of type `GizmoDraw`

These are intended to be only drawn during editor-time by collecting and
delegating the drawing of said debug gizmos to an instance of `GizmosEx` which
in turn renders them in its own `OnDrawGizmos` message.

Custom gizmos types can be derived from `GizmoDraw` and implement the required
methods that will be called to prepare and render the gizmo.

## Installation

MoreGizmos is distributed as a package that can be managed through Unity's
Package Manager UI as of 2018.1.

### 2018.3+

To add this to an existing Unity project, open your project's `manifest.json`
file in your `Packages` directory and add the following line to your list of
dependencies:

```json
  "dependencies": {
    "com.tb.moregizmos": "https://github.com/terrehbyte/MoreGizmos.git",
  }
```

Note that the trailing comma should be omitted if you add it as the last element
in the list, of course. For more information or advanced usage, refer to the
Unity Manual for [specifying dependencies via Git URLs][unityManUPMGit].

[unityManUPMGit]:https://docs.unity3d.com/Manual/upm-git.html

### Pre-2018.3

Support for loading Git repositories as a package was only added in 2018.3, so
you'll need to resort to local file management if you're running something older.

![Screenshot of the "Add package from disk..." entry](https://user-images.githubusercontent.com/4968773/52925274-5a931900-32e5-11e9-990a-6c4dd1356260.png)

Unity allows you to add a package by providing a relative file path to a
package.json file. Best practices would advise that you place it somewhere
co-located with your Unity project (and versioned, of course).

### No Package Manager

Alternatively, you may choose to download this project as a zip (or submodule if
you're using Git) whose contents can be placed in the `Assets` directory of
your project. The end-result should be: `ProjectRoot/Packages/<repo-contents>`
as a built-in package.

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

## License

Copyright (c) 2019-2020 Terry Nguyen - MIT License

The sprites used in the above GIF are CC0-licensed from [Buch][buch]'s dungeon
tileset.

[buch]:https://opengameart.org/users/buch

More information can be found by reading the [LICENSE](LICENSE.md) file in this
repository.
