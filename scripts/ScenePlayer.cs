using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using VisualNovelMono;
using Newtonsoft.Json;

namespace VisualNovelMono
{
    public partial class ScenePlayer : Node
    {
        private const int KEY_END_OF_SCENE = -1;
        private const int KEY_RESTART_SCEN = -2;

        private TextBox textbox;
        private CharacterDisplayer characterDisplayer;
        private TextureRect background;
        private AnimationPlayer anim_player;

        private Vector2 viewportSize;

        public override void _Ready()
        {
            textbox = GetNode<TextBox>("TextBox");
            characterDisplayer = GetNode<CharacterDisplayer>("CharacterDisplayer") ;
            background = GetNode<TextureRect>("Background");
            anim_player = GetNode<AnimationPlayer>("FadeAnimationPlayer");
            viewportSize = GetViewport().GetVisibleRect().Size;
            Vector2 viewportScale = viewportSize / new Vector2(1920, 1080);
            float characterDisplayerScale = Mathf.Max(viewportScale.X, viewportScale.Y);
            characterDisplayer.Scale = new Vector2(characterDisplayerScale, characterDisplayerScale);
            Dictionary<int, Timeline.TimelineEvent> SceneData = Timeline.LoadFromJson("Timeline1.json");
            foreach (var item in SceneData)
            {
                GD.Print(item.ToString());
            }       
            // await appearAsync();  
            // await textbox.DisplayAsync("Hello! My name is Sophia! How are you?", "Sophia");

        }

        async Task appearAsync() 
        {   
            anim_player.Play("fade_in");
            await ToSignal(anim_player, "animation_finished");
            await textbox.FadeInAsync();
        }
    
        async Task disappearAsync() 
        {   
            await textbox.FadeOutAsync();
            anim_player.Play("fade_out");
            await ToSignal(anim_player, "animation_finished");

        }


    }
}
