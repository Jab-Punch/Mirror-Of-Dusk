using System;

public static class HeapAllocator
{
    private static readonly int BytesPerAllocation = 1024;
    private static readonly int AllocationsPerIteration = 1024;

    public static void Allocate(int iterations)
    {
        object[] array = new object[iterations];
        for (int i = 0; i < iterations; i++)
        {
            object[] array2 = new object[HeapAllocator.AllocationsPerIteration];
            for (int j = 0; j < HeapAllocator.AllocationsPerIteration; j++)
            {
                array2[j] = new byte[HeapAllocator.BytesPerAllocation];
            }
            array[i] = array2;
        }
    }
}
