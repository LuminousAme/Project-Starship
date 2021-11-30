#include "DifficultyRead.h"
#include <algorithm>
#include <sstream>
#include <filesystem>

//declare the map
std::unordered_map<std::string, float> DifficultlyRead::difficultySettings = std::unordered_map<std::string, float>();

//functions to trim strings, based on the implementations from this website: https://www.delftstack.com/howto/cpp/how-to-trim-a-string-cpp/
static void leftTrim(std::string& s, std::string& toTrim) {
	//trim from character zero to the first character not being excluded
	s.erase(0, s.find_first_not_of(toTrim));
}


static void rightTrim(std::string& s, std::string& toTrim) {
	//trim everything after the last character that is being included
	s.erase(s.find_last_not_of(toTrim) + 1);
}


static void trim(std::string& s, std::string& toTrim) {
	//trim from the left then from the right
	leftTrim(s, toTrim);
	rightTrim(s, toTrim);
}

void DifficultlyRead::ReadSettings(std::string fileName)
{
	//open the file
	std::ifstream file;
	file.open(fileName, std::ios::in | std::ios::binary);

	//make sure the file exists
	while (!file) {
		//create the file for writting
		std::ofstream newFile;

		//open it and write a single comment explaining what the file is
		newFile.open(fileName, std::ios::out);
		newFile << "# The Difficultly Settings File, format is \"SettingName Multiplier\" 1.0 is normal, lower (min 0) is easier, higher is harder"
			<< std::endl;

		//close it
		newFile.close();

		//open it for reading again
		file.open(fileName, std::ios::in | std::ios::binary);
	}

	//if it opened currently
	if (file.is_open()) {
		std::cout << "File Opened for reading\n";

		//get a single line of the file
		std::string line;

		//make a string with all of the whitespace character that we want to trim out
		std::string whiteSpace = " ";

		//iterate over all of the lines parsing each one
		while (std::getline(file, line)) {
			//trim it so the whitespace is removed
			trim(line, whiteSpace);

			//check if the line is a comment, if there is we don't have to do anything with it, but the if let's us optimize a bit
			if (line.substr(0, 1) == "#" || line.substr(0, 1) == "/");

			//otherwise it must be a line that has a setting
			else {
				//iterate through the string until we find the first space
				int indexOfSpace;
				for (indexOfSpace = 0; indexOfSpace < line.size(); indexOfSpace++) {
					if (line[indexOfSpace] == ' ') {
						break;
					}
				}

				//get a string stream that starts after the space
				std::istringstream stream = std::istringstream(line.substr(indexOfSpace));
				//get the string before the space
				std::string settingName = line.substr(0, indexOfSpace);

				//make a temporary variable to hold the difficult setting
				float temp = 1.0f;
				//read the value from the text file into it
				stream >> temp;

				//save that value in that map, using the name as the key
				difficultySettings[settingName] = temp;
			}
		}
	}

	//close the file
	file.close();
}