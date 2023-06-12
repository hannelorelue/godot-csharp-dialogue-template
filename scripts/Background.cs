using Godot;
using System;

namespace Honeycodes.Dialogue
{
        /// <summary>
        /// Class <c>Background</c> is a custom resource.
        /// </summary>
        public partial class Background : Resource
        {
                [Export]
                public string Id { get; set; }  = "background_id" ;
                

                [Export]
                public Texture2D Texture;
        }
}
