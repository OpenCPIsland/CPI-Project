using System;
using System.Collections;
using System.Collections.Generic;

namespace ZenFulcrum.EmbeddedBrowser
{
	public interface IPromise2<PromisedT2>
	{
		PromisedT2 Value
		{
			get;
		}

		IPromise2<PromisedT2> WithName(string name);

		void Done(Action<PromisedT2> onResolved, Action<Exception> onRejected);

		void Done(Action<PromisedT2> onResolved);

		void Done();

		IPromise2<PromisedT2> Catch(Action<Exception> onRejected);

		IPromise2<ConvertedT> Then<ConvertedT>(Func<PromisedT2, IPromise2<ConvertedT>> onResolved);

		IPromise2 Then(Func<PromisedT2, IPromise2> onResolved);

		IPromise2<PromisedT2> Then(Action<PromisedT2> onResolved);

		IPromise2<ConvertedT> Then<ConvertedT>(Func<PromisedT2, IPromise2<ConvertedT>> onResolved, Action<Exception> onRejected);

		IPromise2 Then(Func<PromisedT2, IPromise2> onResolved, Action<Exception> onRejected);

		IPromise2<PromisedT2> Then(Action<PromisedT2> onResolved, Action<Exception> onRejected);

		IPromise2<ConvertedT> Then<ConvertedT>(Func<PromisedT2, ConvertedT> transform);

		[Obsolete("Use Then instead")]
		IPromise2<ConvertedT> Transform<ConvertedT>(Func<PromisedT2, ConvertedT> transform);

		IPromise2<IEnumerable<ConvertedT>> ThenAll<ConvertedT>(Func<PromisedT2, IEnumerable<IPromise2<ConvertedT>>> chain);

		IPromise2 ThenAll(Func<PromisedT2, IEnumerable<IPromise2>> chain);

		IPromise2<ConvertedT> ThenRace<ConvertedT>(Func<PromisedT2, IEnumerable<IPromise2<ConvertedT>>> chain);

		IPromise2 ThenRace(Func<PromisedT2, IEnumerable<IPromise2>> chain);

		IEnumerator ToWaitFor(bool abortOnFail = false);
	}
	public interface IPromise2
	{
		IPromise2 WithName(string name);

		void Done(Action onResolved, Action<Exception> onRejected);

		void Done(Action onResolved);

		void Done();

		IPromise2 Catch(Action<Exception> onRejected);

		IPromise2<ConvertedT> Then<ConvertedT>(Func<IPromise2<ConvertedT>> onResolved);

		IPromise2 Then(Func<IPromise2> onResolved);

		IPromise2 Then(Action onResolved);

		IPromise2<ConvertedT> Then<ConvertedT>(Func<IPromise2<ConvertedT>> onResolved, Action<Exception> onRejected);

		IPromise2 Then(Func<IPromise2> onResolved, Action<Exception> onRejected);

		IPromise2 Then(Action onResolved, Action<Exception> onRejected);

		IPromise2 ThenAll(Func<IEnumerable<IPromise2>> chain);

		IPromise2<IEnumerable<ConvertedT>> ThenAll<ConvertedT>(Func<IEnumerable<IPromise2<ConvertedT>>> chain);

		IPromise2 ThenSequence(Func<IEnumerable<Func<IPromise2>>> chain);

		IPromise2 ThenRace(Func<IEnumerable<IPromise2>> chain);

		IPromise2<ConvertedT> ThenRace<ConvertedT>(Func<IEnumerable<IPromise2<ConvertedT>>> chain);

		IEnumerator ToWaitFor(bool abortOnFail = false);
	}
}
