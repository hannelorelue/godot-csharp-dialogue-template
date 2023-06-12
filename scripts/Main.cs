using Godot;
using System;
using System.Collections.Generic;
using Honeycodes.Dialogue;

public partial class Main : Node
{
	public override void _Ready()
	{
		Dictionary<int, Timeline.TimelineEvent> SceneData = Timeline.LoadTimeline("DemoTimeline.tsv");
		// ScenePlayer _scenePlayer = new ScenePlayer();
		// AddChild(_scenePlayer);
		ScenePlayer _scenePlayer = (ScenePlayer) GetNode("ScenePlayer");
        _scenePlayer.RunScene(SceneData);
	}

}
