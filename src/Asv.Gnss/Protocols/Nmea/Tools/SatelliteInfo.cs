using System;
using Asv.IO;

namespace Asv.Gnss;

public class SatelliteInfo
{
    private readonly int? _nmeaPrn;
    private readonly int? _elevation;
    private readonly int? _azimuth;
    private readonly int? _snr;
    private readonly int _extPrn;
    private readonly NmeaNavigationSystemEnum _extNavSys;

    public SatelliteInfo(NmeaTalkerId id, int nmeaPrn,
        int? elevation,
        int? azimuth,
        int? snr)    
    {
        _nmeaPrn = nmeaPrn;
        _elevation = elevation;
        _azimuth = azimuth;
        _snr = snr;
        NmeaProtocol.GetPrnFromNmeaSatId(id, nmeaPrn, out _extPrn, out _extNavSys);
        
    }

    public int? NmeaPrn => _nmeaPrn;
    public int? Elevation => _elevation;
    public int? Azimuth => _azimuth;
    public int? Snr => _snr;

    public int ExtPrn => _extPrn;
    public NmeaNavigationSystemEnum ExtNavSys => _extNavSys;
}