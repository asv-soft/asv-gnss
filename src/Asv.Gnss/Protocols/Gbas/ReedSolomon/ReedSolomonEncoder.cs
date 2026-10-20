using System;
using System.Collections.Generic;
using System.Linq;
namespace Asv.Gnss;

/*
    * Copyright 2008 ZXing authors
    *
    * Licensed under the Apache License, Version 2.0 (the "License");
    * you may not use this file except in compliance with the License.
    * You may obtain a copy of the License at
    *
    *      http://www.apache.org/licenses/LICENSE-2.0
    *
    * Unless required by applicable law or agreed to in writing, software
    * distributed under the License is distributed on an "AS IS" BASIS,
    * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    * See the License for the specific language governing permissions and
    * limitations under the License.
    */

/// <summary>
/// Implements Reed-Solomon encoding, as the name implies.
/// </summary>
/// <author>Sean Owen</author>
/// <author>William Rucklidge</author>
internal sealed class ReedSolomonEncoder
{
    private readonly GenericGF field;
    private readonly IList<GenericGFPoly> cachedGenerators;

    public ReedSolomonEncoder(GenericGF field)
    {
        this.field = field;

        cachedGenerators = new List<GenericGFPoly>();
        cachedGenerators.Add(new GenericGFPoly(field, new int[] { 1 }));
    }

    private GenericGFPoly BuildGenerator(int degree)
    {
        if (degree >= cachedGenerators.Count)
        {
            GenericGFPoly lastGenerator = cachedGenerators[cachedGenerators.Count - 1];

            for (int d = cachedGenerators.Count; d <= degree; d++)
            {
                var nextGenerator =
                    lastGenerator.Multiply(new GenericGFPoly(field,
                        new int[] { 1, field.Exp(d - 1 + field.GeneratorBase) }));
                cachedGenerators.Add(nextGenerator);
                lastGenerator = nextGenerator;
            }
        }

        return cachedGenerators[degree];
    }

    public void Encode(int[] toEncode, int ecBytes)
    {
        if (ecBytes == 0)
            throw new ArgumentException("No error correction bytes");

        int dataBytes = toEncode.Length - ecBytes;

        if (dataBytes <= 0)
            throw new ArgumentException("No data bytes provided");

        GenericGFPoly generator = BuildGenerator(ecBytes);
        var infoCoefficients = new int[dataBytes];
        Array.Copy(toEncode, 0, infoCoefficients, 0, dataBytes);

        var info = new GenericGFPoly(field, infoCoefficients);
        info = info.MultiplyByMonomial(ecBytes, 1);

        GenericGFPoly remainder = info.Divide(generator)[1];
        int[] coefficients = remainder.Coefficients;
        int numZeroCoefficients = ecBytes - coefficients.Length;

        for (var i = 0; i < numZeroCoefficients; i++)
            toEncode[dataBytes + i] = 0;

        Array.Copy(coefficients, 0, toEncode, dataBytes + numZeroCoefficients, coefficients.Length);
    }

    // this method has been added by Sebastien ROBERT
    // this implementation makes the mathematician-friendly approach programmer-friendly
    public byte[] EncodeEx(byte[] toEncode, int ecBytes)
    {
        if (ecBytes == 0)
            throw new ArgumentException("No error correction bytes");

        int dataBytes = toEncode.Length - ecBytes;

        if (dataBytes <= 0)
            throw new ArgumentException("No data bytes provided");

        GenericGFPoly generator = BuildGenerator(ecBytes);
        int[] infoCoefficients = toEncode.Select(x => (int)x).ToArray();

        var info = new GenericGFPoly(field, infoCoefficients);
        info = info.MultiplyByMonomial(ecBytes, 1);

        GenericGFPoly remainder = info.Divide(generator)[1];
        int[] coefficients = remainder.Coefficients;
        int numZeroCoefficients = ecBytes - coefficients.Length;

        return Enumerable.Repeat<byte>(0, numZeroCoefficients)
            .Concat(coefficients.Select(x => (byte)x))
            .ToArray();
    }
}
