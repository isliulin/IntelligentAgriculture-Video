#include <Windows.h>
#include "HelloWorld.h"

using namespace Exercise2;

int APIENTRY WinMain(HINSTANCE hinstance, HINSTANCE hPrevInstance, LPSTR IpCmdLine, int nCmdShow)
{
	Application::Run(gcnew HelloWorld());
	return 0;
}