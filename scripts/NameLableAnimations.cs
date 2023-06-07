using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Honeycodes.Dialogue
{
	public partial class NameLableAnimations : Label
	{
		private readonly Color COLOR_WHITE_TRANSPARENT = new Color(1.0f, 1.0f, 1.0f, 0.0f);

		// Duration of the fade-in animation in seconds.
		// The disappear animation is twice as short.
		public float appearDuration = 0.2f;

		// Here, we create a Tween node.
		private Tween _tween;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Modulate = COLOR_WHITE_TRANSPARENT;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public async Task AppearAsync()
		{	
			if (Modulate != Colors.White)
			{
				_tween = GetTree().CreateTween();
				_tween.TweenProperty(this, "modulate", Colors.White, appearDuration).SetTrans(Tween.TransitionType.Linear).From(COLOR_WHITE_TRANSPARENT).SetEase(Tween.EaseType.Out);
				await ToSignal(_tween, "finished");
			}
		}

		public async Task DisappearAsync()
		{				
			if (Modulate != COLOR_WHITE_TRANSPARENT)
			{
				_tween = GetTree().CreateTween();
				_tween.TweenProperty(this, "modulate", COLOR_WHITE_TRANSPARENT, appearDuration/2).SetTrans(Tween.TransitionType.Linear).From(Colors.White).SetEase(Tween.EaseType.Out);
				await ToSignal(_tween, "finished");
			}
		}
	}
}
