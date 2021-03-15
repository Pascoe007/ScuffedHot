using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T>
{

	T[] binaryHeap;
	int ItemCount;

	public Heap(int maxHeapSize)
	{
		binaryHeap = new T[maxHeapSize];
	}

	public void Add(T item)
	{
		item.HeapIndex = ItemCount;
		binaryHeap[ItemCount] = item;
		SortUp(item);
		ItemCount++;
	}

	public T RemoveFirst()
	{
		T firstItem = binaryHeap[0];
		ItemCount--;
		binaryHeap[0] = binaryHeap[ItemCount];
		binaryHeap[0].HeapIndex = 0;
		SortDown(binaryHeap[0]);
		return firstItem;
	}

	public void UpdateItem(T item)
	{
		SortUp(item);
	}

	public int Count
	{
		get
		{
			return ItemCount;
		}
	}

	public bool Contains(T item)
	{
		return Equals(binaryHeap[item.HeapIndex], item);
	}

	void SortDown(T item)
	{
		while (true)
		{
			int IndexLeft = item.HeapIndex * 2 + 1;
			int IndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if (IndexLeft < ItemCount)
			{
				swapIndex = IndexLeft;

				if (IndexRight < ItemCount)
				{
					if (binaryHeap[IndexLeft].CompareTo(binaryHeap[IndexRight]) < 0)
					{
						swapIndex = IndexRight;
					}
				}

				if (item.CompareTo(binaryHeap[swapIndex]) < 0)
				{
					Swap(item, binaryHeap[swapIndex]);
				}
				else
				{
					return;
				}

			}
			else
			{
				return;
			}

		}
	}

	void SortUp(T item)
	{
		int parentIndex = (item.HeapIndex - 1) / 2;

		while (true)
		{
			T parentItem = binaryHeap[parentIndex];
			if (item.CompareTo(parentItem) > 0)
			{
				Swap(item, parentItem);
			}
			else
			{
				break;
			}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	void Swap(T itemA, T itemB)
	{
		binaryHeap[itemA.HeapIndex] = itemB;
		binaryHeap[itemB.HeapIndex] = itemA;
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}



}

public interface IHeapItem<T> : IComparable<T>
{
	int HeapIndex
	{
		get;
		set;
	}
}