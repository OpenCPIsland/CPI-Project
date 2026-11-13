using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace LitJson
{
	public class JsonReader
	{
		private static IDictionary<int, IDictionary<int, int[]>> parse_table;

		private Stack<int> automaton_stack;

		private int current_input;

		private int current_symbol;

		private bool end_of_json;

		private bool end_of_input;

		private Lexer lexer;

		private bool parser_in_string;

		private bool parser_return;

		private bool read_started;

		private TextReader reader;

		private bool reader_is_owned;

		private bool skip_non_members;

		private object token_value;

		private JsonToken token;

		public bool AllowComments
		{
			get
			{
				return lexer.AllowComments;
			}
			set
			{
				lexer.AllowComments = value;
			}
		}

		public bool AllowSingleQuotedStrings
		{
			get
			{
				return lexer.AllowSingleQuotedStrings;
			}
			set
			{
				lexer.AllowSingleQuotedStrings = value;
			}
		}

		public bool SkipNonMembers
		{
			get
			{
				return skip_non_members;
			}
			set
			{
				skip_non_members = value;
			}
		}

		public bool EndOfInput
		{
			get
			{
				return end_of_input;
			}
		}

		public bool EndOfJson
		{
			get
			{
				return end_of_json;
			}
		}

		public JsonToken Token
		{
			get
			{
				return token;
			}
		}

		public object Value
		{
			get
			{
				return token_value;
			}
		}

		static JsonReader()
		{
			PopulateParseTable();
		}

		public JsonReader(string json_text)
			: this(new StringReader(json_text), true)
		{
		}

		public JsonReader(TextReader reader)
			: this(reader, false)
		{
		}

		private JsonReader(TextReader reader, bool owned)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			parser_in_string = false;
			parser_return = false;
			read_started = false;
			automaton_stack = new Stack<int>();
			automaton_stack.Push(65555);
			automaton_stack.Push(65543);
			lexer = new Lexer(reader);
			end_of_input = false;
			end_of_json = false;
			skip_non_members = true;
			this.reader = reader;
			reader_is_owned = owned;
		}

		private static void PopulateParseTable()
		{
			parse_table = new Dictionary<int, IDictionary<int, int[]>>();
			TableAddRow(ParserToken.Array);
			TableAddCol(ParserToken.Array, 91, 91, 65551);
			TableAddRow(ParserToken.ArrayPrime);
			TableAddCol(ParserToken.ArrayPrime, 34, 65552, 65553, 93);
			TableAddCol(ParserToken.ArrayPrime, 91, 65552, 65553, 93);
			TableAddCol(ParserToken.ArrayPrime, 93, 93);
			TableAddCol(ParserToken.ArrayPrime, 123, 65552, 65553, 93);
			TableAddCol(ParserToken.ArrayPrime, 65537, 65552, 65553, 93);
			TableAddCol(ParserToken.ArrayPrime, 65538, 65552, 65553, 93);
			TableAddCol(ParserToken.ArrayPrime, 65539, 65552, 65553, 93);
			TableAddCol(ParserToken.ArrayPrime, 65540, 65552, 65553, 93);
			TableAddRow(ParserToken.Object);
			TableAddCol(ParserToken.Object, 123, 123, 65545);
			TableAddRow(ParserToken.ObjectPrime);
			TableAddCol(ParserToken.ObjectPrime, 34, 65546, 65549, 125);
			TableAddCol(ParserToken.ObjectPrime, 125, 125);
			TableAddCol(ParserToken.ObjectPrime, 65537, 65547, 65548, 125);
			TableAddRow(ParserToken.Colon);
			TableAddCol(ParserToken.Colon, 65537, 65537, 58, 65552);
			TableAddRow(ParserToken.Pair);
			TableAddCol(ParserToken.Pair, 34, 65554, 58, 65552);
			TableAddRow(ParserToken.PairRest);
			TableAddCol(ParserToken.PairRest, 44, 44, 65546, 65549);
			TableAddCol(ParserToken.PairRest, 125, 65556);
			TableAddRow(ParserToken.ColonRest);
			TableAddCol(ParserToken.ColonRest, 44, 44, 65547, 65548);
			TableAddCol(ParserToken.ColonRest, 125, 65556);
			TableAddRow(ParserToken.String);
			TableAddCol(ParserToken.String, 34, 34, 65541, 34);
			TableAddRow(ParserToken.Text);
			TableAddCol(ParserToken.Text, 91, 65550);
			TableAddCol(ParserToken.Text, 123, 65544);
			TableAddRow(ParserToken.Value);
			TableAddCol(ParserToken.Value, 34, 65554);
			TableAddCol(ParserToken.Value, 91, 65550);
			TableAddCol(ParserToken.Value, 123, 65544);
			TableAddCol(ParserToken.Value, 65537, 65537);
			TableAddCol(ParserToken.Value, 65538, 65538);
			TableAddCol(ParserToken.Value, 65539, 65539);
			TableAddCol(ParserToken.Value, 65540, 65540);
			TableAddRow(ParserToken.ValueRest);
			TableAddCol(ParserToken.ValueRest, 44, 44, 65552, 65553);
			TableAddCol(ParserToken.ValueRest, 93, 65556);
		}

		private static void TableAddCol(ParserToken row, int col, params int[] symbols)
		{
			parse_table[(int)row].Add(col, symbols);
		}

		private static void TableAddRow(ParserToken rule)
		{
			parse_table.Add((int)rule, new Dictionary<int, int[]>());
		}

		private void ProcessNumber(string number)
		{
			double result;
			int result2;
			long result3;
			ulong result4;
			if ((number.IndexOf('.') != -1 || number.IndexOf('e') != -1 || number.IndexOf('E') != -1) && double.TryParse(number, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result))
			{
				token = JsonToken.Double;
				token_value = result;
			}
			else if (int.TryParse(number, out result2))
			{
				token = JsonToken.Int;
				token_value = result2;
			}
			else if (long.TryParse(number, out result3))
			{
				token = JsonToken.Long;
				token_value = result3;
			}
			else if (ulong.TryParse(number, out result4))
			{
				token = JsonToken.Long;
				token_value = result4;
			}
			else
			{
				token = JsonToken.Int;
				token_value = 0;
			}
		}

		private void ProcessSymbol()
		{
			if (current_symbol == 91)
			{
				token = JsonToken.ArrayStart;
				parser_return = true;
			}
			else if (current_symbol == 93)
			{
				token = JsonToken.ArrayEnd;
				parser_return = true;
			}
			else if (current_symbol == 123)
			{
				token = JsonToken.ObjectStart;
				parser_return = true;
			}
			else if (current_symbol == 125)
			{
				token = JsonToken.ObjectEnd;
				parser_return = true;
			}
			else if (current_symbol == 34)
			{
				if (parser_in_string)
				{
					parser_in_string = false;
					parser_return = true;
					return;
				}
				if (token == JsonToken.None)
				{
					token = JsonToken.String;
				}
				parser_in_string = true;
			}
			else if (current_symbol == 65541)
			{
				token_value = lexer.StringValue;
			}
			else if (current_symbol == 65539)
			{
				token = JsonToken.Boolean;
				token_value = false;
				parser_return = true;
			}
			else if (current_symbol == 65540)
			{
				token = JsonToken.Null;
				parser_return = true;
			}
			else if (current_symbol == 65537)
			{
				ProcessNumber(lexer.StringValue);
				parser_return = true;
			}
			else if (current_symbol == 65546)
			{
				token = JsonToken.PropertyName;
			}
			else if (current_symbol == 65547)
			{
				token = JsonToken.PropertyName;
			}
			else if (current_symbol == 65538)
			{
				token = JsonToken.Boolean;
				token_value = true;
				parser_return = true;
			}
		}

		private bool ReadToken()
		{
			if (end_of_input)
			{
				return false;
			}
			lexer.NextToken();
			if (lexer.EndOfInput)
			{
				Close();
				return false;
			}
			current_input = lexer.Token;
			return true;
		}

		public void Close()
		{
			if (!end_of_input)
			{
				end_of_input = true;
				end_of_json = true;
				if (reader_is_owned)
				{
					reader.Close();
				}
				reader = null;
			}
		}

		public bool Read()
		{
			if (end_of_input)
			{
				return false;
			}
			if (end_of_json)
			{
				end_of_json = false;
				automaton_stack.Clear();
				automaton_stack.Push(65555);
				automaton_stack.Push(65543);
			}
			parser_in_string = false;
			parser_return = false;
			token = JsonToken.None;
			token_value = null;
			if (!read_started)
			{
				read_started = true;
				if (!ReadToken())
				{
					return false;
				}
			}
			while (true)
			{
				if (parser_return)
				{
					if (automaton_stack.Peek() == 65555)
					{
						end_of_json = true;
					}
					return true;
				}
				current_symbol = automaton_stack.Pop();
				ProcessSymbol();
				if (current_symbol == current_input)
				{
					if (!ReadToken())
					{
						break;
					}
					continue;
				}
				int[] array;
				try
				{
					array = parse_table[current_symbol][current_input];
				}
				catch (KeyNotFoundException inner_exception)
				{
					throw new JsonException((ParserToken)current_input, inner_exception);
				}
				if (array[0] != 65556)
				{
					for (int num = array.Length - 1; num >= 0; num--)
					{
						automaton_stack.Push(array[num]);
					}
				}
			}
			if (automaton_stack.Peek() != 65555)
			{
				throw new JsonException("Input doesn't evaluate to proper JSON text");
			}
			if (parser_return)
			{
				return true;
			}
			return false;
		}
	}
}
