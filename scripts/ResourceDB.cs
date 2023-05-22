using Godot;
using System;

public partial class ResourceDB : Node
{
    private const string NARRATOR_ID = "narrator";
    private Dictionary<string, Character> _characters;
    private Dictionary<string, Background> _backgrounds;

    public override void _Ready()
    {
        _characters = LoadResources<Character>("res://Characters/", "_IsCharacter");
        _backgrounds = LoadResources<Background>("res://Backgrounds/", "_IsBackground");
    }

    public Character GetCharacter(string characterId)
    {
        return _characters.ContainsKey(characterId) ? _characters[characterId] : null;
    }

    public Background GetBackground(string backgroundId)
    {
        return _backgrounds.ContainsKey(backgroundId) ? _backgrounds[backgroundId] : null;
    }

    public Character GetNarrator()
    {
        return GetCharacter(NARRATOR_ID);
    }

    // TODO fix this function
    private Dictionary<string, T> LoadResources<T>(string directoryPath, string checkTypeFunction) where T : Resource
    {
        var directory = new Directory();
        if (directory.Open(directoryPath) != Error.Ok)
            return new Dictionary<string, T>();

        var resources = new Dictionary<string, T>();

        directory.ListDirBegin();
        var filename = directory.GetNext();
        while (!string.IsNullOrEmpty(filename))
        {
            if (filename.EndsWith(".tres"))
            {
                var resource = ResourceLoader.Load<T>(directoryPath.PlusFile(filename));

                var checkTypeMethod = GetType().GetMethod(checkTypeFunction);
                if (checkTypeMethod == null || !(bool)checkTypeMethod.Invoke(this, new object[] { resource }))
                    continue;

                resources[resource.GetId()] = resource;
            }
            filename = directory.GetNext();
        }
        directory.ListDirEnd();

        return resources;
    }

    private bool _IsCharacter(Resource resource)
    {
        return resource is Character;
    }

    private bool _IsBackground(Resource resource)
    {
        return resource is Background;
    }
}

