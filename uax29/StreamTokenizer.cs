﻿namespace uax29;

/// <summary>
/// Tokenizer splits a stream of UTF-8 bytes as words, sentences or graphemes, per the Unicode UAX #29 spec.
/// </summary>
public ref struct StreamTokenizer<T> where T : struct
{
	internal Tokenizer<T> tok;

	internal Buffer<T> buffer;

	/// <summary>
	/// Tokenizer splits strings (or UTF-8 bytes) as words, sentences or graphemes, per the Unicode UAX #29 spec.
	/// </summary>
	/// <param name="stream">A stream of UTF-8 encoded bytes.</param>
	/// <param name="tokenType">Optional, choose to tokenize words, graphemes or sentences. Default is words.</param>
	/// <param name="maxTokenBytes">
	/// Optional, the maximum token size in bytes. Tokens that exceed this size will simply be cut off at this length, no error will occur.
	/// Default is 1024 bytes. The tokenizer is intended for natural language, so we don't expect you'll find text with a token beyond a couple of dozen bytes.
	/// If this cutoff is too small for your data, increase it. If you'd like to save memory, reduce it.
	/// </param>
	internal StreamTokenizer(Buffer<T> buffer, Tokenizer<T> tok)
	{
		this.tok = tok;
		this.buffer = buffer;
	}

	public bool MoveNext()
	{
		buffer.Consume(tok.Current.Length); // the previous token
		var input = buffer.Contents;
		tok.SetText(input);
		return tok.MoveNext();
	}

	public readonly ReadOnlySpan<T> Current => tok.Current;

	public readonly StreamTokenizer<T> GetEnumerator()
	{
		return this;
	}
}

public static class StreamExtensions
{
	public static void SetStream(ref this StreamTokenizer<byte> stok, Stream stream)
	{
		stok.tok.SetText([]);
		stok.buffer.SetRead(stream.Read);
	}

	public static void SetStream(ref this StreamTokenizer<char> stok, TextReader stream)
	{
		stok.tok.SetText([]);
		stok.buffer.SetRead(stream.Read);
	}
}