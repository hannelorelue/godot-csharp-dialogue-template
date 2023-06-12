using Godot;
using System;

namespace Honeycodes.Dialogue
{
    /// <summary>
    /// Class <c>Character</c> is a custom resource.
    /// </summary>
    public partial class Character : Resource
    {
        [Export]
        public string Id { get; set; }  = "character_id" ;
        
        [Export]
        public string DisplayName { get; set; } = "Display Name";

        [Export]
        public string Bio { get; set; } = "";
        
        [Export]
        public int Age { get; set; } = 0;

        [Export]
        public string DefaultImage { get; set; } = "neutral";

        [Export]
        public Vector2 Offset { get; set; } = Vector2.Zero;


        [Export]
        public Godot.Collections.Dictionary<string, Texture2D> Images {get; set; } = new Godot.Collections.Dictionary<string, Texture2D> {
            ["neutral"] = null
        } ;

        
        public Character() : this(null, null, null, 0, null, null, Vector2.Zero) {}

        public Character (string id , string displayName , string bio, int age, string defaultImage, Godot.Collections.Dictionary<string, Texture2D> images, Vector2 offset )
        {
            Id = id;
            DisplayName = displayName;
            Bio = bio;
            Age = age;
            DefaultImage = defaultImage;
            Images = images;
            Offset  = offset;
        }

        public Texture2D GetImage(string Expression) 
        {
            if (Expression == "") {
                return GetDefaultImage();
            } else {
                return Images[Expression];
            }
        }
    
        public Texture2D GetDefaultImage() 
        {
            return Images[DefaultImage];
        }
    }


}