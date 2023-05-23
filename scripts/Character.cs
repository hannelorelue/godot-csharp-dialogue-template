using Godot;
using System;

namespace VisualNovelMono
{
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
        public Godot.Collections.Dictionary<string, Texture2D> Images {get; set; } = new Godot.Collections.Dictionary<string, Texture2D> {
            ["neutral"] = null
        } ;

        
        public Character() : this(null, null, null, 0, null, null) {}

        public Character (string id , string displayName , string bio, int age, string defaultImage , Godot.Collections.Dictionary<string, Texture2D> images )
        {
            Id = id;
            DisplayName = displayName;
            Bio = bio;
            Age = age;
            DefaultImage = defaultImage;
            Images = images;
        }

        public Texture2D GetImage(string Expression) 
        {
            return Images[Expression];
        }
    
        public Texture2D GetDefaultImage() 
        {
            return Images[DefaultImage];
        }
    }


}