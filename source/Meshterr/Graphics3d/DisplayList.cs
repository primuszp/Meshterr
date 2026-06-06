using System;
using OpenTK.Graphics.OpenGL;

namespace Meshterr
{
    public abstract class DisplayList : IDisplayList
    {
        private int list;

        public int List
        {
            get { return list; }
            set { list = value; }
        }

        public DisplayList()
        {
            list =  0;
        }

        public virtual void NewList(ListMode mode)
        {
            //Generate one list
            list = GL.GenLists(1);

            //Initialize the list
            GL.NewList(list, mode);
        }

        public virtual void EndList()
        {
            //End the display list
            GL.EndList();
        }

        public virtual bool IsList()
        {
            //Is the list a display list?
            if (GL.IsList(list) == true)
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public static bool IsList(DisplayList displayList)
        {
            //Is the specified list a proper display list?
            if (GL.IsList(displayList.List) == true)
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public virtual void Call()
        {
            GL.CallList(list);
        }

        public virtual void Delete()
        {
            GL.DeleteLists(list, 1);
            list = 0;
        }

        /// <summary>
        /// Virtual method to Regenerate the display lists based on the parameters for solid fill and edge drawing 
        /// </summary>
        /// <param name="renderingOptions">Rendering options to impose on the regenerated display list objects.</param>
        public virtual void Regenerate(RenderingType renderingOptions)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
