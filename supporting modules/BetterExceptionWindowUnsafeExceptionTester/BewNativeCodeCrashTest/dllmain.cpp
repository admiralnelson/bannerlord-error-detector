// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"

extern "C" __declspec(dllexport) void CrashMe()
{
    char* p = nullptr;
    *p = 0;
}