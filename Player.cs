using Godot;
using System;

public partial class Player : RigidBody3D
{
	float _mouseSensitivity = 0.001f;
	float _twistInput = 0f;
	float _pitchInput = 0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
			Input.MouseMode = Input.MouseModeEnum.Visible;

		Node3D twistPivot = (Node3D)GetNode(new NodePath("TwistPivot"));
		twistPivot.RotateY(_twistInput);

		Node3D pitchPivot = (Node3D)GetNode(new NodePath("TwistPivot/PitchPivot"));
		pitchPivot.RotateX(_pitchInput);

		var minValue = new Vector3(Mathf.DegToRad(-30), float.MinValue, float.MinValue);
		var maxValue = new Vector3(Mathf.DegToRad(30), float.MaxValue, float.MaxValue);
		pitchPivot.Rotation = pitchPivot.Rotation.Clamp(minValue, maxValue);

		_twistInput = 0;
		_pitchInput = 0;

		Move(twistPivot.Basis * GetMoveInput(), delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if (@event is InputEventMouseMotion
			&& Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			var inputEvent = @event as InputEventMouseMotion;
			_twistInput = -1 * inputEvent.Relative.X * _mouseSensitivity;
			_pitchInput = -1 * inputEvent.Relative.Y * _mouseSensitivity;
		}
	}

	Vector3 GetMoveInput()
	{
		var input = Vector3.Zero;
		input.X = Input.GetAxis("move_left", "move_right");
		input.Z = Input.GetAxis("move_forward", "move_back");

		return input;
	}

	void Move(Vector3 moveInput, double delta)
	{
		var move = moveInput * 1200.0f * (float)delta;
		ApplyCentralForce(move);
	}
}
