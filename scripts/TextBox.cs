using Godot;
using System;
using System.Threading.Tasks;

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
	public async override void _Ready()
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
		await FadeInAsync();
		Display("Hello! My name is Sophia! How are you?", "Sophia");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
				_tween.Kill();
				_richTextLabel.VisibleCharacters = -1;
				_blinkingArrow.Show();
			}
		}
	}

	async void Display(string Text, string characterName = "", float speed = -1) 
	{
		
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
	
		await SetBBCodeTextAsync(Text);
		await beginDialogueDisplayAsync();
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


	async Task FadeInAsync()
	{
		_aniPlayer.Play("fade_in");
		await ToSignal(_aniPlayer, "animation_finished");
	}


	async Task FadeOutAsync()
	{
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

}
