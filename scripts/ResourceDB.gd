extends Node

const NARRATOR_ID := "narrator"

@onready var _characters := _load_resources("res://Characters/", "_is_character")
@onready var _backgrounds := _load_resources("res://Backgrounds/", "_is_background")


func get_character(character_id: String) -> Character:
	return _characters.get(character_id)


func get_background(background_id: String) -> Background:
	return _backgrounds.get(background_id)


func get_narrator() -> Character:
	return _characters.get(NARRATOR_ID)


func _load_resources(directory_path: String, check_type_function: String) -> Dictionary:
	var directory = DirAccess.open(directory_path)
	if directory != OK:
		return {}

	var resources := {}

	var files = directory.get_files()
	for item in files:
		if item.ends_with(".tres"):
			var resource: Resource = load(directory_path + item)
			if not call(check_type_function, resource):
				continue
			resources[resource.id] = resource
	return resources


func _is_character(resource: Resource) -> bool:
	return resource is Character


func _is_background(resource: Resource) -> bool:
	return resource is Background
