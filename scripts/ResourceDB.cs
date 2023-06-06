using Godot;
using System;
using System.IO;
using System.Collections.Generic;

namespace Honeycodes.Dialogue
{
    public partial class ResourceDB : Node
    {
        private const string NARRATOR_ID = "Narrator";
        private Dictionary<string, Character> _characters;
        private Dictionary<string, Background> _backgrounds;

        public override void _Ready()
        {
            _characters = LoadCharacters("resources/Characters/");
            _backgrounds = LoadBackgrounds("resources/Background/");
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
                var path  = item;
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
                var path  =  item;
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