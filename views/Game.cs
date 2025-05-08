using Godot;
using JamTemplate.Managers;
using JamTemplate.Util;

public partial class Game : Node2D
{
  const float MOVE_SPEED = 100f;
  private TextureRect _character;
  private bool _paused = false;
  private bool _selectPressed = false;
  private bool _cancelPressed = false;
  [FromServices]
  public void Inject(SceneManager sceneManager)
  {

  }
  public override void _EnterTree()
  {
    _character = GetNode<TextureRect>("%Character");
  }

  public override void _Process(double delta)
  {
    var direction = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
    _character.Position += direction * MOVE_SPEED * (float)delta;

    if (!_selectPressed && Input.GetActionStrength("select") != 0)
    {
      _selectPressed = true;
      _character.Modulate = new Color(0, 1, 0, 1);
    }
    else if (_selectPressed && Input.GetActionStrength("select") == 0)
    {
      _selectPressed = false;
      _character.Modulate = new Color(1, 1, 1, 1);
    }

    if (!_cancelPressed && Input.GetActionStrength("cancel") != 0)
    {
      _cancelPressed = true;
      _character.Modulate = new Color(1, 0, 0, 1);
    }
    else if (_cancelPressed && Input.GetActionStrength("cancel") == 0)
    {
      _cancelPressed = false;
      _character.Modulate = new Color(1, 1, 1, 1);
    }
  }
}
