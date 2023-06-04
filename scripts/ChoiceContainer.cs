using Godot;
using System;
using System.Collections.Generic;

namespace Honeycodes.Dialogue
{
public partial class ChoiceContainer : VBoxContainer
{
     [Signal]
    public delegate void ChoiceMadeEventHandler(int targetId);

    public void Display(List<Timeline.TimelineEvent> choices)
    {
        foreach (var choice in choices)
        {
            var button = new Button();
            button.Text = choice.Get<string>("ChoiceLabel");
            button.Pressed += () => onButtonPressed(choice.Get<int>("Next"));
            AddChild(button);
        }
        ((Button)GetChild(0)).GrabFocus();
    }

    public void onButtonPressed(int targetId)
    {
        EmitSignal("ChoiceMade", targetId);
        Clear();
    }

    private void Clear()
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
    }
}
}