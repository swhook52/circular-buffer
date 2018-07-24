using CircularBuffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class CircularBufferTest
{
    [TestMethod]
    public void Read_PastTheEnd_ReturnsCircular()
    {
        // Arrange
        var uut = new CircularBuffer<byte>(3);
        uut.Write(new byte[] { 0, 1, 2 });

        // Act
        var result = uut.Read(1, 3);

        // Assert
        Assert.AreEqual(3, result.Length);
        Assert.AreEqual(1, result[0]);
        Assert.AreEqual(2, result[1]);
        Assert.AreEqual(0, result[2]);
    }

    [TestMethod]
    public void Read_NoCircular_ReturnsNotCircular()
    {
        // Arrange
        var uut = new CircularBuffer<byte>(3);
        uut.Write(new byte[] { 0, 1, 2 });

        // Act
        var result = uut.Read(0, 2);

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(0, result[0]);
        Assert.AreEqual(1, result[1]);
    }
    
    [TestMethod]
    public void Write_SingleItem_ReturnsBuffer()
    {
        // Arrange
        var uut = new CircularBuffer<byte>(1);

        // Act
        uut.Write(new byte[] { 0 });
        var result = uut.Read(0, 1);

        // Assert
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(0, result[0]);
    }
    
    [TestMethod]
    public void Write_MultipleItems_ReturnsBuffer()
    {
        // Arrange
        var uut = new CircularBuffer<byte>(3);

        // Act
        uut.Write(new byte[] { 0, 1, 2 });
        var result = uut.Read(0, 3);

        // Assert
        Assert.AreEqual(3, result.Length);
        Assert.AreEqual(0, result[0]);
        Assert.AreEqual(1, result[1]);
        Assert.AreEqual(2, result[2]);
    }

    [TestMethod]
    public void Write_MultipleItemsPastLength_ReturnsCircular()
    {
        // Arrange
        var uut = new CircularBuffer<byte>(2);

        // Act
        uut.Write(new byte[] { 0, 1, 2, 3, 4 });
        var result = uut.Read(0, 2);

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(4, result[0]);
        Assert.AreEqual(3, result[1]);
    }

    [TestMethod]
    public void Write_MultipleWritesSingleItems_ReturnsBuffer()
    {
        // Arrange
        var uut = new CircularBuffer<byte>(2);

        // Act
        uut.Write(new byte[] { 0 });
        uut.Write(new byte[] { 1 });
        var result = uut.Read(0, 2);

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(0, result[0]);
        Assert.AreEqual(1, result[1]);
    }

    [TestMethod]
    public void Write_MultipleWritesMultipleItems_ReturnsBuffer()
    {
        // Arrange
        var uut = new CircularBuffer<byte>(4);

        // Act
        uut.Write(new byte[] { 0, 1 });
        uut.Write(new byte[] { 2, 3 });
        var result = uut.Read(0, 4);

        // Assert
        Assert.AreEqual(4, result.Length);
        Assert.AreEqual(0, result[0]);
        Assert.AreEqual(1, result[1]);
        Assert.AreEqual(2, result[2]);
        Assert.AreEqual(3, result[3]);
    }

    [TestMethod]
    public void Write_MultipleWritesMultipleItemsPastLength_ReturnsCircular()
    {
        // Arrange
        var uut = new CircularBuffer<byte>(2);

        // Act
        uut.Write(new byte[] { 0, 1, 2, 3, 4 });
        uut.Write(new byte[] { 5, 6 });
        var result = uut.Read(0, 2);

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(6, result[0]);
        Assert.AreEqual(5, result[1]);
    }

    [TestMethod]
    public void Write_Null_ReturnsOriginalBuffer()
    {
        // Arrange
        var uut = new CircularBuffer<byte>(2);

        // Act
        uut.Write(new byte[] { 0, 1});
        uut.Write(null);
        var result = uut.Read(0, 2);

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(0, result[0]);
        Assert.AreEqual(1, result[1]);
    }
}
