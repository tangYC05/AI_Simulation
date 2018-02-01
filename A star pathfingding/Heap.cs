﻿using System.Collections;
using System;
using UnityEngine;

//This sript is use to optimising the pathfinding by using Heap sort 

public class Heap<T> where T : IHeapItem<T>{

	T[] items;
	int currentItemCount;

	public Heap(int maxHeapSize)
	{
		items = new T[maxHeapSize];
	}

	//adding items to the tree to be sorted
	public void Add(T item)
	{
		item.HeapIndex = currentItemCount;
		items [currentItemCount] = item;
		sortUp (item);
		currentItemCount++;
	}

	public T removeFirst()
	{
		T firstItem = items [0];
		currentItemCount--;
		items [0] = items [currentItemCount];
		items [0].HeapIndex = 0;
		sortDown (items [0]);
		return firstItem;
	}

	//update if there is any changes
	public void UpdateItem(T item)
	{
		sortUp (item);
	}

	//increase the item count
	public int Count
	{
		get{return currentItemCount;}
	}

	//check if heap contains a specific item
	public bool Contains (T item)
	{
		//check if is the same item
		return Equals(items[item.HeapIndex], item);
	}

	//Sport the item by comparing parent with child, 
	//and check if its the leaft the the tree
	void sortDown(T item)
	{
		while (true) 
		{
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if (childIndexLeft < currentItemCount) {
				swapIndex = childIndexLeft;

				if (childIndexRight < currentItemCount) {
					if (items [childIndexLeft].CompareTo (items [childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}

				if (item.CompareTo (items [swapIndex]) < 0) {
					swap (item, items [swapIndex]);
				} else {
					return;
				}
					
			} else {
				return;
			}
		}
	}


	//Sort the items up by comparing children with parent
	void sortUp(T item)
	{
		int parentIndex = (item.HeapIndex - 1) / 2;
		while (true) 
		{
			T parentItem = items [parentIndex];
			if (item.CompareTo (parentItem) > 0) {
				swap (item, parentItem);
			} 
			else 
			{
				break;
			}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	//swap items
	void swap (T itemA, T itemB)
	{
		items [itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}

}

public interface IHeapItem<T> : IComparable <T>
{
	int HeapIndex {
		get;
		set;
	}
}

