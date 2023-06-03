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

        private TextBox textBox;
        private CharacterDisplayer characterDisplayer;
        private TextureRect background;
        private AnimationPlayer animPlayer;
        private ResourceDB resourceDB;

        private Vector2 viewportSize;

        public override void _Ready()
        {
            textBox = GetNode<TextBox>("TextBox");
            characterDisplayer = GetNode<CharacterDisplayer>("CharacterDisplayer") ;
            background = GetNode<TextureRect>("Background");
            animPlayer = GetNode<AnimationPlayer>("FadeAnimationPlayer");
            resourceDB = (ResourceDB) GetNode("/root/ResourceDB");
            viewportSize = GetViewport().GetVisibleRect().Size;
            Vector2 viewportScale = viewportSize / new Vector2(1920, 1080);
            float characterDisplayerScale = Mathf.Max(viewportScale.X, viewportScale.Y);
            characterDisplayer.Scale = new Vector2(characterDisplayerScale, characterDisplayerScale);
            Dictionary<int, Timeline.TimelineEvent> SceneData = Timeline.LoadFromJson("Timeline1.json");
            RunScene("Timeline1.json");

            // foreach (var item in SceneData)
            // {
            //     GD.Print(item.ToString());
            // }       
            // await appearAsync();  
            // await textBox.DisplayAsync("Hello! My name is Sophia! How are you?", "Sophia");

        }

        public async Task RunScene(string Path)
        {
            Dictionary<int, Timeline.TimelineEvent> SceneData = Timeline.LoadFromJson(Path);
            // Scene data has to start at 0!
            int key = 0;
            while (key != KEY_END_OF_SCENE)
            {
                Timeline.TimelineEvent currentEvent = SceneData[key];
                GD.Print(currentEvent.ToString());

                Character character = (currentEvent.Has("Character"))
                    ? resourceDB.GetCharacter(currentEvent.Get<string>("Character"))
                    : resourceDB.GetNarrator();

                if (currentEvent.Has("ChangeBackground"))
                {
                    Background bg = resourceDB.GetBackground(currentEvent.Get<string>("ChangeBackground"));
                    background.Texture = bg.texture;
                }
                if (currentEvent.Has("Character"))
                {
                    string position = currentEvent.Has("Position") ? currentEvent.Get<string>("Position") : "default";
                    string animation = currentEvent.Has("Animation") ? currentEvent.Get<string>("Animation") : "";
                    string expression = currentEvent.Has("Expression") ? currentEvent.Get<string>("Expression") : "";
                    double characterScale = currentEvent.Has("Scale") ? currentEvent.Get<double>("Scale"): 0;
                    characterDisplayer.Display(character, position, expression, animation, characterScale);
                }

                if (currentEvent.Has("Line"))
                {
                    await textBox.DisplayAsync(currentEvent.Get<string>("Line"), character.DisplayName);
                    GD.Print(currentEvent.Get<string>("Line"));
                    key = currentEvent.Get<int>("Next");
                } else 
                {
                    key = currentEvent.Get<int>("Next");
                }
                
        

                
            }
        }

        async Task appearAsync() 
        {   
            animPlayer.Play("fade_in");
            await ToSignal(animPlayer, "animation_finished");
            await textBox.FadeInAsync();
        }
    
        async Task disappearAsync() 
        {   
            await textBox.FadeOutAsync();
            animPlayer.Play("fade_out");
            await ToSignal(animPlayer, "animation_finished");

        }


    }
}
