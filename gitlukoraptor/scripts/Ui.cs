using Godot;
using System;

public partial class Ui : CanvasLayer
{
	[Signal]
	public delegate void BtnPlebPressedEventHandler();
	public override void _Ready() { }
	public override void _Process(double delta) { }

	private void btnPleb_Click()
	{
		EmitSignal(SignalName.BtnPlebPressed);
	}
}
