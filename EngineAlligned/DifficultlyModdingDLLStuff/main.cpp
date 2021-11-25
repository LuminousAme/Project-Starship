#include "DifficultyRead.h"

extern "C" {
	//function to read from the file loading all the data
	void PLUGIN_API LoadData(char* fileName) {
		std::string name = std::string(fileName);
		DifficultlyRead::ReadSettings(name);
	}

	//function to get the difficulty multipler for a given setting
	float PLUGIN_API GetMultiplierByName(char* settingName) {
		std::string name = std::string(settingName);
		return DifficultlyRead::GetDifficultlyMultiplayer(name);
		//return 1.0f;
	}
}

