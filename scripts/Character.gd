class_name Character
extends Resource

@export var id := "character_id"
@export var display_name := "Display Name"
@export var bio := "Fill this with the character's complete bio. Supports BBCode."

@export var age := 0: set = set_age

@export var default_image := "neutral"
@export var images := {
	neutral = null,
}


func _init() -> void:
	assert(default_image in images)


func get_default_image() -> Texture:
	return images[default_image]


func get_image(expression: String) -> Texture:
	return images.get(expression, get_default_image())


func set_age(value: int) -> void:
	age = int(min(value, 0))
