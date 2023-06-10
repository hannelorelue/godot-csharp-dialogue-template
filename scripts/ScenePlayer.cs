using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Honeycodes.Dialogue
{
    public partial class ScenePlayer : Node
    {

        [Signal]
		public delegate void ChangeTimelineEventHandler(string Path);
        [Signal]
		public delegate void TimelineFinishedEventHandler(string Path);

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
            ChangeTimeline += OnChangeTimeline;
            textBox = GetNode<TextBox>("TextBox");
            characterDisplayer = GetNode<CharacterDisplayer>("CharacterDisplayer") ;
            background = GetNode<TextureRect>("Background");
            animPlayer = GetNode<AnimationPlayer>("FadeAnimationPlayer");
            resourceDB = (ResourceDB) GetNode("/root/ResourceDB");
            viewportSize = GetViewport().GetVisibleRect().Size;
            Vector2 viewportScale = viewportSize / new Vector2(1920, 1080);
            float characterDisplayerScale = Mathf.Max(viewportScale.X, viewportScale.Y);
            characterDisplayer.Scale = new Vector2(characterDisplayerScale, characterDisplayerScale);

            Dictionary<int, Timeline.TimelineEvent> SceneData = Timeline.LoadFromTSV("timelines/DemoTimeline.tsv");
            //Dictionary<int, Timeline.TimelineEvent> SceneData = LoadTimeline("Timeline1.json");



            // foreach (var item in SceneData)
            // {
            //     GD.Print(item.ToString());
            // }       

            RunScene(SceneData);
            // await appearAsync();  
            // await textBox.DisplayAsync("Hello! My name is Sophia! How are you?", "Sophia");

        }

        private void OnChangeTimeline(string Path)
        {
            RunScene(LoadTimeline(Path));
        }

        public async Task RunScene(Dictionary<int, Timeline.TimelineEvent> SceneData)
        {
            // Scene data has to start at 0!
            int key = 0;
            while (key != KEY_END_OF_SCENE)
            {
                Timeline.TimelineEvent currentEvent = SceneData[key];
                Character character = (currentEvent.Has("Character"))
                    ? resourceDB.GetCharacter(currentEvent.Get<string>("Character"))
                    : resourceDB.GetNarrator();

                if (currentEvent.Has("ChangeBackground"))
                {
                    Background bg = resourceDB.GetBackground(currentEvent.Get<string>("ChangeBackground"));
                    background.Texture = bg.Texture;
                }
                if (currentEvent.Has("Character"))
                {
                    string position = currentEvent.Has("Position") ? currentEvent.Get<string>("Position") : "default";
                    string animation = currentEvent.Has("Animation") ? currentEvent.Get<string>("Animation") : "";
                    string expression = currentEvent.Has("Expression") ? currentEvent.Get<string>("Expression") : "";
                    float characterScale = currentEvent.Has("Scale") ? currentEvent.Get<float>("Scale"): 0;
                    await characterDisplayer.DisplayAsync(character, position, expression, animation, characterScale);
                }

                if (currentEvent.Has("Choices")) 
                {
                    List<Timeline.TimelineEvent> choices = new List<Timeline.TimelineEvent>();
                    foreach (var item in currentEvent.Get<List<int> >("Choices"))
                    {
                        choices.Add(SceneData[item]);
                    }
                    Task<int> returnedTaskTResult = textBox.DisplayChoices(choices, character.DisplayName, currentEvent.Get<string>("Line"));
                    key = (int) await returnedTaskTResult;
                    continue;
                } else if (currentEvent.Has("Line"))
                {
                    await textBox.DisplayAsync(currentEvent.Get<string>("Line"), character.DisplayName);
                    key = currentEvent.Get<int>("Next");
                    
                }  else 
                {
                    key = currentEvent.Get<int>("Next");
                }

                if (currentEvent.Has("Transition"))
                {
                    string transition = currentEvent.Get<string>("Transition");
                    switch (transition)
                    {
                        case "fadeIn":
                            await appearAsync();
                            break;
                        case "fadeOut":
                            await disappearAsync();
                            break;
                    }
                } else if (currentEvent.Has("ChangeTimeline"))
                {
                    EmitSignal("ChangeTimeline");
                    return;
                }
            }
            EmitSignal("TimelineFinished");
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
