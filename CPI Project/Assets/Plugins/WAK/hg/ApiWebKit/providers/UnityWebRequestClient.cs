using hg.ApiWebKit.core.http;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace hg.ApiWebKit.providers
{
    public class UnityWebRequestClient : HttpAbstractProvider
    {
        private UnityWebRequest _request;

        protected override IEnumerator sendImplementation()
        {
            HttpRequestModel.HttpRequestModelResult requestModel = Request.RequestModelResult;
            _request = new UnityWebRequest(requestModel.Uri, requestModel.Verb);
            _request.downloadHandler = new DownloadHandlerBuffer();

            if (requestModel.Data != null && requestModel.Data.Length > 0 && requestModel.Verb != "GET")
            {
                _request.uploadHandler = new UploadHandlerRaw(requestModel.Data);
            }

            if (requestModel.Headers != null)
            {
                foreach (KeyValuePair<string, string> header in requestModel.Headers)
                {
                    _request.SetRequestHeader(header.Key, header.Value);
                }
            }

            UnityWebRequestAsyncOperation operation = _request.SendWebRequest();
            while (TimeElapsed <= requestModel.Timeout && !operation.isDone && !RequestCancelFlag)
            {
                UpdateTransferProgress();
                yield return null;
            }

            UpdateTransferProgress();

            if (RequestCancelFlag)
            {
                RequestCancelFlag = false;
                _request.Abort();
                disposeInternal();
                ChangeState(HttpRequestState.CANCELLED);
            }
            else if (TimeElapsed > requestModel.Timeout)
            {
                _request.Abort();
                disposeInternal();
                ChangeState(HttpRequestState.TIMEOUT);
            }
            else if (!string.IsNullOrEmpty(getError()))
            {
                ChangeState(HttpRequestState.ERROR);
            }
            else
            {
                ChangeState(HttpRequestState.COMPLETED);
            }

            BehaviorComplete();
            Cleanup();
        }

        protected override float getTransferProgress()
        {
            return Request.RequestModelResult.Verb == "GET" ? _request.downloadProgress : _request.uploadProgress;
        }

        protected override void disposeInternal()
        {
            if (_request != null)
            {
                _request.Dispose();
                _request = null;
            }
        }

        protected override Dictionary<string, string> getResponseHeaders()
        {
            if (_request == null)
            {
                return null;
            }

            Dictionary<string, string> responseHeaders = _request.GetResponseHeaders();
            return responseHeaders == null
                ? null
                : new Dictionary<string, string>(responseHeaders, System.StringComparer.OrdinalIgnoreCase);
        }

        protected override string getError()
        {
            return _request != null ? _request.error : null;
        }

        protected override string getText()
        {
            return _request != null && _request.downloadHandler != null ? _request.downloadHandler.text : null;
        }

        protected override byte[] getData()
        {
            return _request != null && _request.downloadHandler != null ? _request.downloadHandler.data : null;
        }

        protected override HttpStatusCode getStatusCode()
        {
            return _request != null && _request.responseCode > 0
                ? (HttpStatusCode)_request.responseCode
                : HttpStatusCode.Unknown;
        }
    }
}
