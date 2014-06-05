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

#ifndef DEBUG_EVENT_H
#define DEBUG_EVENT_H

#include <string>

#include "Protocol.h"

/**
 * Event class used to pass information from the debug server to the
 * wxWidget UI.
 */
class DebugEvent
{

public:

    /**
     * Constructor.
     */
    DebugEvent(EventId eventId, unsigned int vm);

	/**
	 * Clone
	 */
	DebugEvent* Clone()
	{
		return new DebugEvent(*this);
	}

    /** 
     * Returns the event id of the event.
     */
    EventId GetEventId() const;

    /**
     * Returns the id of the virtual machine the event came from.
     */
    unsigned int GetVm() const;

    /**
     * Returns the index of the script the event relates to.
     */
    unsigned int GetScriptIndex() const;

    /**
     * Sets the index of the script the event relates to.
     */
    void SetScriptIndex(unsigned int scriptIndex);

    /**
     * Returns the number of the line in the script the event relates to.
     */
    unsigned int GetLine() const;

    /**
     * Sets the number of the line in the script the event relates to.
     */
    void SetLine(unsigned int scriptIndex);

    /**
     * Gets the boolean value for the event. This is a generic value that's
     * meaning depends on the event.
     */
    bool GetEnabled() const;

    /**
     * Sets the boolean value for the event. This is a generic value that's
     * meaning depends on the event.
     */
    void SetEnabled(bool enabled);

    /**
     * Returns the message associated with the event. Not all events will have
     * messages.
     */
    const std::string& GetMessageStr() const;

    /**
     * Sets the message associated with the event.
     */
	void SetMessage(const std::string& message);

    /**
     * Returns the type of the string message (error, warning, etc.) This is only
     * relevant when the event deals with a message.
     */
    MessageType GetMessageType() const;

    /**
     * Sets the type of the string message. This is only relevant when the event
     * deals with a message.
     */
    void SetMessageType(MessageType messageType);

private:

    EventId         m_eventId;
    unsigned int    m_vm;

    unsigned int    m_scriptIndex;
    unsigned int    m_line;

    bool            m_enabled;

	std::string     m_message;
    MessageType     m_messageType;

};

#endif