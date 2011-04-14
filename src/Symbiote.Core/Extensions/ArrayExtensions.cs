using System;
namespace Symbiote.Core
{
	public static class ArrayExtensions
	{
		public static T[] GetRange<T>( this T[] array, int offset, int count )
		{
			var newArray = new T[count];
			Buffer.BlockCopy( array, offset, newArray, 0, count );
			return newArray;
		}
		
		public static T[] GetRange<T>( this ArraySegment<T> segment )
		{
			return GetRange( segment.Array, segment.Offset, segment.Count );
		}
	}
}

