# SpeckleUnity

Proof-of-Concept integration of Speckle and Unity

## Disclaimer

This is a very rough proof-of-concept project.

Developed for Unity 2018.3.1f1, using .NET 4.x Equivalent.

## To use
1. Open SpeckleUnity.sln
2. Maybe update broken references to `UnityEngine.dll`
3. Build Solution
4. Copy built dlls (except for `UnityEngine.*`) into your Unity project
5. Create a GameObject with a UnitySpeckle component and a SpeckleConverter component
6. Set the stream ids, server urls, and prefabs.

## Limitations

Does not implement runtime login or stream selection.

Does not implement a sender of any kind.

Only reads mesh, point, and polyline data types.

Prefabs are probably broken.

## Notes on Hololens dev

Hololens uses Universal Windows Platforms. I haven't tested this. Look through previous commits to find a working version.
