using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GeoLocation
/// </summary>
public class GeoLocation
{
    public decimal Lat { get; set; }
    public decimal Lng { get; set; }

    public override string ToString()
    {
        return string.Concat(Lat, " ", Lng);
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as GeoLocation);
    }

    public override int GetHashCode()
    {
        return Lat.GetHashCode() * Lng.GetHashCode();
    }

    public bool Equals(GeoLocation loc)
    {
        if (Object.ReferenceEquals(loc, null))
        {
            return false;
        }

        if (Object.ReferenceEquals(this, loc))
        {
            return true;
        }

        if (this.GetType() != loc.GetType())
            return false;

        return this.ToString() == loc.ToString(); // Instead of comparing numeric values, two instances are "equal" when their string representations are identical.
    }
}

public class GeoLocationComparer: IEqualityComparer<GeoLocation>
{
    public bool Equals(GeoLocation x, GeoLocation y)
    {
        return x.Equals(y);
    }

    public int GetHashCode(GeoLocation obj)
    {
        return obj.GetHashCode();
    }
}