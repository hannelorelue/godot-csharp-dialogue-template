using Godot;
using System;
using System.Collections.Generic;
using Honeycodes.Dialogue;

public partial class Main : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Dictionary<int, Timeline.TimelineEvent> SceneData = Timeline.LoadTimeline("DemoTimeline.tsv");
		// ScenePlayer _scenePlayer = new ScenePlayer();
		// AddChild(_scenePlayer);
		ScenePlayer _scenePlayer = (ScenePlayer) GetNode("ScenePlayer");
        _scenePlayer.RunScene(SceneData);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
