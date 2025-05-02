using Godot;
using JamTemplate.Managers;
using JamTemplate.Stores;
using JamTemplate.Util;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JamTemplate;

public partial class Entry : Node
{
  public static ServiceProvider ServiceProvider;

  public override void _EnterTree()
  {
    var Services = new ServiceCollection()
    .AddSingleton(InjectNodeClass<GameManager>())
    .AddSingleton<PlayerDataStore>()
    .AddSingleton<ConfigStore>()
    .AddSingleton<SettingsStore>()
    .AddSingleton(InjectInstantiatedPackedScene<SceneManager>("res://views/SceneManager.tscn"))
    ;
    AddScenes(Services);
    ServiceProvider = Services.BuildServiceProvider();
    var gameManager = ServiceProvider.GetRequiredService<GameManager>();
    GetTree().Root.CallDeferred("add_child", gameManager);
  }
  private Func<IServiceProvider, T> InjectNodeClass<T>() where T : Node, new()
  {
    return (serviceProvider) =>
    {
      var obj = new T();

      InjectAttributedMethods(obj, serviceProvider);

      return obj;
    };
  }
  private Func<IServiceProvider, T> InjectInstantiatedPackedScene<T>(string path, bool autoParent = true) where T : Node
  {
    return (serviceProvider) =>
    {
      var packed = ResourceLoader.Load<PackedScene>(path);
      var node = packed.Instantiate<T>();

      InjectAttributedMethods(node, serviceProvider);

      if (autoParent)
      {
        GetTree().Root.CallDeferred("add_child", node);
      }
      return node;
    };
  }
  private void InjectAttributedMethods<T>(T obj, IServiceProvider provider)
  {
    var objType = obj.GetType();
    var methods = objType
      .GetMethods(BindingFlags.Instance | BindingFlags.Public)
      .Where(method =>
      {
        return method.GetCustomAttribute<FromServicesAttribute>() != null;
      });

    foreach (var method in methods)
    {
      var args = method
        .GetParameters()
        .Select(param => provider.GetService(param.ParameterType)).ToArray();
      method.Invoke(obj, args);
    }
  }
  public void AddScenes(IServiceCollection collection)
  {
    var paths = SceneManager.ListAvailableScenes();
    foreach (var path in paths)
    {
      collection.AddKeyedTransient(Path.GetFileNameWithoutExtension(path), InjectAvailableScene(path));
    }
  }
  public Func<IServiceProvider, object?, Node> InjectAvailableScene(string path)
  {
    return (ServiceProvider, serviceKey) =>
    {
      return InjectInstantiatedPackedScene<Node>(path, false)(ServiceProvider);
    };
  }
}