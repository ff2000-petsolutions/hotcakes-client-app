using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KliensApp.Tests
{
    [TestClass]
    public class ValueConverterTests
    {
        private ValueConverter converter;

        [TestInitialize]
        public void Setup()
        {
            converter = new ValueConverter();
        }

        [TestMethod]
        public void Convert_StringToInt_ReturnsCorrectInt()
        {
            var result = converter.Convert("123", typeof(int));
            Assert.AreEqual(123, result);
        }

        [TestMethod]
        public void Convert_StringToDecimal_ReturnsCorrectDecimal()
        {
            var result = converter.Convert("12.34", typeof(decimal));
            Assert.AreEqual(12.34m, result);
        }

        [TestMethod]
        public void Convert_StringToBool_ReturnsCorrectBool()
        {
            var result = converter.Convert("true", typeof(bool));
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void Convert_InvalidType_ThrowsException()
        {
            Assert.ThrowsException<ArgumentException>(() => converter.Convert("valami", typeof(float)));
        }
    }
}
