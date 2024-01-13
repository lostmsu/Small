namespace Small.Collections;

using System.Runtime.CompilerServices;

using TypeNum;

public struct SmallList<TSize, T>: IList<T>
    where TSize : unmanaged, INumeral<T>
    where T : unmanaged {
    FixedArray<TSize, T> stackItems;
    int stackCount;
    List<T>? heapItems;

    public int Count => this.stackCount + (this.heapItems?.Count ?? 0);
    public bool IsReadOnly => false;

    public int IndexOf(T item) {
        for (int index = 0; index < this.stackCount; index++)
            if (EqualityComparer<T>.Default.Equals(this.stackItems[index], item))
                return index;

        if (this.heapItems is not { } items)
            return -1;

        return items.IndexOf(item) is var i and >= 0
            ? i + this.stackCount
            : -1;
    }

    public void Insert(int index, T item) {
        if (index < 0 || index > this.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (index == this.stackCount) {
            if (this.stackCount < this.stackItems.Count) {
                this.stackItems[this.stackCount] = item;
                this.stackCount++;
            } else {
                this.heapItems ??= [];
                this.heapItems.Insert(0, item);
            }
            return;
        }

        if (index >= this.stackCount) {
            this.heapItems!.Insert(index - this.stackCount, item);
            return;
        }

        if (this.stackCount < this.stackItems.Count) {
            this.stackCount++;
        } else {
            this.heapItems ??= [];
            this.heapItems.Insert(0, this.stackItems[this.stackCount - 1]);
        }

        for (int moveTo = this.stackCount - 1; moveTo > index; moveTo--)
            this.stackItems[moveTo] = this.stackItems[moveTo - 1];
        this.stackItems[index] = item;
    }

    public void RemoveAt(int index) {
        this.ValidateIndex(index);

        if (index >= this.stackCount) {
            this.heapItems!.RemoveAt(index - this.stackCount);
            return;
        }

        for (int moveTo = index; moveTo < this.stackCount - 1; moveTo++)
            this.stackItems[moveTo] = this.stackItems[moveTo + 1];

        if (this.heapItems is { Count: > 0 } items) {
            this.stackItems[this.stackCount - 1] = items[0];
            items.RemoveAt(0);
        } else {
            this.stackCount--;
        }
    }

    public bool Remove(T item) {
        int index = this.IndexOf(item);
        if (index < 0)
            return false;
        this.RemoveAt(index);
        return true;
    }

    public T this[int index] {
        get {
            this.ValidateIndex(index);

            return index < this.stackCount
                ? this.stackItems[index]
                : this.heapItems![index - this.stackCount];
        }

        set {
            this.ValidateIndex(index);

            if (index < this.stackCount)
                this.stackItems[index] = value;
            else
                this.heapItems![index - this.stackCount] = value;
        }
    }

    public void Add(T item) {
        if (this.stackCount < this.stackItems.Count) {
            this.stackItems[this.stackCount] = item;
            this.stackCount++;
        } else {
            this.heapItems ??= [];
            this.heapItems.Add(item);
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => this.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.GetEnumerator();
    public Enumerator GetEnumerator() => new(this);

    public void Clear() {
        this.heapItems?.Clear();
        this.stackCount = 0;
    }

    public bool Contains(T item) => this.IndexOf(item) >= 0;

    public void CopyTo(T[] array, int arrayIndex) {
        this.stackItems.CopyTo(array, arrayIndex);
        this.heapItems?.CopyTo(array, arrayIndex + this.stackCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ValidateIndex(int index) {
        if (index < 0 || index >= this.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
    }

    public struct Enumerator: IEnumerator<T> {
        int index = -1;
        readonly SmallList<TSize, T> list;

        public bool MoveNext() {
            this.index++;
            return this.index < this.list.Count;
        }

        public void Reset() {
            this.index = -1;
        }

        public T Current => this.list[this.index];
        object System.Collections.IEnumerator.Current => this.Current;

        public void Dispose() { }

        internal Enumerator(SmallList<TSize, T> list) {
            this.list = list;
        }
    }
}