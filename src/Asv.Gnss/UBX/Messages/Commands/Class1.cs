namespace Asv.Gnss
{
    


// 	/// <summary>
// 	/// 8 Get/set Get/set data batching configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_BATCH : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-BATCH";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x93;
// 	}
// 	/// <summary>
// 	/// (12) or (13) Command Clear, save and load configurations
// 	/// </summary>
// 	public abstract class UBX_CFG_CFG : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-CFG";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x09;
// 	}
// 	/// <summary>
// 	/// 44 Set Set user-defined datum
// 	/// </summary>
// 	public abstract class UBX_CFG_DAT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-DAT";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x06;
// 	}
// 	/// <summary>
// 	/// 52 Get Get currently defined datum
// 	/// </summary>
// 	public abstract class UBX_CFG_DAT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-DAT";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x06;
// 	}
// 	/// <summary>
// 	/// 4 Get/set DGNSS configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_DGNSS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-DGNSS";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x70;
// 	}
// 	/// <summary>
// 	/// 4 + 32*numO... Get/set Disciplined oscillator configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_DOSC : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-DOSC";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x61;
// 	}
// 	/// <summary>
// 	/// 12 Get/set Get/set IMU-mount misalignment...
// 	/// </summary>
// 	public abstract class UBX_CFG_ESFALG : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-ESFALG";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x56;
// 	}
// 	/// <summary>
// 	/// 20 Get/set Get/set the Accelerometer (A) sensor...
// 	/// </summary>
// 	public abstract class UBX_CFG_ESFA : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-ESFA";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x4C;
// 	}
// 	/// <summary>
// 	/// 20 Get/set Get/set the Gyroscope (G) sensor...
// 	/// </summary>
// 	public abstract class UBX_CFG_ESFG : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-ESFG";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x4D;
// 	}
// 	/// <summary>
// 	/// 32 Get/set Get/set wheel-tick configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_ESFWT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-ESFWT";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x82;
// 	}
// 	/// <summary>
// 	/// 4 + 36*numS... Get/set External synchronization source...
// 	/// </summary>
// 	public abstract class UBX_CFG_ESRC : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-ESRC";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x60;
// 	}
// 	/// <summary>
// 	/// 8 + 12*numF... Get/set Geofencing configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_GEOFENCE : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-GEOFENCE";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x69;
// 	}
// 	/// <summary>
// 	/// 4 + 8*numCo... Get/set GNSS system configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_GNSS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-GNSS";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x3E;
// 	}
// 	/// <summary>
// 	/// 4 Get/set High navigation rate settings
// 	/// </summary>
// 	public abstract class UBX_CFG_HNR : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-HNR";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x5C;
// 	}
// 	/// <summary>
// 	/// 1 Poll Request Poll configuration for one protocol
// 	/// </summary>
// 	public abstract class UBX_CFG_INF : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-INF";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x02;
// 	}
// 	/// <summary>
// 	/// 0 + 10*N Get/set Information message configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_INF : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-INF";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x02;
// 	}
// 	/// <summary>
// 	/// 8 Get/set Jamming/interference monitor...
// 	/// </summary>
// 	public abstract class UBX_CFG_ITFM : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-ITFM";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x39;
// 	}
// 	/// <summary>
// 	/// 12 Get/set Data logger configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_LOGFILTER : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-LOGFILTER";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x47;
// 	}
// 	/// <summary>
// 	/// 2 Poll Request Poll a message configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_MSG : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-MSG";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x01;
// 	}
// 	/// <summary>
// 	/// 8 Get/set Set message rate(s)
// 	/// </summary>
// 	public abstract class UBX_CFG_MSG : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-MSG";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x01;
// 	}
// 	/// <summary>
// 	/// 3 Get/set Set message rate
// 	/// </summary>
// 	public abstract class UBX_CFG_MSG : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-MSG";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x01;
// 	}
// 	UBX-CFG-NAV5	 0x06	 0x24	 36 Get/set Navigation engine settings
// UBX-CFG-NAVX5	 0x06	 0x23	 40 Get/set Navigation engine expert settings
// UBX-CFG-NAVX5	 0x06	 0x23	 40 Get/set Navigation engine expert settings
// UBX-CFG-NAVX5	 0x06	 0x23	 44 Get/set Navigation engine expert settings
// /// <summary>
// /// 4 Get/set NMEA protocol configuration...
// /// </summary>
// public abstract class UBX_CFG_NMEA : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-NMEA";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x17;
// 	}
// 	/// <summary>
// 	/// 12 Get/set NMEA protocol configuration V0...
// 	/// </summary>
// 	public abstract class UBX_CFG_NMEA : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-NMEA";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x17;
// 	}
// 	/// <summary>
// 	/// 20 Get/set Extended NMEA protocol configuration V1
// 	/// </summary>
// 	public abstract class UBX_CFG_NMEA : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-NMEA";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x17;
// 	}
// 	/// <summary>
// 	/// 20 Get/set Odometer, low-speed COG engine...
// 	/// </summary>
// 	public abstract class UBX_CFG_ODO : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-ODO";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x1E;
// 	}
// // 	UBX-CFG-PM2	 0x06	 0x3B	 44 Get/set Extended power management...
// // UBX-CFG-PM2	 0x06	 0x3B	 48 Get/set Extended power management...
// // UBX-CFG-PM2	 0x06	 0x3B	 48 Get/set Extended power management...
// /// <summary>
// /// 8 Get/set Power mode setup
// /// </summary>
// public abstract class UBX_CFG_PMS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-PMS";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x86;
// 	}
// 	/// <summary>
// 	/// 1 Poll Request Polls the configuration for one I/O port
// 	/// </summary>
// 	public abstract class UBX_CFG_PRT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-PRT";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x00;
// 	}
// 	/// <summary>
// 	/// 20 Get/set Port configuration for UART ports
// 	/// </summary>
// 	public abstract class UBX_CFG_PRT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-PRT";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x00;
// 	}
// 	/// <summary>
// 	/// 20 Get/set Port configuration for USB port
// 	/// </summary>
// 	public abstract class UBX_CFG_PRT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-PRT";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x00;
// 	}
// 	/// <summary>
// 	/// 20 Get/set Port configuration for SPI port
// 	/// </summary>
// 	public abstract class UBX_CFG_PRT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-PRT";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x00;
// 	}
// 	/// <summary>
// 	/// 20 Get/set Port configuration for I2C (DDC) port
// 	/// </summary>
// 	public abstract class UBX_CFG_PRT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-PRT";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x00;
// 	}
// 	/// <summary>
// 	/// 8 Set Put receiver in a defined power state
// 	/// </summary>
// 	public abstract class UBX_CFG_PWR : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-PWR";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x57;
// 	}
// 	/// <summary>
// 	/// 6 Get/set Navigation/measurement rate settings
// 	/// </summary>
// 	public abstract class UBX_CFG_RATE : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-RATE";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x08;
// 	}
// 	/// <summary>
// 	/// 1 + 1*N Get/set Contents of remote inventory
// 	/// </summary>
// 	public abstract class UBX_CFG_RINV : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-RINV";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x34;
// 	}
// 	/// <summary>
// 	/// 4 Command Reset receiver / Clear backup data...
// 	/// </summary>
// 	public abstract class UBX_CFG_RST : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-RST";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x04;
// 	}
// 	/// <summary>
// 	/// 2 Get/set RXM configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_RXM : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-RXM";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x11;
// 	}
// 	/// <summary>
// 	/// 2 Get/set RXM configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_RXM : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-RXM";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x11;
// 	}
// 	/// <summary>
// 	/// 8 Get/set SBAS configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_SBAS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-SBAS";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x16;
// 	}
// 	/// <summary>
// 	/// 6 Get/set I2C sensor interface configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_SENIF : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-SENIF";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x88;
// 	}
// 	/// <summary>
// 	/// 4 Get/set SLAS configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_SLAS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-SLAS";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x8D;
// 	}
// 	/// <summary>
// 	/// 20 Get/set Synchronization manager configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_SMGR : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-SMGR";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x62;
// 	}
// 	/// <summary>
// 	/// 12 Get/set Configure and start a sensor...
// 	/// </summary>
// 	public abstract class UBX_CFG_SPT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-SPT";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x64;
// 	}
// // 	UBX-CFG-TMODE2	 0x06	 0x3D	 28 Get/set Time mode settings 2
// // UBX-CFG-TMODE3	 0x06	 0x71	 40 Get/set Time mode settings 3
// // UBX-CFG-TP5	 0x06	 0x31	 0 Poll Request Poll time pulse parameters for time...
// // UBX-CFG-TP5	 0x06	 0x31	 1 Poll Request Poll time pulse parameters
// // UBX-CFG-TP5	 0x06	 0x31	 32 Get/set Time pulse parameters
// // UBX-CFG-TP5	 0x06	 0x31	 32 Get/set Time pulse parameters
// /// <summary>
// /// 16 Set TX buffer time slots configuration
// /// </summary>
// 	public abstract class UBX_CFG_TXSLOT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-TXSLOT";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x53;
// 	}
// 	/// <summary>
// 	/// 108 Get/set USB configuration
// 	/// </summary>
// 	public abstract class UBX_CFG_USB : UbxMessageBase
// 	{
// 		public override string Name => "UBX-CFG-USB";
// 		public override byte Class => 0x06;
// 		public override byte SubClass => 0x1B;
// 	}
// 	/// <summary>
// 	/// 16 Periodic/Polled IMU alignment information
// 	/// </summary>
// 	public abstract class UBX_ESF_ALG : UbxMessageBase
// 	{
// 		public override string Name => "UBX-ESF-ALG";
// 		public override byte Class => 0x10;
// 		public override byte SubClass => 0x14;
// 	}
// 	/// <summary>
// 	/// 36 Periodic/Polled Vehicle dynamics information
// 	/// </summary>
// 	public abstract class UBX_ESF_INS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-ESF-INS";
// 		public override byte Class => 0x10;
// 		public override byte SubClass => 0x15;
// 	}
// 	/// <summary>
// 	/// (8 + 4*numM... Input/Output External sensor fusion measurements
// 	/// </summary>
// 	public abstract class UBX_ESF_MEAS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-ESF-MEAS";
// 		public override byte Class => 0x10;
// 		public override byte SubClass => 0x02;
// 	}
// 	/// <summary>
// 	/// 4 + 8*N Output Raw sensor measurements
// 	/// </summary>
// 	public abstract class UBX_ESF_RAW : UbxMessageBase
// 	{
// 		public override string Name => "UBX-ESF-RAW";
// 		public override byte Class => 0x10;
// 		public override byte SubClass => 0x03;
// 	}
// 	/// <summary>
// 	/// 16 + 4*numS... Periodic/Polled External sensor fusion status
// 	/// </summary>
// 	public abstract class UBX_ESF_STATUS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-ESF-STATUS";
// 		public override byte Class => 0x10;
// 		public override byte SubClass => 0x10;
// 	}
// 	/// <summary>
// 	/// 32 Periodic/Polled Attitude solution
// 	/// </summary>
// 	public abstract class UBX_HNR_ATT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-HNR-ATT";
// 		public override byte Class => 0x28;
// 		public override byte SubClass => 0x01;
// 	}
// 	/// <summary>
// 	/// 36 Periodic/Polled Vehicle dynamics information
// 	/// </summary>
// 	public abstract class UBX_HNR_INS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-HNR-INS";
// 		public override byte Class => 0x28;
// 		public override byte SubClass => 0x02;
// 	}
// 	/// <summary>
// 	/// 72 Periodic/Polled High rate output of PVT solution
// 	/// </summary>
// 	public abstract class UBX_HNR_PVT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-HNR-PVT";
// 		public override byte Class => 0x28;
// 		public override byte SubClass => 0x00;
// 	}

// 	/// <summary>
// 	/// 0 + 1*N Output ASCII output with warning contents
// 	/// </summary>
// 	public abstract class UBX_INF_WARNING : UbxMessageBase
// 	{
// 		public override string Name => "UBX-INF-WARNING";
// 		public override byte Class => 0x04;
// 		public override byte SubClass => 0x01;
// 	}
// 	/// <summary>
// 	/// 100 Polled Batched data
// 	/// </summary>
// 	public abstract class UBX_LOG_BATCH : UbxMessageBase
// 	{
// 		public override string Name => "UBX-LOG-BATCH";
// 		public override byte Class => 0x21;
// 		public override byte SubClass => 0x11;
// 	}
// 	/// <summary>
// 	/// 8 Command Create log file
// 	/// </summary>
// 	public abstract class UBX_LOG_CREATE : UbxMessageBase
// 	{
// 		public override string Name => "UBX-LOG-CREATE";
// 		public override byte Class => 0x21;
// 		public override byte SubClass => 0x07;
// 	}
// 	/// <summary>
// 	/// 0 Command Erase logged data
// 	/// </summary>
// 	public abstract class UBX_LOG_ERASE : UbxMessageBase
// 	{
// 		public override string Name => "UBX-LOG-ERASE";
// 		public override byte Class => 0x21;
// 		public override byte SubClass => 0x03;
// 	}
// 	/// <summary>
// 	/// 12 Input Find index of a log entry based on a...
// 	/// </summary>
// 	public abstract class UBX_LOG_FINDTIME : UbxMessageBase
// 	{
// 		public override string Name => "UBX-LOG-FINDTIME";
// 		public override byte Class => 0x21;
// 		public override byte SubClass => 0x0E;
// 	}
// 	/// <summary>
// 	/// 8 Output Response to FINDTIME request
// 	/// </summary>
// 	public abstract class UBX_LOG_FINDTIME : UbxMessageBase
// 	{
// 		public override string Name => "UBX-LOG-FINDTIME";
// 		public override byte Class => 0x21;
// 		public override byte SubClass => 0x0E;
// 	}
// 	/// <summary>
// 	/// 0 Poll Request Poll for log information
// 	/// </summary>
// 	public abstract class UBX_LOG_INFO : UbxMessageBase
// 	{
// 		public override string Name => "UBX-LOG-INFO";
// 		public override byte Class => 0x21;
// 		public override byte SubClass => 0x08;
// 	}
// 	/// <summary>
// 	/// 48 Output Log information
// 	/// </summary>
// 	public abstract class UBX_LOG_INFO : UbxMessageBase
// 	{
// 		public override string Name => "UBX-LOG-INFO";
// 		public override byte Class => 0x21;
// 		public override byte SubClass => 0x08;
// 	}
// // 	UBX-LOG-RETRIEVEBA...  0x21	 0x10 4 Command Request batch data
// // UBX-LOG-RETRIEVEPO...  0x21	 0x0f 32 Output Odometer log entry
// /// <summary>
// /// 40 Output Position fix log entry
// /// </summary>
// 	public abstract class UBX_LOG_RETRIEVEPOS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-LOG-RETRIEVEPOS";
// 		public override byte Class => 0x21;
// 		public override byte SubClass => 0x0b;
// 	}
// 	//UBX-LOG-RETRIEVEST...  0x21	 0x0d 16 + 1* byteCo... Output Byte string log entry
// /// <summary>
// /// 12 Command Request log data
// /// </summary>
// public abstract class UBX_LOG_RETRIEVE : UbxMessageBase
// 	{
// 		public override string Name => "UBX-LOG-RETRIEVE";
// 		public override byte Class => 0x21;
// 		public override byte SubClass => 0x09;
// 	}
// 	/// <summary>
// 	/// 0 + 1*N Command Store arbitrary string in on-board flash
// 	/// </summary>
// 	public abstract class UBX_LOG_STRING : UbxMessageBase
// 	{
// 		public override string Name => "UBX-LOG-STRING";
// 		public override byte Class => 0x21;
// 		public override byte SubClass => 0x04;
// 	}
// //	UBX-MGA-ACK	#ИМЯ?	 0x13	 0x60 8 Output Multiple GNSS acknowledge message
// /// <summary>
// /// 76 Input Multiple GNSS AssistNow Offline...
// /// </summary>
// public abstract class UBX_MGA_ANO : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MGA-ANO";
// 		public override byte Class => 0x13;
// 		public override byte SubClass => 0x20;
// 	}
// // 	UBX-MGA-BDS	#ИМЯ?	 0x13	 0x03 88 Input BeiDou ephemeris assistance
// // UBX-MGA-BDS	#ИМЯ?	 0x13	 0x03 40 Input BeiDou almanac assistance
// // UBX-MGA-BDS	#ИМЯ?	 0x13	 0x03 68 Input BeiDou health assistance
// // UBX-MGA-BDS	#ИМЯ?	 0x13	 0x03 20 Input BeiDou UTC assistance
// // UBX-MGA-BDS	#ИМЯ?	 0x13	 0x03 16 Input BeiDou ionosphere assistance
// /// <summary>
// /// 0 Poll Request Poll the navigation database
// /// </summary>
// public abstract class UBX_MGA_DBD : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MGA-DBD";
// 		public override byte Class => 0x13;
// 		public override byte SubClass => 0x80;
// 	}
// 	/// <summary>
// 	/// 12 + 1*N Input/Output Navigation database dump entry
// 	/// </summary>
// 	public abstract class UBX_MGA_DBD : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MGA-DBD";
// 		public override byte Class => 0x13;
// 		public override byte SubClass => 0x80;
// 	}
// // 	UBX-MGA-FLASH	#ИМЯ?	 0x13	 0x21 6 + 1*size Input Transfer MGA-ANO data block to flash
// // UBX-MGA-FLASH	#ИМЯ?	 0x13	 0x21 2 Input Finish flashing MGA-ANO data
// // UBX-MGA-FLASH	#ИМЯ?	 0x13	 0x21 6 Output Acknowledge last FLASH-DATA or -STOP
// // UBX-MGA-GAL	#ИМЯ?	 0x13	 0x02 76 Input Galileo ephemeris assistance
// // UBX-MGA-GAL	#ИМЯ?	 0x13	 0x02 32 Input Galileo almanac assistance
// // UBX-MGA-GAL	#ИМЯ?	...	 0x13 0x02 12 Input Galileo GPS time offset assistance
// // UBX-MGA-GAL	#ИМЯ?	 0x13	 0x02 20 Input Galileo UTC assistance
// // UBX-MGA-GLO	#ИМЯ?	 0x13	 0x06 48 Input GLONASS ephemeris assistance
// // UBX-MGA-GLO	#ИМЯ?	 0x13	 0x06 36 Input GLONASS almanac assistance
// // UBX-MGA-GLO	#ИМЯ?	...	 0x13 0x06 20 Input GLONASS auxiliary time offset assistance
// // UBX-MGA-GPS	#ИМЯ?	 0x13	 0x00 68 Input GPS ephemeris assistance
// // UBX-MGA-GPS	#ИМЯ?	 0x13	 0x00 36 Input GPS almanac assistance
// // UBX-MGA-GPS	#ИМЯ?	 0x13	 0x00 40 Input GPS health assistance
// // UBX-MGA-GPS	#ИМЯ?	 0x13	 0x00 20 Input GPS UTC assistance
// // UBX-MGA-GPS	#ИМЯ?	 0x13	 0x00 16 Input GPS ionosphere assistance
// // UBX-MGA-INI	#ИМЯ?	 0x13	 0x40 20 Input Initial position assistance
// // UBX-MGA-INI	#ИМЯ?	 0x13	 0x40 20 Input Initial position assistance
// // UBX-MGA-INI	#ИМЯ?	 0x13	 0x40 24 Input Initial time assistance
// // UBX-MGA-INI	#ИМЯ?	...	 0x13 0x40 24 Input Initial time assistance
// // UBX-MGA-INI	#ИМЯ?	 0x13	 0x40 12 Input Initial clock drift assistance
// // UBX-MGA-INI	#ИМЯ?	 0x13	 0x40 12 Input Initial frequency assistance
// // UBX-MGA-INI	#ИМЯ?	 0x13	 0x40 72 Input Earth orientation parameters assistance
// // UBX-MGA-QZSS	#ИМЯ?	 0x13	 0x05 68 Input QZSS ephemeris assistance
// // UBX-MGA-QZSS	#ИМЯ?	 0x13	 0x05 36 Input QZSS almanac assistance
// // UBX-MGA-QZSS	#ИМЯ?	...	 0x13 0x05 12 Input QZSS health assistance
// /// <summary>
// /// 12 Polled Data batching buffer status
// /// </summary>
// public abstract class UBX_MON_BATCH : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-BATCH";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x32;
// 	}
// 	/// <summary>
// 	/// 8 Polled Information message major GNSS...
// 	/// </summary>
// 	public abstract class UBX_MON_GNSS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-GNSS";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x28;
// 	}
// 	// UBX-MON-HW2	 0x0A	 0x0B	 28 Periodic/Polled Extended hardware status
// /// <summary>
// /// 60 Periodic/polled Hardware status
// /// </summary>
// 	public abstract class UBX_MON_HW : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-HW";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x09;
// 	}
// 	/// <summary>
// 	/// 0 + 20*N Periodic/Polled I/O system status
// 	/// </summary>
// 	public abstract class UBX_MON_IO : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-IO";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x02;
// 	}
// 	/// <summary>
// 	/// 120 Periodic/Polled Message parse and process status
// 	/// </summary>
// 	public abstract class UBX_MON_MSGPP : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-MSGPP";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x06;
// 	}
// 	/// <summary>
// 	/// 0 Poll Request Poll request for installed patches
// 	/// </summary>
// 	public abstract class UBX_MON_PATCH : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-PATCH";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x27;
// 	}
// 	/// <summary>
// 	/// 4 + 16*nEntries Polled Installed patches
// 	/// </summary>
// 	public abstract class UBX_MON_PATCH : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-PATCH";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x27;
// 	}
// 	/// <summary>
// 	/// 24 Periodic/Polled Receiver buffer status
// 	/// </summary>
// 	public abstract class UBX_MON_RXBUF : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-RXBUF";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x07;
// 	}
// 	/// <summary>
// 	/// 1 Output Receiver status information
// 	/// </summary>
// 	public abstract class UBX_MON_RXR : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-RXR";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x21;
// 	}
// 	/// <summary>
// 	/// 16 Periodic/Polled Synchronization manager status
// 	/// </summary>
// 	public abstract class UBX_MON_SMGR : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-SMGR";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x2E;
// 	}
// 	/// <summary>
// 	/// 4 + 12*numR... Polled Sensor production test
// 	/// </summary>
// 	public abstract class UBX_MON_SPT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-SPT";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x2F;
// 	}
// 	/// <summary>
// 	/// 28 Periodic/Polled Transmitter buffer status
// 	/// </summary>
// 	public abstract class UBX_MON_TXBUF : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-TXBUF";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x08;
// 	}
// 	/// <summary>
// 	/// 0 Poll Request Poll receiver and software version
// 	/// </summary>
// 	public abstract class UBX_MON_VER : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-VER";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x04;
// 	}
// 	/// <summary>
// 	/// 40 + 30*N Polled Receiver and software version
// 	/// </summary>
// 	public abstract class UBX_MON_VER : UbxMessageBase
// 	{
// 		public override string Name => "UBX-MON-VER";
// 		public override byte Class => 0x0A;
// 		public override byte SubClass => 0x04;
// 	}
// 	/// <summary>
// 	/// 16 Periodic/Polled AssistNow Autonomous status
// 	/// </summary>
// 	public abstract class UBX_NAV_AOPSTATUS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-AOPSTATUS";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x60;
// 	}
// 	/// <summary>
// 	/// 32 Periodic/Polled Attitude solution
// 	/// </summary>
// 	public abstract class UBX_NAV_ATT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-ATT";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x05;
// 	}
// 	/// <summary>
// 	/// 20 Periodic/Polled Clock solution
// 	/// </summary>
// 	public abstract class UBX_NAV_CLOCK : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-CLOCK";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x22;
// 	}
// 	/// <summary>
// 	/// 64 Periodic/Polled Covariance matrices
// 	/// </summary>
// 	public abstract class UBX_NAV_COV : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-COV";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x36;
// 	}
// 	/// <summary>
// 	/// 16 + 12*numCh Periodic/Polled DGPS data used for NAV
// 	/// </summary>
// 	public abstract class UBX_NAV_DGPS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-DGPS";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x31;
// 	}
// 	/// <summary>
// 	/// 18 Periodic/Polled Dilution of precision
// 	/// </summary>
// 	public abstract class UBX_NAV_DOP : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-DOP";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x04;
// 	}
// 	/// <summary>
// 	/// 16 Periodic/Polled Position error ellipse parameters
// 	/// </summary>
// 	public abstract class UBX_NAV_EELL : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-EELL";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x3d;
// 	}
// 	/// <summary>
// 	/// 4 Periodic End of epoch
// 	/// </summary>
// 	public abstract class UBX_NAV_EOE : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-EOE";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x61;
// 	}
// 	/// <summary>
// 	/// 8 + 2*numFe... Periodic/Polled Geofencing status
// 	/// </summary>
// 	public abstract class UBX_NAV_GEOFENCE : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-GEOFENCE";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x39;
// 	}
// 	/// <summary>
// 	/// 28 Periodic/Polled High precision position solution in ECEF
// 	/// </summary>
// 	public abstract class UBX_NAV_HPPOSECEF : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-HPPOSECEF";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x13;
// 	}
// 	/// <summary>
// 	/// 36 Periodic/Polled High precision geodetic position solution
// 	/// </summary>
// 	public abstract class UBX_NAV_HPPOSLLH : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-HPPOSLLH";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x14;
// 	}
// 	/// <summary>
// 	/// 16 Periodic/Polled Navigation message cross-check...
// 	/// </summary>
// 	public abstract class UBX_NAV_NMI : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-NMI";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x28;
// 	}
// 	/// <summary>
// 	/// 20 Periodic/Polled Odometer solution
// 	/// </summary>
// 	public abstract class UBX_NAV_ODO : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-ODO";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x09;
// 	}
// 	/// <summary>
// 	/// 8 + 6*numSv Periodic/Polled GNSS orbit database info
// 	/// </summary>
// 	public abstract class UBX_NAV_ORB : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-ORB";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x34;
// 	}
// 	/// <summary>
// 	/// 20 Periodic/Polled Position solution in ECEF
// 	/// </summary>
// 	public abstract class UBX_NAV_POSECEF : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-POSECEF";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x01;
// 	}
// 	/// <summary>
// 	/// 28 Periodic/Polled Geodetic position solution
// 	/// </summary>
// 	public abstract class UBX_NAV_POSLLH : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-POSLLH";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x02;
// 	}
// 	/// <summary>
// 	/// 92 Periodic/Polled Navigation position velocity time solution
// 	/// </summary>
// 	public abstract class UBX_NAV_PVT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-PVT";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x07;
// 	}
// 	/// <summary>
// 	/// 40 Periodic/Polled Relative positioning information in...
// 	/// </summary>
// 	public abstract class UBX_NAV_RELPOSNED : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-RELPOSNED";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x3C;
// 	}
// 	/// <summary>
// 	/// 0 Command Reset odometer
// 	/// </summary>
// 	public abstract class UBX_NAV_RESETODO : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-RESETODO";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x10;
// 	}
// 	/// <summary>
// 	/// 8 + 12*numSvs Periodic/Polled Satellite information
// 	/// </summary>
// 	public abstract class UBX_NAV_SAT : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-SAT";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x35;
// 	}
// 	/// <summary>
// 	/// 12 + 12*cnt Periodic/Polled SBAS status data
// 	/// </summary>
// 	public abstract class UBX_NAV_SBAS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-SBAS";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x32;
// 	}
// 	/// <summary>
// 	/// 20 + 8*cnt Periodic/Polled QZSS L1S SLAS status data
// 	/// </summary>
// 	public abstract class UBX_NAV_SLAS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-SLAS";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x42;
// 	}
// 	/// <summary>
// 	/// 52 Periodic/Polled Navigation solution information
// 	/// </summary>
// 	public abstract class UBX_NAV_SOL : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-SOL";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x06;
// 	}
// 	/// <summary>
// 	/// 16 Periodic/Polled Receiver navigation status
// 	/// </summary>
// 	public abstract class UBX_NAV_STATUS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-STATUS";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x03;
// 	}
// 	/// <summary>
// 	/// 8 + 12*numCh Periodic/Polled Space vehicle information
// 	/// </summary>
// 	public abstract class UBX_NAV_SVINFO : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-SVINFO";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x30;
// 	}
// 	/// <summary>
// 	/// 40 Periodic/Polled Survey-in data
// 	/// </summary>
// 	public abstract class UBX_NAV_SVIN : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-SVIN";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x3B;
// 	}
// 	/// <summary>
// 	/// 20 Periodic/Polled BeiDou time solution
// 	/// </summary>
// 	public abstract class UBX_NAV_TIMEBDS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-TIMEBDS";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x24;
// 	}
// 	/// <summary>
// 	/// 20 Periodic/Polled Galileo time solution
// 	/// </summary>
// 	public abstract class UBX_NAV_TIMEGAL : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-TIMEGAL";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x25;
// 	}
// 	/// <summary>
// 	/// 20 Periodic/Polled GLONASS time solution
// 	/// </summary>
// 	public abstract class UBX_NAV_TIMEGLO : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-TIMEGLO";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x23;
// 	}
// 	/// <summary>
// 	/// 16 Periodic/Polled GPS time solution
// 	/// </summary>
// 	public abstract class UBX_NAV_TIMEGPS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-TIMEGPS";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x20;
// 	}
// 	/// <summary>
// 	/// 24 Periodic/Polled Leap second event information
// 	/// </summary>
// 	public abstract class UBX_NAV_TIMELS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-TIMELS";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x26;
// 	}
// 	/// <summary>
// 	/// 20 Periodic/Polled UTC time solution
// 	/// </summary>
// 	public abstract class UBX_NAV_TIMEUTC : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-TIMEUTC";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x21;
// 	}
// 	/// <summary>
// 	/// 20 Periodic/Polled Velocity solution in ECEF
// 	/// </summary>
// 	public abstract class UBX_NAV_VELECEF : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-VELECEF";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x11;
// 	}
// 	/// <summary>
// 	/// 36 Periodic/Polled Velocity solution in NED frame
// 	/// </summary>
// 	public abstract class UBX_NAV_VELNED : UbxMessageBase
// 	{
// 		public override string Name => "UBX-NAV-VELNED";
// 		public override byte Class => 0x01;
// 		public override byte SubClass => 0x12;
// 	}
// 	/// <summary>
// 	/// 4 + 44*numTx Periodic/Polled Indoor Messaging System information
// 	/// </summary>
// 	public abstract class UBX_RXM_IMES : UbxMessageBase
// 	{
// 		public override string Name => "UBX-RXM-IMES";
// 		public override byte Class => 0x02;
// 		public override byte SubClass => 0x61;
// 	}
// 	/// <summary>
// 	/// 44 + 24*num... Periodic/Polled Satellite measurements for RRLP
// 	/// </summary>
// 	public abstract class UBX_RXM_MEASX : UbxMessageBase
// 	{
// 		public override string Name => "UBX-RXM-MEASX";
// 		public override byte Class => 0x02;
// 		public override byte SubClass => 0x14;
// 	}
// 	/// <summary>
// 	/// 8 Command Power management request
// 	/// </summary>
// 	public abstract class UBX_RXM_PMREQ : UbxMessageBase
// 	{
// 		public override string Name => "UBX-RXM-PMREQ";
// 		public override byte Class => 0x02;
// 		public override byte SubClass => 0x41;
// 	}
// 	/// <summary>
// 	/// 16 Command Power management request
// 	/// </summary>
// 	public abstract class UBX_RXM_PMREQ : UbxMessageBase
// 	{
// 		public override string Name => "UBX-RXM-PMREQ";
// 		public override byte Class => 0x02;
// 		public override byte SubClass => 0x41;
// 	}
// 	/// <summary>
// 	/// 16 + 32*num... Periodic/Polled Multi-GNSS raw measurement data
// 	/// </summary>
// 	public abstract class UBX_RXM_RAWX : UbxMessageBase
// 	{
// 		public override string Name => "UBX-RXM-RAWX";
// 		public override byte Class => 0x02;
// 		public override byte SubClass => 0x15;
// 	}
// 	/// <summary>
// 	/// 16 + 32*num... Periodic/Polled Multi-GNSS raw measurements
// 	/// </summary>
// 	public abstract class UBX_RXM_RAWX : UbxMessageBase
// 	{
// 		public override string Name => "UBX-RXM-RAWX";
// 		public override byte Class => 0x02;
// 		public override byte SubClass => 0x15;
// 	}
// 	/// <summary>
// 	/// 16 Output Galileo SAR short-RLM report
// 	/// </summary>
// 	public abstract class UBX_RXM_RLM : UbxMessageBase
// 	{
// 		public override string Name => "UBX-RXM-RLM";
// 		public override byte Class => 0x02;
// 		public override byte SubClass => 0x59;
// 	}
// 	/// <summary>
// 	/// 28 Output Galileo SAR long-RLM report
// 	/// </summary>
// 	public abstract class UBX_RXM_RLM : UbxMessageBase
// 	{
// 		public override string Name => "UBX-RXM-RLM";
// 		public override byte Class => 0x02;
// 		public override byte SubClass => 0x59;
// 	}
// 	/// <summary>
// 	/// 8 Output RTCM input status
// 	/// </summary>
// 	public abstract class UBX_RXM_RTCM : UbxMessageBase
// 	{
// 		public override string Name => "UBX-RXM-RTCM";
// 		public override byte Class => 0x02;
// 		public override byte SubClass => 0x32;
// 	}
// 	/// <summary>
// 	/// 8 + 4*numW... Output Broadcast navigation data subframe
// 	/// </summary>
// 	public abstract class UBX_RXM_SFRBX : UbxMessageBase
// 	{
// 		public override string Name => "UBX-RXM-SFRBX";
// 		public override byte Class => 0x02;
// 		public override byte SubClass => 0x13;
// 	}
// 	/// <summary>
// 	/// 8 + 4*numW... Output Broadcast navigation data subframe
// 	/// </summary>
// 	public abstract class UBX_RXM_SFRBX : UbxMessageBase
// 	{
// 		public override string Name => "UBX-RXM-SFRBX";
// 		public override byte Class => 0x02;
// 		public override byte SubClass => 0x13;
// 	}
// 	/// <summary>
// 	/// 8 + 6*numSV Periodic/Polled SV status info
// 	/// </summary>
// 	public abstract class UBX_RXM_SVSI : UbxMessageBase
// 	{
// 		public override string Name => "UBX-RXM-SVSI";
// 		public override byte Class => 0x02;
// 		public override byte SubClass => 0x20;
// 	}
// 	/// <summary>
// 	/// 9 Output Unique chip ID
// 	/// </summary>
// 	public abstract class UBX_SEC_UNIQID : UbxMessageBase
// 	{
// 		public override string Name => "UBX-SEC-UNIQID";
// 		public override byte Class => 0x27;
// 		public override byte SubClass => 0x03;
// 	}
// 	/// <summary>
// 	/// 8 Output Disciplined oscillator control
// 	/// </summary>
// 	public abstract class UBX_TIM_DOSC : UbxMessageBase
// 	{
// 		public override string Name => "UBX-TIM-DOSC";
// 		public override byte Class => 0x0D;
// 		public override byte SubClass => 0x11;
// 	}
// 	/// <summary>
// 	/// 32 Periodic/Polled Oscillator frequency changed notification
// 	/// </summary>
// 	public abstract class UBX_TIM_FCHG : UbxMessageBase
// 	{
// 		public override string Name => "UBX-TIM-FCHG";
// 		public override byte Class => 0x0D;
// 		public override byte SubClass => 0x16;
// 	}
// 	/// <summary>
// 	/// 8 Input Host oscillator control
// 	/// </summary>
// 	public abstract class UBX_TIM_HOC : UbxMessageBase
// 	{
// 		public override string Name => "UBX-TIM-HOC";
// 		public override byte Class => 0x0D;
// 		public override byte SubClass => 0x17;
// 	}
// 	/// <summary>
// 	/// 12 + 24*num... Input/Output Source measurement
// 	/// </summary>
// 	public abstract class UBX_TIM_SMEAS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-TIM-SMEAS";
// 		public override byte Class => 0x0D;
// 		public override byte SubClass => 0x13;
// 	}
// 	/// <summary>
// 	/// 28 Periodic/Polled Survey-in data
// 	/// </summary>
// 	public abstract class UBX_TIM_SVIN : UbxMessageBase
// 	{
// 		public override string Name => "UBX-TIM-SVIN";
// 		public override byte Class => 0x0D;
// 		public override byte SubClass => 0x04;
// 	}
// /// <summary>
// /// 56 Periodic Time pulse time and frequency data
// /// </summary>
// 	public abstract class UBX_TIM_TOS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-TIM-TOS";
// 		public override byte Class => 0x0D;
// 		public override byte SubClass => 0x12;
// 	}
// 	/// <summary>
// 	/// 16 Periodic/Polled Time pulse time data
// 	/// </summary>
// 	public abstract class UBX_TIM_TP : UbxMessageBase
// 	{
// 		public override string Name => "UBX-TIM-TP";
// 		public override byte Class => 0x0D;
// 		public override byte SubClass => 0x01;
// 	}
// 	/// <summary>
// 	/// 1 Command Stop calibration
// 	/// </summary>
// 	public abstract class UBX_TIM_VCOCAL : UbxMessageBase
// 	{
// 		public override string Name => "UBX-TIM-VCOCAL";
// 		public override byte Class => 0x0D;
// 		public override byte SubClass => 0x15;
// 	}
// 	/// <summary>
// 	/// 12 Command VCO calibration extended command
// 	/// </summary>
// 	public abstract class UBX_TIM_VCOCAL : UbxMessageBase
// 	{
// 		public override string Name => "UBX-TIM-VCOCAL";
// 		public override byte Class => 0x0D;
// 		public override byte SubClass => 0x15;
// 	}
// 	/// <summary>
// 	/// 12 Periodic/Polled Results of the calibration
// 	/// </summary>
// 	public abstract class UBX_TIM_VCOCAL : UbxMessageBase
// 	{
// 		public override string Name => "UBX-TIM-VCOCAL";
// 		public override byte Class => 0x0D;
// 		public override byte SubClass => 0x15;
// 	}
// 	/// <summary>
// 	/// 20 Periodic/Polled Sourced time verification
// 	/// </summary>
// 	public abstract class UBX_TIM_VRFY : UbxMessageBase
// 	{
// 		public override string Name => "UBX-TIM-VRFY";
// 		public override byte Class => 0x0D;
// 		public override byte SubClass => 0x06;
// 	}
// 	/// <summary>
// 	/// 0 Poll Request Poll backup restore status
// 	/// </summary>
// 	public abstract class UBX_UPD_SOS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-UPD-SOS";
// 		public override byte Class => 0x09;
// 		public override byte SubClass => 0x14;
// 	}
// 	/// <summary>
// 	/// 4 Command Create backup in flash
// 	/// </summary>
// 	public abstract class UBX_UPD_SOS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-UPD-SOS";
// 		public override byte Class => 0x09;
// 		public override byte SubClass => 0x14;
// 	}
// 	/// <summary>
// 	/// 4 Command Clear backup in flash
// 	/// </summary>
// 	public abstract class UBX_UPD_SOS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-UPD-SOS";
// 		public override byte Class => 0x09;
// 		public override byte SubClass => 0x14;
// 	}
// 	/// <summary>
// 	/// 8 Output Backup creation acknowledge
// 	/// </summary>
// 	public abstract class UBX_UPD_SOS : UbxMessageBase
// 	{
// 		public override string Name => "UBX-UPD-SOS";
// 		public override byte Class => 0x09;
// 		public override byte SubClass => 0x14;
// 	}

}