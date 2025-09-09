using System;

namespace Asv.Gnss;

/// <summary>
/// The GlonassWordFactory class is responsible for creating GlonassWordBase objects based on the provided navigation bits.
/// </summary>
public static class GlonassWordFactory
{
    /// <summary>
    /// Creates a GlonassWordBase object based on the given navigation bits array.
    /// </summary>
    /// <param name="navBits">The array of navigation bits.</param>
    /// <returns>
    /// A GlonassWordBase object representing the created Glonass word.
    /// </returns>
    /// <exception cref="System.Exception">
    /// Thrown when the given Glonass word ID is unknown.
    /// </exception>
    public static GlonassWordBase Create(uint[] navBits)
    {
        var wordId = GlonassRawHelper.GetWordId(navBits);
        var data = GlonassRawHelper.GetRawData(navBits);
        GlonassWordBase subframe;
        switch (wordId)
        {
            case 1:
                subframe = new GlonassWord1();
                break;
            case 2:
                subframe = new GlonassWord2();
                break;
            case 3:
                subframe = new GlonassWord3();
                break;
            case 4:
                subframe = new GlonassWord4();
                break;
            case 5:
                subframe = new GlonassWord5();
                break;
            case 6:
            case 8:
            case 10:
            case 12:
            case 14:
                subframe = new GlonassWordEven();
                break;
            case 7:
            case 9:
            case 11:
            case 13:
            case 15:
                subframe = new GlonassWordOdd();
                break;
            default:
                throw new Exception($"Unknown Glonass word ID:{Convert.ToString(wordId, 2),-8}");
        }

        subframe.Deserialize(data);
        return subframe;
    }
}