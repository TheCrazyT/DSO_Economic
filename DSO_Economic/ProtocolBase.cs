using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Protocol
{
	[System.Runtime.InteropServices.ComVisible(false)] public class ProtocolBase
	{
		#region IInternetProtocol Members

		public void Resume()
		{
			Debug.WriteLine("Resume");
		}

		public void Terminate(uint dwOptions)
		{
			Debug.WriteLine("Terminate");
		}

		public void Seek(_LARGE_INTEGER dlibMove, uint dwOrigin, out _ULARGE_INTEGER plibNewPosition)
		{
			Debug.WriteLine("Seek");
			plibNewPosition = new _ULARGE_INTEGER ();
		}

		public void LockRequest(uint dwOptions)
		{
			Debug.WriteLine("LockRequest");
		}

		public void UnlockRequest()
		{
			Debug.WriteLine("UnlockRequest");
		}

		public void Abort(int hrReason, uint dwOptions)
		{
			Debug.WriteLine("Abort");
		}

		public void Suspend()
		{
			Debug.WriteLine("Suspend");
		}

		public void Continue(ref _tagPROTOCOLDATA pProtocolData)
		{
			Debug.WriteLine("Continue");
		}

		const int S_OK = 0;
		const int S_FALSE = 1;
		public UInt32 Read(System.IntPtr pv, uint cb, out uint pcbRead)
		{
			pcbRead = (uint)Math.Min(cb, StreamBuffer.Length);
			pcbRead = (uint)Stream.Read(StreamBuffer, 0, (int)pcbRead);
			Marshal.Copy(StreamBuffer, 0, pv, (int)pcbRead);

			UInt32 response = (pcbRead == 0) ? (UInt32)S_FALSE : (UInt32)S_OK;
			return response;
		}

		#endregion

		[ComRegisterFunction] private static void RegisterProtocol(Type t)
		{
			ProtocolSupport.RegisterProtocol(t);
		}

		[ComUnregisterFunction] private static void UnregisterProtocol(Type t)
		{
			ProtocolSupport.UnregisterProtocol(t);
		}

		public static IHttpNegotiate GetHttpNegotiate(IInternetProtocolSink Sink)
		{
			if ((Sink is IServiceProvider) == false)
				throw new Exception("Error ProtocolSink does not support IServiceProvider.");
			Debug.WriteLine("ServiceProvider");

			IServiceProvider Provider = (IServiceProvider)Sink;
			object obj_Negotiate = new object();
			Provider.QueryService(ref Guids.IID_IHttpNegotiate, ref Guids.IID_IHttpNegotiate, out obj_Negotiate);
			return (IHttpNegotiate)obj_Negotiate;
		}

		public static BINDINFO GetBindInfo(IInternetBindInfo pOIBindInfo)
		{
			BINDINFO BindInfo = new BINDINFO();
			BindInfo.cbSize = (UInt32)Marshal.SizeOf(typeof(BINDINFO));
			UInt32 AsyncFlag;
			pOIBindInfo.GetBindInfo(out AsyncFlag, ref BindInfo);
			return BindInfo;
		}

		protected MemoryStream Stream = new MemoryStream(0x8000);
		protected byte[] StreamBuffer = new byte[0x8000];
	}

}