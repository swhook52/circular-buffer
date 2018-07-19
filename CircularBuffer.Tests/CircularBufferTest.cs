using CircularBuffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class CircularBufferTest
{


    [TestMethod]
    public void Read_PastTheEnd_ReturnsCircular()
    {
        // Arrange
        var uut = new CircularBuffer<byte>(3);
        uut.Add(0);
        uut.Add(1);
        uut.Add(2);

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
        uut.Add(0);
        uut.Add(1);
        uut.Add(2);

        // Act
        var result = uut.Read(0, 2);

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(0, result[0]);
        Assert.AreEqual(1, result[1]);
    }

    [TestMethod]
    public void Add_MultipleItems_ReturnsBuffer()
    {
        // Arrange
        var uut = new CircularBuffer<byte>(3);

        // Act
        uut.Add(0);
        uut.Add(1);
        var result = uut.Read(0, 3);

        // Assert
        Assert.AreEqual(3, result.Length);
        Assert.AreEqual(0, result[0]);
        Assert.AreEqual(1, result[1]);
    }

    [TestMethod]
    public void Add_MultipleItemsPastLength_ReturnsCircular()
    {
        // Arrange
        var uut = new CircularBuffer<byte>(2);

        // Act
        uut.Add(0);
        uut.Add(1);
        uut.Add(2);
        var result = uut.Read(0, 2);

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(2, result[0]);
        Assert.AreEqual(1, result[1]);
    }
}
