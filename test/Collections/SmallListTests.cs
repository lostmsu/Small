namespace Small.Collections;

using TypeNum;

public class SmallListTests {
    [Fact]
    public void Add() {
        var list = new SmallList<N2<int>, int> { 1, 2, 3 };
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void InsertAt() {
        for (int pos = 0; pos <= 3; pos++) {
            var list = new SmallList<N2<int>, int> { 1, 2, 3 };
            var expected = new List<int> { 1, 2, 3 };
            list.Insert(pos, 4);
            expected.Insert(pos, 4);
            Assert.Equal(expected, list);
        }
    }

    [Fact]
    public void RemoveAt() {
        for (int pos = 0; pos < 4; pos++) {
            var list = new SmallList<N2<int>, int> { 1, 2, 3, 4 };
            var expected = new List<int> { 1, 2, 3, 4 };
            list.RemoveAt(pos);
            expected.RemoveAt(pos);
            Assert.Equal(expected, list);
        }
    }

    [Fact]
    public void Indexing() {
        var list = new SmallList<N2<int>, int> { 0, 1, 2, 3 };
        for (int index = 0; index < 4; index++)
            Assert.Equal(index, list[index]);

        var expected = new List<int> { 0, 1, 2, 3 };
        for (int index = 0; index < 4; index++) {
            list[index] = -list[index];
            expected[index] = -expected[index];
            Assert.Equal(expected, list);
        }
    }

    [Fact]
    public void IndexOf() {
        var list = new SmallList<N2<int>, int> { 0, 1, 2, 3 };
        var expected = new List<int> { 0, 1, 2, 3 };
        for (int v = 0; v <= 4; v++)
            Assert.Equal(expected.IndexOf(v), list.IndexOf(v));
    }

    [Fact]
    public void Remove() {
        for (int v = 0; v <= 4; v++) {
            var list = new SmallList<N2<int>, int> { 0, 1, 2, 3 };
            var expected = new List<int> { 0, 1, 2, 3 };
            Assert.Equal(expected.Remove(v), list.Remove(v));
            Assert.Equal(expected, list);
        }
    }
}