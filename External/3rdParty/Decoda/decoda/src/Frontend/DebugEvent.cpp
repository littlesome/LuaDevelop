/*

Decoda
Copyright (C) 2007-2013 Unknown Worlds Entertainment, Inc. 

This file is part of Decoda.

Decoda is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Decoda is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Decoda.  If not, see <http://www.gnu.org/licenses/>.

*/

#include "DebugEvent.h"

DebugEvent::DebugEvent(EventId eventId, unsigned int vm)
{
    m_eventId       = eventId;
    m_vm            = vm;
    m_scriptIndex   = -1;
    m_line          = 0;
    m_enabled       = false;
    m_messageType   = MessageType_Normal;
}

EventId DebugEvent::GetEventId() const
{
    return m_eventId;
}

unsigned int DebugEvent::GetVm() const
{
    return m_vm;
}

void DebugEvent::SetScriptIndex(unsigned int scriptIndex)
{
    m_scriptIndex = scriptIndex;
}

unsigned int DebugEvent::GetScriptIndex() const
{
    return m_scriptIndex;
}

unsigned int DebugEvent::GetLine() const
{
    return m_line;
}

void DebugEvent::SetLine(unsigned int line)
{
    m_line = line;
}

bool DebugEvent::GetEnabled() const
{
    return m_enabled;
}

void DebugEvent::SetEnabled(bool enabled)
{
    m_enabled = enabled;
}

const std::string& DebugEvent::GetMessageStr() const
{
    return m_message;
}

void DebugEvent::SetMessage(const std::string& message)
{
    m_message = message;
}

MessageType DebugEvent::GetMessageType() const
{
    return m_messageType;
}

void DebugEvent::SetMessageType(MessageType messageType)
{
    m_messageType = messageType;
}

