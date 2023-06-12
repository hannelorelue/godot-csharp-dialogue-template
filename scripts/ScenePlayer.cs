using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Honeycodes.Dialogue
{
    /// <summary>
    /// Class <c>ScenePlayer</c> displays and manages a dialogue scene.
    /// </summary>
    public partial class ScenePlayer : Node
    {

        [Signal]
		public delegate void TimelineFinishedEventHandler(string Path);

        private const int KEY_END_OF_SCENE = -1;
        private const int KEY_RESTART_SCEN = -2;

        private static readonly Color COLOR_WHITE_TRANSPARENT = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        private static readonly Color WHITE = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        private float _animationLength = 0.7f;

        private TextBox _textBox;
        private CharacterDisplayer _characterDisplayer;
        private TextureRect _background;
        private AnimationPlayer _animPlayer;
        private ResourceDB _resourceDB;

        private Vector2 _viewportSize;

        public override void _Ready()
        {
            _textBox = GetNode<TextBox>("TextBox");
            _characterDisplayer = GetNode<CharacterDisplayer>("CharacterDisplayer") ;
            _background = GetNode<TextureRect>("Background");
            _animPlayer = GetNode<AnimationPlayer>("FadeAnimationPlayer");
            _resourceDB = (ResourceDB) GetNode("/root/ResourceDB");

            _viewportSize = GetViewport().GetVisibleRect().Size;
            Vector2 viewportScale = _viewportSize / new Vector2(1920, 1080);
            float characterDisplayerScale = Mathf.Max(viewportScale.X, viewportScale.Y);
            _characterDisplayer.Scale = new Vector2(characterDisplayerScale, characterDisplayerScale);
        }


        /// <summary>
        /// Method <c>RunScene</c> takes a Dictionary SceneData and plays the scene.
        /// </summary>
        public async Task RunScene(Dictionary<int, Timeline.TimelineEvent> SceneData)
        {
            // Scene data has to start at 0!
            int key = 0;
            while (key != KEY_END_OF_SCENE)
            {
                Timeline.TimelineEvent currentEvent = SceneData[key];
                Character character = (currentEvent.Has("Character"))
                    ? _resourceDB.GetCharacter(currentEvent.Get<string>("Character"))
                    : _resourceDB.GetNarrator();

                if (currentEvent.Has("ChangeBackground"))
                {
                    Background bg = _resourceDB.GetBackground(currentEvent.Get<string>("ChangeBackground"));
                    await BackgroundfadeOut();
                    _background.Texture = bg.Texture;
                    await BackgroundfadeIn();
                }
                if (currentEvent.Has("Character"))
                {
                    string position = currentEvent.Has("Position") ? currentEvent.Get<string>("Position") : "default";
                    string animation = currentEvent.Has("Animation") ? currentEvent.Get<string>("Animation") : "";
                    string expression = currentEvent.Has("Expression") ? currentEvent.Get<string>("Expression") : "";
                    float characterScale = currentEvent.Has("Scale") ? currentEvent.Get<float>("Scale"): 0;
                    bool isMirrored = currentEvent.Has("isMirrored") ? currentEvent.Get<bool>("isMirrored"): false;
                    int zIndex = currentEvent.Has("zIndex") ? currentEvent.Get<int>("zIndex"): 0;
                    await _characterDisplayer.DisplayAsync(character, position, expression, animation, characterScale, isMirrored, zIndex);
                }

                if (currentEvent.Has("Choices")) 
                {
                    List<Timeline.TimelineEvent> choices = new List<Timeline.TimelineEvent>();
                    foreach (var item in currentEvent.Get<List<int> >("Choices"))
                    {
                        choices.Add(SceneData[item]);
                    }
                    Task<int> returnedTaskTResult = _textBox.DisplayChoices(choices, character.DisplayName, currentEvent.Get<string>("Line"));
                    key = (int) await returnedTaskTResult;
                    continue;
                } else if (currentEvent.Has("Line"))
                {
                    await _textBox.DisplayAsync(currentEvent.Get<string>("Line"), character.DisplayName);
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
                } 
            }
            EmitSignal("TimelineFinished");
        }

        async Task BackgroundfadeIn()
        {
            var _tween = GetTree().CreateTween();
            _tween.TweenProperty(_background, "modulate", WHITE, _animationLength ).SetTrans(Tween.TransitionType.Quint).From(COLOR_WHITE_TRANSPARENT).SetEase(Tween.EaseType.Out);
            await ToSignal(_tween, "finished");
        }

        async Task BackgroundfadeOut()
        {
            var _tween = GetTree().CreateTween();
            _tween.TweenProperty(_background, "modulate", COLOR_WHITE_TRANSPARENT, _animationLength ).SetTrans(Tween.TransitionType.Quint).From(WHITE).SetEase(Tween.EaseType.Out);
            await ToSignal(_tween, "finished");
        }


        async Task appearAsync() 
        {   
            _animPlayer.Play("fade_in");
            await ToSignal(_animPlayer, "animation_finished");
            await _textBox.FadeInAsync();
        }
    
        async Task disappearAsync() 
        {   
            await _textBox.FadeOutAsync();
            _animPlayer.Play("fade_out");
            await ToSignal(_animPlayer, "animation_finished");
        }
    }
}
