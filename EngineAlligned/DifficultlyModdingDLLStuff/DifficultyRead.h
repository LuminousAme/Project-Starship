#pragma once
#include <string>
#include <fstream>
#include <iostream>
#include <unordered_map>

#define PLUGIN_API __declspec(dllexport)

static class PLUGIN_API DifficultlyRead {
public:
	//function to init the map, reads from the file
	static void ReadSettings(std::string fileName);

	//function to look up a difficult settings by name
	static float GetDifficultlyMultiplayer(std::string settingName) {
		//get an iterator into the difficulty settings map
		std::unordered_map<std::string, float>::const_iterator it = difficultySettings.find(settingName);

		//if the iterator has gotten a result, return the value it has
		if (it != difficultySettings.end()) {
			return it->second;
		}

		//if it didn't find anything, just assume the value should be "normal" which is 1
		return 1.0f;
	}

private:
	//map that stores the settings by name
	static std::unordered_map<std::string, float>  difficultySettings;
};