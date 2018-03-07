﻿using System;
using System.Text.RegularExpressions;

namespace Celloc
{
	public enum Offset
	{
		None,
		ZeroBased
	}

	public static class CellIndex
    {
		private const string ColumnRowPattern = "^(?<column>[a-zA-Z]+)(?<row>[0-9]+)$";

		public static (int column, int row) Translate(string cell, Offset offset = Offset.None)
		{
			if (string.IsNullOrEmpty(cell))
				throw new ArgumentNullException(nameof(cell));

			var regex = new Regex(ColumnRowPattern);

			ValidateCellFormat(cell, regex);

			return CalculateIndex(cell, offset, regex);
		}

	    private static (int column, int row) CalculateIndex(string cell, Offset offset, Regex regex)
	    {
		    var match = regex.Match(cell);

		    var column = match.Groups["column"].Value;
		    var row = Convert.ToInt32(match.Groups["row"].Value);

		    ValidateRowRange(row);

		    var columnNumber = ConvertColumnToNumber(column);

		    return ApplyOffset(offset, columnNumber, row);
	    }

	    private static (int column, int row) ApplyOffset(Offset offset, int column, int row)
	    {
		    return offset == Offset.None ?
			    (column, row) : 
			    (column - 1, row - 1);
	    }

	    private static void ValidateRowRange(int row)
	    {
		    if (row < 0 || row > 1048576)
			    throw new ArgumentOutOfRangeException(nameof(row), "Row must be greater than 0 and less than 1048576.");
	    }

	    private static void ValidateCellFormat(string cell, Regex regex)
	    {
		    if (!regex.IsMatch(cell))
			    throw new Exception($"{cell} is not a valid cell format.");
	    }

	    private static int ConvertColumnToNumber(string column)
	    {
		    if (string.IsNullOrEmpty(column))
			    throw new ArgumentNullException(nameof(column));

		    column = column.ToUpperInvariant();

		    var sum = 0;

		    foreach (var letter in column)
		    {
			    sum *= 26;
			    sum += (letter - 'A' + 1);
		    }

		    return sum;
	    }
	}
}