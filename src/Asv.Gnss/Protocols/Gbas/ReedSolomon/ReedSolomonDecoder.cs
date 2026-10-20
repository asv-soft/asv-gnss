using System;
using System.Collections.Generic;
using System.Linq;
namespace Asv.Gnss;

/*
    * Copyright 2007 ZXing authors
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

/// <summary> <p>Implements Reed-Solomon decoding, as the name implies.</p>
///
/// <p>The algorithm will not be explained here, but the following references were helpful
/// in creating this implementation:</p>
///
/// <ul>
/// <li>Bruce Maggs.
/// <a href="http://www.cs.cmu.edu/afs/cs.cmu.edu/project/pscico-guyb/realworld/www/rs_decode.ps">
/// "Decoding Reed-Solomon Codes"</a> (see discussion of Forney's Formula)</li>
/// <li>J.I. Hall. <a href="www.mth.msu.edu/~jhall/classes/codenotes/GRS.pdf">
/// "Chapter 5. Generalized Reed-Solomon Codes"</a>
/// (see discussion of Euclidean algorithm)</li>
/// </ul>
///
/// <p>Much credit is due to William Rucklidge since portions of this code are an indirect
/// port of his C++ Reed-Solomon implementation.</p>
///
/// </summary>
/// <author>Sean Owen</author>
/// <author>William Rucklidge</author>
/// <author>sanfordsquires</author>
internal sealed class ReedSolomonDecoder
{
    private readonly GenericGF field;

    public ReedSolomonDecoder(GenericGF field)
    {
        this.field = field;
    }

    /// <summary>
    ///   <p>Decodes given set of received codewords, which include both data and error-correction
    /// codewords. Really, this means it uses Reed-Solomon to detect and correct errors, in-place,
    /// in the input.</p>
    /// </summary>
    /// <param name="received">data and error-correction codewords</param>
    /// <param name="twoS">number of error-correction codewords available</param>
    /// <returns>false: decoding fails</returns>
    public bool Decode(int[] received, int twoS)
    {
        var poly = new GenericGFPoly(field, received);
        var syndromeCoefficients = new int[twoS];
        var noError = true;

        for (var i = 0; i < twoS; i++)
        {
            int eval = poly.EvaluateAt(field.Exp(i + field.GeneratorBase));
            syndromeCoefficients[syndromeCoefficients.Length - 1 - i] = eval;
            if (eval != 0)
                noError = false;
        }

        if (noError)
            return true;

        var syndrome = new GenericGFPoly(field, syndromeCoefficients);

        GenericGFPoly[] sigmaOmega = RunEuclideanAlgorithm(field.BuildMonomial(twoS, 1), syndrome, twoS);
        if (sigmaOmega == null)
            return false;

        GenericGFPoly sigma = sigmaOmega[0];
        int[] errorLocations = FindErrorLocations(sigma);

        if (errorLocations == null)
            return false;

        GenericGFPoly omega = sigmaOmega[1];
        int[] errorMagnitudes = FindErrorMagnitudes(omega, errorLocations);

        for (int i = 0; i < errorLocations.Length; i++)
        {
            int position = received.Length - 1 - field.Log(errorLocations[i]);
            if (position < 0)
            {
                // throw new ReedSolomonException("Bad error location");
                return false;
            }

            received[position] = GenericGF.AddOrSubtract(received[position], errorMagnitudes[i]);
        }

        return true;
    }

    // this method has been added by Sebastien ROBERT (sebastien.saigo@gmail.com)
    // this implementation makes the mathematician-friendly approach programmer-friendly
    public byte[]? DecodeEx(byte[] message, byte[] ecc)
    {
        int[] received = message
            .Select(x => (int)x)
            .Concat(ecc.Select(x => (int)x))
            .ToArray();

        if (Decode(received, ecc.Length) == false)
            return null;

        return received
            .Take(message.Length)
            .Select(x => (byte)x)
            .ToArray();
    }

    internal GenericGFPoly[] RunEuclideanAlgorithm(GenericGFPoly a, GenericGFPoly b, int R)
    {
        // Assume a's degree is >= b's
        if (a.Degree < b.Degree)
        {
            GenericGFPoly temp = a;
            a = b;
            b = temp;
        }

        GenericGFPoly rLast = a;
        GenericGFPoly r = b;
        GenericGFPoly tLast = field.Zero;
        GenericGFPoly t = field.One;

        int halfR = R / 2;

        // Run Euclidean algorithm until r's degree is less than R/2
        while (r.Degree >= halfR)
        {
            GenericGFPoly rLastLast = rLast;
            GenericGFPoly tLastLast = tLast;

            rLast = r;
            tLast = t;

            // Divide rLastLast by rLast, with quotient in q and remainder in r
            if (rLast.IsZero)
            {
                // Oops, Euclidean algorithm already terminated?
                // throw new ReedSolomonException("r_{i-1} was zero");
                return null;
            }

            r = rLastLast;

            GenericGFPoly q = field.Zero;
            int denominatorLeadingTerm = rLast.GetCoefficient(rLast.Degree);
            int dltInverse = field.Inverse(denominatorLeadingTerm);

            while (r.Degree >= rLast.Degree && !r.IsZero)
            {
                int degreeDiff = r.Degree - rLast.Degree;
                int scale = field.Multiply(r.GetCoefficient(r.Degree), dltInverse);
                q = q.AddOrSubtract(field.BuildMonomial(degreeDiff, scale));
                r = r.AddOrSubtract(rLast.MultiplyByMonomial(degreeDiff, scale));
            }

            t = q.Multiply(tLast).AddOrSubtract(tLastLast);

            if (r.Degree >= rLast.Degree)
            {
                // throw new IllegalStateException("Division algorithm failed to reduce polynomial?");
                return null;
            }
        }

        int sigmaTildeAtZero = t.GetCoefficient(0);

        if (sigmaTildeAtZero == 0)
        {
            // throw new ReedSolomonException("sigmaTilde(0) was zero");
            return null;
        }

        int inverse = field.Inverse(sigmaTildeAtZero);
        GenericGFPoly sigma = t.Multiply(inverse);
        GenericGFPoly omega = r.Multiply(inverse);

        return new GenericGFPoly[] { sigma, omega };
    }

    private int[] FindErrorLocations(GenericGFPoly errorLocator)
    {
        // This is a direct application of Chien's search
        int numErrors = errorLocator.Degree;

        if (numErrors == 1)
        {
            // shortcut
            return new int[] { errorLocator.GetCoefficient(1) };
        }

        int[] result = new int[numErrors];
        int e = 0;

        for (int i = 1; i < field.Size && e < numErrors; i++)
        {
            if (errorLocator.EvaluateAt(i) == 0)
            {
                result[e] = field.Inverse(i);
                e++;
            }
        }

        if (e != numErrors)
        {
            // throw new ReedSolomonException("Error locator degree does not match number of roots");
            return null;
        }

        return result;
    }

    private int[] FindErrorMagnitudes(GenericGFPoly errorEvaluator, int[] errorLocations)
    {
        // This is directly applying Forney's Formula
        int s = errorLocations.Length;
        int[] result = new int[s];

        for (int i = 0; i < s; i++)
        {
            int xiInverse = field.Inverse(errorLocations[i]);
            int denominator = 1;

            for (int j = 0; j < s; j++)
            {
                if (i != j)
                {
                    //denominator = field.multiply(denominator,
                    //    GenericGF.addOrSubtract(1, field.multiply(errorLocations[j], xiInverse)));
                    // Above should work but fails on some Apple and Linux JDKs due to a Hotspot bug.
                    // Below is a funny-looking workaround from Steven Parkes
                    int term = field.Multiply(errorLocations[j], xiInverse);
                    int termPlus1 = (term & 0x1) == 0 ? term | 1 : term & ~1;
                    denominator = field.Multiply(denominator, termPlus1);

                    // removed in java version, not sure if this is right
                    // denominator = field.multiply(denominator, GenericGF.addOrSubtract(1, field.multiply(errorLocations[j], xiInverse)));
                }
            }

            result[i] = field.Multiply(errorEvaluator.EvaluateAt(xiInverse), field.Inverse(denominator));

            for (int j = 0; j < field.GeneratorBase; j++)
            {
                result[i] = field.Multiply(result[i], xiInverse);
            }
        }

        return result;
    }
}
