/** @file  VMCConst.h: common type declarations.
* The file is a C++ header file containing all declarations usefull for VMC implementation.
*       
*
* \n Product: All VMC implementations
* \n Country: All
* \n Protocol: All protocols
* 
*/
#ifndef _CONSTS
#define _CONSTS


/**	\struct _Time
	\brief	The _Time struct represets time value used for VMC operations

*/
struct _Time
{
	BYTE hours;//!< Hours since the midnight 
	BYTE mins; //!< Minutes 
	BYTE secs; //!< Seconds
};

/**	\struct _Cassete
	\brief	The _Cassete struct describes a recycling cassette

*/
struct _Cassete
	{
		BYTE Status;//!< Cassette status (one of the values from the \link CSStatus Possible cassette status codes \endlink group)
		BYTE BillNumber;//!< Number of bills in the cassete
		BYTE  BillType; //!< A bill type assigned to the cassette
		double dValue;	//!< Denomination of the bill type assigned to the cassette
		BYTE UnloadLevel;	//!< Number of bills to leave in the cassette when unloading 
		_Time UnloadTime;	//!< Time when start automatic unloading
		int iBillsThisTransaction;	//!< Number of bills packed in the cassette during current transaction
		double dTransactionAmount;	//!< Total amount packed in the cassette during current transaction
		bool bUnload;	//!< A flag specifiying whether the cassette should be unloaded
		bool bEscrow;	//!< A flag specifiying whether cassette is assigned to multi-escrow
		BYTE Index;		//!< A cassette index(position) for sorting purposes
		BYTE DecimalPlaces;	//!< Number of decimal places to calculate denomination
		int billsToUld;		//!< Number of bills to unload
		char sCountry[16];	//!< Country code assigned to the cassette
		
		/**	\brief	The default _Cassete constructor
		*/
		_Cassete()
		{
		Status=0xfe;dValue=UnloadTime.hours=
		BillNumber=UnloadLevel=0;
		UnloadTime.mins=UnloadTime.secs=0xff;
		bEscrow=bUnload=false;		
		billsToUld=-1;
		iBillsThisTransaction=0;
		dTransactionAmount=0;
		*sCountry=0;
		};
	}; 


/**	\struct _BillRecord
	\brief	The _BillRecord struct represents a record in the bill table.

  The structure describes denomination of the bill, country or currency code and whether
  the bill is forwarded to the cassette. The bill table usually is an array of _BillRecord
  structures, where the position is representing a billtype.

*/
struct _BillRecord
	{
		float Denomination;		//!< Denomination of the bill
		char sCountryCode[4];	//!< Country or currency code
		bool bRouted;			//!< A bool variable specifiying whether the bill is forwarded to a cassette
	};


const int MAX_CAS_NUMBER	=16; //!< maximum number of cassettes supported by VMC

#endif