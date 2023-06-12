using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Honeycodes.Dialogue
{
	public partial class NameBackground : TextureRect
	{
		private readonly Color COLOR_WHITE_TRANSPARENT = new Color(1.0f, 1.0f, 1.0f, 0.0f);

		public float appearDuration = 0.2f;

		private Tween _tween;

		public override void _Ready()
		{
			Modulate = COLOR_WHITE_TRANSPARENT;
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