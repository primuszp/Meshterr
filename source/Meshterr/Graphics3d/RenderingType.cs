using System;

namespace Meshterr
{
    /// <summary>
    /// The SceneType enum is used to choose what sort of scene you want to initialise
    /// with, to save you having to look up all the settings.
    /// </summary>
    [Flags]
    public enum RenderingType
    {
        Vertex = 0,

        Wireframe = 1,

        Solid = 2,

        Shaded = 3
    }
}
