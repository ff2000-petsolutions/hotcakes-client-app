using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KliensApp.Tests
{
    [TestClass]
    public class ValueConverterTests
    {
        [TestMethod]
        public void Convert_StringToInt_ReturnsCorrectInt()
        {
            var converter = new ValueConverter();
            var result = converter.Convert("123", typeof(int));
            Assert.AreEqual(123, result);
        }

        [TestMethod]
        public void Convert_StringToDecimal_ReturnsCorrectDecimal()
        {
            var converter = new ValueConverter();
            var result = converter.Convert("12.5", typeof(decimal));
            Assert.AreEqual(12.5m, result);
        }

        [TestMethod]
        public void Convert_UnsupportedType_ThrowsException()
        {
            var converter = new ValueConverter();
            Assert.ThrowsException<ArgumentException>(() => converter.Convert("1.23", typeof(float)));
        }
    }
}
