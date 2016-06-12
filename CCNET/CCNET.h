// CCNET.h : main header file for the CCNET DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CCCNETApp
// See CCNET.cpp for the implementation of this class
//

class CCCNETApp : public CWinApp
{
public:
	CCCNETApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
