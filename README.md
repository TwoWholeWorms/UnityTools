# UnityTools

A collection of utilities designed to help with various tasks in Unity3D projects.

**Note**: This lot is targetted at .NET 4.5, meaning you can't compile it with Unity's ancient version of MonoDevelop,
so the easiest way to compile it is just to grab [Xamarin Studio](http://www.monodevelop.com/download/) and open
`UnityTools.sln`.

## Raw16Splitter

```
mono Raw16Splitter.exe <gridSize> <inputFile> <outputDir>

    gridSize  — the size of the grid to cut the RAW file up into. Must be square,
                and >= 2x2
    inputFile — the 16-bit RAW file to slice up
    outputDir — the directory to save the sliced-up tiles to
```

This utility takes a 16-bit raw file and splits it into tiles. I tend to generate large (>=40960x40960), combined
heightmaps in World Machine and then use this to slice it up so I can load them onto the terrains (usually via
[Terrain Composer](http://www.terraincomposer.com) as it lets you do it in fewer steps by batching the process up).

### Example usage

```
mono Raw16Splitter.exe 10x10 wm_heightmap_40960x40960.r16 ./tiles/
```

# License

This code is licensed under the MIT License. See [`LICENSE`](https://github.com/TwoWholeWorms/UnityTools/blob/master/LICENSE)
for the full terms, what few of them are.
