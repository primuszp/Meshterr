
namespace Meshterr
{
    /// <summary>
    /// Interface definition for a DisplayList
    /// </summary>
    public interface IDisplayList
    {
        /// <summary>
        /// Call the list to display
        /// </summary>
        void Call();

        /// <summary>
        /// Delete the DisplayList from memory
        /// </summary>
        void Delete();

        /// <summary>
        /// Is this DisplayList an actual list.
        /// </summary>
        /// <returns>true if the list an actual DisplayList</returns>
        bool IsList();

        /// <summary>
        /// Regenerate the display lists based on the parameters for solid fill and edge drawing 
        /// </summary>
        /// <param name="renderingOptions">Rendering options to impose on the regenerated display list objects.</param>
        void Regenerate(RenderingType renderingOptions);
    }
}
