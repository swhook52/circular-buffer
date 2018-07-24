using CircularBuffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class CircularStreamTest
{
    #region Write
    [TestMethod]
    public void Write_FillBuffer_WritesToBuffer()
    {
        // Arrange
        var uut = new CircularStream(5);
        var bytesToWrite = new Byte[] { 0, 1, 2, 3, 4 };

        // Act
        uut.Write(bytesToWrite, 111, 222);
        var result = new Byte[5];
        uut.Read(result, 0, 5);

        // Assert
        Assert.AreEqual(0, result[0]);
        Assert.AreEqual(1, result[1]);
        Assert.AreEqual(2, result[2]);
        Assert.AreEqual(3, result[3]);
        Assert.AreEqual(4, result[4]);
    }

    [TestMethod]
    public void Write_FillPastBuffer_ReturnsJustLengthOfBuffer()
    {
        // Arrange
        var uut = new CircularStream(2);
        var bytesToWrite = new Byte[] { 0, 1, 2, 3 };

        // Act
        uut.Write(bytesToWrite, 111, 222);
        var result = new Byte[2];
        uut.Read(result, 0, 2);

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(2, result[0]);
        Assert.AreEqual(3, result[1]);
    }

    [TestMethod]
    public void Write_DoNotCompletelyFillBuffer_ReturnsJustWhatIsWritten()
    {
        // Arrange
        var uut = new CircularStream(2);
        var bytesToWrite = new Byte[] { 0 };

        // Act
        uut.Write(bytesToWrite, 111, 222);
        var result = new Byte[1];
        uut.Read(result, 0, 1);

        // Assert
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(0, result[0]);
    }
    #endregion

    #region Read
    [TestMethod]
    public void Read_FillBuffer_WritesToBuffer()
    {
        // Arrange
        var uut = new CircularStream(5);
        uut.Write(new byte[] { 0, 1, 2, 3, 4 }, 0, 5);
        var result = new byte[5];

        // Act
        uut.Read(result, 0, 5);

        // Assert
        Assert.AreEqual(5, result.Length);
        Assert.AreEqual(0, result[0]);
        Assert.AreEqual(1, result[1]);
        Assert.AreEqual(2, result[2]);
        Assert.AreEqual(3, result[3]);
        Assert.AreEqual(4, result[4]);
    }

    [TestMethod]
    public void Read_PastBuffer_ReturnsCircularly()
    {
        // Arrange
        var uut = new CircularStream(2);
        uut.Write(new byte[] { 0, 1 }, 0, 2);
        var result = new byte[4];

        // Act
        uut.Read(result, 0, 4);

        // Assert
        Assert.AreEqual(4, result.Length);
        Assert.AreEqual(0, result[0]);
        Assert.AreEqual(1, result[1]);
        Assert.AreEqual(0, result[2]);
        Assert.AreEqual(1, result[3]);
    }

    [TestMethod]
    public void Read_ReadLessThanBufferLength_ReturnsSomeOfTheBuffer()
    {
        // Arrange
        var uut = new CircularStream(2);
        uut.Write(new byte[] { 0, 1 }, 0, 2);
        var result = new byte[1];

        // Act
        uut.Read(result, 0, 1);

        // Assert
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(0, result[0]);
    }
    #endregion
}
