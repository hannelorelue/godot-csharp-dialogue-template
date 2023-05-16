using Godot;
using System;

public partial class TextBox : TextureRect
{

	[Signal]
	public delegate void NextRequestedEventHandler();

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
	private Label _nameLabel;
	private Control  _blinkingArrow;
	private Tween _tween;
	private AnimationPlayer _aniPlayer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Hide()
		_richTextLabel = GetNode<RichTextLabel>("RichTextLabel");
		_nameLabel = GetNode<Label>("NameBackground/NameLabel");
		_blinkingArrow = GetNode<Control>("RichTextLabel/BlinkingArrow");
		_aniPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		_blinkingArrow.Hide();
		_nameLabel.Text = "";
		_richTextLabel.Text = "";
		_richTextLabel.VisibleCharacters  = 0;
		FadeInAsync();
		Display("Hello!", "Sophia");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void unhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept") == true)
		{
			if (_blinkingArrow.Visible == true)
			{
				EmitSignal("NextRequested");
			} else
			{
				_tween.Kill();
				_richTextLabel.VisibleCharacters = -1;
			}
		}
	}

	public void Display(string Text, string characterName = "", float speed = -1) 
	{
		SetBBCodeText(Text);
		if (speed == -1) 
		{
			speed = DisplaySpeed;
		}
		if (speed != DisplaySpeed)
		{
			DisplaySpeed = speed;
		}
		if (characterName != "")
		{
			_nameLabel.Text = characterName;
		}

	}

	async void SetBBCodeText(string Text) 
	{
		_bbcodeText = Text;
		if (IsInsideTree() != true) 
		{
			await ToSignal(this, "ready");
		}
		_blinkingArrow.Hide();
		_richTextLabel.Text = _bbcodeText;
		CallDeferred(nameof(beginDialogueDisplay));
		
	}


	async void FadeInAsync()
	{
		_aniPlayer.Play("fade_in");
		await ToSignal(_aniPlayer, "animation_finished");
	}


	async void FadeOutAsync()
	{
		_aniPlayer.Play("fade_out");
		await ToSignal(_aniPlayer, "animation_finished");
	}

	async void beginDialogueDisplay() 
	{
		int characterCount = _richTextLabel.GetTotalCharacterCount();
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(_richTextLabel, "visible_characters", characterCount, characterCount / DisplaySpeed).SetTrans(Tween.TransitionType.Quint).From(0);
		await ToSignal(_tween, "finished");
		_blinkingArrow.Show();
	}

}
