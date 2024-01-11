// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Yuika.YImGui;

internal static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Truncate(this float f) => (int) f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Truncate(this Vector2 v) => new Vector2(v.X.Truncate(), v.Y.Truncate());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF AsPoint(this Vector2 v) => new PointF(v.X, v.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 AsVector(this PointF p) => new Vector2(p.X, p.Y);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF AsSize(this Vector2 v) => new SizeF(v.X, v.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 LowerRight(this RectangleF r) => (r.Location + r.Size).AsVector();

    
#if !NET6_0_OR_GREATER
    public static IEnumerable<TSource[]> Chunk<TSource>(this IEnumerable<TSource> source, int size)
    {
        if (source == null!)
        {
            // ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            throw new ArgumentNullException(nameof(source));
        }

        if (size < 1)
        {
            // ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.size);
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        return source is TSource[] {Length: 0} ? Array.Empty<TSource[]>() : ChunkIterator(source, size);
    }
    
    private static IEnumerable<TSource[]> ChunkIterator<TSource>(IEnumerable<TSource> source, int size)
    {
        using IEnumerator<TSource> e = source.GetEnumerator();

        // Before allocating anything, make sure there's at least one element.
        if (e.MoveNext())
        {
            // Now that we know we have at least one item, allocate an initial storage array. This is not
            // the array we'll yield.  It starts out small in order to avoid significantly overallocating
            // when the source has many fewer elements than the chunk size.
            int arraySize = Math.Min(size, 4);
            int i;
            do
            {
                var array = new TSource[arraySize];

                // Store the first item.
                array[0] = e.Current;
                i = 1;

                if (size != array.Length)
                {
                    // This is the first chunk. As we fill the array, grow it as needed.
                    for (; i < size && e.MoveNext(); i++)
                    {
                        if (i >= array.Length)
                        {
                            arraySize = (int)Math.Min((uint)size, 2 * (uint)array.Length);
                            Array.Resize(ref array, arraySize);
                        }

                        array[i] = e.Current;
                    }
                }
                else
                {
                    // For all but the first chunk, the array will already be correctly sized.
                    // We can just store into it until either it's full or MoveNext returns false.
                    TSource[] local = array; // avoid bounds checks by using cached local (`array` is lifted to iterator object as a field)
                    Debug.Assert(local.Length == size);
                    for (; (uint)i < (uint)local.Length && e.MoveNext(); i++)
                    {
                        local[i] = e.Current;
                    }
                }

                if (i != array.Length)
                {
                    Array.Resize(ref array, i);
                }

                yield return array;
            }
            while (i >= size && e.MoveNext());
        }
    }
#endif
}