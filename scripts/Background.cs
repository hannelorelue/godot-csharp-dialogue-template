using Godot;
using System;

namespace Honeycodes.Dialogue
{
        public partial class Background : Resource
        {
                [Export]
                public string Id { get; set; }  = "background_id" ;
                

                [Export]
                public Texture2D Texture;
        }
}
