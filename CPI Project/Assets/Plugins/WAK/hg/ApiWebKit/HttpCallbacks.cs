using hg.ApiWebKit.core.http;
using System;

namespace hg.ApiWebKit
{
	public class HttpCallbacks<OT>
	{
		public Action<OT, HttpResponse> done;

		public Action<OT, HttpResponse> fail;

		public Action<OT, HttpResponse> always;
	}
}
