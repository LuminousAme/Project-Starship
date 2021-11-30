#define EXPORT_API __declspec(dllexport)

#include <iostream>

extern "C"
{
	//Modify Value		//Restore Value
	float mod = 1.5f;	//1.0f

	float EXPORT_API orbitMod()
	{
		return mod;
	}
}

int main() {}
