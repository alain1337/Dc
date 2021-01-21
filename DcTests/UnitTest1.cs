using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DcTests
{
    [TestClass]
    public class UnitTest1
    {
        Dc.Dc dc = new Dc.Dc ();

        [TestMethod]
        public void SimpleTests ()
        {
            Assert.AreEqual (1, dc.Calc ("1"));
            Assert.AreEqual (2, dc.Calc ("1+1"));
            Assert.AreEqual (7, dc.Calc ("1+2*3"));
            Assert.AreEqual (9, dc.Calc ("(1+2)*3"));
        }

        [TestMethod]
        public void Factorial ()
        {
            Assert.AreEqual (1, dc.Calc ("1!"));
            Assert.AreEqual (2, dc.Calc ("2!"));
            Assert.AreEqual (6, dc.Calc ("3!"));
            Assert.AreEqual (9, dc.Calc ("1!+2!+3!"));
            Assert.AreEqual (9.33262154439441E+157, dc.Calc ("100!"));
            Assert.AreEqual (720, dc.Calc ("3!!"));
        }

        [TestMethod]
        public void Pow ()
        {
            Assert.AreEqual (8, dc.Calc ("2^3"));
            Assert.AreEqual (2.4178516392292583E+24, dc.Calc ("2^3^4"));
        }

        [TestMethod]
        public void Unary ()
        {
            Assert.AreEqual (-1, dc.Calc ("-1"));
            Assert.AreEqual (1, dc.Calc ("+1"));
            Assert.AreEqual (2, dc.Calc ("1++1"));
            Assert.AreEqual (2, dc.Calc ("1++++++++++++1"));
            Assert.AreEqual (-9, dc.Calc ("-(1+2)*3"));
        }

        [TestMethod]
        public void Functions ()
        {
            Assert.AreEqual (1, dc.Calc ("log(e)"));
            Assert.AreEqual (1, dc.Calc ("log(+e)"));
            Assert.AreEqual (0, Math.Round (dc.Calc ("sin(pi)"), 8));
            Assert.AreEqual (1000, dc.Calc ("pow(10,3)"));
        }

        [TestMethod]
        public void Wikipedia ()
        {
            Assert.AreEqual (-7, dc.Calc ("(3 + 4)*(5 - 6)"));
            Assert.AreEqual (3.0001220703125, dc.Calc ("3+4*2/(1- 5)^2^3"));
            Assert.AreEqual (-0.23612335628063247, dc.Calc ("cos(1 + sin(log(5) - exp(8))^2)"));
        }

        [TestMethod]
        public void Complex ()
        {
            Assert.AreEqual (-9.830338748941081, dc.Calc ("25-37+2*(1.22+cos(5))*sin(5)*2+5%2*3*sqrt(5+2)"));
        }
    }
}
