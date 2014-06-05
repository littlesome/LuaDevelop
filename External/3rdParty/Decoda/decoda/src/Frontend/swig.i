%module(directors="1") Decoda

%{
#include "DebugEvent.h"
#include "DebugFrontend.h"
%}

%include "std_string.i"

#define WINAPI 

%feature("director") IEvtHandler;

%include "../Shared/Protocol.h"
%include "DebugEvent.h"
%include "DebugFrontend.h"
