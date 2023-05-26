using Godot;
using System;
using System.IO;
using System.Collections.Generic;

namespace VisualNovelMono
{
    public partial class ResourceDB : Node
    {
        private const string NARRATOR_ID = "narrator";
        private Dictionary<string, Character> _characters;
        private Dictionary<string, Background> _backgrounds;

        public override void _Ready()
        {
            _characters = LoadCharacters("res://Characters/");
            _backgrounds = LoadBackgrounds("res://Backgrounds/");
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

        private Dictionary<string, Character> LoadCharacters(string directoryPath) 
        {
            var resources = new Dictionary<string, Character>();
            string[] fileNames = Directory.GetFiles(directoryPath, "*.tres");

            foreach (var item in fileNames)
            {
                var path  = directoryPath + "/" + item;
                var resource = ResourceLoader.Load(path) as Character;

                if (!_IsCharacter(resource))
                    continue;

                if (resource.Id != null)
                    resources[resource.Id] = resource;
            }
            return resources;
        }

        private Dictionary<string, Background> LoadBackgrounds(string directoryPath) 
        {
            var resources = new Dictionary<string, Background>();
            string[] fileNames = Directory.GetFiles(directoryPath, "*.tres");

            foreach (var item in fileNames)
            {
                var path  = directoryPath + "/" + item;
                var resource = ResourceLoader.Load(path) as Background;

                if (!_IsBackground(resource))
                    continue;

                if (resource.Id != null)
                    resources[resource.Id] = resource;
            }
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

}