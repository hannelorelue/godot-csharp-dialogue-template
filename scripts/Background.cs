using Godot;
using System;

public partial class Background : Resource
{
        [Export]
        public string Id { get; set; }  = "background_id" ;
        
        [Export]
        public string DisplayName { get; set; } = "Display Name";

        [Export]
        public Texture2D texture ;
}
