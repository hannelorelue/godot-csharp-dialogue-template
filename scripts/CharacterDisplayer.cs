using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Honeycodes.Dialogue
{
    /// <summary>
    /// Class <c>CharacterDisplayer</c> manages display and animation of character sprites.
    /// </summary>
    public partial class CharacterDisplayer : Node2D
    {
        [Signal]
        public delegate void DisplayFinishedEventHandler();

        private const string OUTER_LEFT = "outer_left";
        private const string LEFT = "left";
        private const string MIDDLE = "middle";
        private const string RIGHT = "right";
        private const string OUTER_RIGHT = "outer_right";
        private const string DEFAULT = "default";
        private static readonly Color COLOR_WHITE_TRANSPARENT = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        private static readonly Color WHITE = new Color(1.0f, 1.0f, 1.0f, 1.0f);
 

        private Dictionary<string, Character> _displayed = new Dictionary<string, Character>
        {
            {OUTER_LEFT, null},
            {LEFT, null},
            {MIDDLE, null},
            {RIGHT, null},
            {OUTER_RIGHT, null}
        };

        private float _animationLength = 0.7f;

        private Tween _tween;
        private Sprite2D _outerLeftSprite;
        private Sprite2D _leftSprite;
        private Sprite2D _middleSprite;
        private Sprite2D _rightSprite;
        private Sprite2D _outerRightSprite;

        public override void _Ready()
        {
            _outerLeftSprite = GetNode<Sprite2D >("OuterLeft");
            _leftSprite = GetNode<Sprite2D >("Left");
            _middleSprite = GetNode<Sprite2D >("Middle");
            _rightSprite = GetNode<Sprite2D >("Right");
            _outerRightSprite = GetNode<Sprite2D >("OuterRight");
        }

        public async Task DisplayAsync(Character character, string postition, string expression = "", string animation = "", float spriteScale = 0 , bool isMirrored = false, int spriteZIndex = 0)
        {
            
            Sprite2D sprite = null;
 

            if (_displayed[OUTER_LEFT] == character)
            {
                sprite = _outerLeftSprite;
            }
            else if (_displayed[LEFT] == character)
            {
                sprite = _leftSprite;
            }
            else if (_displayed[MIDDLE] == character)
            {
                sprite = _middleSprite;
            }
            else if (_displayed[RIGHT] == character)
            {
                sprite = _rightSprite;
            }
            else if (_displayed[OUTER_RIGHT] == character)
            {
                sprite = _outerRightSprite;
            }
            else
            {
                _displayed[postition] = character;
            }

            switch (postition)
            {
                case OUTER_LEFT:
                    sprite = _outerLeftSprite;
                    break;
                case LEFT:
                    sprite = _leftSprite;
                    break;
                case MIDDLE:
                    sprite = _middleSprite;
                    break;
                case RIGHT:
                    sprite = _rightSprite;
                    break;
                case OUTER_RIGHT:
                    sprite = _outerRightSprite;
                    break;
            }

            if (sprite != null)
            {
                sprite.Texture = character.GetImage(expression);
                sprite.Position += character.Offset;
                sprite.FlipH = isMirrored;
                sprite.ZIndex = spriteZIndex;
                if (spriteScale != 0) 
                {
                    sprite.Scale = new Vector2((float) spriteScale, (float) spriteScale);
                }
                switch (animation)
                {
                    case "enter":
                        await EnterAsync(postition, sprite);
                        break;
                    case "leave":
                        await LeaveAsync(postition, sprite);
                        break;
                }
            }
        }

        async Task EnterAsync(string fromSide, Sprite2D sprite)
        {
            float offset = 0;
            switch (fromSide)
            {
                case OUTER_LEFT:
                case LEFT:
                case MIDDLE:
                    offset = -400.0f;
                    break;
                case RIGHT:
                case OUTER_RIGHT:
                    offset = 400.0f;
                    break;
            };

            Vector2 start = sprite.Position + new Vector2(offset, 0.0f);
            Vector2 end = sprite.Position;

            _tween = GetTree().CreateTween();
            _tween.TweenProperty(sprite, "position", end, _animationLength).SetTrans(Tween.TransitionType.Quint).From(start).SetEase(Tween.EaseType.Out);
            _tween.Parallel().TweenProperty(sprite, "modulate", WHITE, _animationLength * 0.2f).SetTrans(Tween.TransitionType.Quint).From(COLOR_WHITE_TRANSPARENT).SetEase(Tween.EaseType.Out);
            await ToSignal(_tween, "finished");
        }

        async Task LeaveAsync(string fromSide, Sprite2D  sprite)
        {
            float offset = 0.0f;

            switch (fromSide)
            {
                case OUTER_LEFT:
                case LEFT:
                case MIDDLE:
                    offset = -200.0f;
                    break;
                case RIGHT:
                case OUTER_RIGHT:
                    offset = 200.0f;
                    break;
            }

            _displayed[fromSide] = null;

            Vector2 start = sprite.Position;
            Vector2 end = sprite.Position + new Vector2(offset, 0.0f);
            
            _tween = GetTree().CreateTween();
            _tween.TweenProperty(sprite, "position", end, _animationLength).SetTrans(Tween.TransitionType.Quint).From(start).SetEase(Tween.EaseType.Out);
            _tween.Parallel().TweenProperty(sprite, "modulate", COLOR_WHITE_TRANSPARENT, _animationLength * 0.5f).SetTrans(Tween.TransitionType.Linear).From( WHITE).SetEase(Tween.EaseType.Out);
            await ToSignal(_tween, "finished");
        }

    }
}