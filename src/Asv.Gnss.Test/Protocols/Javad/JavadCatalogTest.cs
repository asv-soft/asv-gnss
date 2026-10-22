using System;
using System.Linq;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class JavadCatalogTest
{
    // Independently transcribed from the pre-implementation manual catalog, with body-only sj
    // and compatibility messages from Appendix A.4 added explicitly.
    private const string ExpectedIds = """
        JP MF ~~ :: RD TO DO BP GT GO NT NO EO WO QO CO Io UO
        WU EU QU CU IU NU ST PO Po VE PV PG Pg VG SG mp bp DP
        SP SV BL bL mR PS PT SX AN NN EL AZ RX RC R1 R2 R3 R5
        Rl rx rc r1 r2 r3 r5 rl CR 1R 2R 3R 5R lR cr 1r 2r 3r
        5r lr cm 1m 2m 3m 5m lm CC C1 C2 C3 C5 Cl cc c1 c2 c3
        c5 cl PC P1 P2 P3 P5 Pl pc p1 p2 p3 p5 pl CP 1P 2P 3P
        5P lP cp 1p 2p 3p 5p lp cf 1f 2f 3f 5f lf DX DC D1 D2
        D3 D5 Dl 0d 1d 2d 3d 5d ld EC E1 E2 E3 E5 El CE 1E 2E
        3E 5E lE s0 s1 s2 s3 s5 sl j0 j1 j2 j3 j5 jl FC F1 F2
        F3 F5 Fl ec e1 e2 e3 e5 qc q1 q2 q3 q5 x0 x1 x2 x3 x5
        x4 y0 y1 y2 y3 y5 y4 IQ TC SS ID rr vr GA EA QA CA IA
        NA WA GE EN QE CN NE WE IE gd qd lD WD ED cd id xd sp
        sP Sp ms gC g1 g2 g3 gR MR mr AR AV IM MA fA lA tA aV
        mA dV PE pV pE pe XA XB ZA ZB YA YB ha RO RG RE ER IO
        QI CI II == LT >> PM LH AI BI SE SM TT OO || DL GS ES
        WS NS QS CS Is LS RS TX RM NP MP NR SY TR TM RP RK AP
        AB QQ WW ZZ TD SI sj rE rM rV rT GD QD LD MS
        """;

    [Fact]
    public void DefaultFactory_CoversEveryManualMessageIncludingCompatibilityAppendix()
    {
        var builder = new ProtocolMessageFactoryBuilder<JavadMessageBase, string>(
            JavadProtocol.Info
        );
        JavadBinaryMessageFactory.Register(builder);
        JavadTextMessageFactory.Register(builder);
        var factory = builder.Build();
        var expected = ExpectedIds.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(303, expected.Length);
        Assert.Equal(
            expected.Order(StringComparer.Ordinal),
            factory.GetSupportedIds().Order(StringComparer.Ordinal)
        );
        foreach (var id in expected)
        {
            var message = Assert.IsAssignableFrom<JavadMessageBase>(factory.Create(id));
            Assert.IsNotType<JavadRawMessage>(message);
            Assert.Equal(id, message.Id);
            Assert.Equal(JavadProtocol.Info, message.Protocol);
        }
    }
}
