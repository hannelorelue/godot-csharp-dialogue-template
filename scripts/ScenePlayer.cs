using Godot;
using System;

public partial class ScenePlayer : Node
{
    private const int KEY_END_OF_SCENE = -1;
    private const int KEY_RESTART_SCEN = -2;

    private TextureRect textbox;
	private Node2D characterDisplayer;
	private TextureRect background;
    private AnimationPlayer anim_player;

    private Vector2 viewportSize;

    public override void _Ready()
	{
        textbox = GetNode<TextureRect>("TextBox");
        characterDisplayer = GetNode<Node2D>("CharacterDisplayer") ;
        background = GetNode<TextureRect>("Background");
        anim_player = GetNode<AnimationPlayer>("FadeAnimationPlayer");
        viewportSize = GetViewport().GetVisibleRect().Size;
        Vector2 viewportScale = viewportSize / new Vector2(1920, 1080);
        float characterDisplayerScale = Mathf.Max(viewportScale.X, viewportScale.Y);
        characterDisplayer.Scale = new Vector2(characterDisplayerScale, characterDisplayerScale);
        GD.Print(viewportSize);
    }


}
