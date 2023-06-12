using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Honeycodes.Dialogue
{
	public partial class TextBox : TextureRect
	{

		[Signal]
		public delegate void NextRequestedEventHandler();

		 [Signal]
    	public delegate void ChoiceMadeEventHandler(int targetId);

		[Export]
		public float DisplaySpeed = 20;

		[Export]
		public string BbcodeText 
		{ 
			get {return _bbcodeText ;} 
			set { _bbcodeText = value ;}
		}

		private string _bbcodeText = "";

		private RichTextLabel _richTextLabel;
		private NameLabel _nameLabel;
		private NameBackground _nameBackground ;
		private Control _blinkingArrow;
		private Tween _tween;
		private ChoiceContainer _choiceContainer;
		private AnimationPlayer _aniPlayer;

		int choice = 0;
		
		public async override void _Ready()
		{
			//Hide();
			_richTextLabel = GetNode<RichTextLabel>("RichTextLabel");
			_nameLabel = GetNode<NameLabel>("NameBackground/NameLabel");
			_nameBackground = GetNode<NameBackground>("NameBackground");
			_blinkingArrow = GetNode<Control>("RichTextLabel/BlinkingArrow");
			_aniPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
			_choiceContainer = GetNode<ChoiceContainer>("ChoiceContainer");

			_blinkingArrow.Hide();
			_nameLabel.Text = "";
			_richTextLabel.Text = "";
			_richTextLabel.VisibleCharacters  = 0;
			_choiceContainer.ChoiceMade += OnChoiceSelectorChoiceMade;
		}


		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event.IsActionPressed("ui_accept") == true)
			{
				if (_blinkingArrow.Visible == true)
				{
					EmitSignal("NextRequested");
				} else
				{
					_tween.EmitSignal("finished");
					_tween.Kill();
					_richTextLabel.VisibleCharacters = -1;
					_blinkingArrow.Show();
				}
			}
		}

		public async Task DisplayAsync(string Text, string characterName = "", float speed = -1) 
		{
			_richTextLabel.Text = "";
			_richTextLabel.VisibleCharacters  = 0;
			_blinkingArrow.Hide();
			if (SelfModulate  == new Color(1,1,1,0)) {
				await FadeInAsync();
			}

			switch (speed)
			{
				case -1:
				speed = DisplaySpeed;
				break;

				default:
				DisplaySpeed = speed;
				break;
			}

			if (characterName != "")
			{
				_nameLabel.Text = characterName;
				await _nameBackground.AppearAsync();
				await _nameLabel.AppearAsync();
				
			} else 
			{
				await _nameLabel.DisappearAsync();
				await _nameBackground.DisappearAsync();
			}
		
			await SetBBCodeTextAsync(Text);
			await beginDialogueDisplayAsync();
			await ToSignal(this, "NextRequested");
		}

		async Task SetBBCodeTextAsync(string Text) 
		{
			_bbcodeText = Text;
			if (IsInsideTree() != true) 
			{
				await ToSignal(this, "ready");
			}
			_blinkingArrow.Hide();
			_richTextLabel.Text = _bbcodeText;
		}


		public async Task FadeInAsync()
		{

			_aniPlayer.Play("fade_in");
			await ToSignal(_aniPlayer, "animation_finished");
		}


		public async Task FadeOutAsync()
		{
			await _nameLabel.DisappearAsync();
			await _nameBackground.DisappearAsync();
			_aniPlayer.Play("fade_out");
			await ToSignal(_aniPlayer, "animation_finished");
		}

		async Task beginDialogueDisplayAsync() 
		{
			int characterCount = _richTextLabel.GetTotalCharacterCount();
			_tween = GetTree().CreateTween();
			_tween.TweenProperty(_richTextLabel, "visible_characters", characterCount, characterCount / DisplaySpeed).From(0);
			await ToSignal(_tween, "finished");
			_blinkingArrow.Show();
		}

		public async Task<int> DisplayChoices(List<Timeline.TimelineEvent> choices, string displayName, string line)
		{
			_blinkingArrow.Hide();
			await SetBBCodeTextAsync(line);
			_choiceContainer.Display(choices);
			_nameLabel.Text = displayName;
			_richTextLabel.VisibleCharacters = -1;
			await ToSignal(this, SignalName.ChoiceMade);
			return choice;
		}

		private void OnChoiceSelectorChoiceMade(int targetId)
		{
			choice = targetId;
			EmitSignal(SignalName.ChoiceMade);
		}
	}
}