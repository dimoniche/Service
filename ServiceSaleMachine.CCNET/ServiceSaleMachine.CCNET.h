// ServiceSaleMachine.CCNET.h : main header file for the ServiceSaleMachine.CCNET DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CServiceSaleMachineCCNETApp
// See ServiceSaleMachine.CCNET.cpp for the implementation of this class
//

class CServiceSaleMachineCCNETApp : public CWinApp
{
public:
	CServiceSaleMachineCCNETApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
