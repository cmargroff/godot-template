
using Godot;
using JamTemplate.Managers;
using JamTemplate.Util;

namespace JamTemplate.Components;

public partial class ThemeCard : TextureRect
{
  private Label _label;
  private ConfigManager _config;
  public override void _EnterTree()
  {
    _label = GetNode<Label>("%Label");
    _label.Text = (string)_config.GetValue("game", "GWJ_THEME");
  }
  [FromServices]
  public void Inject(ConfigManager config)
  {
    _config = config;
  }
}